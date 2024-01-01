using Forum.Application.Common.Interfaces;
using Forum.Application.Permissions.Dtos;
using Forum.Domain.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Forum.Application.Permissions.Queries.GetAllPermissions;

public class GetAllPermissionsRequest : IRequest<IEnumerable<PermissionGetDto>>
{
    public bool? FilterIsGlobal { get; set; }
    public int? FilterForumId { get; set; }
}

public class GetAllPermissionsRequestHandler(IForumDbContext context,
    IRoleManager roleManager) : IRequestHandler<GetAllPermissionsRequest, IEnumerable<PermissionGetDto>>
{
    public async Task<IEnumerable<PermissionGetDto>> Handle(GetAllPermissionsRequest request, CancellationToken cancellationToken)
    {
        var permissions = await FilterPermissions(request).ToListAsync(cancellationToken);
        List<PermissionGetDto> dtos = [];
        var allRoles = await roleManager.GetAllRolesAsync(cancellationToken);
        permissions.ForEach(p => dtos.Add(new PermissionGetDto(p, GetRoleNames(p, allRoles))));
        return dtos;
    }

    private IQueryable<Permission> FilterPermissions(GetAllPermissionsRequest request)
    {
        if (request.FilterForumId != null)
        {
            return context.Permissions.Where(p => p.ForumId == request.FilterForumId);
        }
        else if (request.FilterIsGlobal != null)
        {
            return context.Permissions.Where(p => p.IsGlobal == request.FilterIsGlobal);
        }
        else
        {
            return context.Permissions;
        }
    }

    private List<string> GetRoleNames(Permission permission, IEnumerable<IRole> allRoles)
    {
        List<string> permRoles = [];
        foreach (var role in permission.Roles)
        {
            string? name = allRoles.FirstOrDefault(r => r.Id == role.IdentityRoleId)?.Name;
            if (name != null)
                permRoles.Add(name);
        }
        return permRoles;
    }
}
