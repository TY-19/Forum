using Forum.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Forum.Infrastructure.Data.Configurations;

public class UserProfileConfiguration : IEntityTypeConfiguration<UserProfile>
{
    public void Configure(EntityTypeBuilder<UserProfile> builder)
    {
        builder.HasKey(x => x.Id);
        builder.HasMany(x => x.Messages)
            .WithOne()
            .HasForeignKey(m => m.UserProfileId);
        builder.HasMany(x => x.UnreadTopics)
            .WithOne()
            .HasForeignKey(x => x.UserProfileId);

    }
}
