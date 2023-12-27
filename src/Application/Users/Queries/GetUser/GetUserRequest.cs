using Forum.Application.Users.Dtos;
using MediatR;

namespace Forum.Application.Users.Queries.GetUser;

public class GetUserRequest : IRequest<UserDto?>
{
    public int UserProfileId { get; set; }
}
