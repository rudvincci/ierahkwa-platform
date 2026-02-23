namespace Mamey.FWID.Identities.Application.Services;

/// <summary>
/// Service interface for biometric authentication with blockchain verification.
/// Handles biometric template hash storage and verification against blockchain.
/// 
/// TDD Reference: Lines 1594-1703 (Identity), Lines 1822-1895 (Biometric Verification)
/// BDD Reference: Lines 326-378 (V.2 Biometric Identity)
/// </summary>
public interface IBiometricBlockchainService
{
    /// <summary>
    /// Enrolls a biometric template by storing its hash on the blockchain.
    /// </summary>
    /// <param name="identityId">The identity ID.</param>
    /// <param name="modality">The biometric modality (face, fingerprint, voice).</param>
    /// <param name="templateHash">The hash of the encrypted biometric template.</param>
    /// <param name="metadata">Additional metadata about the biometric.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>The blockchain transaction ID if successful.</returns>
    Task<BiometricEnrollmentResult> EnrollBiometricAsync(
        Guid identityId,
        BiometricModality modality,
        string templateHash,
        BiometricMetadata? metadata = null,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Verifies a biometric template against the blockchain-stored hash.
    /// </summary>
    /// <param name="identityId">The identity ID.</param>
    /// <param name="modality">The biometric modality.</param>
    /// <param name="templateHash">The hash of the template being verified.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>The verification result.</returns>
    Task<BiometricVerificationResult> VerifyBiometricOnBlockchainAsync(
        Guid identityId,
        BiometricModality modality,
        string templateHash,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets the blockchain-verified biometric status for an identity.
    /// </summary>
    /// <param name="identityId">The identity ID.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>The biometric status.</returns>
    Task<BiometricBlockchainStatus> GetBiometricStatusAsync(
        Guid identityId,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Revokes a biometric enrollment on the blockchain.
    /// </summary>
    /// <param name="identityId">The identity ID.</param>
    /// <param name="modality">The biometric modality.</param>
    /// <param name="reason">The reason for revocation.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>True if revocation was successful.</returns>
    Task<bool> RevokeBiometricAsync(
        Guid identityId,
        BiometricModality modality,
        string? reason = null,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Updates a biometric template hash on the blockchain.
    /// </summary>
    /// <param name="identityId">The identity ID.</param>
    /// <param name="modality">The biometric modality.</param>
    /// <param name="newTemplateHash">The new template hash.</param>
    /// <param name="reason">The reason for update (e.g., "re-enrollment").</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>The update result.</returns>
    Task<BiometricEnrollmentResult> UpdateBiometricAsync(
        Guid identityId,
        BiometricModality modality,
        string newTemplateHash,
        string? reason = null,
        CancellationToken cancellationToken = default);
}

/// <summary>
/// Biometric modality types.
/// </summary>
public enum BiometricModality
{
    /// <summary>Facial recognition.</summary>
    Face,
    
    /// <summary>Fingerprint scanning.</summary>
    Fingerprint,
    
    /// <summary>Voice recognition.</summary>
    Voice,
    
    /// <summary>Iris scanning.</summary>
    Iris,
    
    /// <summary>Palm print.</summary>
    Palm
}

/// <summary>
/// Metadata about a biometric enrollment.
/// </summary>
public class BiometricMetadata
{
    /// <summary>The capture device identifier.</summary>
    public string? DeviceId { get; set; }
    
    /// <summary>The quality score of the captured biometric.</summary>
    public double? QualityScore { get; set; }
    
    /// <summary>The algorithm used for template generation.</summary>
    public string? Algorithm { get; set; }
    
    /// <summary>The algorithm version.</summary>
    public string? AlgorithmVersion { get; set; }
    
    /// <summary>When the biometric was captured.</summary>
    public DateTime? CapturedAt { get; set; }
    
    /// <summary>Whether this is a re-enrollment.</summary>
    public bool IsReEnrollment { get; set; }
    
    /// <summary>Previous enrollment ID if re-enrolling.</summary>
    public string? PreviousEnrollmentId { get; set; }
}

/// <summary>
/// Result of biometric enrollment.
/// </summary>
public class BiometricEnrollmentResult
{
    /// <summary>Whether the enrollment was successful.</summary>
    public bool Success { get; set; }
    
    /// <summary>The enrollment ID on the blockchain.</summary>
    public string? EnrollmentId { get; set; }
    
    /// <summary>The blockchain transaction hash.</summary>
    public string? TransactionHash { get; set; }
    
    /// <summary>The block number where enrollment was recorded.</summary>
    public long? BlockNumber { get; set; }
    
    /// <summary>When the enrollment was recorded.</summary>
    public DateTime? EnrolledAt { get; set; }
    
    /// <summary>Error message if enrollment failed.</summary>
    public string? ErrorMessage { get; set; }
}

/// <summary>
/// Result of biometric verification against blockchain.
/// </summary>
public class BiometricVerificationResult
{
    /// <summary>Whether the biometric matches the blockchain record.</summary>
    public bool IsVerified { get; set; }
    
    /// <summary>Whether the biometric hash matches.</summary>
    public bool HashMatches { get; set; }
    
    /// <summary>Whether the biometric is enrolled on blockchain.</summary>
    public bool IsEnrolled { get; set; }
    
    /// <summary>Whether the biometric has been revoked.</summary>
    public bool IsRevoked { get; set; }
    
    /// <summary>Whether tampering was detected.</summary>
    public bool TamperDetected { get; set; }
    
    /// <summary>The confidence score of the match (0-1).</summary>
    public double? MatchScore { get; set; }
    
    /// <summary>The enrollment ID from blockchain.</summary>
    public string? EnrollmentId { get; set; }
    
    /// <summary>The blockchain transaction hash of the enrollment.</summary>
    public string? EnrollmentTransactionHash { get; set; }
    
    /// <summary>When the enrollment was recorded on blockchain.</summary>
    public DateTime? EnrolledAt { get; set; }
    
    /// <summary>Error message if verification failed.</summary>
    public string? ErrorMessage { get; set; }
    
    /// <summary>Details about why verification failed.</summary>
    public string? FailureReason { get; set; }
}

/// <summary>
/// Biometric status for an identity on blockchain.
/// </summary>
public class BiometricBlockchainStatus
{
    /// <summary>The identity ID.</summary>
    public Guid IdentityId { get; set; }
    
    /// <summary>List of enrolled biometric modalities.</summary>
    public List<EnrolledBiometricInfo> EnrolledBiometrics { get; set; } = new();
    
    /// <summary>Whether biometric verification is enabled.</summary>
    public bool BiometricVerificationEnabled { get; set; }
    
    /// <summary>Last verification time.</summary>
    public DateTime? LastVerificationAt { get; set; }
    
    /// <summary>Total verification attempts.</summary>
    public int TotalVerificationAttempts { get; set; }
    
    /// <summary>Successful verification count.</summary>
    public int SuccessfulVerifications { get; set; }
}

/// <summary>
/// Information about an enrolled biometric.
/// </summary>
public class EnrolledBiometricInfo
{
    /// <summary>The biometric modality.</summary>
    public BiometricModality Modality { get; set; }
    
    /// <summary>The enrollment ID on blockchain.</summary>
    public string? EnrollmentId { get; set; }
    
    /// <summary>The hash of the enrolled template.</summary>
    public string? TemplateHash { get; set; }
    
    /// <summary>When the biometric was enrolled.</summary>
    public DateTime? EnrolledAt { get; set; }
    
    /// <summary>Whether this enrollment is active.</summary>
    public bool IsActive { get; set; }
    
    /// <summary>When the enrollment was last updated.</summary>
    public DateTime? UpdatedAt { get; set; }
}
