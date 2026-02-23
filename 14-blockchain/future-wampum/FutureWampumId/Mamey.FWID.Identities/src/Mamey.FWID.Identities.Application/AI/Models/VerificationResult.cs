namespace Mamey.FWID.Identities.Application.AI.Models;

/// <summary>
/// Result of AI identity verification.
/// </summary>
public class VerificationResult
{
    public Guid VerificationId { get; set; } = Guid.NewGuid();
    public Guid IdentityId { get; set; }
    public VerificationStatus Status { get; set; }
    public double OverallConfidence { get; set; }
    public DocumentAnalysisResult? DocumentResult { get; set; }
    public BiometricMatchResult? BiometricResult { get; set; }
    public FraudScore? FraudAnalysis { get; set; }
    public List<VerificationFlag> Flags { get; set; } = new();
    public DateTime VerifiedAt { get; set; } = DateTime.UtcNow;
    public TimeSpan ProcessingTime { get; set; }
    public List<AIDecisionAudit> DecisionAuditTrail { get; set; } = new();
}

public enum VerificationStatus
{
    Verified = 1,
    PendingReview = 2,
    Failed = 3,
    Rejected = 4,
    InsufficientData = 5
}

public class VerificationFlag
{
    public string Code { get; set; } = null!;
    public string Description { get; set; } = null!;
    public FlagSeverity Severity { get; set; }
    public string? Recommendation { get; set; }
}

public enum FlagSeverity
{
    Info = 1,
    Warning = 2,
    Critical = 3
}

/// <summary>
/// Audit trail for AI decisions.
/// </summary>
public class AIDecisionAudit
{
    public DateTime Timestamp { get; set; }
    public string DecisionType { get; set; } = null!;
    public string ModelUsed { get; set; } = null!;
    public string ModelVersion { get; set; } = null!;
    public double Confidence { get; set; }
    public Dictionary<string, double> FeatureImportance { get; set; } = new();
    public string? Explanation { get; set; }
}
