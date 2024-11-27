using Forum.Application.Common.Extensions;
using Forum.Application.Common.Interfaces;
using Forum.Application.Common.Models;
using Forum.Application.Users.Dtos;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Forum.Application.Users.Queries.GetAllUsers;

public class GetAllUsersRequest : IRequest<PaginatedResponse<UserDto>>
{
    public RequestParameters RequestParameters { get; set; } = new();
}

public class GetAllUsersRequestHandler(IUserManager userManager) : IRequestHandler<GetAllUsersRequest, PaginatedResponse<UserDto>>
{
    public async Task<PaginatedResponse<UserDto>> Handle(GetAllUsersRequest request, CancellationToken cancellationToken)
    {
        IQueryable<UserWithRoles> usersWithRoles = userManager.GetAllUsersWithRoles();
        Filter(ref usersWithRoles, request.RequestParameters);
        Order(ref usersWithRoles, request.RequestParameters);
        return await GetPaginatedResponseAsync(usersWithRoles, request.RequestParameters, cancellationToken);
    }

    private const int defaultPageSize = 20;
    private const int maxPageSize = 1000;

    private static Dictionary<string, string[]> PropertyNames => new()
    {
        { "id", ["id", "userId"] },
        { "name", ["name", "userName"] },
        { "email", ["email", "userEmail"] },
        { "profileId", ["profile", "profileId"] }
    };

    [System.Diagnostics.CodeAnalysis.SuppressMessage("Performance", "CA1862:Use the 'StringComparison' method overloads to perform case-insensitive string comparisons",
       Justification = "https://github.com/dotnet/efcore/issues/20995#issuecomment-631358780 EF Core does not translate the overload that accepts StringComparison.InvariantCultureIgnoreCase (or any other StringComparison).")]
    private static void Filter(ref IQueryable<UserWithRoles> users, RequestParameters requestParameters)
    {
        if (requestParameters.FilterText == null)
            return;

        if (requestParameters.FilterBy == null)
            users = users.Where(u => (u.UserName != null && u.UserName.ToLower().Contains(requestParameters.FilterText.ToLower()))
                || (u.Email != null && u.Email.Contains(requestParameters.FilterText)));

        else if (requestParameters.FilterBy.CheckStringEquality(PropertyNames["name"]))
            users = users.Where(u => u.UserName != null && u.UserName.ToLower().Contains(requestParameters.FilterText.ToLower()));

        else if (requestParameters.FilterBy.CheckStringEquality(PropertyNames["email"]))
            users = users.Where(u => u.Email != null && u.Email.ToLower().Contains(requestParameters.FilterText.ToLower()));
    }

    private static void Order(ref IQueryable<UserWithRoles> users, RequestParameters requestParameters)
    {
        if (requestParameters.OrderBy == null)
        {
            users = users.OrderBy(u => u.Id);
            return;
        }

        if (requestParameters.OrderBy.CheckStringEquality(PropertyNames["id"]))
        {
            users = requestParameters.OrderAscending == null || requestParameters.OrderAscending == true
                ? users.OrderBy(u => u.Id)
                : users.OrderByDescending(u => u.Id);
        }
        else if (requestParameters.OrderBy.CheckStringEquality(PropertyNames["name"]))
        {
            users = requestParameters.OrderAscending == null || requestParameters.OrderAscending == true
                ? users.OrderBy(u => u.UserName)
                : users.OrderByDescending(u => u.UserName);
        }
        else if (requestParameters.OrderBy.CheckStringEquality(PropertyNames["email"]))
        {
            users = requestParameters.OrderAscending == null || requestParameters.OrderAscending == true
                ? users.OrderBy(u => u.Email)
                : users.OrderByDescending(u => u.Email);
        }
        else if (requestParameters.OrderBy.CheckStringEquality(PropertyNames["profileId"]))
        {
            users = requestParameters.OrderAscending == null || requestParameters.OrderAscending == true
                ? users.OrderBy(u => u.UserProfileId)
                : users.OrderByDescending(u => u.UserProfileId);
        }
    }

    private static async Task<PaginatedResponse<UserDto>> GetPaginatedResponseAsync(IQueryable<UserWithRoles> users,
        RequestParameters requestParameters, CancellationToken cancellationToken)
    {
        requestParameters.SetPageOptions(defaultPageSize, maxPageSize, out int pageSize, out int pageNumber);
        var response = new PaginatedResponse<UserDto>
        {
            PageNumber = pageNumber,
            PageSize = pageSize,
            TotalPagesCount = (int)Math.Ceiling(await users.CountAsync(cancellationToken) / (double)pageSize)
        };

        users = users.Skip((pageNumber - 1) * pageSize).Take(pageSize);

        var uwr = await users.ToListAsync(cancellationToken);
        response.Elements = uwr.Select(u => new UserDto()
        {
            Id = u.Id,
            UserName = u.UserName,
            UserProfileId = u.UserProfileId,
            Email = u.Email,
            Roles = u.Roles?.Split(",").ToList() ?? []
        }).ToList();

        return response;
    }
}
