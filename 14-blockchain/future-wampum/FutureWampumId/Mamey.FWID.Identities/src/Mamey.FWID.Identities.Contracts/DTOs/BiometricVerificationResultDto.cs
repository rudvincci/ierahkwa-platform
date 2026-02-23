namespace Mamey.FWID.Identities.Contracts.DTOs;

/// <summary>
/// Data transfer object for biometric verification results.
/// </summary>
public class BiometricVerificationResultDto
{
    /// <summary>
    /// Gets or sets whether the biometric verification was successful.
    /// </summary>
    public bool IsVerified { get; set; }

    /// <summary>
    /// Gets or sets the confidence score of the biometric match (0-100).
    /// </summary>
    public double ConfidenceScore { get; set; }

    /// <summary>
    /// Gets or sets the type of biometric used for verification.
    /// </summary>
    public BiometricType BiometricType { get; set; }

    /// <summary>
    /// Gets or sets the quality of the biometric scan.
    /// </summary>
    public BiometricQuality Quality { get; set; }

    /// <summary>
    /// Gets or sets the timestamp of the verification.
    /// </summary>
    public DateTime VerifiedAt { get; set; }

    /// <summary>
    /// Gets or sets any error message if verification failed.
    /// </summary>
    public string? ErrorMessage { get; set; }
}
