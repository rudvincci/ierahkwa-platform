namespace Mamey.FWID.Identities.Application.AI.Models;

/// <summary>
/// AI-generated fraud score for identity verification.
/// </summary>
public class FraudScore
{
    public Guid ScoreId { get; set; } = Guid.NewGuid();
    public Guid IdentityId { get; set; }
    public double OverallScore { get; set; }
    public FraudRiskLevel RiskLevel { get; set; }
    public List<FraudSignal> Signals { get; set; } = new();
    public VelocityAnalysis? VelocityAnalysis { get; set; }
    public DeviceAnalysis? DeviceAnalysis { get; set; }
    public NetworkAnalysis? NetworkAnalysis { get; set; }
    public BehaviorAnalysis? BehaviorAnalysis { get; set; }
    public string? Recommendation { get; set; }
    public DateTime ScoredAt { get; set; } = DateTime.UtcNow;
    public string ModelVersion { get; set; } = "1.0.0";
}

public enum FraudRiskLevel
{
    VeryLow = 1,
    Low = 2,
    Medium = 3,
    High = 4,
    VeryHigh = 5
}

public class FraudSignal
{
    public string SignalId { get; set; } = null!;
    public string SignalType { get; set; } = null!;
    public string Description { get; set; } = null!;
    public double Weight { get; set; }
    public double Score { get; set; }
    public string? Evidence { get; set; }
}

public class VelocityAnalysis
{
    public int RegistrationsLast24Hours { get; set; }
    public int RegistrationsLast7Days { get; set; }
    public int SameDeviceRegistrations { get; set; }
    public int SameIPRegistrations { get; set; }
    public double VelocityScore { get; set; }
    public bool IsAnomalous { get; set; }
}

public class DeviceAnalysis
{
    public string? DeviceId { get; set; }
    public string? DeviceType { get; set; }
    public string? OperatingSystem { get; set; }
    public string? Browser { get; set; }
    public bool IsEmulator { get; set; }
    public bool IsRooted { get; set; }
    public bool IsVPN { get; set; }
    public bool IsProxy { get; set; }
    public bool IsTor { get; set; }
    public int DeviceAge { get; set; }
    public double DeviceTrustScore { get; set; }
}

public class NetworkAnalysis
{
    public string? IPAddress { get; set; }
    public string? Country { get; set; }
    public string? City { get; set; }
    public string? ISP { get; set; }
    public bool IsDataCenter { get; set; }
    public bool IsKnownBadIP { get; set; }
    public double IPReputationScore { get; set; }
    public bool GeoLocationMismatch { get; set; }
}

public class BehaviorAnalysis
{
    public double MouseMovementScore { get; set; }
    public double KeystrokeDynamicsScore { get; set; }
    public double SessionDuration { get; set; }
    public int PageInteractions { get; set; }
    public bool IsBotLike { get; set; }
    public double HumanLikelihoodScore { get; set; }
}
