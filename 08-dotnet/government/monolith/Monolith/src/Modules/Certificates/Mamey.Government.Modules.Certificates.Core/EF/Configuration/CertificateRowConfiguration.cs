using Mamey.Government.Modules.Certificates.Core.Domain.ValueObjects;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Mamey.Government.Modules.Certificates.Core.EF.Configuration;

internal class CertificateRowConfiguration : IEntityTypeConfiguration<CertificateRow>
{
    public void Configure(EntityTypeBuilder<CertificateRow> builder)
    {
        builder.ToTable("certificates", "certificates");
        
        builder.HasKey(e => e.Id);
        builder.Property(e => e.Id).ValueGeneratedNever();
        builder.Property(e => e.TenantId).IsRequired();
        builder.Property(e => e.CertificateNumber).IsRequired().HasMaxLength(100);
        builder.Property(e => e.CertificateType).HasConversion<int>();
        
        builder.HasIndex(e => e.CertificateNumber)
            .IsUnique()
            .HasDatabaseName("IX_certificates_certificate_number");
        
        builder.HasIndex(e => e.CitizenId)
            .HasDatabaseName("IX_certificates_citizen_id")
            .HasFilter("\"citizen_id\" IS NOT NULL");
        
        builder.HasIndex(e => e.TenantId)
            .HasDatabaseName("IX_certificates_tenant_id");
        
        builder.HasIndex(e => new { e.TenantId, e.CertificateType })
            .HasDatabaseName("IX_certificates_tenant_id_certificate_type");
    }
}
