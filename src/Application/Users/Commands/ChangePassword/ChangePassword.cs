using Forum.Application.Common.Interfaces;
using Forum.Application.Common.Models;
using MediatR;

namespace Forum.Application.Users.Commands.ChangePassword;

public class ChangePasswordCommand : IRequest<CustomResponse>
{
    public string UserId { get; set; } = null!;
    public string CurrentPassword { get; set; } = null!;
    public string NewPassword { get; set; } = null!;
}

public class ChangePasswordCommandHandler(IUserManager userManager) : IRequestHandler<ChangePasswordCommand, CustomResponse>
{
    public async Task<CustomResponse> Handle(ChangePasswordCommand command, CancellationToken cancellationToken)
    {
        var user = await userManager.GetUserByIdAsync(command.UserId, cancellationToken);
        if (user == null)
            return new CustomResponse() { Succeeded = false, Message = "A user with such an id does not exist" };

        return await userManager.ChangePasswordAsync(user, command.CurrentPassword, command.NewPassword);
    }
}
