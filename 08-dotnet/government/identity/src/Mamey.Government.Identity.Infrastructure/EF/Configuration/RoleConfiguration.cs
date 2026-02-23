using System.Text.Json;
using Mamey.Government.Identity.Domain.Entities;
using Mamey.Persistence.SQL;
using Mamey.Types;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Mamey.Government.Identity.Infrastructure.EF.Configuration;

public class RoleConfiguration : IEntityTypeConfiguration<Role>
{
    void IEntityTypeConfiguration<Role>.Configure(EntityTypeBuilder<Role> builder)
    {
        builder.ToTable("roles");

        // Primary key
        builder.HasKey(r => r.Id);

        // Properties with value conversions
        builder.Property(r => r.Id)
            .HasConversion(
                id => id.Value,
                value => new RoleId(value))
            .IsRequired();

        builder.Property(r => r.Name)
            .HasMaxLength(100)
            .IsRequired();

        builder.Property(r => r.Description)
            .HasMaxLength(500);

        builder.Property(r => r.Status)
            .HasConversion<int>();

        // Permissions collection stored as JSONB
        builder.Property(r => r.Permissions)
            .HasConversion(
                permissions => JsonSerializer.Serialize(
                    permissions.Select(p => p.Value), 
                    JsonExtensions.SerializerOptions),
                json => JsonSerializer.Deserialize<List<Guid>>(
                    json, 
                    JsonExtensions.SerializerOptions)
                    .Select(g => new PermissionId(g))
                    .ToList())
            .HasColumnType("jsonb");

        // Indexes
        builder.HasIndex(r => r.Name).IsUnique();

        // Ignore domain events
        builder.Ignore(r => r.Events);
    }
}
