using Forum.Application.Common.Extensions;
using Forum.Application.Common.Interfaces;
using Forum.Application.Common.Models;
using Forum.Application.Permissions.Dtos;
using Forum.Domain.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System.Data;

namespace Forum.Application.Permissions.Queries.GetAllPermissions;

public class GetAllPermissionsRequest : IRequest<PaginatedResponse<PermissionGetDto>>
{
    public bool? FilterIsGlobal { get; set; }
    public int? FilterForumId { get; set; }
    public RequestParameters RequestParameters { get; set; } = new();
}

public class GetAllPermissionsRequestHandler(IForumDbContext context,
    IRoleManager roleManager) : IRequestHandler<GetAllPermissionsRequest, PaginatedResponse<PermissionGetDto>>
{
    public async Task<PaginatedResponse<PermissionGetDto>> Handle(GetAllPermissionsRequest request, CancellationToken cancellationToken)
    {
        IQueryable<Permission> permissions = context.Permissions;
        permissions = FilterPermissions(permissions, request);
        permissions = OrderPermissions(permissions, request.RequestParameters);

        request.RequestParameters.SetPageOptions(defaultPageSize, maxPageSize, out int pageSize, out int pageNumber);
        int pagesCount = (int)Math.Ceiling(await permissions.CountAsync(cancellationToken) / (double)pageSize);

        permissions = permissions.Skip(pageSize * (pageNumber - 1)).Take(pageSize);

        List<PermissionGetDto> dtos = [];
        var allRoles = await roleManager.GetAllRolesAsync(cancellationToken);
        (await permissions.ToListAsync(cancellationToken))
            .ForEach(p => dtos.Add(new PermissionGetDto(p, GetRoleNames(p, allRoles))));

        return new PaginatedResponse<PermissionGetDto>()
        {
            PageNumber = pageNumber,
            PageSize = pageSize,
            TotalPagesCount = pagesCount,
            Elements = dtos
        };
    }

    private const int defaultPageSize = 10;
    private const int maxPageSize = 100;

    [System.Diagnostics.CodeAnalysis.SuppressMessage("Performance", "CA1862:Use the 'StringComparison' method overloads to perform case-insensitive string comparisons",
        Justification = "https://github.com/dotnet/efcore/issues/20995#issuecomment-631358780 EF Core does not translate the overload that accepts StringComparison.InvariantCultureIgnoreCase (or any other StringComparison).")]
    private static IQueryable<Permission> FilterPermissions(IQueryable<Permission> permissions, GetAllPermissionsRequest request)
    {
        if (request.RequestParameters.FilterText != null)
        {
            permissions = permissions.Where(p => p.Name.ToLower().Contains(request.RequestParameters.FilterText.ToLower()));
        }

        if (request.FilterForumId != null)
        {
            permissions = permissions.Where(p => p.ForumId == request.FilterForumId);
        }
        else if (request.FilterIsGlobal != null)
        {
            permissions = permissions.Where(p => p.IsGlobal == request.FilterIsGlobal);
        }

        return permissions;
    }

    private static IQueryable<Permission> OrderPermissions(IQueryable<Permission> permissions, RequestParameters requestParameters)
        => requestParameters.OrderAscending switch
        {
            true => permissions.OrderBy(p => p.Name),
            false => permissions.OrderByDescending(p => p.Name),
            _ => permissions
        };

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
