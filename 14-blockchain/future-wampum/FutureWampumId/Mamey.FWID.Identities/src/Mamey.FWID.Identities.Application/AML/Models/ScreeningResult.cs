namespace Mamey.FWID.Identities.Application.AML.Models;

/// <summary>
/// Result of sanctions or PEP screening.
/// </summary>
public class ScreeningResult
{
    /// <summary>
    /// Unique screening ID.
    /// </summary>
    public Guid ScreeningId { get; set; } = Guid.NewGuid();
    
    /// <summary>
    /// Identity being screened.
    /// </summary>
    public Guid IdentityId { get; set; }
    
    /// <summary>
    /// Type of screening performed.
    /// </summary>
    public ScreeningType ScreeningType { get; set; }
    
    /// <summary>
    /// Overall screening status.
    /// </summary>
    public ScreeningStatus Status { get; set; }
    
    /// <summary>
    /// Whether any matches were found.
    /// </summary>
    public bool HasMatches => Matches.Any();
    
    /// <summary>
    /// List of matches found.
    /// </summary>
    public List<ScreeningMatch> Matches { get; set; } = new();
    
    /// <summary>
    /// Highest match score (0-100).
    /// </summary>
    public double HighestMatchScore => Matches.Any() ? Matches.Max(m => m.MatchScore) : 0;
    
    /// <summary>
    /// Data sources checked.
    /// </summary>
    public List<string> SourcesChecked { get; set; } = new();
    
    /// <summary>
    /// When screening was performed.
    /// </summary>
    public DateTime ScreenedAt { get; set; } = DateTime.UtcNow;
    
    /// <summary>
    /// Processing time.
    /// </summary>
    public TimeSpan ProcessingTime { get; set; }
    
    /// <summary>
    /// Whether manual review is required.
    /// </summary>
    public bool RequiresManualReview { get; set; }
    
    /// <summary>
    /// Review notes (if reviewed).
    /// </summary>
    public string? ReviewNotes { get; set; }
    
    /// <summary>
    /// Reviewer ID (if reviewed).
    /// </summary>
    public Guid? ReviewedBy { get; set; }
    
    /// <summary>
    /// When reviewed.
    /// </summary>
    public DateTime? ReviewedAt { get; set; }
    
    /// <summary>
    /// Cache expiration.
    /// </summary>
    public DateTime CacheExpiresAt { get; set; }
}

/// <summary>
/// Type of screening.
/// </summary>
public enum ScreeningType
{
    Sanctions = 1,
    PEP = 2,
    AdverseMedia = 3,
    Combined = 4
}

/// <summary>
/// Screening status.
/// </summary>
public enum ScreeningStatus
{
    Clear = 1,
    PotentialMatch = 2,
    ConfirmedMatch = 3,
    FalsePositive = 4,
    PendingReview = 5,
    Error = 6
}

/// <summary>
/// A single match from screening.
/// </summary>
public class ScreeningMatch
{
    /// <summary>
    /// Match ID.
    /// </summary>
    public Guid MatchId { get; set; } = Guid.NewGuid();
    
    /// <summary>
    /// Source list/database.
    /// </summary>
    public string Source { get; set; } = null!;
    
    /// <summary>
    /// Matched entity name.
    /// </summary>
    public string MatchedName { get; set; } = null!;
    
    /// <summary>
    /// Matched entity aliases.
    /// </summary>
    public List<string> Aliases { get; set; } = new();
    
    /// <summary>
    /// Match score (0-100).
    /// </summary>
    public double MatchScore { get; set; }
    
    /// <summary>
    /// Match category.
    /// </summary>
    public string Category { get; set; } = null!;
    
    /// <summary>
    /// Additional details.
    /// </summary>
    public Dictionary<string, string> Details { get; set; } = new();
    
    /// <summary>
    /// Matching criteria that triggered the match.
    /// </summary>
    public List<MatchCriteria> MatchingCriteria { get; set; } = new();
    
    /// <summary>
    /// Whether this is likely a false positive.
    /// </summary>
    public bool LikelyFalsePositive { get; set; }
    
    /// <summary>
    /// Match status after review.
    /// </summary>
    public MatchStatus Status { get; set; } = MatchStatus.Pending;
}

/// <summary>
/// Match status.
/// </summary>
public enum MatchStatus
{
    Pending = 1,
    ConfirmedMatch = 2,
    FalsePositive = 3,
    Escalated = 4
}

/// <summary>
/// Criteria that triggered a match.
/// </summary>
public class MatchCriteria
{
    public string Field { get; set; } = null!;
    public string InputValue { get; set; } = null!;
    public string MatchedValue { get; set; } = null!;
    public double Score { get; set; }
    public string Algorithm { get; set; } = null!;
}
