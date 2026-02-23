using Mamey.FWID.Identities.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Mamey.FWID.Identities.Infrastructure.EF.Configuration;

/// <summary>
/// EF Core configuration for PermissionMapping entity.
/// </summary>
internal class PermissionMappingConfiguration : IEntityTypeConfiguration<PermissionMapping>
{
    public void Configure(EntityTypeBuilder<PermissionMapping> builder)
    {
        builder.ToTable("permission_mapping");

        // Primary key
        builder.HasKey(p => p.Id);

        // Properties
        builder.Property(p => p.Id)
            .IsRequired();

        builder.Property(p => p.ServiceName)
            .HasMaxLength(200)
            .IsRequired();

        builder.Property(p => p.Permissions)
            .HasConversion(
                v => string.Join(",", v),
                v => v.Split(',', StringSplitOptions.RemoveEmptyEntries).ToList())
            .HasColumnType("text")
            .IsRequired();

        builder.Property(p => p.CertificateSubject)
            .HasMaxLength(500);

        builder.Property(p => p.CertificateIssuer)
            .HasMaxLength(500);

        builder.Property(p => p.CertificateThumbprint)
            .HasMaxLength(100);

        builder.Property(p => p.CreatedAt)
            .IsRequired();

        builder.Property(p => p.UpdatedAt)
            .IsRequired();

        builder.Property(p => p.IsActive)
            .IsRequired()
            .HasDefaultValue(true);

        // Indexes
        builder.HasIndex(p => p.ServiceName)
            .IsUnique()
            .HasDatabaseName("ix_permission_mapping_service_name");

        builder.HasIndex(p => p.IsActive)
            .HasDatabaseName("ix_permission_mapping_is_active");
    }
}

