using Forum.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Forum.Infrastructure.Data.Configurations;

public class UnreadElementConfiguration : IEntityTypeConfiguration<UnreadElement>
{
    public void Configure(EntityTypeBuilder<UnreadElement> builder)
    {
        builder.HasKey(u => u.Id);
        builder.HasOne(u => u.Topic)
            .WithMany()
            .HasForeignKey(u => u.TopicId)
            .OnDelete(DeleteBehavior.NoAction);
        builder.HasOne(u => u.Message)
            .WithMany()
            .HasForeignKey(u => u.MessageId)
            .OnDelete(DeleteBehavior.NoAction);
    }
}
