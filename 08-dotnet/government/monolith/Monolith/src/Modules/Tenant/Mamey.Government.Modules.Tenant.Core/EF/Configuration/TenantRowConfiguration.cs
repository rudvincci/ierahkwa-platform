using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Mamey.Government.Modules.Tenant.Core.EF.Configuration;

internal class TenantRowConfiguration : IEntityTypeConfiguration<TenantRow>
{
    public void Configure(EntityTypeBuilder<TenantRow> builder)
    {
        builder.ToTable("tenants", "tenant");
        
        builder.HasKey(e => e.Id);
        builder.Property(e => e.Id).ValueGeneratedNever();
        builder.Property(e => e.DisplayName).IsRequired().HasMaxLength(255);
        builder.Property(e => e.Domain).HasMaxLength(255);
        
        builder.HasIndex(e => e.Domain)
            .IsUnique()
            .HasDatabaseName("IX_tenants_domain")
            .HasFilter("\"domain\" IS NOT NULL");
    }
}
