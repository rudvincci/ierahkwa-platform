using System;
using Pupitre.Aftercare.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Pupitre.Aftercare.Infrastructure.EF.Configuration;

public class AftercarePlanConfiguration : IEntityTypeConfiguration<AftercarePlan>
{
    public AftercarePlanConfiguration()
    {
    }

    void IEntityTypeConfiguration<AftercarePlan>.Configure(EntityTypeBuilder<AftercarePlan> builder)
    {
        builder.ToTable("aftercareplan");

        // Primary key
        builder.HasKey(c => c.Id);

        // Properties
        builder.Property(c => c.Id)
        .HasConversion(c=> c.Value, c=> new AftercarePlanId(c))
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

