using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Mamey.Government.Modules.Tenant.Core.EF.Configuration;

internal class TenantUserMappingRowConfiguration : IEntityTypeConfiguration<TenantUserMappingRow>
{
    public void Configure(EntityTypeBuilder<TenantUserMappingRow> builder)
    {
        builder.ToTable("tenant_user_mappings", "tenant");
        
        builder.HasKey(e => new { e.Issuer, e.Subject });
        builder.Property(e => e.Issuer).IsRequired().HasMaxLength(500);
        builder.Property(e => e.Subject).IsRequired().HasMaxLength(500);
        builder.Property(e => e.TenantId).IsRequired();
        builder.Property(e => e.Email).HasMaxLength(255);
        
        builder.HasIndex(e => e.TenantId)
            .HasDatabaseName("IX_tenant_user_mappings_tenant_id");
        
        builder.HasIndex(e => e.Email)
            .HasDatabaseName("IX_tenant_user_mappings_email")
            .HasFilter("\"email\" IS NOT NULL");
    }
}
