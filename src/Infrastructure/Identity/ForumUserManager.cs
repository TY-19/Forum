using Forum.Application.Common.Interfaces;
using Forum.Application.Common.Models;
using Forum.Application.Users.Dtos;
using Forum.Infrastructure.Common.Extensions;
using Forum.Infrastructure.Common.Interfaces;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace Forum.Infrastructure.Identity;

public class ForumUserManager(UserManager<User> userManager,
    IDatabaseHelper databaseHelper) : IUserManager
{
    public IQueryable<IUser> GetAllUsers()
        => userManager.Users.Include(u => u.UserProfile).AsNoTracking();

    public IQueryable<UserWithRoles> GetAllUsersWithRoles()
        => databaseHelper.GetUsersWithRoles();

    public async Task<IEnumerable<IUser>> GetAllUsersAsync(CancellationToken cancellationToken)
        => await userManager.Users
            .Include(u => u.UserProfile)
            .AsNoTracking()
            .ToListAsync(cancellationToken);

    public async Task<IUser?> GetUserByIdAsync(string userId, CancellationToken cancellationToken)
        => await userManager.Users
            .Include(u => u.UserProfile)
            .FirstOrDefaultAsync(u => u.Id == userId, cancellationToken);

    public async Task<IUser?> GetUserByNameAsync(string userName, CancellationToken cancellationToken)
        => await userManager.Users
            .Include(u => u.UserProfile)
            .FirstOrDefaultAsync(u => u.UserName == userName, cancellationToken);
    public async Task<IUser?> GetUserByProfileIdAsync(int userProfileId, CancellationToken cancellationToken)
        => await userManager.Users
            .Include(u => u.UserProfile)
            .FirstOrDefaultAsync(u => u.UserProfile.Id == userProfileId, cancellationToken);

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
        => GetCustomResponse(await userManager.UpdateAsync((User)user));

    public async Task<CustomResponse> CheckPasswordAsync(IUser user, string password, CancellationToken cancellationToken)
        => new CustomResponse() { Succeeded = await userManager.CheckPasswordAsync((User)user, password) };

    public async Task<CustomResponse> ChangePasswordAsync(IUser user, string currentPassword, string newPassword, CancellationToken cancellationToken)
        => GetCustomResponse(await userManager.ChangePasswordAsync((User)user, currentPassword, newPassword));


    public async Task<CustomResponse> SetPasswordAsync(IUser user, string newPassword, CancellationToken cancellationToken)
    {
        var resetToken = await userManager.GeneratePasswordResetTokenAsync((User)user);
        return GetCustomResponse(await userManager.ResetPasswordAsync((User)user, resetToken, newPassword));
    }

    public async Task<CustomResponse> DeleteUserAsync(IUser user, CancellationToken cancellationToken)
        => GetCustomResponse(await userManager.DeleteAsync((User)user));

    public async Task<IEnumerable<string>> GetRolesAsync(IUser user, CancellationToken cancellationToken)
        => await userManager.GetRolesAsync((User)user);

    public async Task<CustomResponse> AddToRoleAsync(IUser user, string role, CancellationToken cancellationToken)
        => GetCustomResponse(await userManager.AddToRoleAsync((User)user, role));

    public async Task<CustomResponse> AddToRolesAsync(IUser user, IEnumerable<string> roles, CancellationToken cancellationToken)
        => GetCustomResponse(await userManager.AddToRolesAsync((User)user, roles));

    public async Task<CustomResponse> RemoveFromRoleAsync(IUser user, string role, CancellationToken cancellationToken)
        => GetCustomResponse(await userManager.RemoveFromRoleAsync((User)user, role));

    public async Task<CustomResponse> RemoveFromRolesAsync(IUser user, IEnumerable<string> roles, CancellationToken cancellationToken)
        => GetCustomResponse(await userManager.RemoveFromRolesAsync((User)user, roles));

    public async Task<IEnumerable<IUser>> GetUsersInRoleAsync(string role, CancellationToken cancellationToken)
        => await userManager.GetUsersInRoleAsync(role);

    private static CustomResponse GetCustomResponse(IdentityResult result)
        => new() { Succeeded = result.Succeeded, Message = result.Errors.ToErrorString() };
}
