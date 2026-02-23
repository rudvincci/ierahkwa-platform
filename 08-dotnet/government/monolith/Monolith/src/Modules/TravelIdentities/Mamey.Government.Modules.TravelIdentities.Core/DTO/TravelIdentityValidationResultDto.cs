using System;

namespace Mamey.Government.Modules.TravelIdentities.Core.DTO;

public class TravelIdentityValidationResultDto
{
    public bool IsValid { get; set; }
    public string IdNumber { get; set; } = string.Empty;
    public string Status { get; set; } = string.Empty;
    public string? HolderName { get; set; }
    public DateTime? ExpiresAt { get; set; }
    public DateTime VerifiedAt { get; set; }
    public string? Message { get; set; }
}
