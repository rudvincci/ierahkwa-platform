using Mamey.FWID.Identities.Application.AI.Models;

namespace Mamey.FWID.Identities.Application.AI.Services;

/// <summary>
/// Interface for AI-powered fraud detection service.
/// </summary>
public interface IFraudDetectionService
{
    /// <summary>
    /// Calculates fraud score for an identity registration.
    /// </summary>
    Task<FraudScore> CalculateFraudScoreAsync(
        FraudDetectionRequest request,
        CancellationToken cancellationToken = default);
    
    /// <summary>
    /// Analyzes velocity patterns.
    /// </summary>
    Task<VelocityAnalysis> AnalyzeVelocityAsync(
        Guid identityId,
        string? ipAddress,
        string? deviceId,
        CancellationToken cancellationToken = default);
    
    /// <summary>
    /// Analyzes device trustworthiness.
    /// </summary>
    Task<DeviceAnalysis> AnalyzeDeviceAsync(
        DeviceFingerprint fingerprint,
        CancellationToken cancellationToken = default);
    
    /// <summary>
    /// Analyzes network/IP reputation.
    /// </summary>
    Task<NetworkAnalysis> AnalyzeNetworkAsync(
        string ipAddress,
        CancellationToken cancellationToken = default);
    
    /// <summary>
    /// Detects fraud patterns across identities.
    /// </summary>
    Task<List<FraudSignal>> DetectFraudPatternsAsync(
        Guid identityId,
        CancellationToken cancellationToken = default);
}

public class FraudDetectionRequest
{
    public Guid IdentityId { get; set; }
    public string? IPAddress { get; set; }
    public DeviceFingerprint? DeviceFingerprint { get; set; }
    public BehaviorData? BehaviorData { get; set; }
    public string? Email { get; set; }
    public string? Phone { get; set; }
}

public class DeviceFingerprint
{
    public string? DeviceId { get; set; }
    public string? DeviceType { get; set; }
    public string? OperatingSystem { get; set; }
    public string? Browser { get; set; }
    public string? ScreenResolution { get; set; }
    public string? Language { get; set; }
    public string? Timezone { get; set; }
    public Dictionary<string, string>? AdditionalAttributes { get; set; }
}

public class BehaviorData
{
    public double? MouseMovementScore { get; set; }
    public double? KeystrokeDynamicsScore { get; set; }
    public double SessionDurationSeconds { get; set; }
    public int PageInteractionCount { get; set; }
    public List<string>? NavigationPath { get; set; }
}
