using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Mamey.Government.Modules.Tenant.Core.EF.Configuration;

internal class TenantInviteRowConfiguration : IEntityTypeConfiguration<TenantInviteRow>
{
    public void Configure(EntityTypeBuilder<TenantInviteRow> builder)
    {
        builder.ToTable("tenant_invites", "tenant");
        
        builder.HasKey(e => e.Id);
        builder.Property(e => e.Id).ValueGeneratedNever();
        builder.Property(e => e.TenantId).IsRequired();
        builder.Property(e => e.Email).IsRequired().HasMaxLength(255);
        builder.Property(e => e.Role).HasMaxLength(100);
        
        builder.HasIndex(e => new { e.TenantId, e.Email })
            .HasDatabaseName("IX_tenant_invites_tenant_id_email");
    }
}
