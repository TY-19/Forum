using Forum.Application.Common.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace Forum.WebAPI.Configurations.Authorization;

public class PermissionHandler(IForumDbContext dbContext, IRoleManager roleManager) : AuthorizationHandler<PermissionRequirement>
{
    protected override async Task HandleRequirementAsync(AuthorizationHandlerContext context,
        PermissionRequirement requirement)
    {
        var allowedRoleIds = await dbContext.Permissions
            .Include(p => p.Roles)
            .Where(p => p.Name == requirement.PermissionTypeName)
            .SelectMany(p => p.Roles.Select(r => r.IdentityRoleId))
            .ToListAsync();

        var claims = context.User.Claims
            .Where(c => c.Type == ClaimTypes.Role)
            .Select(c => c.Value)
            .ToList();

        foreach (var roleId in allowedRoleIds)
        {
            var role = await roleManager.GetRoleByIdAsync(roleId);
            if (role != null && claims.Contains(role))
            {
                context.Succeed(requirement);
            }
        }
    }
}
