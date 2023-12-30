using Forum.Application.Common.Interfaces;
using Forum.Application.Users.Dtos;
using MediatR;

namespace Forum.Application.Users.Queries.GetUser;

public class GetUserRequest : IRequest<UserDto?>
{
    public int UserProfileId { get; set; }
}

public class GetUserRequestHandler(IUserManager userManager) : IRequestHandler<GetUserRequest, UserDto?>
{
    public async Task<UserDto?> Handle(GetUserRequest request, CancellationToken cancellationToken)
    {
        var user = await userManager.GetUserByProfileIdAsync(request.UserProfileId, cancellationToken);
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
}
