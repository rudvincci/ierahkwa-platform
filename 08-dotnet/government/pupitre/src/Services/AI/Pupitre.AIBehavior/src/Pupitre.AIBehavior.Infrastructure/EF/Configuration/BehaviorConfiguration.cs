using System;
using Pupitre.AIBehavior.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Pupitre.AIBehavior.Infrastructure.EF.Configuration;

public class BehaviorConfiguration : IEntityTypeConfiguration<Behavior>
{
    public BehaviorConfiguration()
    {
    }

    void IEntityTypeConfiguration<Behavior>.Configure(EntityTypeBuilder<Behavior> builder)
    {
        builder.ToTable("behavior");

        // Primary key
        builder.HasKey(c => c.Id);

        // Properties
        builder.Property(c => c.Id)
        .HasConversion(c=> c.Value, c=> new BehaviorId(c))
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

