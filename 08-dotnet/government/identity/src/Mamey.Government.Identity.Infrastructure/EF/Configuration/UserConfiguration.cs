using Mamey.Government.Identity.Domain.Entities;
using Mamey.Types;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Mamey.Government.Identity.Infrastructure.EF.Configuration;

public class UserConfiguration : IEntityTypeConfiguration<User>
{
    void IEntityTypeConfiguration<User>.Configure(EntityTypeBuilder<User> builder)
    {
        builder.ToTable("users");

        // Primary key
        builder.HasKey(u => u.Id);

        // Properties with value conversions
        builder.Property(u => u.Id)
            .HasConversion(
                id => id.Value,
                value => new UserId(value))
            .IsRequired();

        builder.Property(u => u.SubjectId)
            .HasConversion(
                id => id.Value,
                value => new SubjectId(value))
            .IsRequired();

        builder.Property(u => u.Email)
            .HasConversion(
                email => email.Value,
                value => new Email(value))
            .HasMaxLength(100)
            .IsRequired();

        builder.Property(u => u.Username)
            .HasMaxLength(50)
            .IsRequired();

        builder.Property(u => u.PasswordHash)
            .HasMaxLength(255)
            .IsRequired();

        builder.Property(u => u.Status)
            .HasConversion<int>();

        // Indexes
        builder.HasIndex(u => u.Username).IsUnique();
        builder.HasIndex(u => u.Email).IsUnique();
        builder.HasIndex(u => u.SubjectId);

        // Ignore domain events
        builder.Ignore(u => u.Events);
    }
}
