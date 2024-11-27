using Forum.Application.Common.Interfaces;
using Forum.Application.Common.Models;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Forum.Application.Users.Commands.ChangePassword;

public class ChangePasswordCommand : IRequest<CustomResponse>
{
    public string UserName { get; set; } = null!;
    public string CurrentPassword { get; set; } = null!;
    public string NewPassword { get; set; } = null!;
}

public class ChangePasswordCommandHandler(IUserManager userManager,
    ILogger<ChangePasswordCommandHandler> logger) : IRequestHandler<ChangePasswordCommand, CustomResponse>
{
    public async Task<CustomResponse> Handle(ChangePasswordCommand command, CancellationToken cancellationToken)
    {
        var user = await userManager.GetUserByNameAsync(command.UserName, cancellationToken);
        if (user == null)
            return new CustomResponse() { Succeeded = false, Message = "A user with such a name does not exist" };

        try
        {
            return await userManager.ChangePasswordAsync(user, command.CurrentPassword, command.NewPassword, cancellationToken);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "An error occurred while changing the password.");
            return new CustomResponse(ex);
        }
    }
}
