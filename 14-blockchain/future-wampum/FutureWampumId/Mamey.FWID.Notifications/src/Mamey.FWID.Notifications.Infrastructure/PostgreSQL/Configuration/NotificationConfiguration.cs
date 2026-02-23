using Mamey.FWID.Identities.Domain.Entities;
using Mamey.FWID.Notifications.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Mamey.FWID.Notifications.Infrastructure.PostgreSQL.Configuration;

/// <summary>
/// Entity Framework Core configuration for the Notification entity.
/// </summary>
internal class NotificationConfiguration : IEntityTypeConfiguration<Notification>
{
    public void Configure(EntityTypeBuilder<Notification> builder)
    {
        builder.ToTable("Notifications");

        builder.HasKey(n => n.Id);

        // Store NotificationId.Value and IdentityId separately
        // When loading, we'll reconstruct NotificationId using the stored IdentityId in the repository
        // Use a custom value converter that handles the conversion
        builder.Property(n => n.Id)
            .HasConversion(
                id => id.Value,
                value => new NotificationId(value, new IdentityId(value))) // Temporary: will be fixed in repository after loading
            .IsRequired();

        builder.Property(n => n.IdentityId)
            .HasConversion(
                id => id.Value,
                value => new IdentityId(value))
            .IsRequired();

        builder.Property(n => n.Title)
            .IsRequired()
            .HasMaxLength(500);

        builder.Property(n => n.Description)
            .HasMaxLength(2000);

        builder.Property(n => n.Message)
            .IsRequired()
            .HasMaxLength(5000);

        builder.Property(n => n.Type)
            .HasConversion<int>()
            .IsRequired();

        builder.Property(n => n.Status)
            .HasConversion<int>()
            .IsRequired();

        builder.Property(n => n.CreatedAt)
            .IsRequired();

        builder.Property(n => n.SentAt);

        builder.Property(n => n.ReadAt);

        builder.Property(n => n.IsRead)
            .IsRequired()
            .HasDefaultValue(false);

        builder.Property(n => n.RelatedEntityType)
            .HasMaxLength(100);

        builder.Property(n => n.RelatedEntityId);

        builder.Ignore(n => n.Events);
        builder.Ignore(n => n.Version);
    }
}

