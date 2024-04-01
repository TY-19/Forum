using Forum.Application.Common.Interfaces;
using Forum.Domain.Entities;
using Forum.Infrastructure.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using System.Reflection;

namespace Forum.Infrastructure.Data;

public class ForumDbContext(DbContextOptions<ForumDbContext> options)
    : IdentityDbContext<User, Role, string>(options), IForumDbContext
{
    public DbSet<ForumEntity> Forums => Set<ForumEntity>();
    public DbSet<Category> Categories => Set<Category>();
    public DbSet<Topic> Topics => Set<Topic>();
    public DbSet<Message> Messages => Set<Message>();
    public DbSet<UserProfile> UserProfiles => Set<UserProfile>();
    public DbSet<ApplicationRole> ApplicationRoles => Set<ApplicationRole>();
    public DbSet<Permission> Permissions => Set<Permission>();
    public DbSet<UnreadElement> UnreadElements => Set<UnreadElement>();

    protected override void OnModelCreating(ModelBuilder builder)
    {
        builder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());
        base.OnModelCreating(builder);
    }
}
