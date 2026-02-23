using Mamey.Government.Modules.Citizens.Core.Domain.ValueObjects;
using Mamey.Government.Modules.TravelIdentities.Core.Domain.Events;
using Mamey.Government.Modules.TravelIdentities.Core.Domain.ValueObjects;
using Mamey.Types;

namespace Mamey.Government.Modules.TravelIdentities.Core.Domain.Entities;

/// <summary>
/// Travel Identity aggregate root - AAMVA-compliant ID card.
/// Issued to citizens for domestic travel and identification.
/// </summary>
internal class TravelIdentity : AggregateRoot<TravelIdentityId>
{
    private TravelIdentity() { }

    public TravelIdentity(
        TravelIdentityId id,
        TenantId tenantId,
        Guid citizenId,
        TravelIdentityNumber travelIdentityNumber,
        DateTime issuedDate,
        DateTime expiryDate,
        string? pdf417Barcode = null,
        int version = 0)
        : base(id, version)
    {
        TenantId = tenantId;
        CitizenId = citizenId;
        TravelIdentityNumber = travelIdentityNumber;
        IssuedDate = issuedDate;
        ExpiryDate = expiryDate;
        Pdf417Barcode = pdf417Barcode;
        IsActive = true;
        CreatedAt = DateTime.UtcNow;
        UpdatedAt = DateTime.UtcNow;
        
        AddEvent(new TravelIdentityIssued(Id, CitizenId, TravelIdentityNumber));
    }

    public TenantId TenantId { get; private set; }
    public Guid CitizenId { get; private set; }
    public TravelIdentityNumber TravelIdentityNumber { get; private set; } = null!;
    public DateTime IssuedDate { get; private set; }
    public DateTime ExpiryDate { get; private set; }
    public string? Pdf417Barcode { get; private set; } // AAMVA PDF417 barcode data
    public string? DocumentPath { get; private set; } // MinIO path to generated ID card PDF
    public bool IsActive { get; private set; }
    public DateTime? RevokedAt { get; private set; }
    public string? RevocationReason { get; private set; }
    public DateTime CreatedAt { get; private set; }
    public DateTime UpdatedAt { get; private set; }

    public void Renew(DateTime newExpiryDate)
    {
        ExpiryDate = newExpiryDate;
        UpdatedAt = DateTime.UtcNow;
        
        AddEvent(new TravelIdentityRenewed(Id, ExpiryDate));
    }

    public void Revoke(string reason)
    {
        IsActive = false;
        RevokedAt = DateTime.UtcNow;
        RevocationReason = reason;
        UpdatedAt = DateTime.UtcNow;
        
        AddEvent(new TravelIdentityRevoked(Id, reason));
    }

    public void UpdatePdf417Barcode(string pdf417Barcode)
    {
        Pdf417Barcode = pdf417Barcode;
        UpdatedAt = DateTime.UtcNow;
        
        AddEvent(new TravelIdentityModified(this));
    }

    public void UpdateDocumentPath(string documentPath)
    {
        DocumentPath = documentPath;
        UpdatedAt = DateTime.UtcNow;
        
        AddEvent(new TravelIdentityModified(this));
    }

    public bool IsExpired => DateTime.UtcNow > ExpiryDate;
}
