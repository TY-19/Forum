using Forum.Application.Users.Dtos;
using MediatR;

namespace Forum.Application.Users.Commands.CreateUser;

public class CreateUserCommand : IRequest<UserDto?>
{
    public string UserName { get; set; } = null!;
    public string UserEmail { get; set; } = null!;
    public string Password { get; set; } = null!;
}
