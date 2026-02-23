using System;

namespace Mamey.Government.Modules.Passports.Core.DTO;

public class PassportValidationResultDto
{
    public bool IsValid { get; set; }
    public string PassportNumber { get; set; } = string.Empty;
    public string Status { get; set; } = string.Empty;
    public string? HolderName { get; set; }
    public DateTime? ExpiresAt { get; set; }
    public DateTime VerifiedAt { get; set; }
    public string? Message { get; set; }
}
