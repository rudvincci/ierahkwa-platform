using System.Text.Json;
using Mamey.Government.Identity.Domain.Entities;
using Mamey.Persistence.SQL;
using Mamey.Types;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Mamey.Government.Identity.Infrastructure.EF.Configuration;

public class MultiFactorAuthConfiguration : IEntityTypeConfiguration<MultiFactorAuth>
{
    void IEntityTypeConfiguration<MultiFactorAuth>.Configure(EntityTypeBuilder<MultiFactorAuth> builder)
    {
        builder.ToTable("multi_factor_auths");

        // Primary key
        builder.HasKey(m => m.Id);

        // Properties with value conversions
        builder.Property(m => m.Id)
            .HasConversion(
                id => id.Value,
                value => new MultiFactorAuthId(value))
            .IsRequired();

        builder.Property(m => m.UserId)
            .HasConversion(
                id => id.Value,
                value => new UserId(value))
            .IsRequired();

        // EnabledMethods collection stored as JSONB
        builder.Property(m => m.EnabledMethods)
            .HasConversion(
                methods => JsonSerializer.Serialize(
                    methods.Select(m => (int)m), 
                    JsonExtensions.SerializerOptions),
                json => JsonSerializer.Deserialize<List<int>>(
                    json, 
                    JsonExtensions.SerializerOptions)
                    .Select(i => (MfaMethod)i)
                    .ToList())
            .HasColumnType("jsonb");

        builder.Property(m => m.Status)
            .HasConversion<int>();

        // Indexes
        builder.HasIndex(m => m.UserId).IsUnique();

        // Ignore domain events
        builder.Ignore(m => m.Events);
    }
}
