using Forum.Application.Common.Extensions;
using Forum.Application.Common.Interfaces;
using Forum.Application.Common.Models;
using Forum.Application.Users.Dtos;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Forum.Application.Users.Queries.GetAllUsers;

public class GetAllUsersRequest : IRequest<PaginatedResponse<UserDto>>
{
    public PageOptions? PageOptions { get; set; }
    public FilterOptions? FilterOptions { get; set; }
    public OrderOptions? OrderOptions { get; set; }
}

public class GetAllUsersRequestHandler(IUserManager userManager) : IRequestHandler<GetAllUsersRequest, PaginatedResponse<UserDto>>
{
    public async Task<PaginatedResponse<UserDto>> Handle(GetAllUsersRequest request, CancellationToken cancellationToken)
    {
        var users = userManager.GetAllUsers();
        FilterUsers(ref users, request.FilterOptions);
        OrderUsers(users, request.OrderOptions);
        return await GetPaginatedResponseAsync(users, request.PageOptions, cancellationToken);
    }

    private const int DefaultPageSize = 20;
    private const int MaxPageSize = 1000;

    private static Dictionary<string, string[]> PropertyNames => new()
    {
        { "id", ["id", "userId"] },
        { "name", ["name", "userName"] },
        { "email", ["email", "userEmail"] },
        { "profileId", ["profile", "profileId"] }
    };

    [System.Diagnostics.CodeAnalysis.SuppressMessage("Performance", "CA1862:Use the 'StringComparison' method overloads to perform case-insensitive string comparisons",
        Justification = "https://github.com/dotnet/efcore/issues/20995#issuecomment-631358780 EF Core does not translate the overload that accepts StringComparison.InvariantCultureIgnoreCase (or any other StringComparison).")]
    private static void FilterUsers(ref IQueryable<IUser> users, FilterOptions? filterOptions)
    {
        if (filterOptions == null || filterOptions.FilterText == null)
            return;

        if (filterOptions.FilterBy == null)
            users = users.Where(u => (u.UserName != null && u.UserName.ToLower().Contains(filterOptions.FilterText.ToLower()))
                || (u.Email != null && u.Email.Contains(filterOptions.FilterText)));

        else if (filterOptions.FilterBy.CheckStringEquality(PropertyNames["name"]))
            users = users.Where(u => u.UserName != null && u.UserName.ToLower().Contains(filterOptions.FilterText.ToLower()));

        else if (filterOptions.FilterBy.CheckStringEquality(PropertyNames["email"]))
            users = users.Where(u => u.Email != null && u.Email.ToLower().Contains(filterOptions.FilterText.ToLower()));
    }

    private static void OrderUsers(IQueryable<IUser> users, OrderOptions? orderOptions)
    {
        if (orderOptions == null || orderOptions.OrderBy == null)
            return;

        if (orderOptions.OrderBy.CheckStringEquality(PropertyNames["id"]))
        {
            users = orderOptions.OrderAscending == null || orderOptions.OrderAscending == true
                ? users.OrderBy(u => u.Id)
                : users.OrderByDescending(u => u.Id);
        }

        if (orderOptions.OrderBy.CheckStringEquality(PropertyNames["name"]))
        {
            users = orderOptions.OrderAscending == null || orderOptions.OrderAscending == true
                ? users.OrderBy(u => u.UserName)
                : users.OrderByDescending(u => u.UserName);
        }

        if (orderOptions.OrderBy.CheckStringEquality(PropertyNames["email"]))
        {
            users = orderOptions.OrderAscending == null || orderOptions.OrderAscending == true
                ? users.OrderBy(u => u.Email)
                : users.OrderByDescending(u => u.Email);
        }

        if (orderOptions.OrderBy.CheckStringEquality(PropertyNames["profileId"]))
        {
            users = orderOptions.OrderAscending == null || orderOptions.OrderAscending == true
                ? users.OrderBy(u => u.UserProfile.Id)
                : users.OrderByDescending(u => u.UserProfile.Id);
        }
    }

    private async Task<PaginatedResponse<UserDto>> GetPaginatedResponseAsync(IQueryable<IUser> users,
        PageOptions? pageOptions, CancellationToken cancellationToken)
    {
        var response = new PaginatedResponse<UserDto>()
        {
            PageNumber = pageOptions?.PageNumber ?? 1,
            PageSize = GetPageSize(pageOptions?.PageSize)
        };
        response.TotalPagesCount = await GetTotalPagesCountAsync(users, response.PageSize, cancellationToken);

        users = users.Skip((response.PageNumber - 1) * response.PageSize).Take(response.PageSize);

        response.Elements = await GetUserDtosAsync(users, cancellationToken);

        return response;
    }

    private static int GetPageSize(int? pageSize)
    {
        if (pageSize == null)
            return DefaultPageSize;

        return pageSize.Value < MaxPageSize ? pageSize.Value : MaxPageSize;
    }

    private static async Task<int> GetTotalPagesCountAsync(IQueryable<IUser> users, int pageSize, CancellationToken cancellationToken)
    {
        var count = await users.CountAsync(cancellationToken);
        var totalPages = count / pageSize;
        return totalPages * pageSize == count ? totalPages : totalPages + 1;
    }

    private async Task<IEnumerable<UserDto>> GetUserDtosAsync(IQueryable<IUser> users, CancellationToken cancellationToken)
    {
        List<UserDto> userDtos = [];
        foreach (var user in await users.ToListAsync(cancellationToken))
        {
            userDtos.Add(new UserDto()
            {
                Id = user.Id,
                Email = user.Email,
                UserName = user.UserName,
                UserProfileId = user.UserProfile?.Id,
                Roles = await userManager.GetRolesAsync(user, cancellationToken)
            });
        }
        return userDtos;
    }
}
