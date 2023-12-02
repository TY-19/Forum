using Forum.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Forum.Infrastructure.Data.Configurations;

public class ForumConfiguration : IEntityTypeConfiguration<ForumEntity>
{
    public void Configure(Microsoft.EntityFrameworkCore.Metadata.Builders.EntityTypeBuilder<ForumEntity> builder)
    {
        builder.HasKey(x => x.Id);
        builder.HasMany(x => x.Subforums)
            .WithOne()
            .HasForeignKey(x => x.ParentForumId);
        builder.HasMany(x => x.Topics)
            .WithOne()
            .HasForeignKey(x => x.ParentForumId);
    }
}
