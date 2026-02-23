using Mamey.FWID.Identities.Domain.Entities;
using Mamey.FWID.Identities.Domain.ValueObjects;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using static Mamey.EnumExtensions;

namespace Mamey.FWID.Identities.Infrastructure.EF.Configuration;

internal class SmsConfirmationConfiguration : IEntityTypeConfiguration<SmsConfirmation>
{
    void IEntityTypeConfiguration<SmsConfirmation>.Configure(EntityTypeBuilder<SmsConfirmation> builder)
    {
        builder.ToTable("sms_confirmation");

        // Primary key
        builder.HasKey(c => c.Id);

        // Properties
        builder.Property(c => c.Id)
            .HasConversion(c => c.Value, c => new SmsConfirmationId(c))
            .IsRequired();

        builder.Property(c => c.IdentityId)
            .HasConversion(c => c.Value, c => new IdentityId(c))
            .IsRequired();

        builder.Property(c => c.PhoneNumber)
            .HasMaxLength(50)
            .IsRequired();

        builder.Property(c => c.Code)
            .HasMaxLength(10)
            .IsRequired();

        builder.Property(c => c.ExpiresAt)
            .IsRequired();

        // Store ConfirmationStatus enum as string
        builder.Property(c => c.Status)
            .HasConversion(
                v => v.ToString(),
                v => v.ToEnum<ConfirmationStatus>())
            .HasMaxLength(50)
            .IsRequired();

        builder.Property(c => c.CreatedAt)
            .IsRequired();

        builder.Property(c => c.ConfirmedAt);

        builder.Property(c => c.VerificationAttempts)
            .IsRequired()
            .HasDefaultValue(0);

        // Configure Version property as concurrency token
        builder.Property(c => c.Version)
            .IsConcurrencyToken()
            .HasColumnName("version")
            .IsRequired();

        // Indexes
        builder.HasIndex(c => c.IdentityId);
        builder.HasIndex(c => c.PhoneNumber);
        builder.HasIndex(c => c.Code);
        builder.HasIndex(c => c.Status);
        builder.HasIndex(c => c.ExpiresAt);

        // Ignore domain events
        builder.Ignore(c => c.Events);
    }
}









