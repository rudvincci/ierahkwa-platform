using Mamey.FWID.Identities.Domain.ValueObjects;

namespace Mamey.FWID.Identities.Application.Services;

/// <summary>
/// Client interface for integrating with the external Biometric Verification Microservice.
/// Compliant with Biometric Verification Microservice spec (§11).
/// 
/// Note: This is an integration interface for the external Biometric Verification Microservice.
/// The actual Mamey.Security.Biometrics SDK would be implemented separately.
/// </summary>
public interface IBiometricClient
{
    /// <summary>
    /// Verifies liveness (PAD - Presentation Attack Detection) for provided frames.
    /// </summary>
    /// <param name="frames">The biometric frames to verify.</param>
    /// <param name="sessionId">The session identifier.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>The liveness result with score and decision.</returns>
    Task<LivenessResult> LivenessVerifyAsync(
        IEnumerable<byte[]> frames,
        string sessionId,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Extracts a biometric template from a frame.
    /// </summary>
    /// <param name="frame">The biometric frame.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>The extracted template result.</returns>
    Task<ExtractTemplateResult> ExtractTemplateAsync(
        byte[] frame,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Enrolls a subject with biometric data.
    /// </summary>
    /// <param name="subjectId">The subject identifier (IdentityId).</param>
    /// <param name="did">The DID associated with the subject (optional).</param>
    /// <param name="frames">The biometric frames (short clip or burst).</param>
    /// <param name="requireLiveness">Whether to require liveness verification.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>The enrollment result with template ID and evidence JWS.</returns>
    Task<EnrollResult> EnrollAsync(
        string subjectId,
        string? did,
        IEnumerable<byte[]> frames,
        bool requireLiveness = true,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Verifies a subject's biometric data.
    /// </summary>
    /// <param name="subjectId">The subject identifier (IdentityId) or DID.</param>
    /// <param name="frame">The live capture frame.</param>
    /// <param name="requireLiveness">Whether to require liveness verification.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>The verification result with similarity, decision, and evidence JWS.</returns>
    Task<VerifyResult> VerifyAsync(
        string subjectId,
        byte[] frame,
        bool requireLiveness = true,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Deletes a biometric template.
    /// </summary>
    /// <param name="subjectId">The subject identifier (IdentityId) or DID.</param>
    /// <param name="templateId">The template identifier.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>True if deleted, false otherwise.</returns>
    Task<bool> DeleteTemplateAsync(
        string subjectId,
        string templateId,
        CancellationToken cancellationToken = default);
}

/// <summary>
/// Liveness verification result.
/// Compliant with Biometric Verification Microservice spec (§2.2).
/// </summary>
public class LivenessResult
{
    public string SessionId { get; set; } = string.Empty;
    public double Score { get; set; } // 0..1
    public string Decision { get; set; } = string.Empty; // PASS | FAIL | INCONCLUSIVE
    public List<string> Reasons { get; set; } = new();
    public string AlgoVersion { get; set; } = string.Empty;
}

/// <summary>
/// Template extraction result.
/// Compliant with Biometric Verification Microservice spec (§2.2).
/// </summary>
public class ExtractTemplateResult
{
    public string TemplateId { get; set; } = string.Empty;
    public byte[] Vector { get; set; } = Array.Empty<byte>(); // Fixed-length embedding
    public string AlgoVersion { get; set; } = string.Empty;
    public string Format { get; set; } = string.Empty; // e.g., "ISO39794-5:Face"
    public string Quality { get; set; } = string.Empty; // GOOD | FAIR | POOR
}

/// <summary>
/// Enrollment result.
/// Compliant with Biometric Verification Microservice spec (§2.2).
/// </summary>
public class EnrollResult
{
    public string TemplateId { get; set; } = string.Empty;
    public string SubjectId { get; set; } = string.Empty;
    public string? Did { get; set; }
    public string EvidenceJws { get; set; } = string.Empty; // Signed evidence payload (§2.4)
}

/// <summary>
/// Verification result.
/// Compliant with Biometric Verification Microservice spec (§2.2).
/// </summary>
public class VerifyResult
{
    public double Similarity { get; set; } // 0..1
    public string Decision { get; set; } = string.Empty; // PASS | FAIL | INCONCLUSIVE
    public double? LivenessScore { get; set; } // 0..1 (if requested)
    public string EvidenceJws { get; set; } = string.Empty; // Signed evidence payload (§2.4)
}

