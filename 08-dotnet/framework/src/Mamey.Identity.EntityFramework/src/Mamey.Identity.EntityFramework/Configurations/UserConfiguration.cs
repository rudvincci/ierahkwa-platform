using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Mamey.Identity.EntityFramework.Entities;

namespace Mamey.Identity.EntityFramework.Configurations;

public class UserConfiguration : IEntityTypeConfiguration<ApplicationUser>
{
    public void Configure(EntityTypeBuilder<ApplicationUser> builder)
    {
        builder.ToTable("Users");

        builder.Property(u => u.FirstName)
            .IsRequired()
            .HasMaxLength(100);

        builder.Property(u => u.LastName)
            .IsRequired()
            .HasMaxLength(100);

        builder.Property(u => u.FullName)
            .HasMaxLength(200);

        builder.Property(u => u.TenantId)
            .IsRequired(false);

        builder.Property(u => u.CreatedAt)
            .IsRequired()
            .HasDefaultValueSql("CURRENT_TIMESTAMP");

        builder.Property(u => u.LastLoginAt)
            .IsRequired(false);

        builder.Property(u => u.IsActive)
            .IsRequired()
            .HasDefaultValue(true);

        builder.Property(u => u.ProfilePictureUrl)
            .HasMaxLength(500);

        builder.Property(u => u.TimeZone)
            .HasMaxLength(50);

        builder.Property(u => u.Locale)
            .HasMaxLength(10);

        // Indexes
        builder.HasIndex(u => u.TenantId);
        builder.HasIndex(u => u.Email);
        builder.HasIndex(u => u.UserName);
        builder.HasIndex(u => u.IsActive);
        builder.HasIndex(u => u.CreatedAt);

        // Relationships
        builder.HasMany(u => u.UserRoles)
            .WithOne(ur => ur.User)
            .HasForeignKey(ur => ur.UserId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasMany(u => u.UserClaims)
            .WithOne(uc => uc.User)
            .HasForeignKey(uc => uc.UserId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasMany(u => u.UserLogins)
            .WithOne(ul => ul.User)
            .HasForeignKey(ul => ul.UserId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasMany(u => u.UserTokens)
            .WithOne(ut => ut.User)
            .HasForeignKey(ut => ut.UserId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}

