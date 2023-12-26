using Forum.Application.Common.Interfaces;
using Forum.Application.Common.Interfaces.Identyty;
using Forum.Application.Users.Commands.CreateUser;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace Forum.Infrastructure.Identity;

public class IdentityUserManager(UserManager<User> userManager) : IUserManager
{
    public async Task<IUser> CreateUserAsync(CreateUserCommand command, CancellationToken cancellationToken)
    {
        var user = new User()
        {
            UserName = command.UserName,
            Email = command.UserEmail,
        };
        await userManager.CreateAsync(user, command.Password);
        return user;
    }

    public async Task<IEnumerable<IUser>> GetAllUsersAsync(CancellationToken cancellationToken)
    {
        return await userManager.Users
            .Include(u => u.UserProfile)
            .ToListAsync(cancellationToken);
    }

    public async Task<IUser?> GetUserByProfileIdAsync(int userProfileId, CancellationToken cancellationToken)
    {
        return await userManager.Users
            .Include(u => u.UserProfile)
            .FirstOrDefaultAsync(u => u.UserProfileId == userProfileId, cancellationToken);
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
}
