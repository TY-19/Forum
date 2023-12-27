using Forum.Application.Common.Interfaces.Identyty;
using Forum.Application.Common.Models;
using Forum.Domain.Common;
using MediatR;

namespace Forum.Application.Users.Commands.DeleteUser;

public class DeleteUserCommandHandler(IUserManager userManager) : IRequestHandler<DeleteUserCommand, CustomResponse>
{
    public async Task<CustomResponse> Handle(DeleteUserCommand command, CancellationToken cancellationToken)
    {
        if (await IsLastAdmin(command.UserId, cancellationToken))
            return new CustomResponse() { Succeed = false, Message = "The last administrator cannot be deleted" };

        await userManager.DeleteUserAsync(command.UserId, cancellationToken);
        return new CustomResponse() { Succeed = true };
    }

    private async Task<bool> IsLastAdmin(string userId, CancellationToken cancellationToken)
    {
        var admins = await userManager.GetUsersInRoleAsync(DefaultRoles.ADMIN, cancellationToken);
        return admins.Count() == 1 && admins.First().Id == userId;
    }
}
