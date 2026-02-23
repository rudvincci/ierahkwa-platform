using System.Text.Json;
using Mamey.FWID.Identities.Domain.Entities;
using Mamey.FWID.Identities.Domain.ValueObjects;
using Mamey.Persistence.SQL;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using static Mamey.EnumExtensions;

namespace Mamey.FWID.Identities.Infrastructure.EF.Configuration;

internal class MfaConfigurationConfiguration : IEntityTypeConfiguration<MfaConfiguration>
{
    void IEntityTypeConfiguration<MfaConfiguration>.Configure(EntityTypeBuilder<MfaConfiguration> builder)
    {
        builder.ToTable("mfa_configuration");

        // Primary key
        builder.HasKey(c => c.Id);

        // Properties
        builder.Property(c => c.Id)
            .HasConversion(c => c.Value, c => new MfaConfigurationId(c))
            .IsRequired();

        builder.Property(c => c.IdentityId)
            .HasConversion(c => c.Value, c => new IdentityId(c))
            .IsRequired();

        // Store MfaMethod enum as string
        builder.Property(c => c.Method)
            .HasConversion(
                v => v.ToString(),
                v => v.ToEnum<MfaMethod>())
            .HasMaxLength(50)
            .IsRequired();

        builder.Property(c => c.IsEnabled)
            .IsRequired()
            .HasDefaultValue(false);

        builder.Property(c => c.SecretKey)
            .HasMaxLength(500);

        // Store BackupCodes list as JSONB
        builder.Property(c => c.BackupCodes)
            .HasConversion(
                v => JsonSerializer.Serialize(v, (JsonSerializerOptions?)null),
                v => JsonSerializer.Deserialize<List<string>>(v, (JsonSerializerOptions?)null) ?? new List<string>())
            .HasColumnType("jsonb");

        builder.Property(c => c.CreatedAt)
            .IsRequired();

        builder.Property(c => c.EnabledAt);

        builder.Property(c => c.LastUsedAt);

        // Configure Version property as concurrency token
        builder.Property(c => c.Version)
            .IsConcurrencyToken()
            .HasColumnName("version")
            .IsRequired();

        // Indexes
        builder.HasIndex(c => c.IdentityId);
        builder.HasIndex(c => c.Method);
        builder.HasIndex(c => c.IsEnabled);

        // Ignore domain events
        builder.Ignore(c => c.Events);
    }
}









