using Mamey.AI.Identity.Models;

namespace Mamey.AI.Identity.Services;

/// <summary>
/// Service for detecting anomalies in identity-related patterns and behaviors.
/// </summary>
public interface IAnomalyDetectionService
{
    /// <summary>
    /// Detects anomalies in identity access patterns.
    /// </summary>
    Task<AnomalyResult> DetectAccessAnomaliesAsync(
        object accessPatternData,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Detects anomalies in credential usage patterns.
    /// </summary>
    Task<AnomalyResult> DetectCredentialAnomaliesAsync(
        object credentialUsageData,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Detects anomalies in DID resolution patterns.
    /// </summary>
    Task<AnomalyResult> DetectDidResolutionAnomaliesAsync(
        object didResolutionData,
        CancellationToken cancellationToken = default);
}
