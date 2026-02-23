namespace ESignature.Core.Models;

public class SignatureAuditLog
{
    public Guid Id { get; set; }
    public Guid SignatureRequestId { get; set; }
    public Guid? SignerId { get; set; }
    public string Action { get; set; } = string.Empty;
    public string? Details { get; set; }
    public string? IpAddress { get; set; }
    public string? UserAgent { get; set; }
    public string? GeoLocation { get; set; }
    public string? Country { get; set; }
    public string? City { get; set; }
    public DateTime Timestamp { get; set; }
    public string? Hash { get; set; }
    public string? BlockchainTxId { get; set; }

    // Navigation
    public SignatureRequest? SignatureRequest { get; set; }
    public Signer? Signer { get; set; }
}

public class SignatureVerification
{
    public Guid Id { get; set; }
    public Guid SignatureRequestId { get; set; }
    public string DocumentHash { get; set; } = string.Empty;
    public string SignatureHash { get; set; } = string.Empty;
    public bool IsValid { get; set; }
    public string? ValidationMessage { get; set; }
    public string? CertificateSerialNumber { get; set; }
    public string? CertificateSubject { get; set; }
    public DateTime? CertificateValidFrom { get; set; }
    public DateTime? CertificateValidTo { get; set; }
    public bool CertificateIsValid { get; set; }
    public bool TimestampIsValid { get; set; }
    public DateTime? TimestampDate { get; set; }
    public string? TimestampAuthority { get; set; }
    public string? BlockchainVerification { get; set; }
    public DateTime VerifiedAt { get; set; }
}

public class SignatureTemplate
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    public Guid CreatedBy { get; set; }
    public string? DocumentTemplateId { get; set; }
    public string FieldsDefinition { get; set; } = "[]"; // JSON array
    public string SignersDefinition { get; set; } = "[]"; // JSON array
    public SignatureType DefaultSignatureType { get; set; }
    public SigningOrder DefaultSigningOrder { get; set; }
    public bool RequireAuthentication { get; set; }
    public AuthenticationType DefaultAuthenticationType { get; set; }
    public int? DefaultExpirationDays { get; set; }
    public bool IsActive { get; set; } = true;
    public int UsageCount { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
}
