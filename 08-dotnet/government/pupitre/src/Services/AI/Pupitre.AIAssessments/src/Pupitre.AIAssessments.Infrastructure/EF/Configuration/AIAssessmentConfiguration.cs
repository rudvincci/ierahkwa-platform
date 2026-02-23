using System;
using Pupitre.AIAssessments.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Pupitre.AIAssessments.Infrastructure.EF.Configuration;

public class AIAssessmentConfiguration : IEntityTypeConfiguration<AIAssessment>
{
    public AIAssessmentConfiguration()
    {
    }

    void IEntityTypeConfiguration<AIAssessment>.Configure(EntityTypeBuilder<AIAssessment> builder)
    {
        builder.ToTable("aiassessment");

        // Primary key
        builder.HasKey(c => c.Id);

        // Properties
        builder.Property(c => c.Id)
        .HasConversion(c=> c.Value, c=> new AIAssessmentId(c))
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

