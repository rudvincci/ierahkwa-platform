using Mamey.Government.Modules.Citizens.Core.Domain.ValueObjects;
using Mamey.Government.Modules.Passports.Core.Domain.Events;
using Mamey.Government.Modules.Passports.Core.Domain.ValueObjects;
using Mamey.Types;
using Mamey.Types;

namespace Mamey.Government.Modules.Passports.Core.Domain.Entities;

/// <summary>
/// Passport aggregate root - represents an issued passport.
/// Mandatory after citizenship approval.
/// </summary>
internal class Passport : AggregateRoot<PassportId>
{
    private Passport() { }

    public Passport(
        PassportId id,
        TenantId tenantId,
        Guid citizenId,
        PassportNumber passportNumber,
        DateTime issuedDate,
        DateTime expiryDate,
        string? mrz = null,
        int version = 0)
        : base(id, version)
    {
        TenantId = tenantId;
        CitizenId = citizenId;
        PassportNumber = passportNumber;
        IssuedDate = issuedDate;
        ExpiryDate = expiryDate;
        Mrz = mrz;
        IsActive = true;
        CreatedAt = DateTime.UtcNow;
        UpdatedAt = DateTime.UtcNow;
        
        AddEvent(new PassportIssued(Id, CitizenId, PassportNumber));
    }

    public TenantId TenantId { get; private set; }
    public Guid CitizenId { get; private set; }
    public PassportNumber PassportNumber { get; private set; } = null!;
    public DateTime IssuedDate { get; private set; }
    public DateTime ExpiryDate { get; private set; }
    public string? Mrz { get; private set; } // ICAO MRZ (Machine Readable Zone)
    public string? DocumentPath { get; private set; } // MinIO path to generated PDF
    public bool IsActive { get; private set; }
    public DateTime? RevokedAt { get; private set; }
    public string? RevocationReason { get; private set; }
    public DateTime CreatedAt { get; private set; }
    public DateTime UpdatedAt { get; private set; }

    public void Renew(DateTime newExpiryDate)
    {
        ExpiryDate = newExpiryDate;
        UpdatedAt = DateTime.UtcNow;
        
        AddEvent(new PassportRenewed(Id, ExpiryDate));
    }

    public void Revoke(string reason)
    {
        IsActive = false;
        RevokedAt = DateTime.UtcNow;
        RevocationReason = reason;
        UpdatedAt = DateTime.UtcNow;
        
        AddEvent(new PassportRevoked(Id, reason));
    }

    public void UpdateMrz(string mrz)
    {
        Mrz = mrz;
        UpdatedAt = DateTime.UtcNow;
        
        AddEvent(new PassportModified(this));
    }

    public void UpdateDocumentPath(string documentPath)
    {
        DocumentPath = documentPath;
        UpdatedAt = DateTime.UtcNow;
        
        AddEvent(new PassportModified(this));
    }

    public bool IsExpired => DateTime.UtcNow > ExpiryDate;
}
