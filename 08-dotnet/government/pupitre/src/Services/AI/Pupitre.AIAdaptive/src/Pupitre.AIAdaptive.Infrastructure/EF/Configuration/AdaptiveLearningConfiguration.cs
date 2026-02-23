using System;
using Pupitre.AIAdaptive.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Pupitre.AIAdaptive.Infrastructure.EF.Configuration;

public class AdaptiveLearningConfiguration : IEntityTypeConfiguration<AdaptiveLearning>
{
    public AdaptiveLearningConfiguration()
    {
    }

    void IEntityTypeConfiguration<AdaptiveLearning>.Configure(EntityTypeBuilder<AdaptiveLearning> builder)
    {
        builder.ToTable("adaptivelearning");

        // Primary key
        builder.HasKey(c => c.Id);

        // Properties
        builder.Property(c => c.Id)
        .HasConversion(c=> c.Value, c=> new AdaptiveLearningId(c))
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

