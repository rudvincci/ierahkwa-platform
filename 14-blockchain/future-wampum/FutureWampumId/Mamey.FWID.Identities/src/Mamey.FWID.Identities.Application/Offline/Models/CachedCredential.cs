namespace Mamey.FWID.Identities.Application.Offline.Models;

/// <summary>
/// Represents a cached credential for offline verification.
/// Credentials are signed JWTs with offline validity periods.
/// </summary>
public class CachedCredential
{
    /// <summary>
    /// Unique identifier for this cached entry.
    /// </summary>
    public Guid Id { get; set; } = Guid.NewGuid();
    
    /// <summary>
    /// The identity ID this credential belongs to.
    /// </summary>
    public Guid IdentityId { get; set; }
    
    /// <summary>
    /// The DID associated with this credential.
    /// </summary>
    public string? DID { get; set; }
    
    /// <summary>
    /// The signed credential JWT.
    /// </summary>
    public string CredentialJwt { get; set; } = null!;
    
    /// <summary>
    /// The credential type.
    /// </summary>
    public string CredentialType { get; set; } = null!;
    
    /// <summary>
    /// The issuer DID.
    /// </summary>
    public string IssuerDid { get; set; } = null!;
    
    /// <summary>
    /// When the credential was cached locally.
    /// </summary>
    public DateTime CachedAt { get; set; } = DateTime.UtcNow;
    
    /// <summary>
    /// When the credential was originally issued.
    /// </summary>
    public DateTime IssuedAt { get; set; }
    
    /// <summary>
    /// When the credential expires.
    /// </summary>
    public DateTime? ExpiresAt { get; set; }
    
    /// <summary>
    /// Offline validity period - how long the credential is valid offline.
    /// </summary>
    public DateTime OfflineValidUntil { get; set; }
    
    /// <summary>
    /// Hash of the credential for integrity verification.
    /// </summary>
    public string CredentialHash { get; set; } = null!;
    
    /// <summary>
    /// Whether the credential has been verified online since caching.
    /// </summary>
    public bool OnlineVerified { get; set; }
    
    /// <summary>
    /// Last time credential status was checked online.
    /// </summary>
    public DateTime? LastOnlineCheck { get; set; }
    
    /// <summary>
    /// Whether the credential is known to be revoked.
    /// </summary>
    public bool IsRevoked { get; set; }
    
    /// <summary>
    /// Revocation check timestamp.
    /// </summary>
    public DateTime? RevocationCheckAt { get; set; }
    
    /// <summary>
    /// Priority for sync (higher = more important).
    /// </summary>
    public int SyncPriority { get; set; } = 0;
    
    /// <summary>
    /// Additional metadata.
    /// </summary>
    public Dictionary<string, string>? Metadata { get; set; }
    
    /// <summary>
    /// Checks if the cached credential is still valid for offline use.
    /// </summary>
    public bool IsValidOffline()
    {
        if (IsRevoked) return false;
        if (DateTime.UtcNow > OfflineValidUntil) return false;
        if (ExpiresAt.HasValue && DateTime.UtcNow > ExpiresAt.Value) return false;
        return true;
    }
    
    /// <summary>
    /// Checks if the credential needs online verification.
    /// </summary>
    public bool NeedsOnlineVerification(TimeSpan maxOfflineAge)
    {
        if (!LastOnlineCheck.HasValue) return true;
        return DateTime.UtcNow - LastOnlineCheck.Value > maxOfflineAge;
    }
}

/// <summary>
/// Cached biometric template for offline matching.
/// </summary>
public class CachedBiometricTemplate
{
    /// <summary>
    /// Unique identifier for this cached entry.
    /// </summary>
    public Guid Id { get; set; } = Guid.NewGuid();
    
    /// <summary>
    /// The identity ID this template belongs to.
    /// </summary>
    public Guid IdentityId { get; set; }
    
    /// <summary>
    /// The biometric modality.
    /// </summary>
    public BiometricModality Modality { get; set; }
    
    /// <summary>
    /// The encrypted biometric template data.
    /// </summary>
    public byte[] EncryptedTemplate { get; set; } = null!;
    
    /// <summary>
    /// Template format/algorithm version.
    /// </summary>
    public string TemplateFormat { get; set; } = "ISO-19794-2";
    
    /// <summary>
    /// Quality score when template was captured (0.0 to 1.0).
    /// </summary>
    public double QualityScore { get; set; }
    
    /// <summary>
    /// When the template was cached.
    /// </summary>
    public DateTime CachedAt { get; set; } = DateTime.UtcNow;
    
    /// <summary>
    /// When the template was originally enrolled.
    /// </summary>
    public DateTime EnrolledAt { get; set; }
    
    /// <summary>
    /// Hash of the template for integrity verification.
    /// </summary>
    public string TemplateHash { get; set; } = null!;
    
    /// <summary>
    /// Encryption key identifier (for key rotation).
    /// </summary>
    public string EncryptionKeyId { get; set; } = null!;
    
    /// <summary>
    /// IV/Nonce used for encryption.
    /// </summary>
    public byte[] EncryptionIV { get; set; } = null!;
    
    /// <summary>
    /// Whether this template is still valid.
    /// </summary>
    public bool IsActive { get; set; } = true;
    
    /// <summary>
    /// Expiration time for the cached template.
    /// </summary>
    public DateTime? ExpiresAt { get; set; }
    
    /// <summary>
    /// Checks if the template is valid for offline matching.
    /// </summary>
    public bool IsValidForMatching()
    {
        if (!IsActive) return false;
        if (ExpiresAt.HasValue && DateTime.UtcNow > ExpiresAt.Value) return false;
        return true;
    }
}
