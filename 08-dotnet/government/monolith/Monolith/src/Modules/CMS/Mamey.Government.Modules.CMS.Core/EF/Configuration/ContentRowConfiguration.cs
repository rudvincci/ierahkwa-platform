using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Mamey.Government.Modules.CMS.Core.EF.Configuration;

internal class ContentRowConfiguration : IEntityTypeConfiguration<ContentRow>
{
    public void Configure(EntityTypeBuilder<ContentRow> builder)
    {
        builder.ToTable("contents", "cms");
        
        builder.HasKey(e => e.Id);
        builder.Property(e => e.Id).ValueGeneratedNever();
        builder.Property(e => e.TenantId).IsRequired();
        builder.Property(e => e.Title).IsRequired().HasMaxLength(255);
        builder.Property(e => e.Slug).IsRequired().HasMaxLength(255);
        builder.Property(e => e.ContentType).IsRequired().HasMaxLength(100);
        builder.Property(e => e.Status).HasConversion<int>();
        builder.Property(e => e.MetadataJson).HasColumnType("jsonb");
        
        builder.HasIndex(e => new { e.TenantId, e.Slug })
            .IsUnique()
            .HasDatabaseName("IX_contents_tenant_id_slug");
        
        builder.HasIndex(e => e.TenantId)
            .HasDatabaseName("IX_contents_tenant_id");
        
        builder.HasIndex(e => new { e.TenantId, e.Status })
            .HasDatabaseName("IX_contents_tenant_id_status");
        
        builder.HasIndex(e => new { e.TenantId, e.ContentType })
            .HasDatabaseName("IX_contents_tenant_id_content_type");
    }
}
