using System;

namespace Mamey.Government.Modules.TravelIdentities.Core.DTO;

public class TravelIdentitySummaryDto
{
    public Guid Id { get; set; }
    public string TravelIdentityNumber { get; set; } = string.Empty;
    public Guid CitizenId { get; set; }
    public string? CitizenName { get; set; }
    public DateTime ExpiryDate { get; set; }
    public bool IsActive { get; set; }
    public string Status => GetStatus();

    private string GetStatus()
    {
        if (!IsActive) return "Revoked";
        if (ExpiryDate < DateTime.UtcNow) return "Expired";
        if (ExpiryDate < DateTime.UtcNow.AddMonths(6)) return "ExpiringSoon";
        return "Active";
    }
}
