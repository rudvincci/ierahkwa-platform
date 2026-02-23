using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Mamey.Identity.EntityFramework.Entities;

namespace Mamey.Identity.EntityFramework.Configurations;

public class RoleConfiguration : IEntityTypeConfiguration<ApplicationRole>
{
    public void Configure(EntityTypeBuilder<ApplicationRole> builder)
    {
        builder.ToTable("Roles");

        builder.Property(r => r.Description)
            .HasMaxLength(500);

        builder.Property(r => r.TenantId)
            .IsRequired(false);

        builder.Property(r => r.CreatedAt)
            .IsRequired()
            .HasDefaultValueSql("CURRENT_TIMESTAMP");

        builder.Property(r => r.IsActive)
            .IsRequired()
            .HasDefaultValue(true);

        // Indexes
        builder.HasIndex(r => r.TenantId);
        builder.HasIndex(r => r.Name);
        builder.HasIndex(r => r.IsActive);
        builder.HasIndex(r => r.CreatedAt);

        // Relationships
        builder.HasMany(r => r.UserRoles)
            .WithOne(ur => ur.Role)
            .HasForeignKey(ur => ur.RoleId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasMany(r => r.RoleClaims)
            .WithOne(rc => rc.Role)
            .HasForeignKey(rc => rc.RoleId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}

