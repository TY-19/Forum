using Forum.Application.Common.Interfaces.Identyty;
using Forum.Application.Common.Interfaces.Repositories;
using Forum.Application.Users.Dtos;
using MediatR;

namespace Forum.Application.Users.Commands.CreateUser;

public class CreateUserCommandHandler(IUserManager userManager,
    IUserRepository repository) : IRequestHandler<CreateUserCommand, UserDto?>
{
    public async Task<UserDto?> Handle(CreateUserCommand command, CancellationToken cancellationToken)
    {
        var response = await userManager.CreateUserAsync(command, cancellationToken);
        if (response.Payload != null && response.Payload.Id != null)
        {
            await repository.AddProfileToUserAsync(response.Payload, cancellationToken);
        }
        return await userManager.GetUserDtoByProfileIdAsync(response.Payload!.UserProfileId, cancellationToken);
    }
}
