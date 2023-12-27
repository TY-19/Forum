using Forum.Application.Common.Models;
using MediatR;

namespace Forum.Application.Users.Commands.DeleteUser;

public class DeleteUserCommand : IRequest<CustomResponse>
{
    public string UserId { get; set; } = null!;
}
