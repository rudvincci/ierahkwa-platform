using Mamey.FWID.Identities.Application.AI.Models;

namespace Mamey.FWID.Identities.Application.AI.Services;

/// <summary>
/// Interface for AI-powered biometric matching service.
/// </summary>
public interface IBiometricMatchingService
{
    /// <summary>
    /// Matches a selfie against a document photo.
    /// </summary>
    Task<BiometricMatchResult> MatchFaceToDocumentAsync(
        byte[] selfieImage,
        byte[] documentImage,
        CancellationToken cancellationToken = default);
    
    /// <summary>
    /// Matches a selfie against a stored biometric template.
    /// </summary>
    Task<BiometricMatchResult> MatchFaceToDatabaseAsync(
        byte[] selfieImage,
        Guid identityId,
        CancellationToken cancellationToken = default);
    
    /// <summary>
    /// Performs liveness detection.
    /// </summary>
    Task<LivenessResult> PerformLivenessCheckAsync(
        byte[] image,
        LivenessCheckType checkType = LivenessCheckType.Passive,
        CancellationToken cancellationToken = default);
    
    /// <summary>
    /// Performs spoof detection.
    /// </summary>
    Task<SpoofDetectionResult> DetectSpoofAsync(
        byte[] image,
        CancellationToken cancellationToken = default);
    
    /// <summary>
    /// Assesses biometric quality.
    /// </summary>
    Task<List<BiometricQualityMetric>> AssessQualityAsync(
        byte[] image,
        CancellationToken cancellationToken = default);
}
