using Forum.Application.Common.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace Forum.WebAPI.Configurations.Authorization;

public class PermissionHandler(IHttpContextAccessor httpContextAccessor,
    IForumDbContext dbContext,
    IRoleManager roleManager) : AuthorizationHandler<PermissionRequirement>
{
    protected override async Task HandleRequirementAsync(AuthorizationHandlerContext context,
        PermissionRequirement requirement)
    {
        int? requestedForumId = GetForumIdFromRoute();
        var forumsInScope = requestedForumId == null
            ? []
            : await GetAllowedScope((int)requestedForumId);

        var allowedRoleIds = await dbContext.Permissions
            .Include(p => p.Roles)
            .Where(p => p.Name == requirement.PermissionType &&
                (p.IsGlobal || p.Forums.Select(f => f.Id).Any(id => forumsInScope.Contains(id))))
            .SelectMany(p => p.Roles.Select(r => r.IdentityRoleId))
            .Distinct()
            .ToListAsync();

        var userRoles = context.User.Claims
            .Where(c => c.Type == ClaimTypes.Role)
            .Select(c => c.Value)
            .ToList();

        foreach (var roleId in allowedRoleIds)
        {
            var role = await roleManager.GetRoleByIdAsync(roleId);
            if (role != null && userRoles.Contains(role))
            {
                context.Succeed(requirement);
                return;
            }
        }
    }

    private int? GetForumIdFromRoute()
    {
        object? forumIdObj = httpContextAccessor.HttpContext?.GetRouteValue("forumId");
        return int.TryParse(forumIdObj?.ToString(), out int forumId) ? forumId : null;
    }

    private async Task<List<int>> GetAllowedScope(int entryId)
    {
        var forums = await dbContext.Forums.Select(f => new { f.Id, f.ParentForumId })
            .ToDictionaryAsync(k => k.Id, e => e.ParentForumId);
        var ancestors = new List<int>() { entryId };
        GetAncestors(entryId, forums, ancestors);
        return ancestors;
    }

    private static void GetAncestors(int currentId, Dictionary<int, int?> forums, List<int> ancestors)
    {
        int? parentId = forums[currentId];
        if (parentId != null)
        {
            ancestors.Add((int)parentId);
            GetAncestors((int)parentId, forums, ancestors);
        }
    }
}
