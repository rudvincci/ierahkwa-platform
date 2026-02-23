using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Mamey.Government.Modules.Documents.Core.EF.Configuration;

internal class DocumentRowConfiguration : IEntityTypeConfiguration<DocumentRow>
{
    public void Configure(EntityTypeBuilder<DocumentRow> builder)
    {
        builder.ToTable("documents", "documents");
        
        builder.HasKey(e => e.Id);
        builder.Property(e => e.Id).ValueGeneratedNever();
        builder.Property(e => e.TenantId).IsRequired();
        builder.Property(e => e.FileName).IsRequired().HasMaxLength(255);
        builder.Property(e => e.ContentType).IsRequired().HasMaxLength(100);
        builder.Property(e => e.StorageBucket).IsRequired().HasMaxLength(100);
        builder.Property(e => e.StorageKey).IsRequired().HasMaxLength(500);
        builder.Property(e => e.Category).HasMaxLength(100);
        builder.Property(e => e.MetadataJson).HasColumnType("jsonb");
        
        builder.HasIndex(e => e.TenantId)
            .HasDatabaseName("IX_documents_tenant_id");
        
        builder.HasIndex(e => e.Category)
            .HasDatabaseName("IX_documents_category")
            .HasFilter("\"category\" IS NOT NULL");
        
        builder.HasIndex(e => e.StorageKey)
            .HasDatabaseName("IX_documents_storage_key");
    }
}
