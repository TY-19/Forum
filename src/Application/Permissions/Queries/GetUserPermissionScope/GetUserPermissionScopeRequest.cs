using Forum.Application.Common.Interfaces;
using Forum.Application.Permissions.Dtos;
using Forum.Domain.Constants;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Forum.Application.Permissions.Queries.GetUserPermissionScope;

public class GetUserPermissionScopeRequest : IRequest<PermissionScopeDto>
{
    public string PermissionName { get; set; } = null!;
    public string? UserId { get; set; }
    public string? UserName { get; set; }
}

public class GetUserPermissionScopeRequestHandler(IForumDbContext context, IUserManager userManager,
    IRoleManager roleManager) : IRequestHandler<GetUserPermissionScopeRequest, PermissionScopeDto>
{
    public async Task<PermissionScopeDto> Handle(GetUserPermissionScopeRequest request, CancellationToken cancellationToken)
    {
        var userAppRoles = await GetUserRolesAsync(request.UserId, request.UserName, cancellationToken);
        var roles = await roleManager.GetAllRolesAsync(cancellationToken);
        var appRoles = roles
            .Where(r => r.Name != null && userAppRoles.Contains(r.Name))
            .Select(r => r.Id)
            .ToList();

        var permissions = await context.Permissions
            .Where(p => p.Name == request.PermissionName
                && p.Roles.Any(r => appRoles.Contains(r.IdentityRoleId)))
            .ToListAsync(cancellationToken);

        List<int> scope = permissions
            .Where(r => r.ForumId != null)
            .Select(r => r.ForumId!.Value)
            .Distinct()
            .ToList();

        List<int> subScope = [];
        subScope.AddRange(scope);
        while (subScope.Count != 0)
        {
            subScope = [.. context.Forums.Where(f => subScope.Contains(f.Id) && f.Subforums != null && f.Subforums.Any())
                                .SelectMany(f => f.Subforums.Select(s => s.Id))];
            scope.AddRange(subScope);
        }

        return new PermissionScopeDto()
        {
            Name = request.PermissionName,
            Description = permissions[0]?.Description,
            IsGlobal = permissions.Exists(p => p.IsGlobal),
            ForumIds = scope
        };
    }

    private async Task<List<string>> GetUserRolesAsync(string? userId, string? userName, CancellationToken cancellationToken)
    {
        List<string> userRoles = [DefaultRoles.GUEST];
        var user = await GetUserAsync(userId, userName, cancellationToken);
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
}
