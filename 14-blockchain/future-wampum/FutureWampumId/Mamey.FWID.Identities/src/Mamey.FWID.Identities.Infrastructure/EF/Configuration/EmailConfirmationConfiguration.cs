using Mamey.FWID.Identities.Domain.Entities;
using Mamey.FWID.Identities.Domain.ValueObjects;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using static Mamey.EnumExtensions;

namespace Mamey.FWID.Identities.Infrastructure.EF.Configuration;

internal class EmailConfirmationConfiguration : IEntityTypeConfiguration<EmailConfirmation>
{
    void IEntityTypeConfiguration<EmailConfirmation>.Configure(EntityTypeBuilder<EmailConfirmation> builder)
    {
        builder.ToTable("email_confirmation");

        // Primary key
        builder.HasKey(c => c.Id);

        // Properties
        builder.Property(c => c.Id)
            .HasConversion(c => c.Value, c => new EmailConfirmationId(c))
            .IsRequired();

        builder.Property(c => c.IdentityId)
            .HasConversion(c => c.Value, c => new IdentityId(c))
            .IsRequired();

        builder.Property(c => c.Email)
            .HasMaxLength(255)
            .IsRequired();

        builder.Property(c => c.Token)
            .HasMaxLength(256)
            .IsRequired();

        builder.Property(c => c.ExpiresAt)
            .IsRequired();

        // Store Status enum as string instead of int
        builder.Property(c => c.Status)
            .HasConversion(
                v => v.ToString(),
                v => v.ToEnum<ConfirmationStatus>())
            .HasMaxLength(50)
            .IsRequired();

        builder.Property(c => c.CreatedAt)
            .IsRequired();

        builder.Property(c => c.ConfirmedAt);

        // Indexes
        builder.HasIndex(c => c.IdentityId);
        builder.HasIndex(c => c.Token).IsUnique();
        builder.HasIndex(c => c.Status);
        builder.HasIndex(c => c.ExpiresAt);

        // Ignore domain events
        builder.Ignore(c => c.Events);
    }
}










