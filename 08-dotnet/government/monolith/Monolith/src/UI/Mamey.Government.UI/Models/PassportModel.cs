namespace Mamey.Government.UI.Models;

/// <summary>
/// Client-side passport model.
/// </summary>
public class PassportModel
{
    public Guid Id { get; set; }
    public Guid CitizenId { get; set; }
    public string PassportNumber { get; set; } = string.Empty;
    public string? PassportType { get; set; }
    public DateTime IssueDate { get; set; }
    public DateTime ExpiryDate { get; set; }
    public string? Mrz { get; set; }
    public bool IsActive { get; set; }
    public DateTime? RevokedAt { get; set; }
    public string? RevocationReason { get; set; }
    
    // Holder details
    public string? HolderName { get; set; }
    public DateTime DateOfBirth { get; set; }
    public string? Gender { get; set; }
    public string? Nationality { get; set; }
    public string? PlaceOfBirth { get; set; }
    
    // Issuing details
    public string? PlaceOfIssue { get; set; }
    public string? IssuingAuthority { get; set; }
    
    public bool IsExpired => ExpiryDate < DateTime.UtcNow;
    public bool IsExpiringSoon => ExpiryDate < DateTime.UtcNow.AddMonths(3) && !IsExpired;
    
    public string Status
    {
        get
        {
            if (!IsActive) return "Revoked";
            if (IsExpired) return "Expired";
            if (IsExpiringSoon) return "Expiring";
            return "Active";
        }
    }
    
    // Citizen details (populated from lookup) - alias for HolderName
    public string? CitizenName
    {
        get => HolderName;
        set => HolderName = value;
    }
}

/// <summary>
/// Passport summary for list views.
/// </summary>
public class PassportSummaryModel
{
    public Guid Id { get; set; }
    public Guid CitizenId { get; set; }
    public string PassportNumber { get; set; } = string.Empty;
    public DateTime IssuedDate { get; set; }
    public DateTime ExpiryDate { get; set; }
    public bool IsActive { get; set; }
    
    public bool IsExpired => ExpiryDate < DateTime.UtcNow;
    public bool IsExpiringSoon => ExpiryDate < DateTime.UtcNow.AddMonths(3) && !IsExpired;
    
    public string? CitizenName { get; set; }
    
    // Alias for IssueDate
    public DateTime IssueDate
    {
        get => IssuedDate;
        set => IssuedDate = value;
    }
    
    public string Status
    {
        get
        {
            if (!IsActive) return "Revoked";
            if (IsExpired) return "Expired";
            if (IsExpiringSoon) return "Expiring";
            return "Active";
        }
    }
}

/// <summary>
/// Request to issue a new passport.
/// </summary>
public class IssuePassportRequest
{
    public Guid TenantId { get; set; }
    public Guid CitizenId { get; set; }
    public int ValidityYears { get; set; } = 10;
}

/// <summary>
/// Request to renew a passport.
/// </summary>
public class RenewPassportRequest
{
    public int ValidityYears { get; set; } = 10;
}

/// <summary>
/// Request to revoke a passport.
/// </summary>
public class RevokePassportRequest
{
    public string Reason { get; set; } = string.Empty;
}

/// <summary>
/// Passport validation result.
/// </summary>
public class PassportValidationResult
{
    public bool IsValid { get; set; }
    public string PassportNumber { get; set; } = string.Empty;
    public string Status { get; set; } = string.Empty;
    public string? HolderName { get; set; }
    public DateTime? ExpiresAt { get; set; }
    public DateTime VerifiedAt { get; set; }
    public string? Message { get; set; }
}
