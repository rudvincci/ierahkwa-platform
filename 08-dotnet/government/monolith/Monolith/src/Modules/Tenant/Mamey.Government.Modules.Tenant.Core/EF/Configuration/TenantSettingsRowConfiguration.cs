using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Mamey.Government.Modules.Tenant.Core.EF.Configuration;

internal class TenantSettingsRowConfiguration : IEntityTypeConfiguration<TenantSettingsRow>
{
    public void Configure(EntityTypeBuilder<TenantSettingsRow> builder)
    {
        builder.ToTable("tenant_settings", "tenant");
        
        builder.HasKey(e => e.TenantId);
        builder.Property(e => e.TenantId).ValueGeneratedNever();
        builder.Property(e => e.BrandingJson).IsRequired();
        
        builder.HasOne<TenantRow>()
            .WithOne()
            .HasForeignKey<TenantSettingsRow>(e => e.TenantId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
