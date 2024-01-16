using Forum.Application.Common.Interfaces;
using Forum.Application.Permissions.Dtos;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Forum.Application.Permissions.Queries.GetPermission;

public class GetPermissionRequest : IRequest<PermissionGetDto?>
{
    public int Id { get; set; }
}

public class GetPermissionRequestHandler(IForumDbContext context,
    IRoleManager roleManager) : IRequestHandler<GetPermissionRequest, PermissionGetDto?>
{
    public async Task<PermissionGetDto?> Handle(GetPermissionRequest request, CancellationToken cancellationToken)
    {
        var permission = await context.Permissions
            .Include(p => p.Roles)
            .FirstOrDefaultAsync(p => p.Id == request.Id, cancellationToken);
        if (permission == null)
            return null;

        var roles = await roleManager.GetAllRoles().ToListAsync();
        List<string> rolesNames = roles
            .Where(r => permission.Roles.Any(pr => pr.IdentityRoleId == r.Id) && r.Name != null)
            .Select(r => r.Name!)
            .ToList();

        return new PermissionGetDto(permission, rolesNames);
    }
}
