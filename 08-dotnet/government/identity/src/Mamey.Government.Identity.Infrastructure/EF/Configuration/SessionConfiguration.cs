using Mamey.Government.Identity.Domain.Entities;
using Mamey.Types;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Mamey.Government.Identity.Infrastructure.EF.Configuration;

public class SessionConfiguration : IEntityTypeConfiguration<Session>
{
    void IEntityTypeConfiguration<Session>.Configure(EntityTypeBuilder<Session> builder)
    {
        builder.ToTable("sessions");

        // Primary key
        builder.HasKey(s => s.Id);

        // Properties with value conversions
        builder.Property(s => s.Id)
            .HasConversion(
                id => id.Value,
                value => new SessionId(value))
            .IsRequired();

        builder.Property(s => s.UserId)
            .HasConversion(
                id => id.Value,
                value => new UserId(value))
            .IsRequired();

        builder.Property(s => s.AccessToken)
            .HasColumnType("text")
            .IsRequired();

        builder.Property(s => s.RefreshToken)
            .HasColumnType("text")
            .IsRequired();

        builder.Property(s => s.IpAddress)
            .HasMaxLength(45);

        builder.Property(s => s.UserAgent)
            .HasMaxLength(500);

        builder.Property(s => s.Status)
            .HasConversion<int>();

        // Indexes
        builder.HasIndex(s => s.AccessToken).IsUnique();
        builder.HasIndex(s => s.RefreshToken).IsUnique();
        builder.HasIndex(s => s.UserId);

        // Ignore domain events
        builder.Ignore(s => s.Events);
    }
}
