using Mamey.FWID.Identities.Domain.ValueObjects;

namespace Mamey.FWID.Identities.Application.Services;

/// <summary>
/// Service for creating and validating biometric evidence JWS (JSON Web Signature).
/// Compliant with Biometric Verification Microservice spec (§2.4).
/// </summary>
public interface IBiometricEvidenceService
{
    /// <summary>
    /// Creates a signed evidence JWS for biometric enrollment.
    /// </summary>
    /// <param name="identityId">The identity identifier.</param>
    /// <param name="did">The DID associated with the identity.</param>
    /// <param name="templateId">The template identifier from Biometric Verification Microservice.</param>
    /// <param name="biometricData">The biometric data containing PAD scores, quality, etc.</param>
    /// <param name="sessionId">The session identifier.</param>
    /// <returns>The signed evidence JWS.</returns>
    Task<string> CreateEnrollmentEvidenceAsync(
        Domain.Entities.IdentityId identityId,
        string? did,
        string templateId,
        BiometricData biometricData,
        string? sessionId = null,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Creates a signed evidence JWS for biometric verification.
    /// </summary>
    /// <param name="identityId">The identity identifier.</param>
    /// <param name="did">The DID associated with the identity.</param>
    /// <param name="similarity">The match similarity score (0..1).</param>
    /// <param name="biometricData">The biometric data containing PAD scores, quality, etc.</param>
    /// <param name="decision">The verification decision (PASS | FAIL | INCONCLUSIVE).</param>
    /// <param name="sessionId">The session identifier.</param>
    /// <returns>The signed evidence JWS.</returns>
    Task<string> CreateVerificationEvidenceAsync(
        Domain.Entities.IdentityId identityId,
        string? did,
        double similarity,
        BiometricData biometricData,
        string decision,
        string? sessionId = null,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Validates a signed evidence JWS.
    /// </summary>
    /// <param name="evidenceJws">The evidence JWS to validate.</param>
    /// <returns>True if the evidence is valid, false otherwise.</returns>
    Task<bool> ValidateEvidenceAsync(string evidenceJws, CancellationToken cancellationToken = default);
}

