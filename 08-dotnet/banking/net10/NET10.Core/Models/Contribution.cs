namespace NET10.Core.Models;

/// <summary>
/// Represents a contribution/activity entry (like GitHub commits)
/// </summary>
public class Contribution
{
    public string Id { get; set; } = Guid.NewGuid().ToString();
    public string UserId { get; set; } = string.Empty;
    public string ProjectId { get; set; } = string.Empty;
    public string ProjectName { get; set; } = string.Empty;
    public ContributionType Type { get; set; } = ContributionType.Commit;
    public string Title { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public string Hash { get; set; } = string.Empty; // Unique identifier like commit hash
    public int LinesAdded { get; set; }
    public int LinesRemoved { get; set; }
    public int FilesChanged { get; set; }
    public DateTime Timestamp { get; set; } = DateTime.UtcNow;
    public string Branch { get; set; } = "main";
    public Dictionary<string, object> Metadata { get; set; } = new();
}

/// <summary>
/// Types of contributions
/// </summary>
public enum ContributionType
{
    Commit,
    PullRequest,
    Issue,
    Review,
    Deployment,
    Transaction,
    Vote,
    Stake,
    Swap,
    Farm,
    Document,
    Meeting,
    Task,
    Other
}

/// <summary>
/// Daily contribution summary
/// </summary>
public class DailyContribution
{
    public DateTime Date { get; set; }
    public int Count { get; set; }
    public int Level { get; set; } // 0-4 for intensity (like GitHub)
    public List<Contribution> Contributions { get; set; } = new();
}

/// <summary>
/// Contribution statistics
/// </summary>
public class ContributionStats
{
    public string UserId { get; set; } = string.Empty;
    public string Username { get; set; } = string.Empty;
    public int TotalContributions { get; set; }
    public int CurrentStreak { get; set; }
    public int LongestStreak { get; set; }
    public int TotalProjects { get; set; }
    public DateTime FirstContribution { get; set; }
    public DateTime LastContribution { get; set; }
    public int ContributionsThisYear { get; set; }
    public int ContributionsThisMonth { get; set; }
    public int ContributionsThisWeek { get; set; }
    public int ContributionsToday { get; set; }
    public Dictionary<string, int> ByType { get; set; } = new();
    public Dictionary<string, int> ByProject { get; set; } = new();
    public DayOfWeek MostActiveDay { get; set; }
    public int MostActiveHour { get; set; }
}

/// <summary>
/// Full contribution graph data
/// </summary>
public class ContributionGraph
{
    public string UserId { get; set; } = string.Empty;
    public string Username { get; set; } = string.Empty;
    public int Year { get; set; } = DateTime.UtcNow.Year;
    public List<DailyContribution> Days { get; set; } = new();
    public ContributionStats Stats { get; set; } = new();
    public List<WeekContribution> Weeks { get; set; } = new();
    public string[] MonthLabels { get; set; } = Array.Empty<string>();
}

/// <summary>
/// Weekly contribution for graph rendering
/// </summary>
public class WeekContribution
{
    public int WeekNumber { get; set; }
    public List<DailyContribution> Days { get; set; } = new();
}

/// <summary>
/// Project with contribution activity
/// </summary>
public class ProjectActivity
{
    public string ProjectId { get; set; } = string.Empty;
    public string ProjectName { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public string Language { get; set; } = string.Empty;
    public string LanguageColor { get; set; } = "#3178c6"; // TypeScript blue default
    public int TotalContributions { get; set; }
    public int Stars { get; set; }
    public int Forks { get; set; }
    public DateTime LastActivity { get; set; }
    public bool IsPrivate { get; set; }
    public List<string> Contributors { get; set; } = new();
}
