namespace Mamey.Government.UI.Models;

/// <summary>
/// Client-side certificate model.
/// </summary>
public class CertificateModel
{
    public Guid Id { get; set; }
    public Guid? CitizenId { get; set; }
    public string CertificateNumber { get; set; } = string.Empty;
    public string CertificateType { get; set; } = string.Empty;
    public DateTime IssuedDate { get; set; }
    public string? DocumentPath { get; set; }
    public bool IsActive { get; set; }
    public DateTime? RevokedAt { get; set; }
    public string? RevocationReason { get; set; }
    
    // Holder details
    public DateTime DateOfBirth { get; set; }
    public string? PlaceOfBirth { get; set; }
    
    // Issuing details
    public string? IssuingAuthority { get; set; }
    public string? PlaceOfIssue { get; set; }
    public string? IssuedBy { get; set; }
    public string? VerificationCode { get; set; }
    
    public string Status
    {
        get
        {
            if (!IsActive) return "Revoked";
            return "Active";
        }
    }

    // Alias for Type
    public string? Type
    {
        get => CertificateType;
        set => CertificateType = value ?? string.Empty;
    }

    // Alias for IssueDate
    public DateTime IssueDate
    {
        get => IssuedDate;
        set => IssuedDate = value;
    }
    
    // Citizen details (populated from lookup)
    public string? CitizenName { get; set; }
}

/// <summary>
/// Certificate summary for list views.
/// </summary>
public class CertificateSummaryModel
{
    public Guid Id { get; set; }
    public Guid? CitizenId { get; set; }
    public string CertificateNumber { get; set; } = string.Empty;
    public string CertificateType { get; set; } = string.Empty;
    public DateTime IssuedDate { get; set; }
    public bool IsActive { get; set; }
    public string? CitizenName { get; set; }
    
    // Alias for Type
    public string? Type
    {
        get => CertificateType;
        set => CertificateType = value ?? string.Empty;
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
            return "Active";
        }
    }
}

/// <summary>
/// Request to issue a new certificate.
/// </summary>
public class IssueCertificateRequest
{
    public Guid TenantId { get; set; }
    public Guid? CitizenId { get; set; }
    public string CertificateType { get; set; } = string.Empty;
}

/// <summary>
/// Request to revoke a certificate.
/// </summary>
public class RevokeCertificateRequest
{
    public string Reason { get; set; } = string.Empty;
}

/// <summary>
/// Certificate validation result.
/// </summary>
public class CertificateValidationResult
{
    public bool IsValid { get; set; }
    public string CertificateNumber { get; set; } = string.Empty;
    public string CertificateType { get; set; } = string.Empty;
    public string Status { get; set; } = string.Empty;
    public DateTime? IssueDate { get; set; }
    public DateTime VerifiedAt { get; set; }
    public string? Message { get; set; }
}
