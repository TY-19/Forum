using Forum.Application.Common.Interfaces;
using MediatR;

namespace Forum.Application.Users.Commands.CreateUser;

public class CreateUserCommand : IRequest<IUser?>
{
    public string UserName { get; set; } = null!;
    public string UserEmail { get; set; } = null!;
    public string Password { get; set; } = null!;
}
