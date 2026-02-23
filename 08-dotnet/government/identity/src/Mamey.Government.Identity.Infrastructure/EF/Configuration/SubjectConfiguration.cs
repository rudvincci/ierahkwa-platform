using System.Text.Json;
using Mamey.Government.Identity.Domain.Entities;
using Mamey.Persistence.SQL;
using Mamey.Types;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Mamey.Government.Identity.Infrastructure.EF.Configuration;

public class SubjectConfiguration : IEntityTypeConfiguration<Subject>
{
    void IEntityTypeConfiguration<Subject>.Configure(EntityTypeBuilder<Subject> builder)
    {
        builder.ToTable("subjects");

        // Primary key
        builder.HasKey(s => s.Id);

        // Properties with value conversions
        builder.Property(s => s.Id)
            .HasConversion(
                id => id.Value,
                value => new SubjectId(value))
            .IsRequired();

        builder.Property(s => s.Name)
            .HasMaxLength(200)
            .IsRequired();

        builder.Property(s => s.Email)
            .HasConversion(
                email => email.Value,
                value => new Email(value))
            .HasMaxLength(100)
            .IsRequired();

        builder.Property(s => s.Status)
            .HasConversion<int>();

        // Tags Collection (storing as comma-separated string)
        builder.Property(s => s.Tags)
            .HasConversion(
                v => string.Join(',', v),
                v => v.Split(',', StringSplitOptions.RemoveEmptyEntries))
            .IsRequired(false);

        // Roles collection stored as JSONB
        builder.Property(s => s.Roles)
            .HasConversion(
                roles => JsonSerializer.Serialize(
                    roles.Select(r => r.Value), 
                    JsonExtensions.SerializerOptions),
                json => JsonSerializer.Deserialize<List<Guid>>(
                    json, 
                    JsonExtensions.SerializerOptions)
                    .Select(g => new RoleId(g))
                    .ToList())
            .HasColumnType("jsonb");

        // Indexes
        builder.HasIndex(s => s.Name);
        builder.HasIndex(s => s.Email).IsUnique();

        // Ignore domain events
        builder.Ignore(s => s.Events);
    }
}

