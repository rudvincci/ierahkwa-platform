using System;
using Mamey.ServiceName.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Mamey.ServiceName.Infrastructure.EF.Configuration;

public class EntityNameConfiguration : IEntityTypeConfiguration<EntityName>
{
    public EntityNameConfiguration()
    {
    }

    void IEntityTypeConfiguration<EntityName>.Configure(EntityTypeBuilder<EntityName> builder)
    {
        builder.ToTable("entityname");

        // Primary key
        builder.HasKey(c => c.Id);

        // Properties
        builder.Property(c => c.Id)
        .HasConversion(c=> c.Value, c=> new EntityNameId(c))
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

