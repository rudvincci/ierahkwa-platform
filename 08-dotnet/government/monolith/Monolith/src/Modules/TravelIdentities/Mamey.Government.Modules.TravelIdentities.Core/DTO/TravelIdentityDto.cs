using System;

namespace Mamey.Government.Modules.TravelIdentities.Core.DTO;

public class TravelIdentityDto
{
    public Guid Id { get; set; }
    public Guid TenantId { get; set; }
    public Guid CitizenId { get; set; }
    public string TravelIdentityNumber { get; set; } = string.Empty;
    public string? CitizenName { get; set; }
    public DateTime IssuedDate { get; set; }
    public DateTime ExpiryDate { get; set; }
    public string? Pdf417Barcode { get; set; }
    public string? DocumentPath { get; set; }
    public bool IsActive { get; set; }
    public DateTime? RevokedAt { get; set; }
    public string? RevocationReason { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
    public string Status => GetStatus();

    private string GetStatus()
    {
        if (!IsActive) return "Revoked";
        if (ExpiryDate < DateTime.UtcNow) return "Expired";
        if (ExpiryDate < DateTime.UtcNow.AddMonths(6)) return "ExpiringSoon";
        return "Active";
    }
}
