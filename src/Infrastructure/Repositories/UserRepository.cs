using Forum.Application.Common.Interfaces;
using Forum.Application.Common.Interfaces.Identyty;
using Forum.Application.Common.Interfaces.Repositories;
using Forum.Application.Messages.Dtos;
using Forum.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Forum.Infrastructure.Repositories;

public class UserRepository(IForumDbContext context, IUserManager userManager) : IUserRepository
{
    public async Task<UserProfile> AddProfileToUserAsync(IUser user, CancellationToken cancellationToken)
    {
        var profile = new UserProfile() { IdentityUserId = user?.Id ?? string.Empty };
        await context.UserProfiles.AddAsync(profile, cancellationToken);
        await context.SaveChangesAsync(cancellationToken);
        user!.UserProfileId = profile.Id;
        await context.SaveChangesAsync(cancellationToken);
        return profile;
    }
    public UserMessageDto? GetUserMessageDtoById(int id)
    {
        var user = userManager.GetUserByProfileId(id);
        string userName = user?.UserName ?? string.Empty;
        IEnumerable<string> userRoles = user == null
            ? Enumerable.Empty<string>()
            : userManager.GetRolesOfUserAsync(user!).Result;
        return SelectDto(id, userName, userRoles)
            .FirstOrDefault();
    }
    public async Task<UserMessageDto?> GetUserMessageDtoByIdAsync(int id, CancellationToken cancellationToken)
    {
        var user = await userManager.GetUserByProfileIdAsync(id, cancellationToken);
        string userName = user?.UserName ?? string.Empty;
        IEnumerable<string> userRoles = user == null
            ? Enumerable.Empty<string>()
            : await userManager.GetRolesOfUserAsync(user!);
        return await SelectDto(id, userName, userRoles)
            .FirstOrDefaultAsync(cancellationToken);
    }

    private IQueryable<UserMessageDto> SelectDto(int id, string userName, IEnumerable<string> userRoles)
    {
        return context.UserProfiles
            .Where(up => up.Id == id)
            .Select(up => new UserMessageDto()
            {
                UserProfileId = up.Id,
                UserName = userName,
                UserRoles = userRoles
            });
    }
}
