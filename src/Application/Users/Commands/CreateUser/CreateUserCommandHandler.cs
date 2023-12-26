using Forum.Application.Common.Interfaces;
using Forum.Application.Common.Interfaces.Identyty;
using Forum.Application.Common.Interfaces.Repositories;
using MediatR;

namespace Forum.Application.Users.Commands.CreateUser;

public class CreateUserCommandHandler(IUserManager userManager,
    IUserRepository repository) : IRequestHandler<CreateUserCommand, IUser?>
{
    public async Task<IUser?> Handle(CreateUserCommand command, CancellationToken cancellationToken)
    {
        var user = await userManager.CreateUserAsync(command, cancellationToken);
        if (user != null && user.Id != null)
        {
            await repository.AddProfileToUserAsync(user, cancellationToken);
        }
        return user;
    }
}
