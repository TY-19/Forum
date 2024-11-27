using Forum.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Forum.Infrastructure.Data.Configurations;

public class TopicConfiguration : IEntityTypeConfiguration<Topic>
{
    public void Configure(EntityTypeBuilder<Topic> builder)
    {
        builder.HasKey(x => x.Id);
        builder.HasMany(x => x.Messages)
            .WithOne()
            .HasForeignKey(m => m.TopicId);
    }
}
