using Mamey.Government.Modules.Citizens.Core.Domain.ValueObjects;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Mamey.Government.Modules.Citizens.Core.EF.Configuration;

internal class CitizenRowConfiguration : IEntityTypeConfiguration<CitizenRow>
{
    public void Configure(EntityTypeBuilder<CitizenRow> builder)
    {       
        builder.HasKey(e => e.Id);
        builder.Property(e => e.Id).ValueGeneratedNever();
        builder.Property(e => e.TenantId).IsRequired();
        builder.Property(e => e.FirstName).IsRequired().HasMaxLength(255);
        builder.Property(e => e.LastName).IsRequired().HasMaxLength(255);
        builder.Property(e => e.Email).HasMaxLength(255);
        builder.Property(e => e.Phone).HasMaxLength(50);
        builder.Property(e => e.Status).HasConversion<int>();
        builder.Property(e => e.PhotoPath).HasMaxLength(500);
        
        builder.HasIndex(e => e.TenantId)
            .HasDatabaseName("IX_citizens_tenant_id");
        
        builder.HasIndex(e => new { e.TenantId, e.Status })
            .HasDatabaseName("IX_citizens_tenant_id_status");
    }
}
