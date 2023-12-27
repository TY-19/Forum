using Forum.Application.Users.Dtos;
using MediatR;

namespace Forum.Application.Users.Queries.GetAllUsers;

public class GetAllUsersRequest : IRequest<IEnumerable<UserDto>>
{
}
