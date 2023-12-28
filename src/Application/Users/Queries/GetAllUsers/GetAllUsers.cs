using Forum.Application.Common.Interfaces;
using Forum.Application.Users.Dtos;
using MediatR;

namespace Forum.Application.Users.Queries.GetAllUsers;

public class GetAllUsersRequest : IRequest<IEnumerable<UserDto>>
{
}

public class GetAllUsersRequestHandler(IUserManager userManager) : IRequestHandler<GetAllUsersRequest, IEnumerable<UserDto>>
{
    public async Task<IEnumerable<UserDto>> Handle(GetAllUsersRequest request, CancellationToken cancellationToken)
    {
        var users = await userManager.GetAllUsersAsync(cancellationToken);
        List<UserDto> userDtos = [];
        foreach (var user in users)
        {
            userDtos.Add(new UserDto()
            {
                Id = user.Id,
                Email = user.Email,
                UserName = user.UserName,
                UserProfileId = user.UserProfileId,
                Roles = await userManager.GetRolesAsync(user, cancellationToken)
            });
        }
        return userDtos;
    }
}
