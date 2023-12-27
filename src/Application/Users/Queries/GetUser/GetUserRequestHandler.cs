using Forum.Application.Common.Interfaces.Identyty;
using Forum.Application.Users.Dtos;
using MediatR;

namespace Forum.Application.Users.Queries.GetUser;

public class GetUserRequestHandler(IUserManager userManager) : IRequestHandler<GetUserRequest, UserDto?>
{
    public async Task<UserDto?> Handle(GetUserRequest request, CancellationToken cancellationToken)
    {
        return await userManager.GetUserDtoByProfileIdAsync(request.UserProfileId, cancellationToken);
    }
}
