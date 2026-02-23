namespace MameyNode.Portals.Mocks.Models;

// Models based on did.proto and general.proto for FutureWampumID

public class IdentityVerificationInfo
{
    public string VerificationId { get; set; } = string.Empty;
    public string IdentityId { get; set; } = string.Empty;
    public string VerificationType { get; set; } = string.Empty; // Document, Biometric, etc.
    public bool Verified { get; set; }
    public string VerificationResult { get; set; } = string.Empty;
    public DateTime VerifiedAt { get; set; }
    public string VerifierId { get; set; } = string.Empty;
    public string Status { get; set; } = "Pending";
}

public class DigitalCredentialInfo
{
    public string CredentialId { get; set; } = string.Empty;
    public string IdentityId { get; set; } = string.Empty;
    public string CredentialType { get; set; } = string.Empty;
    public string Issuer { get; set; } = string.Empty;
    public DateTime IssuedAt { get; set; }
    public DateTime? ExpiryDate { get; set; }
    public bool IsRevoked { get; set; }
    public string Status { get; set; } = "Active";
    public Dictionary<string, string> Claims { get; set; } = new();
}

public class IdentityWalletInfo
{
    public string WalletId { get; set; } = string.Empty;
    public string IdentityId { get; set; } = string.Empty;
    public string WalletType { get; set; } = string.Empty; // Standard, Hardware, etc.
    public List<string> DIDs { get; set; } = new();
    public List<string> Credentials { get; set; } = new();
    public DateTime CreatedAt { get; set; }
    public bool IsActive { get; set; } = true;
}

public class AttestationInfo
{
    public string AttestationId { get; set; } = string.Empty;
    public string IdentityId { get; set; } = string.Empty;
    public string AttestationType { get; set; } = string.Empty;
    public string AttesterId { get; set; } = string.Empty;
    public string Content { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; }
    public bool IsVerified { get; set; }
    public string Status { get; set; } = "Active";
}

public class RecoveryInfo
{
    public string RecoveryId { get; set; } = string.Empty;
    public string IdentityId { get; set; } = string.Empty;
    public string RecoveryType { get; set; } = string.Empty; // Social, Backup, etc.
    public string Status { get; set; } = "Pending";
    public DateTime RequestedAt { get; set; }
    public DateTime? CompletedAt { get; set; }
    public List<string> RecoveryMethods { get; set; } = new();
}

// DID Models (from did.proto)
public class DIDDocumentInfo
{
    public string DID { get; set; } = string.Empty;
    public string DIDDocument { get; set; } = string.Empty; // JSON string
    public DateTime Created { get; set; }
    public DateTime Updated { get; set; }
    public ulong VersionId { get; set; }
    public bool Deactivated { get; set; }
    public string CanonicalId { get; set; } = string.Empty;
}

public class VerificationMethodInfo
{
    public string Id { get; set; } = string.Empty;
    public string Type { get; set; } = string.Empty;
    public string Controller { get; set; } = string.Empty;
    public string PublicKeyMultibase { get; set; } = string.Empty;
    public string PublicKeyJwk { get; set; } = string.Empty;
}

public class ServiceEndpointInfo
{
    public string Id { get; set; } = string.Empty;
    public string Type { get; set; } = string.Empty;
    public string ServiceEndpoint { get; set; } = string.Empty;
}

public class DIDHistoryEntryInfo
{
    public ulong Version { get; set; }
    public string DIDDocument { get; set; } = string.Empty;
    public string Action { get; set; } = string.Empty; // created, updated, deactivated
    public string Actor { get; set; } = string.Empty;
    public DateTime Timestamp { get; set; }
    public string TransactionHash { get; set; } = string.Empty;
    public string Reason { get; set; } = string.Empty;
}

