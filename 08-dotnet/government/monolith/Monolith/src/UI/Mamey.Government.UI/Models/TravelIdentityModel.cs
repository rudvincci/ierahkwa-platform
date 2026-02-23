namespace Mamey.Government.UI.Models;

/// <summary>
/// Client-side travel identity model.
/// </summary>
public class TravelIdentityModel
{
    public Guid Id { get; set; }
    public Guid CitizenId { get; set; }
    public string TravelIdentityNumber { get; set; } = string.Empty;
    public DateTime IssuedDate { get; set; }
    public DateTime ExpiryDate { get; set; }
    public string? Pdf417Barcode { get; set; }
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
    public string? IssuedBy { get; set; }
    
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

    // Alias for IdNumber
    public string? IdNumber
    {
        get => TravelIdentityNumber;
        set => TravelIdentityNumber = value ?? string.Empty;
    }

    // Alias for IssueDate
    public DateTime IssueDate
    {
        get => IssuedDate;
        set => IssuedDate = value;
    }
    
    // Citizen details (populated from lookup) - alias for HolderName
    public string? CitizenName
    {
        get => HolderName;
        set => HolderName = value;
    }
}

/// <summary>
/// Travel identity summary for list views.
/// </summary>
public class TravelIdentitySummaryModel
{
    public Guid Id { get; set; }
    public Guid CitizenId { get; set; }
    public string TravelIdentityNumber { get; set; } = string.Empty;
    public DateTime IssuedDate { get; set; }
    public DateTime ExpiryDate { get; set; }
    public bool IsActive { get; set; }
    
    public bool IsExpired => ExpiryDate < DateTime.UtcNow;
    public bool IsExpiringSoon => ExpiryDate < DateTime.UtcNow.AddMonths(3) && !IsExpired;
    
    public string? CitizenName { get; set; }
    
    // Alias for IdNumber
    public string? IdNumber
    {
        get => TravelIdentityNumber;
        set => TravelIdentityNumber = value ?? string.Empty;
    }
    
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
/// Request to issue a new travel identity.
/// </summary>
public class IssueTravelIdentityRequest
{
    public Guid TenantId { get; set; }
    public Guid CitizenId { get; set; }
    public int ValidityYears { get; set; } = 8;
}

/// <summary>
/// Request to renew a travel identity.
/// </summary>
public class RenewTravelIdentityRequest
{
    public int ValidityYears { get; set; } = 8;
}

/// <summary>
/// Request to revoke a travel identity.
/// </summary>
public class RevokeTravelIdentityRequest
{
    public string Reason { get; set; } = string.Empty;
}

/// <summary>
/// Travel identity validation result.
/// </summary>
public class TravelIdentityValidationResult
{
    public bool IsValid { get; set; }
    public string IdNumber { get; set; } = string.Empty;
    public string Status { get; set; } = string.Empty;
    public string? HolderName { get; set; }
    public DateTime? ExpiresAt { get; set; }
    public DateTime VerifiedAt { get; set; }
    public string? Message { get; set; }
}
