using Forum.Application.Common.Interfaces;
using Forum.Application.Common.Models;
using Forum.Domain.Constants;
using MediatR;

namespace Forum.Application.Users.Commands.ChangeUserRoles;

public class ChangeUserRolesCommand : IRequest<CustomResponse>
{
    public string UserId { get; set; } = null!;
    public IEnumerable<string> Roles { get; set; } = Enumerable.Empty<string>();
}

public class ChangeUserRolesCommandHandler(IUserManager userManager, IRoleManager roleManager) : IRequestHandler<ChangeUserRolesCommand, CustomResponse>
{
    public async Task<CustomResponse> Handle(ChangeUserRolesCommand command, CancellationToken cancellationToken)
    {
        var user = await userManager.GetUserByIdAsync(command.UserId, cancellationToken);
        if (user == null)
            return new CustomResponse() { Succeeded = false, Message = "There are no user with such an id" };

        var currentRoles = await userManager.GetRolesAsync(user, cancellationToken);
        var addingRolesResult = await AddRolesAsync(user, command.Roles.Except(currentRoles), cancellationToken);
        var removingRolesResult = await RemoveRolesAsync(user, currentRoles.Except(command.Roles), cancellationToken);

        return new CustomResponse()
        {
            Succeeded = addingRolesResult.Succeeded && removingRolesResult.Succeeded,
            Message = addingRolesResult.Message + " \n" + removingRolesResult.Message
        };
    }

    private async Task<CustomResponse> AddRolesAsync(IUser user, IEnumerable<string> rolesToAdd, CancellationToken cancellationToken)
    {
        var response = new CustomResponse() { Succeeded = true };
        var allRoles = await roleManager.GetAllRolesAsync(cancellationToken);
        List<string> notRoles = [];
        foreach (var role in rolesToAdd)
        {
            if (!allRoles.Select(r => r.Name).Contains(role))
            {
                notRoles.Add(role);
                response.Succeeded = false;
                response.Message = $"The role {role} does not exist. ";
            }
        }
        rolesToAdd = rolesToAdd.Except(notRoles);

        var addingResult = await userManager.AddToRolesAsync(user, rolesToAdd, cancellationToken);
        if (!addingResult.Succeeded)
        {
            response.Succeeded = false;
            response.Message += addingResult.Message;
        }
        return response;
    }

    private async Task<CustomResponse> RemoveRolesAsync(IUser user, IEnumerable<string> rolesToRemove, CancellationToken cancellationToken)
    {
        var response = new CustomResponse { Succeeded = true };
        if (rolesToRemove.Contains(DefaultRoles.ADMIN) && await IsUserTheLastAdminAsync(user.Id!, cancellationToken))
        {
            rolesToRemove = rolesToRemove.Except(new string[] { DefaultRoles.ADMIN });
            response.Succeeded = false;
            response.Message += $"The user was not removed from the role {DefaultRoles.ADMIN} because they are the last admin";
        }
        var removingResult = await userManager.RemoveFromRolesAsync(user, rolesToRemove, cancellationToken);
        if (!removingResult.Succeeded)
        {
            response.Succeeded = false;
            response.Message += removingResult.Message;
        }
        return response;
    }

    private async Task<bool> IsUserTheLastAdminAsync(string userId, CancellationToken cancellationToken)
    {
        var admins = await userManager.GetUsersInRoleAsync(DefaultRoles.ADMIN, cancellationToken);
        return admins.Count() == 1 && admins.First().Id == userId;
    }
}
