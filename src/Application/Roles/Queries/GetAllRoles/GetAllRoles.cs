using Forum.Application.Common.Interfaces;
using Forum.Application.Roles.Dtos;
using MediatR;

namespace Forum.Application.Roles.Queries.GetAllRoles;

public class GetAllRolesRequest : IRequest<IEnumerable<RoleDto>>
{
}

public class GetAllRolesRequestHandler(IRoleManager roleManager) : IRequestHandler<GetAllRolesRequest, IEnumerable<RoleDto>>
{
    public async Task<IEnumerable<RoleDto>> Handle(GetAllRolesRequest request, CancellationToken cancellationToken)
    {
        var roles = await roleManager.GetAllRolesAsync(cancellationToken);
        return roles
            .Where(r => r.Name != null)
            .Select(r => new RoleDto() { RoleName = r.Name! })
            .ToList();
    }
}
