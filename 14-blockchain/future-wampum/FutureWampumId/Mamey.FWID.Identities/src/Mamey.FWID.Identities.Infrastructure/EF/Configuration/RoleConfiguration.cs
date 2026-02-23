using System.Text.Json;
using Mamey.FWID.Identities.Domain.Entities;
using Mamey.FWID.Identities.Domain.ValueObjects;
using Mamey.Persistence.SQL;
using Mamey.Types;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using static Mamey.EnumExtensions;

namespace Mamey.FWID.Identities.Infrastructure.EF.Configuration;

internal class RoleConfiguration : IEntityTypeConfiguration<Role>
{
    void IEntityTypeConfiguration<Role>.Configure(EntityTypeBuilder<Role> builder)
    {
        builder.ToTable("role");

        // Primary key
        builder.HasKey(c => c.Id);

        // Properties
        builder.Property(c => c.Id)
            .HasConversion(c => c.Value, c => new RoleId(c))
            .IsRequired();

        builder.Property(c => c.Name)
            .HasMaxLength(200)
            .IsRequired();

        builder.Property(c => c.Description)
            .HasMaxLength(1000);

        // Store RoleStatus enum as string
        builder.Property(c => c.Status)
            .HasConversion(
                v => v.ToString(),
                v => v.ToEnum<RoleStatus>())
            .HasMaxLength(50)
            .IsRequired();

        // Store Permissions list as JSONB
        builder.Property(c => c.Permissions)
            .HasConversion(
                v => JsonSerializer.Serialize(
                    v.Select(p => p.Value),
                    (JsonSerializerOptions?)null),
                v => (JsonSerializer.Deserialize<List<Guid>>(v, (JsonSerializerOptions?)null) ?? new List<Guid>())
                    .Select(g => new PermissionId(g))
                    .ToList())
            .HasColumnType("jsonb");

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

