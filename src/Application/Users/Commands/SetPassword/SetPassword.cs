using Forum.Application.Common.Interfaces;
using Forum.Application.Common.Models;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Forum.Application.Users.Commands.SetPassword;

public class SetPasswordCommand : IRequest<CustomResponse>
{
    public string UserId { get; set; } = null!;
    public string NewPassword { get; set; } = null!;
}

public class SetPasswordCommandHandler(IUserManager userManager,
    ILogger<SetPasswordCommandHandler> logger) : IRequestHandler<SetPasswordCommand, CustomResponse>
{
    public async Task<CustomResponse> Handle(SetPasswordCommand command, CancellationToken cancellationToken)
    {
        var user = await userManager.GetUserByIdAsync(command.UserId, cancellationToken);
        if (user == null)
            return new CustomResponse() { Succeeded = false, Message = "A user with such an id does not exist" };

        try
        {
            return await userManager.SetPasswordAsync(user, command.NewPassword, cancellationToken);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "An error occurred while setting the password.");
            return new CustomResponse(ex);
        }
    }
}
