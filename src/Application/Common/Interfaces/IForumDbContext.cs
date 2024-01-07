using Forum.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Forum.Application.Common.Interfaces;

public interface IForumDbContext
{
    DbSet<ForumEntity> Forums { get; }
    DbSet<Topic> Topics { get; }
    DbSet<Message> Messages { get; }
    DbSet<UserProfile> UserProfiles { get; }
    DbSet<ApplicationRole> ApplicationRoles { get; }
    DbSet<Permission> Permissions { get; }
    DbSet<UnreadElement> UnreadElements { get; }

    Task<int> SaveChangesAsync(CancellationToken cancellationToken);
}
