using Forum.Application.Common.Interfaces;
using Forum.Application.Common.Models;
using Forum.Infrastructure.Common.Extensions;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace Forum.Infrastructure.Identity;

public class ForumUserManager(UserManager<User> userManager) : IUserManager
{
    public async Task<IEnumerable<IUser>> GetAllUsersAsync(CancellationToken cancellationToken)
    {
        return await userManager.Users
            .Include(u => u.UserProfile)
            .ToListAsync(cancellationToken);
    }

    public async Task<IUser?> GetUserByIdAsync(string userId, CancellationToken cancellationToken)
    {
        return await userManager.FindByIdAsync(userId);
    }
    public async Task<IUser?> GetUserByNameAsync(string userName, CancellationToken cancellationToken)
    {
        return await userManager.FindByNameAsync(userName);
    }

    public async Task<IUser?> GetUserByProfileIdAsync(int userProfileId, CancellationToken cancellationToken)
    {
        return await userManager.Users
            .Where(u => u.UserProfileId == userProfileId)
            .Include(u => u.UserProfile)
            .FirstOrDefaultAsync(cancellationToken);
    }
    public async Task<CustomResponse<IUser>> CreateUserAsync(string userName, string email, string password, CancellationToken cancellationToken)
    {
        var user = new User()
        {
            UserName = userName,
            Email = email,
        };
        var result = await userManager.CreateAsync(user, password);
        return new CustomResponse<IUser>() { Succeeded = result.Succeeded, Message = result.Errors.ToErrorString(), Payload = user };
    }

    public async Task<CustomResponse> UpdateUserAsync(IUser user, CancellationToken cancellationToken)
    {
        return GetCustomResponse(await userManager.UpdateAsync((User)user));
    }

    public async Task<CustomResponse> CheckPasswordAsync(IUser user, string password, CancellationToken cancellationToken)
    {
        return new CustomResponse() { Succeeded = await userManager.CheckPasswordAsync((User)user, password) };
    }

    public async Task<CustomResponse> ChangePasswordAsync(IUser user, string currentPassword, string newPassword)
    {
        return GetCustomResponse(await userManager.ChangePasswordAsync((User)user, currentPassword, newPassword));
    }

    public async Task<CustomResponse> DeleteUserAsync(IUser user, CancellationToken cancellationToken)
    {
        return GetCustomResponse(await userManager.DeleteAsync((User)user));
    }

    public async Task<IEnumerable<string>> GetRolesAsync(IUser user, CancellationToken cancellationToken)
    {
        return await userManager.GetRolesAsync((User)user);
    }

    public async Task<CustomResponse> AddToRoleAsync(IUser user, string role, CancellationToken cancellationToken)
    {
        return GetCustomResponse(await userManager.AddToRoleAsync((User)user, role));
    }
    public async Task<CustomResponse> AddToRolesAsync(IUser user, IEnumerable<string> roles, CancellationToken cancellationToken)
    {
        return GetCustomResponse(await userManager.AddToRolesAsync((User)user, roles));
    }
    public async Task<CustomResponse> RemoveFromRoleAsync(IUser user, string role, CancellationToken cancellationToken)
    {
        return GetCustomResponse(await userManager.RemoveFromRoleAsync((User)user, role));
    }
    public async Task<CustomResponse> RemoveFromRolesAsync(IUser user, IEnumerable<string> roles, CancellationToken cancellationToken)
    {
        return GetCustomResponse(await userManager.RemoveFromRolesAsync((User)user, roles));
    }

    public async Task<IEnumerable<IUser>> GetUsersInRoleAsync(string role, CancellationToken cancellationToken)
    {
        return await userManager.GetUsersInRoleAsync(role);
    }

    private static CustomResponse GetCustomResponse(IdentityResult result)
    {
        return new CustomResponse() { Succeeded = result.Succeeded, Message = result.Errors.ToErrorString() };
    }

}
