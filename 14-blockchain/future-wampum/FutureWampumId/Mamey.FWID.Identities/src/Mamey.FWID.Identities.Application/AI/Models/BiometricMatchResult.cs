namespace Mamey.FWID.Identities.Application.AI.Models;

/// <summary>
/// Result of AI biometric matching.
/// </summary>
public class BiometricMatchResult
{
    public Guid MatchId { get; set; } = Guid.NewGuid();
    public BiometricMatchType MatchType { get; set; }
    public double MatchScore { get; set; }
    public double Confidence { get; set; }
    public bool IsMatch { get; set; }
    public LivenessResult? LivenessCheck { get; set; }
    public SpoofDetectionResult? SpoofDetection { get; set; }
    public List<BiometricQualityMetric> QualityMetrics { get; set; } = new();
    public DateTime MatchedAt { get; set; } = DateTime.UtcNow;
    public string ModelVersion { get; set; } = "1.0.0";
}

public enum BiometricMatchType
{
    FaceToDocument = 1,
    FaceToSelfie = 2,
    FaceToDatabase = 3,
    FingerprintMatch = 4,
    IrisMatch = 5,
    VoiceMatch = 6
}

public class LivenessResult
{
    public bool IsLive { get; set; }
    public double LivenessScore { get; set; }
    public LivenessCheckType CheckType { get; set; }
    public List<LivenessChallenge>? ChallengesCompleted { get; set; }
}

public enum LivenessCheckType
{
    Passive = 1,
    Active = 2,
    Hybrid = 3
}

public class LivenessChallenge
{
    public string ChallengeType { get; set; } = null!;
    public bool Passed { get; set; }
    public double Score { get; set; }
}

public class SpoofDetectionResult
{
    public bool SpoofDetected { get; set; }
    public double SpoofProbability { get; set; }
    public SpoofType? DetectedSpoofType { get; set; }
    public List<string> SpoofIndicators { get; set; } = new();
}

public enum SpoofType
{
    PrintAttack = 1,
    ScreenReplay = 2,
    Mask3D = 3,
    Deepfake = 4,
    Other = 99
}

public class BiometricQualityMetric
{
    public string MetricName { get; set; } = null!;
    public double Score { get; set; }
    public bool MeetsThreshold { get; set; }
    public double Threshold { get; set; }
}
