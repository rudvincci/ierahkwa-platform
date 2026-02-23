namespace Mamey.FWID.Identities.Application.AML.Models;

/// <summary>
/// AML risk profile for an identity.
/// </summary>
public class RiskProfile
{
    /// <summary>
    /// Profile ID.
    /// </summary>
    public Guid Id { get; set; } = Guid.NewGuid();
    
    /// <summary>
    /// Identity ID.
    /// </summary>
    public Guid IdentityId { get; set; }
    
    /// <summary>
    /// Overall risk score (0-100).
    /// </summary>
    public double RiskScore { get; set; }
    
    /// <summary>
    /// Risk level derived from score.
    /// </summary>
    public RiskLevel RiskLevel { get; set; }
    
    /// <summary>
    /// Individual risk factors.
    /// </summary>
    public List<RiskFactor> RiskFactors { get; set; } = new();
    
    /// <summary>
    /// PEP status.
    /// </summary>
    public bool IsPEP { get; set; }
    
    /// <summary>
    /// PEP category (if applicable).
    /// </summary>
    public PEPCategory? PEPCategory { get; set; }
    
    /// <summary>
    /// Has sanctions matches.
    /// </summary>
    public bool HasSanctionsMatches { get; set; }
    
    /// <summary>
    /// Required due diligence level.
    /// </summary>
    public DueDiligenceLevel RequiredDueDiligence { get; set; }
    
    /// <summary>
    /// Recommended authentication level.
    /// </summary>
    public AuthenticationLevel RecommendedAuthLevel { get; set; }
    
    /// <summary>
    /// Monitoring frequency.
    /// </summary>
    public MonitoringFrequency MonitoringFrequency { get; set; }
    
    /// <summary>
    /// Next scheduled review.
    /// </summary>
    public DateTime? NextReviewDate { get; set; }
    
    /// <summary>
    /// When the profile was created.
    /// </summary>
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    
    /// <summary>
    /// When the profile was last updated.
    /// </summary>
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
    
    /// <summary>
    /// Risk score history.
    /// </summary>
    public List<RiskScoreHistoryEntry> History { get; set; } = new();
    
    /// <summary>
    /// Notes from reviewers.
    /// </summary>
    public List<RiskReviewNote> ReviewNotes { get; set; } = new();
}

/// <summary>
/// Individual risk factor.
/// </summary>
public class RiskFactor
{
    public string FactorId { get; set; } = null!;
    public string Name { get; set; } = null!;
    public string Category { get; set; } = null!;
    public double Weight { get; set; }
    public double Score { get; set; }
    public double WeightedScore => Weight * Score;
    public string? Source { get; set; }
    public string? Rationale { get; set; }
    public DateTime EvaluatedAt { get; set; } = DateTime.UtcNow;
}

/// <summary>
/// Risk level.
/// </summary>
public enum RiskLevel
{
    Low = 1,
    Medium = 2,
    High = 3,
    Critical = 4
}

/// <summary>
/// Required due diligence level.
/// </summary>
public enum DueDiligenceLevel
{
    Standard = 1,
    Enhanced = 2,
    Comprehensive = 3
}

/// <summary>
/// Recommended authentication level.
/// </summary>
public enum AuthenticationLevel
{
    Standard = 1,
    Enhanced = 2,
    StepUp = 3,
    ManualReview = 4
}

/// <summary>
/// Monitoring frequency.
/// </summary>
public enum MonitoringFrequency
{
    Annual = 1,
    SemiAnnual = 2,
    Quarterly = 3,
    Monthly = 4,
    Continuous = 5
}

/// <summary>
/// Historical risk score entry.
/// </summary>
public class RiskScoreHistoryEntry
{
    public DateTime Timestamp { get; set; }
    public double Score { get; set; }
    public RiskLevel Level { get; set; }
    public string? ChangeReason { get; set; }
}

/// <summary>
/// Review note on risk profile.
/// </summary>
public class RiskReviewNote
{
    public Guid ReviewerId { get; set; }
    public string ReviewerName { get; set; } = null!;
    public DateTime Timestamp { get; set; }
    public string Note { get; set; } = null!;
    public string? Action { get; set; }
}
