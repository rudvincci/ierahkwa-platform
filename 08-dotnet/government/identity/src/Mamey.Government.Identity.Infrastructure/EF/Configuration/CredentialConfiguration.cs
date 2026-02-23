using Mamey.Government.Identity.Domain.Entities;
using Mamey.Types;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Mamey.Government.Identity.Infrastructure.EF.Configuration;

public class CredentialConfiguration : IEntityTypeConfiguration<Credential>
{
    void IEntityTypeConfiguration<Credential>.Configure(EntityTypeBuilder<Credential> builder)
    {
        builder.ToTable("credentials");

        // Primary key
        builder.HasKey(c => c.Id);

        // Properties with value conversions
        builder.Property(c => c.Id)
            .HasConversion(
                id => id.Value,
                value => new CredentialId(value))
            .IsRequired();

        builder.Property(c => c.UserId)
            .HasConversion(
                id => id.Value,
                value => new UserId(value))
            .IsRequired();

        builder.Property(c => c.Type)
            .HasConversion<int>();

        builder.Property(c => c.Value)
            .HasMaxLength(500)
            .IsRequired();

        builder.Property(c => c.Status)
            .HasConversion<int>();

        // Indexes
        builder.HasIndex(c => c.UserId);

        // Ignore domain events
        builder.Ignore(c => c.Events);
    }
}
