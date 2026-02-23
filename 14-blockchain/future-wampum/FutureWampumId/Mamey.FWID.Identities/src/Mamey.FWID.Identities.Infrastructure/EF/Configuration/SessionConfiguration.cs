using Mamey.FWID.Identities.Domain.Entities;
using Mamey.FWID.Identities.Domain.ValueObjects;
using Mamey.Persistence.SQL;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using static Mamey.EnumExtensions;

namespace Mamey.FWID.Identities.Infrastructure.EF.Configuration;

internal class SessionConfiguration : IEntityTypeConfiguration<Session>
{
    void IEntityTypeConfiguration<Session>.Configure(EntityTypeBuilder<Session> builder)
    {
        builder.ToTable("session");

        // Primary key
        builder.HasKey(c => c.Id);

        // Properties
        builder.Property(c => c.Id)
            .HasConversion(c => c.Value, c => new SessionId(c))
            .IsRequired();

        builder.Property(c => c.IdentityId)
            .HasConversion(c => c.Value, c => new IdentityId(c))
            .IsRequired();

        builder.Property(c => c.AccessToken)
            .HasMaxLength(2000)
            .IsRequired();

        builder.Property(c => c.RefreshToken)
            .HasMaxLength(500)
            .IsRequired();

        builder.Property(c => c.ExpiresAt)
            .IsRequired();

        // Store SessionStatus enum as string
        builder.Property(c => c.Status)
            .HasConversion(
                v => v.ToString(),
                v => v.ToEnum<SessionStatus>())
            .HasMaxLength(50)
            .IsRequired();

        builder.Property(c => c.CreatedAt)
            .IsRequired();

        builder.Property(c => c.LastAccessedAt)
            .IsRequired();

        builder.Property(c => c.RevokedAt);

        builder.Property(c => c.IpAddress)
            .HasMaxLength(45);

        builder.Property(c => c.UserAgent)
            .HasMaxLength(500);

        // Configure Version property as concurrency token
        builder.Property(c => c.Version)
            .IsConcurrencyToken()
            .HasColumnName("version")
            .IsRequired();

        // Indexes
        builder.HasIndex(c => c.IdentityId);
        builder.HasIndex(c => c.RefreshToken).IsUnique();
        builder.HasIndex(c => c.Status);
        builder.HasIndex(c => c.ExpiresAt);

        // Ignore domain events
        builder.Ignore(c => c.Events);
    }
}









