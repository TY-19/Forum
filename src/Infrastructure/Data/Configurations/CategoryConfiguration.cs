using Forum.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Forum.Infrastructure.Data.Configurations;
public class CategoryConfiguration : IEntityTypeConfiguration<Category>
{
    public void Configure(EntityTypeBuilder<Category> builder)
    {
        builder.HasKey(x => x.Id);
        builder.HasMany(x => x.Forums)
            .WithOne(x => x.Category)
            .HasForeignKey(x => x.CategoryId);
    }
}