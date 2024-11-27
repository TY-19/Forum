using Forum.Application.Common.Interfaces;
using Forum.Application.Users.Dtos;
using MediatR;

namespace Forum.Application.Users.Queries.GetUser;

public class GetUserRequest : IRequest<UserDto?>
{
    public string? UserId { get; set; }
    public string? UserName { get; set; }
}

public class GetUserRequestHandler(IUserManager userManager) : IRequestHandler<GetUserRequest, UserDto?>
{
    public async Task<UserDto?> Handle(GetUserRequest request, CancellationToken cancellationToken)
    {
        var user = await GetUserAsync(request, cancellationToken);
        if (user == null)
            return null;

        var roles = await userManager.GetRolesAsync(user, cancellationToken);
        return new UserDto()
        {
            Id = user.Id,
            Email = user.Email,
            UserName = user.UserName,
            UserProfileId = user.UserProfile.Id,
            Roles = roles
        };
    }

    private async Task<IUser?> GetUserAsync(GetUserRequest request, CancellationToken cancellationToken)
    {
        if (request.UserId != null)
            return await userManager.GetUserByIdAsync(request.UserId, cancellationToken);

        if (request.UserName != null)
            return await userManager.GetUserByNameAsync(request.UserName, cancellationToken);

        return null;
    }
}
