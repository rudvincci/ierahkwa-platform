using Mamey.Government.Identity.Domain.Entities;
using Mamey.Types;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Mamey.Government.Identity.Infrastructure.EF.Configuration;

public class TwoFactorAuthConfiguration : IEntityTypeConfiguration<TwoFactorAuth>
{
    void IEntityTypeConfiguration<TwoFactorAuth>.Configure(EntityTypeBuilder<TwoFactorAuth> builder)
    {
        builder.ToTable("two_factor_auths");

        // Primary key
        builder.HasKey(t => t.Id);

        // Properties with value conversions
        builder.Property(t => t.Id)
            .HasConversion(
                id => id.Value,
                value => new TwoFactorAuthId(value))
            .IsRequired();

        builder.Property(t => t.UserId)
            .HasConversion(
                id => id.Value,
                value => new UserId(value))
            .IsRequired();

        builder.Property(t => t.SecretKey)
            .HasMaxLength(100)
            .IsRequired();

        builder.Property(t => t.Status)
            .HasConversion<int>();

        // Indexes
        builder.HasIndex(t => t.UserId).IsUnique();

        // Ignore domain events
        builder.Ignore(t => t.Events);
    }
}
