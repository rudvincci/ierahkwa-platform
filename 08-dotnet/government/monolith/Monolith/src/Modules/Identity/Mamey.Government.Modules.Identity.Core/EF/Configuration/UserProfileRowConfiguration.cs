using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Mamey.Government.Modules.Identity.Core.EF.Configuration;

internal class UserProfileRowConfiguration : IEntityTypeConfiguration<UserProfileRow>
{
    public void Configure(EntityTypeBuilder<UserProfileRow> builder)
    {
        builder.ToTable("user_profiles", "identity");
        
        builder.HasKey(e => e.Id);
        builder.Property(e => e.Id)
            .ValueGeneratedNever()
            .HasColumnName("id");
        builder.Property(e => e.AuthenticatorIssuer)
            .IsRequired()
            .HasMaxLength(500)
            .HasColumnName("authenticator_issuer");
        builder.Property(e => e.AuthenticatorSubject)
            .IsRequired()
            .HasMaxLength(500)
            .HasColumnName("authenticator_subject");
        builder.Property(e => e.Email)
            .HasMaxLength(255)
            .HasColumnName("email");
        builder.Property(e => e.DisplayName)
            .HasMaxLength(255)
            .HasColumnName("display_name");
        builder.Property(e => e.TenantId)
            .HasColumnName("tenant_id");
        builder.Property(e => e.CreatedAt)
            .HasColumnName("created_at");
        builder.Property(e => e.UpdatedAt)
            .HasColumnName("updated_at");
        builder.Property(e => e.LastLoginAt)
            .HasColumnName("last_login_at");
        builder.Property(e => e.Version)
            .HasColumnName("version");
        
        builder.HasIndex(e => new { e.AuthenticatorIssuer, e.AuthenticatorSubject })
            .IsUnique()
            .HasDatabaseName("IX_user_profiles_authenticator_issuer_subject");
        
        builder.HasIndex(e => e.Email)
            .IsUnique()
            .HasDatabaseName("IX_user_profiles_email")
            .HasFilter("\"email\" IS NOT NULL");
        
        builder.HasIndex(e => e.TenantId)
            .HasDatabaseName("IX_user_profiles_tenant_id");
    }
}
