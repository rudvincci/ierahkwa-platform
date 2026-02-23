using Mamey.Government.Modules.Certificates.Core.Domain.Entities;
using Mamey.Types;

namespace Mamey.Government.Modules.Certificates.Core.Mongo.Documents;

internal class CertificateDocument : MicroMonolith.Infrastructure.Mongo.IIdentifiable<Guid>
{
    public CertificateDocument()
    {
    }

    public CertificateDocument(Certificate certificate)
    {
        Id = certificate.Id.Value;
        TenantId = certificate.TenantId.Value;
        CitizenId = certificate.CitizenId;
        CertificateType = certificate.CertificateType.ToString();
        CertificateNumber = certificate.CertificateNumber;
        IssuedDate = certificate.IssuedDate;
        DocumentPath = certificate.DocumentPath;
        IsActive = certificate.IsActive;
        RevokedAt = certificate.RevokedAt;
        RevocationReason = certificate.RevocationReason;
        CreatedAt = certificate.CreatedAt;
        UpdatedAt = certificate.UpdatedAt;
    }

    public Guid Id { get; set; }
    public Guid TenantId { get; set; }
    public Guid? CitizenId { get; set; }
    public string CertificateType { get; set; } = string.Empty;
    public string CertificateNumber { get; set; } = string.Empty;
    public DateTime IssuedDate { get; set; }
    public string? DocumentPath { get; set; }
    public bool IsActive { get; set; }
    public DateTime? RevokedAt { get; set; }
    public string? RevocationReason { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }

    public Certificate AsEntity()
    {
        var certificateId = new Domain.ValueObjects.CertificateId(Id);
        var tenantId = new TenantId(TenantId);
        var citizenId = CitizenId.HasValue ? CitizenId : null;
        var certificateType = Enum.Parse<Domain.ValueObjects.CertificateType>(CertificateType);
        
        var certificate = new Certificate(
            certificateId,
            tenantId,
            citizenId,
            certificateType,
            CertificateNumber,
            IssuedDate,
            DocumentPath);
        
        typeof(Certificate).GetProperty("IsActive")?.SetValue(certificate, IsActive);
        typeof(Certificate).GetProperty("RevokedAt")?.SetValue(certificate, RevokedAt);
        typeof(Certificate).GetProperty("RevocationReason")?.SetValue(certificate, RevocationReason);
        typeof(Certificate).GetProperty("CreatedAt")?.SetValue(certificate, CreatedAt);
        typeof(Certificate).GetProperty("UpdatedAt")?.SetValue(certificate, UpdatedAt);
        
        return certificate;
    }
}
