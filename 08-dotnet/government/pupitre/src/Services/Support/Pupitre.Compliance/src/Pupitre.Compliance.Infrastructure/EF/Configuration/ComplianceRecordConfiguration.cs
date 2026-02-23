using System;
using Pupitre.Compliance.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Pupitre.Compliance.Infrastructure.EF.Configuration;

public class ComplianceRecordConfiguration : IEntityTypeConfiguration<ComplianceRecord>
{
    public ComplianceRecordConfiguration()
    {
    }

    void IEntityTypeConfiguration<ComplianceRecord>.Configure(EntityTypeBuilder<ComplianceRecord> builder)
    {
        builder.ToTable("compliancerecord");

        // Primary key
        builder.HasKey(c => c.Id);

        // Properties
        builder.Property(c => c.Id)
        .HasConversion(c=> c.Value, c=> new ComplianceRecordId(c))
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

