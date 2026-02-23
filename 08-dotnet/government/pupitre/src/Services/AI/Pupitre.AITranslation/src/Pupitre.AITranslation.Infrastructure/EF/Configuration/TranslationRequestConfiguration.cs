using System;
using Pupitre.AITranslation.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Pupitre.AITranslation.Infrastructure.EF.Configuration;

public class TranslationRequestConfiguration : IEntityTypeConfiguration<TranslationRequest>
{
    public TranslationRequestConfiguration()
    {
    }

    void IEntityTypeConfiguration<TranslationRequest>.Configure(EntityTypeBuilder<TranslationRequest> builder)
    {
        builder.ToTable("translationrequest");

        // Primary key
        builder.HasKey(c => c.Id);

        // Properties
        builder.Property(c => c.Id)
        .HasConversion(c=> c.Value, c=> new TranslationRequestId(c))
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

