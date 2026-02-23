using Mamey.Government.Modules.Certificates.Core.Domain.Events;
using Mamey.Government.Modules.Certificates.Core.Domain.ValueObjects;
using Mamey.Government.Modules.Citizens.Core.Domain.ValueObjects;
using Mamey.Government.Modules.Tenant.Core.Domain.ValueObjects;
using Mamey.Types;
using TenantId = Mamey.Types.TenantId;

namespace Mamey.Government.Modules.Certificates.Core.Domain.Entities;

/// <summary>
/// Certificate aggregate root - represents an issued certificate.
/// </summary>
internal class Certificate : AggregateRoot<CertificateId>
{
    private Certificate() { }

    public Certificate(
        CertificateId id,
        TenantId tenantId,
        Guid? citizenId,
        CertificateType certificateType,
        string certificateNumber,
        DateTime issuedDate,
        string? documentPath = null,
        int version = 0)
        : base(id, version)
    {
        TenantId = tenantId;
        CitizenId = citizenId;
        CertificateType = certificateType;
        CertificateNumber = certificateNumber;
        IssuedDate = issuedDate;
        DocumentPath = documentPath;
        IsActive = true;
        CreatedAt = DateTime.UtcNow;
        UpdatedAt = DateTime.UtcNow;
        
        AddEvent(new CertificateIssued(Id, CitizenId, CertificateType, CertificateNumber));
    }

    public TenantId TenantId { get; private set; }
    public Guid? CitizenId { get; private set; }
    public CertificateType CertificateType { get; private set; }
    public string CertificateNumber { get; private set; } = string.Empty;
    public DateTime IssuedDate { get; private set; }
    public string? DocumentPath { get; private set; } // MinIO path to generated PDF
    public bool IsActive { get; private set; }
    public DateTime? RevokedAt { get; private set; }
    public string? RevocationReason { get; private set; }
    public DateTime CreatedAt { get; private set; }
    public DateTime UpdatedAt { get; private set; }

    public void Revoke(string reason)
    {
        IsActive = false;
        RevokedAt = DateTime.UtcNow;
        RevocationReason = reason;
        UpdatedAt = DateTime.UtcNow;
        
        AddEvent(new CertificateRevoked(Id, reason));
    }

    public void UpdateDocumentPath(string documentPath)
    {
        DocumentPath = documentPath;
        UpdatedAt = DateTime.UtcNow;
        
        AddEvent(new CertificateModified(this));
    }
}
