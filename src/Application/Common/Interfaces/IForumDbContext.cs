using Forum.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Forum.Application.Common.Interfaces;

public interface IForumDbContext
{
    DbSet<ForumEntity> Forums { get; }
    DbSet<Topic> Topics { get; }
    DbSet<Message> Messages { get; }
    DbSet<UserProfile> UserProfiles { get; }

    Task<int> SaveChangesAsync(CancellationToken cancellationToken);
}
