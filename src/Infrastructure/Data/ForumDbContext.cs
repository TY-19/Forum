using Forum.Application.Common.Interfaces;
using Forum.Domain.Entities;
using Forum.Infrastructure.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using System.Reflection;

namespace Forum.Infrastructure.Data;

public class ForumDbContext : IdentityDbContext<User>, IForumDbContext
{
    public ForumDbContext(DbContextOptions<ForumDbContext> options) : base(options)
    {
    }
    public DbSet<ForumEntity> Forums => Set<ForumEntity>();
    public DbSet<Topic> Topics => Set<Topic>();
    public DbSet<Message> Messages => Set<Message>();
    public DbSet<UserProfile> UserProfiles => Set<UserProfile>();

    protected override void OnModelCreating(ModelBuilder builder)
    {
        builder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());
        base.OnModelCreating(builder);
    }
}
