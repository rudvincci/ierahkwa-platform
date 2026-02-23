using Mamey.FWID.Identities.Domain.Entities;
using Mamey.FWID.Identities.Domain.ValueObjects;
using Mamey.Persistence.SQL;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using static Mamey.EnumExtensions;

namespace Mamey.FWID.Identities.Infrastructure.EF.Configuration;

internal class PermissionConfiguration : IEntityTypeConfiguration<Permission>
{
    void IEntityTypeConfiguration<Permission>.Configure(EntityTypeBuilder<Permission> builder)
    {
        builder.ToTable("permission");

        // Primary key
        builder.HasKey(c => c.Id);

        // Properties
        builder.Property(c => c.Id)
            .HasConversion(c => c.Value, c => new PermissionId(c))
            .IsRequired();

        builder.Property(c => c.Name)
            .HasMaxLength(200)
            .IsRequired();

        builder.Property(c => c.Description)
            .HasMaxLength(1000);

        // Store PermissionStatus enum as string
        builder.Property(c => c.Status)
            .HasConversion(
                v => v.ToString(),
                v => v.ToEnum<PermissionStatus>())
            .HasMaxLength(50)
            .IsRequired();

        builder.Property(c => c.CreatedAt)
            .IsRequired();

        builder.Property(c => c.UpdatedAt);

        // Configure Version property as concurrency token
        builder.Property(c => c.Version)
            .IsConcurrencyToken()
            .HasColumnName("version")
            .IsRequired();

        // Indexes
        builder.HasIndex(c => c.Name).IsUnique();
        builder.HasIndex(c => c.Status);

        // Ignore domain events
        builder.Ignore(c => c.Events);
    }
}









