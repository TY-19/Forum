using Forum.Domain.Entities;
using Forum.Infrastructure.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Forum.Infrastructure.Data.Configurations;

public class RoleConfiguration : IEntityTypeConfiguration<Role>
{
    public void Configure(EntityTypeBuilder<Role> builder)
    {
        builder.HasKey(x => x.Id);
        builder.HasOne(x => x.ApplicationRole)
            .WithOne()
            .HasForeignKey<ApplicationRole>(a => a.IdentityRoleId);
    }
}
