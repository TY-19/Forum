using Forum.Application.Common.Interfaces;
using MediatR;

namespace Forum.Application.Users.Queries.GetAllUsers;

public class GetAllUsersRequest : IRequest<IEnumerable<IUser>>
{
}
