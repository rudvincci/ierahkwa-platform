using System;
using Pupitre.AIRecommendations.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Pupitre.AIRecommendations.Infrastructure.EF.Configuration;

public class AIRecommendationConfiguration : IEntityTypeConfiguration<AIRecommendation>
{
    public AIRecommendationConfiguration()
    {
    }

    void IEntityTypeConfiguration<AIRecommendation>.Configure(EntityTypeBuilder<AIRecommendation> builder)
    {
        builder.ToTable("airecommendation");

        // Primary key
        builder.HasKey(c => c.Id);

        // Properties
        builder.Property(c => c.Id)
        .HasConversion(c=> c.Value, c=> new AIRecommendationId(c))
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

