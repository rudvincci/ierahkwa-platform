using System;

namespace Mamey.Government.Modules.Passports.Core.DTO;

public class PassportDto
{
    public Guid Id { get; set; }
    public Guid TenantId { get; set; }
    public Guid CitizenId { get; set; }
    public string PassportNumber { get; set; } = string.Empty;
    public string? CitizenName { get; set; }
    public string PassportType { get; set; } = "Standard";
    public string? MrzLine1 { get; set; }
    public string? MrzLine2 { get; set; }
    public DateTime IssuedDate { get; set; }
    public DateTime ExpiryDate { get; set; }
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
