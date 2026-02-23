using Mamey.Government.Identity.Domain.Entities;
using Mamey.Types;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Mamey.Government.Identity.Infrastructure.EF.Configuration;

public class MfaChallengeConfiguration : IEntityTypeConfiguration<MfaChallenge>
{
    void IEntityTypeConfiguration<MfaChallenge>.Configure(EntityTypeBuilder<MfaChallenge> builder)
    {
        builder.ToTable("mfa_challenges");

        // Primary key
        builder.HasKey(m => m.Id);

        // Properties with value conversions
        builder.Property(m => m.Id)
            .HasConversion(
                id => id.Value,
                value => new MfaChallengeId(value))
            .IsRequired();

        builder.Property(m => m.MultiFactorAuthId)
            .HasConversion(
                id => id.Value,
                value => new MultiFactorAuthId(value))
            .IsRequired();

        builder.Property(m => m.ChallengeData)
            .HasMaxLength(100)
            .IsRequired();

        builder.Property(m => m.Method)
            .HasConversion<int>();

        builder.Property(m => m.IpAddress)
            .HasMaxLength(45);

        builder.Property(m => m.UserAgent)
            .HasMaxLength(500);

        builder.Property(m => m.Status)
            .HasConversion<int>();

        // Indexes
        builder.HasIndex(m => m.MultiFactorAuthId);
        builder.HasIndex(m => m.ChallengeData).IsUnique();

        // Ignore domain events
        builder.Ignore(m => m.Events);
    }
}
