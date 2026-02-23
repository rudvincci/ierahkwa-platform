using System;
using Pupitre.Analytics.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Pupitre.Analytics.Infrastructure.EF.Configuration;

public class AnalyticConfiguration : IEntityTypeConfiguration<Analytic>
{
    public AnalyticConfiguration()
    {
    }

    void IEntityTypeConfiguration<Analytic>.Configure(EntityTypeBuilder<Analytic> builder)
    {
        builder.ToTable("analytic");

        // Primary key
        builder.HasKey(c => c.Id);

        // Properties
        builder.Property(c => c.Id)
        .HasConversion(c=> c.Value, c=> new AnalyticId(c))
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

