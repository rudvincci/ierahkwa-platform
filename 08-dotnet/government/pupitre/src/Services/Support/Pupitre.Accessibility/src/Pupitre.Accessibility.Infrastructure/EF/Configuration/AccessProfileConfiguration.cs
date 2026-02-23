using System;
using Pupitre.Accessibility.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Pupitre.Accessibility.Infrastructure.EF.Configuration;

public class AccessProfileConfiguration : IEntityTypeConfiguration<AccessProfile>
{
    public AccessProfileConfiguration()
    {
    }

    void IEntityTypeConfiguration<AccessProfile>.Configure(EntityTypeBuilder<AccessProfile> builder)
    {
        builder.ToTable("accessprofile");

        // Primary key
        builder.HasKey(c => c.Id);

        // Properties
        builder.Property(c => c.Id)
        .HasConversion(c=> c.Value, c=> new AccessProfileId(c))
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

