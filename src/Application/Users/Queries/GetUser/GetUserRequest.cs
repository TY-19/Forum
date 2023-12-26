using Forum.Application.Common.Interfaces;
using MediatR;

namespace Forum.Application.Users.Queries.GetUser;

public class GetUserRequest : IRequest<IUser?>
{
    public int UserProfileId { get; set; }
}
