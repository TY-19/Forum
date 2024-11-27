using Forum.Application.Common.Interfaces;
using Forum.Application.Common.Models;
using Forum.Domain.Constants;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Forum.Application.Users.Commands.ChangeUserRoles;

public class ChangeUserRolesCommand : IRequest<CustomResponse>
{
    public string UserId { get; set; } = null!;
    public IEnumerable<string> Roles { get; set; } = [];
}

public class ChangeUserRolesCommandHandler(IUserManager userManager,
    IRoleManager roleManager,
    ILogger<ChangeUserRolesCommandHandler> logger) : IRequestHandler<ChangeUserRolesCommand, CustomResponse>
{
    public async Task<CustomResponse> Handle(ChangeUserRolesCommand command, CancellationToken cancellationToken)
    {
        var user = await userManager.GetUserByIdAsync(command.UserId, cancellationToken);
        if (user == null)
            return new CustomResponse() { Succeeded = false, Message = "There are no user with such an id" };

        try
        {
            var currentRoles = await userManager.GetRolesAsync(user, cancellationToken);
            var addingRolesResult = await AddRolesAsync(user, command.Roles.Except(currentRoles), cancellationToken);
            var removingRolesResult = await RemoveRolesAsync(user, currentRoles.Except(command.Roles), cancellationToken);

            return new CustomResponse()
            {
                Succeeded = addingRolesResult.Succeeded && removingRolesResult.Succeeded,
                Message = addingRolesResult.Errors.Any() || removingRolesResult.Errors.Any()
                    ? "Some roles was not added or removed."
                    : null,
                Errors = [..addingRolesResult.Errors, ..removingRolesResult.Errors]
            };
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "An error occurred while changing the user roles.");
            return new CustomResponse(ex);
        }
    }

    private async Task<CustomResponse> AddRolesAsync(IUser user, IEnumerable<string> rolesToAdd, CancellationToken cancellationToken)
    {
        bool succeeded = true;
        List<string> errors = [];
        var allRoles = await roleManager.GetAllRolesAsync(cancellationToken);
        List<string> notRoles = [];
        foreach (var role in rolesToAdd)
        {
            if (!allRoles.Select(r => r.Name).Contains(role))
            {
                notRoles.Add(role);
                succeeded = false;
                errors.Add($"The role {role} does not exist. ");
            }
        }
        rolesToAdd = rolesToAdd.Except(notRoles);

        var addingResult = await userManager.AddToRolesAsync(user, rolesToAdd, cancellationToken);
        if (!addingResult.Succeeded)
        {
            succeeded = false;
            errors.Add(addingResult.Message ?? string.Empty);
        }
        return new CustomResponse() { Succeeded = succeeded, Errors = errors };;
    }

    private async Task<CustomResponse> RemoveRolesAsync(IUser user, IEnumerable<string> rolesToRemove, CancellationToken cancellationToken)
    {
        bool succeeded = true;
        List<string> errors = [];
        if (rolesToRemove.Contains(DefaultRoles.ADMIN) && await IsUserTheLastAdminAsync(user.Id!, cancellationToken))
        {
            rolesToRemove = rolesToRemove.Except(new string[] { DefaultRoles.ADMIN });
            succeeded = false;
            errors.Add($"The user was not removed from the role {DefaultRoles.ADMIN} because they are the last admin");
        }
        var removingResult = await userManager.RemoveFromRolesAsync(user, rolesToRemove, cancellationToken);
        if (!removingResult.Succeeded)
        {
            succeeded = false;
            errors.Add(removingResult.Message ?? string.Empty);
        }
        return new CustomResponse { Succeeded = succeeded, Errors = errors };
    }

    private async Task<bool> IsUserTheLastAdminAsync(string userId, CancellationToken cancellationToken)
    {
        var admins = await userManager.GetUsersInRoleAsync(DefaultRoles.ADMIN, cancellationToken);
        return admins.Count() == 1 && admins.First().Id == userId;
    }
}
