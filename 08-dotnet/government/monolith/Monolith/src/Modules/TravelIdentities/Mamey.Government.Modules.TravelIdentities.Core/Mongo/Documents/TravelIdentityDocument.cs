using Mamey.Government.Modules.TravelIdentities.Core.Domain.Entities;
using Mamey.MicroMonolith.Infrastructure.Mongo;
using Mamey.Types;

namespace Mamey.Government.Modules.TravelIdentities.Core.Mongo.Documents;

internal class TravelIdentityDocument : MicroMonolith.Infrastructure.Mongo.IIdentifiable<Guid>
{
    public TravelIdentityDocument()
    {
    }

    public TravelIdentityDocument(TravelIdentity travelIdentity)
    {
        Id = travelIdentity.Id.Value;
        TenantId = travelIdentity.TenantId.Value;
        CitizenId = travelIdentity.CitizenId;
        TravelIdentityNumber = travelIdentity.TravelIdentityNumber.Value;
        IssuedDate = travelIdentity.IssuedDate;
        ExpiryDate = travelIdentity.ExpiryDate;
        Pdf417Barcode = travelIdentity.Pdf417Barcode;
        DocumentPath = travelIdentity.DocumentPath;
        IsActive = travelIdentity.IsActive;
        RevokedAt = travelIdentity.RevokedAt;
        RevocationReason = travelIdentity.RevocationReason;
        CreatedAt = travelIdentity.CreatedAt;
        UpdatedAt = travelIdentity.UpdatedAt;
    }

    public Guid Id { get; set; }
    public Guid TenantId { get; set; }
    public Guid CitizenId { get; set; }
    public string TravelIdentityNumber { get; set; } = string.Empty;
    public DateTime IssuedDate { get; set; }
    public DateTime ExpiryDate { get; set; }
    public string? Pdf417Barcode { get; set; }
    public string? DocumentPath { get; set; }
    public bool IsActive { get; set; }
    public DateTime? RevokedAt { get; set; }
    public string? RevocationReason { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }

    public TravelIdentity AsEntity()
    {
        var travelIdentityId = new Domain.ValueObjects.TravelIdentityId(Id);
        var tenantId = new TenantId(TenantId);
        var citizenId = CitizenId;
        var travelIdentityNumber = new Domain.ValueObjects.TravelIdentityNumber(TravelIdentityNumber);
        
        var travelIdentity = new TravelIdentity(
            travelIdentityId,
            tenantId,
            citizenId,
            travelIdentityNumber,
            IssuedDate,
            ExpiryDate,
            Pdf417Barcode);
        
        typeof(TravelIdentity).GetProperty("DocumentPath")?.SetValue(travelIdentity, DocumentPath);
        typeof(TravelIdentity).GetProperty("IsActive")?.SetValue(travelIdentity, IsActive);
        typeof(TravelIdentity).GetProperty("RevokedAt")?.SetValue(travelIdentity, RevokedAt);
        typeof(TravelIdentity).GetProperty("RevocationReason")?.SetValue(travelIdentity, RevocationReason);
        typeof(TravelIdentity).GetProperty("CreatedAt")?.SetValue(travelIdentity, CreatedAt);
        typeof(TravelIdentity).GetProperty("UpdatedAt")?.SetValue(travelIdentity, UpdatedAt);
        
        return travelIdentity;
    }
}
