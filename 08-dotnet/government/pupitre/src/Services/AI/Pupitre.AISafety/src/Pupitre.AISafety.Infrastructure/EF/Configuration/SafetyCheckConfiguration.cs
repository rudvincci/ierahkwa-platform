using System;
using Pupitre.AISafety.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Pupitre.AISafety.Infrastructure.EF.Configuration;

public class SafetyCheckConfiguration : IEntityTypeConfiguration<SafetyCheck>
{
    public SafetyCheckConfiguration()
    {
    }

    void IEntityTypeConfiguration<SafetyCheck>.Configure(EntityTypeBuilder<SafetyCheck> builder)
    {
        builder.ToTable("safetycheck");

        // Primary key
        builder.HasKey(c => c.Id);

        // Properties
        builder.Property(c => c.Id)
        .HasConversion(c=> c.Value, c=> new SafetyCheckId(c))
        .IsRequired();

        // Tags Collection (assuming many-to-many or storing as a serialized array)
        builder.Property(c => c.Tags)
            .HasConversion(
                v => string.Join(',', v),
                v => v.Split(',', StringSplitOptions.RemoveEmptyEntries))
            .IsRequired(false);

        builder.Ignore(c => c.Events);
        // Indexes
        builder.HasIndex(c => c.Name)
            .IsUnique(false);
    }
}

