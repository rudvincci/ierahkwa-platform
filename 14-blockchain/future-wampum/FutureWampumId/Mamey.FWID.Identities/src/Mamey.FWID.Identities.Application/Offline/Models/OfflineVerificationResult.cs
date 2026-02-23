namespace Mamey.FWID.Identities.Application.Offline.Models;

/// <summary>
/// Result of an offline biometric verification operation.
/// </summary>
public class OfflineVerificationResult
{
    /// <summary>
    /// Whether the verification was successful.
    /// </summary>
    public bool IsVerified { get; set; }
    
    /// <summary>
    /// The identity ID that was verified.
    /// </summary>
    public Guid? IdentityId { get; set; }
    
    /// <summary>
    /// The biometric modality used for verification.
    /// </summary>
    public BiometricModality Modality { get; set; }
    
    /// <summary>
    /// Match confidence score (0.0 to 1.0).
    /// </summary>
    public double MatchScore { get; set; }
    
    /// <summary>
    /// Quality score of the captured biometric (0.0 to 1.0).
    /// </summary>
    public double QualityScore { get; set; }
    
    /// <summary>
    /// Whether the verification was performed offline.
    /// </summary>
    public bool WasOffline { get; set; }
    
    /// <summary>
    /// Timestamp of the verification.
    /// </summary>
    public DateTime VerifiedAt { get; set; }
    
    /// <summary>
    /// Error message if verification failed.
    /// </summary>
    public string? ErrorMessage { get; set; }
    
    /// <summary>
    /// Error code if verification failed.
    /// </summary>
    public string? ErrorCode { get; set; }
    
    /// <summary>
    /// Device identifier where verification occurred.
    /// </summary>
    public string? DeviceId { get; set; }
    
    /// <summary>
    /// Verification transaction ID for audit purposes.
    /// </summary>
    public string TransactionId { get; set; } = Guid.NewGuid().ToString();
    
    /// <summary>
    /// Whether sync to server is pending.
    /// </summary>
    public bool SyncPending { get; set; }
    
    /// <summary>
    /// Creates a successful verification result.
    /// </summary>
    public static OfflineVerificationResult Success(
        Guid identityId,
        BiometricModality modality,
        double matchScore,
        double qualityScore,
        bool wasOffline = true)
    {
        return new OfflineVerificationResult
        {
            IsVerified = true,
            IdentityId = identityId,
            Modality = modality,
            MatchScore = matchScore,
            QualityScore = qualityScore,
            WasOffline = wasOffline,
            VerifiedAt = DateTime.UtcNow,
            SyncPending = wasOffline
        };
    }
    
    /// <summary>
    /// Creates a failed verification result.
    /// </summary>
    public static OfflineVerificationResult Failure(
        string errorCode,
        string errorMessage,
        BiometricModality modality = BiometricModality.Unknown)
    {
        return new OfflineVerificationResult
        {
            IsVerified = false,
            ErrorCode = errorCode,
            ErrorMessage = errorMessage,
            Modality = modality,
            VerifiedAt = DateTime.UtcNow
        };
    }
    
    /// <summary>
    /// Creates a "no match" result.
    /// </summary>
    public static OfflineVerificationResult NoMatch(
        BiometricModality modality,
        double matchScore,
        double qualityScore)
    {
        return new OfflineVerificationResult
        {
            IsVerified = false,
            ErrorCode = "NO_MATCH",
            ErrorMessage = "Biometric did not match any cached templates",
            Modality = modality,
            MatchScore = matchScore,
            QualityScore = qualityScore,
            VerifiedAt = DateTime.UtcNow
        };
    }
}

/// <summary>
/// Supported biometric modalities.
/// </summary>
public enum BiometricModality
{
    Unknown = 0,
    Fingerprint = 1,
    Face = 2,
    Iris = 3,
    Voice = 4,
    Palm = 5,
    Vein = 6
}
