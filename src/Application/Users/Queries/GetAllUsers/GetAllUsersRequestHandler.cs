using Forum.Application.Common.Interfaces;
using Forum.Application.Common.Interfaces.Identyty;
using MediatR;

namespace Forum.Application.Users.Queries.GetAllUsers;

public class GetAllUsersRequestHandler(IUserManager userManager) : IRequestHandler<GetAllUsersRequest, IEnumerable<IUser>>
{
    public async Task<IEnumerable<IUser>> Handle(GetAllUsersRequest request, CancellationToken cancellationToken)
    {
        return await userManager.GetAllUsersAsync(cancellationToken);
    }
}
