using System;
using Pupitre.AIContent.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Pupitre.AIContent.Infrastructure.EF.Configuration;

public class ContentGenerationConfiguration : IEntityTypeConfiguration<ContentGeneration>
{
    public ContentGenerationConfiguration()
    {
    }

    void IEntityTypeConfiguration<ContentGeneration>.Configure(EntityTypeBuilder<ContentGeneration> builder)
    {
        builder.ToTable("contentgeneration");

        // Primary key
        builder.HasKey(c => c.Id);

        // Properties
        builder.Property(c => c.Id)
        .HasConversion(c=> c.Value, c=> new ContentGenerationId(c))
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

