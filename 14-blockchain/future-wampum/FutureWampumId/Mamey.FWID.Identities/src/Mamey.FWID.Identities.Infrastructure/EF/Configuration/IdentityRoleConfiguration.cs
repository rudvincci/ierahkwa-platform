using Mamey.FWID.Identities.Domain.Entities;
using Mamey.Persistence.SQL;
using Mamey.Types;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Mamey.FWID.Identities.Infrastructure.EF.Configuration;

internal class IdentityRoleConfiguration : IEntityTypeConfiguration<IdentityRole>
{
    void IEntityTypeConfiguration<IdentityRole>.Configure(EntityTypeBuilder<IdentityRole> builder)
    {
        builder.ToTable("identity_role");

        // Primary key
        builder.HasKey(c => c.Id);

        // Properties
        builder.Property(c => c.Id)
            .IsRequired();

        builder.Property(c => c.IdentityId)
            .HasConversion(c => c.Value, c => new IdentityId(c))
            .IsRequired();

        builder.Property(c => c.RoleId)
            .HasConversion(c => c.Value, c => new RoleId(c))
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
        builder.HasIndex(c => c.RoleId);
        builder.HasIndex(c => new { c.IdentityId, c.RoleId }).IsUnique();

        // Ignore domain events
        builder.Ignore(c => c.Events);
    }
}

