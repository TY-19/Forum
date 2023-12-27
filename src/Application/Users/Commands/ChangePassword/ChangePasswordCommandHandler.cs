using Forum.Application.Common.Interfaces.Identyty;
using Forum.Application.Common.Models;
using MediatR;

namespace Forum.Application.Users.Commands.ChangePassword;

public class ChangePasswordCommandHandler(IUserManager userManager) : IRequestHandler<ChangePasswordCommand, CustomResponse>
{
    public async Task<CustomResponse> Handle(ChangePasswordCommand command, CancellationToken cancellationToken)
    {
        return await userManager.ChangeUserPasswordAsync(command);
    }
}
