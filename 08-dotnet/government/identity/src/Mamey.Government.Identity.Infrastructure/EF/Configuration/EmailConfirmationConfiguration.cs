using Mamey.Government.Identity.Domain.Entities;
using Mamey.Types;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Mamey.Government.Identity.Infrastructure.EF.Configuration;

public class EmailConfirmationConfiguration : IEntityTypeConfiguration<EmailConfirmation>
{
    void IEntityTypeConfiguration<EmailConfirmation>.Configure(EntityTypeBuilder<EmailConfirmation> builder)
    {
        builder.ToTable("email_confirmations");

        // Primary key
        builder.HasKey(e => e.Id);

        // Properties with value conversions
        builder.Property(e => e.Id)
            .HasConversion(
                id => id.Value,
                value => new EmailConfirmationId(value))
            .IsRequired();

        builder.Property(e => e.UserId)
            .HasConversion(
                id => id.Value,
                value => new UserId(value))
            .IsRequired();

        builder.Property(e => e.Email)
            .HasMaxLength(100)
            .IsRequired();

        builder.Property(e => e.ConfirmationCode)
            .HasMaxLength(10)
            .IsRequired();

        builder.Property(e => e.IpAddress)
            .HasMaxLength(45);

        builder.Property(e => e.UserAgent)
            .HasMaxLength(500);

        builder.Property(e => e.Status)
            .HasConversion<int>();

        // Indexes
        builder.HasIndex(e => e.UserId);
        builder.HasIndex(e => e.Email);
        builder.HasIndex(e => e.ConfirmationCode).IsUnique();

        // Ignore domain events
        builder.Ignore(e => e.Events);
    }
}
