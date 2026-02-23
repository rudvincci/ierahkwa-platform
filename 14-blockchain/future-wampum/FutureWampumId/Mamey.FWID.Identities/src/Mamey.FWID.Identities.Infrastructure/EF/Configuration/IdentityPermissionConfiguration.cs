using Mamey.FWID.Identities.Domain.Entities;
using Mamey.Persistence.SQL;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Mamey.FWID.Identities.Infrastructure.EF.Configuration;

internal class IdentityPermissionConfiguration : IEntityTypeConfiguration<IdentityPermission>
{
    void IEntityTypeConfiguration<IdentityPermission>.Configure(EntityTypeBuilder<IdentityPermission> builder)
    {
        builder.ToTable("identity_permission");

        // Primary key
        builder.HasKey(c => c.Id);

        // Properties
        builder.Property(c => c.Id)
            .IsRequired();

        builder.Property(c => c.IdentityId)
            .HasConversion(c => c.Value, c => new IdentityId(c))
            .IsRequired();

        builder.Property(c => c.PermissionId)
            .HasConversion(c => c.Value, c => new PermissionId(c))
            .IsRequired();

        builder.Property(c => c.AssignedAt)
            .IsRequired();

        // Configure Version property as concurrency token
        builder.Property(c => c.Version)
            .IsConcurrencyToken()
            .HasColumnName("version")
            .IsRequired();

        // Indexes
        builder.HasIndex(c => c.IdentityId);
        builder.HasIndex(c => c.PermissionId);
        builder.HasIndex(c => new { c.IdentityId, c.PermissionId }).IsUnique();

        // Ignore domain events
        builder.Ignore(c => c.Events);
    }
}









