using System;
using Pupitre.AIVision.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Pupitre.AIVision.Infrastructure.EF.Configuration;

public class VisionAnalysisConfiguration : IEntityTypeConfiguration<VisionAnalysis>
{
    public VisionAnalysisConfiguration()
    {
    }

    void IEntityTypeConfiguration<VisionAnalysis>.Configure(EntityTypeBuilder<VisionAnalysis> builder)
    {
        builder.ToTable("visionanalysis");

        // Primary key
        builder.HasKey(c => c.Id);

        // Properties
        builder.Property(c => c.Id)
        .HasConversion(c=> c.Value, c=> new VisionAnalysisId(c))
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

