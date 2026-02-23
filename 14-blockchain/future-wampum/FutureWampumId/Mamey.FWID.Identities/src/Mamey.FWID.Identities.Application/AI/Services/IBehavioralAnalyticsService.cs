using Mamey.FWID.Identities.Application.AI.Models;

namespace Mamey.FWID.Identities.Application.AI.Services;

/// <summary>
/// Interface for AI-powered behavioral analytics service.
/// </summary>
public interface IBehavioralAnalyticsService
{
    /// <summary>
    /// Gets or creates behavioral profile for an identity.
    /// </summary>
    Task<BehavioralProfile> GetOrCreateProfileAsync(
        Guid identityId,
        CancellationToken cancellationToken = default);
    
    /// <summary>
    /// Records behavioral event and updates profile.
    /// </summary>
    Task RecordBehaviorAsync(
        BehavioralEvent behaviorEvent,
        CancellationToken cancellationToken = default);
    
    /// <summary>
    /// Analyzes current behavior against baseline.
    /// </summary>
    Task<BehavioralAnalysisResult> AnalyzeBehaviorAsync(
        Guid identityId,
        AuthenticationContext context,
        CancellationToken cancellationToken = default);
    
    /// <summary>
    /// Evaluates for adaptive authentication.
    /// </summary>
    Task<AdaptiveAuthResult> EvaluateForAdaptiveAuthAsync(
        Guid identityId,
        AuthenticationContext context,
        CancellationToken cancellationToken = default);
    
    /// <summary>
    /// Detects anomalies in recent behavior.
    /// </summary>
    Task<List<BehavioralAnomaly>> DetectAnomaliesAsync(
        Guid identityId,
        CancellationToken cancellationToken = default);
}

public class BehavioralEvent
{
    public Guid IdentityId { get; set; }
    public BehavioralEventType EventType { get; set; }
    public DateTime Timestamp { get; set; } = DateTime.UtcNow;
    public string? IPAddress { get; set; }
    public string? DeviceId { get; set; }
    public string? Location { get; set; }
    public Dictionary<string, object>? Metadata { get; set; }
}

public enum BehavioralEventType
{
    Login = 1,
    Logout = 2,
    PageView = 3,
    FeatureAccess = 4,
    SessionStart = 5,
    SessionEnd = 6,
    KeystrokeCapture = 7,
    MouseMovement = 8
}

public class BehavioralAnalysisResult
{
    public double DeviationScore { get; set; }
    public bool IsAnomalous { get; set; }
    public List<BehavioralDeviation> Deviations { get; set; } = new();
    public double Confidence { get; set; }
}

public class BehavioralAnomaly
{
    public Guid AnomalyId { get; set; } = Guid.NewGuid();
    public Guid IdentityId { get; set; }
    public string AnomalyType { get; set; } = null!;
    public string Description { get; set; } = null!;
    public double Severity { get; set; }
    public DateTime DetectedAt { get; set; } = DateTime.UtcNow;
    public Dictionary<string, string> Evidence { get; set; } = new();
}
