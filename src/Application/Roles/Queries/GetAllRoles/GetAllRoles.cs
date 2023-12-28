using Forum.Application.Common.Interfaces;
using MediatR;

namespace Forum.Application.Roles.Queries.GetAllRoles;

public class GetAllRolesRequest : IRequest<IEnumerable<string?>>
{
}

public class GetAllRolesRequestHandler(IRoleManager roleManager) : IRequestHandler<GetAllRolesRequest, IEnumerable<string?>>
{
    public async Task<IEnumerable<string?>> Handle(GetAllRolesRequest request, CancellationToken cancellationToken)
    {
        return await roleManager.GetAllRoles();
    }
}
