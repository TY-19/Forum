using Forum.Application.Common.Interfaces;
using Forum.Application.Common.Interfaces.Identyty;
using Forum.Application.Common.Models;
using Forum.Application.Users.Commands.ChangePassword;
using Forum.Application.Users.Commands.CreateUser;
using Forum.Application.Users.Commands.UpdateUser;
using Forum.Application.Users.Dtos;
using Forum.Domain.Common;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace Forum.Infrastructure.Identity;

public class IdentityUserManager(UserManager<User> userManager) : IUserManager
{
    public async Task<CustomResponse<IUser>> CreateUserAsync(CreateUserCommand command, CancellationToken cancellationToken)
    {
        if (await userManager.FindByNameAsync(command.UserName) != null)
            return new CustomResponse<IUser> { Succeed = false, Message = "User with such a name already exist" };

        var user = new User()
        {
            UserName = command.UserName,
            Email = command.UserEmail,
        };
        await userManager.CreateAsync(user, command.Password);
        await userManager.AddToRoleAsync(user, DefaultRoles.USER);
        return new CustomResponse<IUser> { Succeed = true, Payload = user };
    }

    public async Task<IEnumerable<IUser>> GetAllUsersAsync(CancellationToken cancellationToken)
    {
        return await userManager.Users
            .Include(u => u.UserProfile)
            .ToListAsync(cancellationToken);
    }

    public async Task<IEnumerable<UserDto>> GetAllUsersDtoAsync(CancellationToken cancellationToken)
    {
        List<UserDto> userDtos = [];
        foreach (var user in await userManager.Users.Include(u => u.UserProfile).ToListAsync(cancellationToken))
        {
            userDtos.Add(new UserDto()
            {
                Id = user.Id,
                Email = user.Email,
                UserName = user.UserName,
                UserProfileId = user.UserProfileId,
                Roles = await userManager.GetRolesAsync(user)
            });
        }
        return userDtos;
    }
    public async Task<IUser?> GetUserByProfileIdAsync(int userProfileId, CancellationToken cancellationToken)
    {
        return await userManager.Users
            .Where(u => u.UserProfileId == userProfileId)
            .Include(u => u.UserProfile)
            .FirstOrDefaultAsync(cancellationToken);
    }

    public async Task<UserDto?> GetUserDtoByProfileIdAsync(int userProfileId, CancellationToken cancellationToken)
    {
        var user = await userManager.Users.Where(u => u.UserProfileId == userProfileId).FirstOrDefaultAsync(cancellationToken);
        if (user == null)
            return null;

        var roles = await userManager.GetRolesAsync(user);
        return new UserDto()
        {
            Id = user.Id,
            Email = user.Email,
            UserName = user.UserName,
            UserProfileId = user.UserProfileId,
            Roles = roles
        };
    }

    public IUser? GetUserByProfileId(int userProfileId)
    {
        return userManager.Users
            .Include(u => u.UserProfile)
            .FirstOrDefault(u => u.UserProfileId == userProfileId);
    }

    public async Task<IEnumerable<string>> GetRolesOfUserAsync(IUser user)
    {
        return await userManager.GetRolesAsync((User)user);
    }

    public async Task UpdateUserAsync(UpdateUserCommand command, CancellationToken cancellationToken)
    {
        var user = await userManager.FindByIdAsync(command.UserId);
        if (user == null)
            return;

        if (command.UpdatedName != null)
            user.UserName = command.UpdatedName;

        if (command.UpdatedEmail != null)
            user.Email = command.UpdatedEmail;

        await userManager.UpdateAsync(user);
    }

    public async Task DeleteUserAsync(string userId, CancellationToken cancellationToken)
    {
        var toDelete = await userManager.FindByIdAsync(userId);

        if (toDelete == null)
            return;

        await userManager.DeleteAsync(toDelete);
    }

    public async Task<IEnumerable<IUser>> GetUsersInRoleAsync(string role, CancellationToken cancellationToken)
    {
        return await userManager.GetUsersInRoleAsync(role);
    }

    public async Task<CustomResponse> ChangeUserPasswordAsync(ChangePasswordCommand command)
    {
        var user = await userManager.FindByIdAsync(command.UserId);
        if (user == null)
            return new CustomResponse() { Succeed = false, Message = "User with such an id does not exist" };

        var result = await userManager.ChangePasswordAsync(user, command.CurrentPassword, command.NewPassword);
        return result.Succeeded
            ? new CustomResponse() { Succeed = true }
            : new CustomResponse() { Succeed = false, Message = string.Join("\n", result.Errors.Select(error => error.Description)) };
    }
}
