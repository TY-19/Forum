using Forum.Application.Common.Interfaces;
using Forum.Application.Common.Models;
using MediatR;

namespace Forum.Application.Users.Commands.UpdateUser;

public class UpdateUserCommand : IRequest<CustomResponse>
{
    public string UserId { get; set; } = null!;
    public string? UpdatedName { get; set; }
    public string? UpdatedEmail { get; set; }
}

public class UpdateUserCommandHandler(IUserManager userManager) : IRequestHandler<UpdateUserCommand, CustomResponse>
{
    public async Task<CustomResponse> Handle(UpdateUserCommand command, CancellationToken cancellationToken)
    {
        var user = await userManager.GetUserByIdAsync(command.UserId, cancellationToken);
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
            return new CustomResponse(ex);
        }
    }
}
