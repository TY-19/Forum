using Forum.Application.Common.Interfaces.Identyty;
using Forum.Application.Users.Dtos;
using MediatR;

namespace Forum.Application.Users.Queries.GetAllUsers;

public class GetAllUsersRequestHandler(IUserManager userManager) : IRequestHandler<GetAllUsersRequest, IEnumerable<UserDto>>
{
    public async Task<IEnumerable<UserDto>> Handle(GetAllUsersRequest request, CancellationToken cancellationToken)
    {
        return await userManager.GetAllUsersDtoAsync(cancellationToken);
    }
}
