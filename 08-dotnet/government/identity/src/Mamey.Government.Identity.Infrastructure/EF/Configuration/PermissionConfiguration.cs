using Mamey.Government.Identity.Domain.Entities;
using Mamey.Types;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Mamey.Government.Identity.Infrastructure.EF.Configuration;

public class PermissionConfiguration : IEntityTypeConfiguration<Permission>
{
    void IEntityTypeConfiguration<Permission>.Configure(EntityTypeBuilder<Permission> builder)
    {
        builder.ToTable("permissions");

        // Primary key
        builder.HasKey(p => p.Id);

        // Properties with value conversions
        builder.Property(p => p.Id)
            .HasConversion(
                id => id.Value,
                value => new PermissionId(value))
            .IsRequired();

        builder.Property(p => p.Name)
            .HasMaxLength(100)
            .IsRequired();

        builder.Property(p => p.Description)
            .HasMaxLength(500);

        builder.Property(p => p.Resource)
            .HasMaxLength(100)
            .IsRequired();

        builder.Property(p => p.Action)
            .HasMaxLength(50)
            .IsRequired();

        builder.Property(p => p.Status)
            .HasConversion<int>();

        // Indexes
        builder.HasIndex(p => new { p.Resource, p.Action }).IsUnique();
        builder.HasIndex(p => p.Name).IsUnique();

        // Ignore domain events
        builder.Ignore(p => p.Events);
    }
}
