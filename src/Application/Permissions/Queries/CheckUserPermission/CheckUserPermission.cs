using Forum.Application.Common.Interfaces;
using Forum.Application.Common.Models;
using Forum.Domain.Constants;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Forum.Application.Permissions.Queries.CheckUserPermission;

public class CheckUserPermissionRequest : IRequest<CustomResponse>
{
    public string PermissionName { get; set; } = null!;
    public string? UserId { get; set; }
    public string? UserName { get; set; }
    public int? ForumId { get; set; }
}

public class CheckUserPermissionRequestHandler(IForumDbContext context,
    IUserManager userManager,
    IRoleManager roleManager) : IRequestHandler<CheckUserPermissionRequest, CustomResponse>
{
    public async Task<CustomResponse> Handle(CheckUserPermissionRequest request, CancellationToken cancellationToken)
    {
        if (string.IsNullOrEmpty(request.PermissionName))
            return new CustomResponse() { Succeeded = false };

        var scopeIds = await GetAllowedScope(request.ForumId);
        var allowedRoleIds = await GetAllowedRoleIdsAsync(request.PermissionName, scopeIds, cancellationToken);
        var userRoles = await GetUserRolesAsync(request, cancellationToken);
        return await IsInAllowedRole(allowedRoleIds, userRoles, cancellationToken);
    }

    private async Task<List<int>> GetAllowedScope(int? forumId)
    {
        var forums = await context.Forums.Select(f => new { f.Id, f.ParentForumId })
            .ToDictionaryAsync(k => k.Id, e => e.ParentForumId);
        List<int> allowedScope = [];
        while (forumId != null)
        {
            allowedScope.Add((int)forumId);
            forumId = forums[(int)forumId];
        }
        return allowedScope;
    }

    private async Task<List<string>> GetAllowedRoleIdsAsync(string permissionName, List<int> scopeIds, CancellationToken cancellationToken)
    {
        return await context.Permissions
            .Include(p => p.Roles)
            .Where(p => p.Name != null && p.Name == permissionName &&
                (p.IsGlobal || p.ForumId != null && scopeIds.Contains((int)p.ForumId)))
            .SelectMany(p => p.Roles.Select(r => r.IdentityRoleId))
            .Distinct()
            .ToListAsync(cancellationToken);
    }

    private async Task<List<string>> GetUserRolesAsync(CheckUserPermissionRequest request, CancellationToken cancellationToken)
    {
        List<string> userRoles = [DefaultRoles.GUEST];
        var user = await GetUserAsync(request.UserId, request.UserName, cancellationToken);
        if (user != null)
            userRoles.AddRange(await userManager.GetRolesAsync(user, cancellationToken));

        return userRoles;
    }

    private async Task<IUser?> GetUserAsync(string? userId, string? userName, CancellationToken cancellationToken)
    {
        IUser? user = null;

        if (userId != null)
            user = await userManager.GetUserByIdAsync(userId, cancellationToken);
        else if (userName != null)
            user = await userManager.GetUserByNameAsync(userName, cancellationToken);

        return user;
    }

    private async Task<CustomResponse> IsInAllowedRole(List<string> allowedRoleIds, List<string> userRoles, CancellationToken cancellationToken)
    {
        var allRoles = await roleManager.GetAllRolesAsync(cancellationToken);
        var userRoleIds = allRoles.Where(r => r.Name != null && userRoles.Contains(r.Name))
            .Select(r => r.Id);

        return userRoleIds.Any(allowedRoleIds.Contains)
            ? new CustomResponse() { Succeeded = true }
            : new CustomResponse() { Succeeded = false };
    }
}