namespace Mamey.FWID.Identities.Application.AI.Models;

/// <summary>
/// Context for adaptive authentication decisions.
/// </summary>
public class AuthenticationContext
{
    public Guid IdentityId { get; set; }
    public DateTime Timestamp { get; set; } = DateTime.UtcNow;
    
    // Location context
    public string? IPAddress { get; set; }
    public string? Country { get; set; }
    public string? City { get; set; }
    public double? Latitude { get; set; }
    public double? Longitude { get; set; }
    
    // Device context
    public string? DeviceId { get; set; }
    public string? DeviceType { get; set; }
    public string? Browser { get; set; }
    public string? OperatingSystem { get; set; }
    
    // Session context
    public TimeSpan? SessionDuration { get; set; }
    public int? ActionCount { get; set; }
    public string? LastAction { get; set; }
    
    // Behavioral context
    public double? KeystrokeScore { get; set; }
    public double? MouseMovementScore { get; set; }
}

/// <summary>
/// Result of adaptive authentication evaluation.
/// </summary>
public class AdaptiveAuthResult
{
    public Guid EvaluationId { get; set; } = Guid.NewGuid();
    public Guid IdentityId { get; set; }
    public double RiskScore { get; set; }
    public AdaptiveAuthAction RequiredAction { get; set; }
    public List<string> RiskFactors { get; set; } = new();
    public List<BehavioralDeviation> Deviations { get; set; } = new();
    public double Confidence { get; set; }
    public DateTime EvaluatedAt { get; set; } = DateTime.UtcNow;
}

public enum AdaptiveAuthAction
{
    Allow = 1,
    Challenge = 2,
    StepUp = 3,
    Block = 4,
    ManualReview = 5
}

public class BehavioralDeviation
{
    public string DeviationType { get; set; } = null!;
    public string Description { get; set; } = null!;
    public double DeviationScore { get; set; }
    public string ExpectedValue { get; set; } = null!;
    public string ActualValue { get; set; } = null!;
}
