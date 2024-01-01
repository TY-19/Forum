using Forum.Application.Common.Interfaces;
using Forum.Application.Common.Models;
using Forum.Domain.Constants;
using MediatR;

namespace Forum.Application.Users.Commands.DeleteUser;

public class DeleteUserCommand : IRequest<CustomResponse>
{
    public string UserId { get; set; } = null!;
}

public class DeleteUserCommandHandler(IUserManager userManager) : IRequestHandler<DeleteUserCommand, CustomResponse>
{
    public async Task<CustomResponse> Handle(DeleteUserCommand command, CancellationToken cancellationToken)
    {
        var user = await userManager.GetUserByIdAsync(command.UserId, cancellationToken);

        if (user == null)
            return new CustomResponse() { Succeeded = true, Message = "A user with such an id hasn't exist already" };

        if (await IsTheLastAdmin(command.UserId, cancellationToken))
            return new CustomResponse() { Succeeded = false, Message = "The last administrator cannot be deleted" };

        try
        {
            return await userManager.DeleteUserAsync(user, cancellationToken);
        }
        catch (Exception ex)
        {
            return new CustomResponse(ex);
        }
    }

    private async Task<bool> IsTheLastAdmin(string userId, CancellationToken cancellationToken)
    {
        var admins = await userManager.GetUsersInRoleAsync(DefaultRoles.ADMIN, cancellationToken);
        return admins.Count() == 1 && admins.First().Id == userId;
    }
}
