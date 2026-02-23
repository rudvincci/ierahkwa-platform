using System;
using Pupitre.AITutors.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Pupitre.AITutors.Infrastructure.EF.Configuration;

public class TutorConfiguration : IEntityTypeConfiguration<Tutor>
{
    public TutorConfiguration()
    {
    }

    void IEntityTypeConfiguration<Tutor>.Configure(EntityTypeBuilder<Tutor> builder)
    {
        builder.ToTable("tutor");

        // Primary key
        builder.HasKey(c => c.Id);

        // Properties
        builder.Property(c => c.Id)
        .HasConversion(c=> c.Value, c=> new TutorId(c))
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

