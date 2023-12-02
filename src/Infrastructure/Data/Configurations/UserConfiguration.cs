using Forum.Domain.Entities;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;
using Forum.Infrastructure.Identity;

namespace Forum.Infrastructure.Data.Configurations;

public class UserConfiguration : IEntityTypeConfiguration<User>
{
    public void Configure(EntityTypeBuilder<User> builder)
    {
        builder.HasKey(x => x.Id);
        builder.HasOne(x => x.UserProfile)
            .WithOne()
            .HasForeignKey<UserProfile>(u => u.IdentityUserId);
    }
}
