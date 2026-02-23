namespace ESignature.Core.Models;

public class Certificate
{
    public Guid Id { get; set; }
    public string SerialNumber { get; set; } = string.Empty;
    public Guid UserId { get; set; }
    public string UserName { get; set; } = string.Empty;
    public string UserEmail { get; set; } = string.Empty;
    public CertificateType Type { get; set; }
    public CertificateStatus Status { get; set; }
    public string Subject { get; set; } = string.Empty;
    public string Issuer { get; set; } = string.Empty;
    public string? Organization { get; set; }
    public string? Department { get; set; }
    public string PublicKey { get; set; } = string.Empty;
    public string? PrivateKeyEncrypted { get; set; }
    public string Algorithm { get; set; } = "RSA-SHA256";
    public int KeySize { get; set; } = 2048;
    public string Thumbprint { get; set; } = string.Empty;
    public DateTime ValidFrom { get; set; }
    public DateTime ValidTo { get; set; }
    public bool IsRevoked { get; set; }
    public DateTime? RevokedAt { get; set; }
    public string? RevocationReason { get; set; }
    public string? CrlDistributionPoint { get; set; }
    public string? OcspResponder { get; set; }
    public string? CertificateChain { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
}

public enum CertificateType
{
    Personal,
    Organization,
    Government,
    Timestamp,
    CodeSigning,
    RootCA,
    IntermediateCA
}

public enum CertificateStatus
{
    Pending,
    Active,
    Suspended,
    Revoked,
    Expired
}
