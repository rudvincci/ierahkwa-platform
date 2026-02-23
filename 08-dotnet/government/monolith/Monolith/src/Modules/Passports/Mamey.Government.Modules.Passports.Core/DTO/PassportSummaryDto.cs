using System;

namespace Mamey.Government.Modules.Passports.Core.DTO;

public class PassportSummaryDto
{
    public Guid Id { get; set; }
    public string PassportNumber { get; set; } = string.Empty;
    public Guid CitizenId { get; set; }
    public string? CitizenName { get; set; }
    public string PassportType { get; set; } = "Standard";
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
