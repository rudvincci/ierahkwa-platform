using Mamey.Government.Modules.CitizenshipApplications.Core.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Mamey.Government.Modules.CitizenshipApplications.Core.EF.Configuration;
using AppId = Mamey.Government.Modules.CitizenshipApplications.Core.Domain.ValueObjects.ApplicationId;

internal class UploadedDocumentConfiguration : IEntityTypeConfiguration<UploadedDocument>
{
    public void Configure(EntityTypeBuilder<UploadedDocument> builder)
    {

        // Primary Key
        builder.HasKey(e => e.Id);
        builder.Property(e => e.Id)
            .ValueGeneratedNever();
        builder.Property(e => e.ApplicationId)
            .HasConversion(
                v => v.Value,
                v => new AppId(v))
            .ValueGeneratedNever();
        // Properties
        builder.Property(e => e.FileName)
            .IsRequired()
            .HasMaxLength(255);

        builder.Property(e => e.StoragePath)
            .IsRequired()
            .HasMaxLength(500);

        builder.Property(e => e.DocumentType)
            .IsRequired()
            .HasMaxLength(100);

        builder.Property(e => e.FileSize)
            .IsRequired();

        builder.Property(e => e.UploadedAt)
            .IsRequired();
        
        // Navigation property relationship is configured in CitizenshipApplicationsConfiguration
        // to ensure proper value object conversion handling

        // Indexes
        builder.HasIndex(c=> c.ApplicationId)
            .HasDatabaseName("IX_uploaded_documents_application_id");

        builder.HasIndex(e => e.DocumentType)
            .HasDatabaseName("IX_uploaded_documents_document_type");

        builder.HasIndex(e => new { e.ApplicationId, e.DocumentType })
            .HasDatabaseName("IX_uploaded_documents_application_id_document_type");

        builder.HasIndex(e => e.UploadedAt)
            .HasDatabaseName("IX_uploaded_documents_uploaded_at");
    }
}
