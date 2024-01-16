using Forum.Application.Common.Interfaces;
using Forum.Application.Common.Models;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Forum.Application.Users.Commands.UpdateUser;

public class UpdateUserCommand : IRequest<CustomResponse>
{
    public string UserId { get; set; } = null!;
    public string? UserName { get; set; }
    public string? UpdatedName { get; set; }
    public string? UpdatedEmail { get; set; }
}

public class UpdateUserCommandHandler(IUserManager userManager,
    ILogger<UpdateUserCommandHandler> logger) : IRequestHandler<UpdateUserCommand, CustomResponse>
{
    public async Task<CustomResponse> Handle(UpdateUserCommand command, CancellationToken cancellationToken)
    {
        var user = await GetUserAsync(command, cancellationToken);
        if (user == null)
            return new CustomResponse() { Succeeded = false, Message = "A user with such an id does not exist" };

        if (command.UpdatedName != null)
            user.UserName = command.UpdatedName;

        if (command.UpdatedEmail != null)
            user.Email = command.UpdatedEmail;

        try
        {
            return await userManager.UpdateUserAsync(user, cancellationToken);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "An error occurred while updating the user.");
            return new CustomResponse(ex);
        }
    }

    private async Task<IUser?> GetUserAsync(UpdateUserCommand command, CancellationToken cancellationToken)
    {
        if (command.UserId != null)
            return await userManager.GetUserByIdAsync(command.UserId, cancellationToken);

        if (command.UserName != null)
            return await userManager.GetUserByNameAsync(command.UserName, cancellationToken);

        return null;
    }
}
