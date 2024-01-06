using Forum.Application.Common.Extensions;
using Forum.Application.Common.Interfaces;
using Forum.Application.Common.Models;
using Forum.Application.Roles.Dtos;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Forum.Application.Roles.Queries.GetAllRoles;

public class GetAllRolesRequest : IRequest<PaginatedResponse<RoleDto>>
{
    public RequestParameters RequestParameters { get; set; } = new();
}

public class GetAllRolesRequestHandler(IRoleManager roleManager) : IRequestHandler<GetAllRolesRequest, PaginatedResponse<RoleDto>>
{
    public async Task<PaginatedResponse<RoleDto>> Handle(GetAllRolesRequest request, CancellationToken cancellationToken)
    {
        request.RequestParameters.SetPageOptions(defaultPageSize, maxPageSize, out int pageSize, out int pageNumber);

        var roles = roleManager.GetAllRoles()
            .Where(r => r.Name != null)
            .Select(r => new RoleDto() { RoleName = r.Name! });
        roles = OrderRoles(roles, request.RequestParameters);
        return new PaginatedResponse<RoleDto>()
        {
            PageNumber = pageNumber,
            PageSize = pageSize,
            TotalPagesCount = await roles.CountAsync(cancellationToken),
            Elements = await roles
                .Skip(pageSize * (pageNumber - 1))
                .Take(pageSize)
                .ToListAsync(cancellationToken)
        };
    }

    private const int defaultPageSize = 10;
    private const int maxPageSize = 100;

    private static IQueryable<RoleDto> OrderRoles(IQueryable<RoleDto> roles, RequestParameters parameters)
    {
        return parameters.OrderAscending switch
        {
            true => roles.OrderBy(r => r.RoleName),
            false => roles.OrderByDescending(r => r.RoleName),
            _ => roles
        };
    }
}
