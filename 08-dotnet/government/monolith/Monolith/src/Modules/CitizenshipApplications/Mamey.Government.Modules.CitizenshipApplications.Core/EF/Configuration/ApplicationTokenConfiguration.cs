using Mamey.Government.Modules.CitizenshipApplications.Core.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Mamey.Government.Modules.CitizenshipApplications.Core.EF.Configuration;
using AppId = Mamey.Government.Modules.CitizenshipApplications.Core.Domain.ValueObjects.ApplicationId;

internal class ApplicationTokenConfiguration : IEntityTypeConfiguration<ApplicationToken>
{
    public void Configure(EntityTypeBuilder<ApplicationToken> builder)
    {

        // Primary Key
        builder.HasKey(e => e.Id);
        builder.Property(e => e.Id)
            .ValueGeneratedNever();

        // ApplicationId conversion
        builder.Property(e => e.ApplicationId)
            .HasConversion(
                v => v.Value,
                v => new AppId(v))
            .IsRequired();

        // Properties
        builder.Property(e => e.TokenHash)
            .IsRequired()
            .HasMaxLength(64); // SHA256 produces 64 character hex string

        builder.Property(e => e.Email)
            .IsRequired()
            .HasMaxLength(255);

        builder.Property(e => e.ExpiresAt)
            .IsRequired();

        builder.Property(e => e.UsedAt)
            .IsRequired(false);

        builder.Property(e => e.CreatedAt)
            .IsRequired();

        // Indexes
        builder.HasIndex(e => e.TokenHash)
            .HasDatabaseName("IX_application_tokens_token_hash");

        builder.HasIndex(e => e.Email)
            .HasDatabaseName("IX_application_tokens_email");

        builder.HasIndex(e => e.ApplicationId)
            .HasDatabaseName("IX_application_tokens_application_id");

        // Composite index for lookup
        builder.HasIndex(e => new { e.TokenHash, e.Email })
            .HasDatabaseName("IX_application_tokens_token_hash_email");

        // Index for cleanup of expired tokens
        builder.HasIndex(e => e.ExpiresAt)
            .HasDatabaseName("IX_application_tokens_expires_at");

        // Index for used tokens
        builder.HasIndex(e => e.UsedAt)
            .HasDatabaseName("IX_application_tokens_used_at")
            .HasFilter("\"used_at\" IS NOT NULL");
    }
}
