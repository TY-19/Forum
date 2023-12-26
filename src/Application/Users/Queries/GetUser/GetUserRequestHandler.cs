using Forum.Application.Common.Interfaces;
using Forum.Application.Common.Interfaces.Identyty;
using MediatR;

namespace Forum.Application.Users.Queries.GetUser;

public class GetUserRequestHandler(IUserManager userManager) : IRequestHandler<GetUserRequest, IUser?>
{
    public async Task<IUser?> Handle(GetUserRequest request, CancellationToken cancellationToken)
    {
        return await userManager.GetUserByProfileIdAsync(request.UserProfileId, cancellationToken);
    }
}
