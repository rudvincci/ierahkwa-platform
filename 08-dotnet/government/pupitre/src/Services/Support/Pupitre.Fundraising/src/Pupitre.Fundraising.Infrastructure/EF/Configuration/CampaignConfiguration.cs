using System;
using Pupitre.Fundraising.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Pupitre.Fundraising.Infrastructure.EF.Configuration;

public class CampaignConfiguration : IEntityTypeConfiguration<Campaign>
{
    public CampaignConfiguration()
    {
    }

    void IEntityTypeConfiguration<Campaign>.Configure(EntityTypeBuilder<Campaign> builder)
    {
        builder.ToTable("campaign");

        // Primary key
        builder.HasKey(c => c.Id);

        // Properties
        builder.Property(c => c.Id)
        .HasConversion(c=> c.Value, c=> new CampaignId(c))
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

