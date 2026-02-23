using System;
using Pupitre.AISpeech.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Pupitre.AISpeech.Infrastructure.EF.Configuration;

public class SpeechRequestConfiguration : IEntityTypeConfiguration<SpeechRequest>
{
    public SpeechRequestConfiguration()
    {
    }

    void IEntityTypeConfiguration<SpeechRequest>.Configure(EntityTypeBuilder<SpeechRequest> builder)
    {
        builder.ToTable("speechrequest");

        // Primary key
        builder.HasKey(c => c.Id);

        // Properties
        builder.Property(c => c.Id)
        .HasConversion(c=> c.Value, c=> new SpeechRequestId(c))
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

