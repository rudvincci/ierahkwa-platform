using System;
using Pupitre.Operations.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Pupitre.Operations.Infrastructure.EF.Configuration;

public class OperationMetricConfiguration : IEntityTypeConfiguration<OperationMetric>
{
    public OperationMetricConfiguration()
    {
    }

    void IEntityTypeConfiguration<OperationMetric>.Configure(EntityTypeBuilder<OperationMetric> builder)
    {
        builder.ToTable("operationmetric");

        // Primary key
        builder.HasKey(c => c.Id);

        // Properties
        builder.Property(c => c.Id)
        .HasConversion(c=> c.Value, c=> new OperationMetricId(c))
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

