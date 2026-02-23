using NET10.Core.Interfaces;
using NET10.Core.Models;
using System.Globalization;

namespace NET10.Infrastructure.Services;

/// <summary>
/// GitHub-style contribution graph service
/// Tracks all platform activity: commits, transactions, votes, stakes, etc.
/// </summary>
public class ContributionService : IContributionService
{
    private readonly List<Contribution> _contributions = new();
    private readonly List<ProjectActivity> _projects = new();
    private static readonly Random _random = new();

    public ContributionService()
    {
        // Initialize with sample data for the platform
        InitializeSampleData();
    }

    private void InitializeSampleData()
    {
        // Generate realistic contribution data for the past year
        var startDate = DateTime.UtcNow.AddYears(-1);
        var endDate = DateTime.UtcNow;
        
        var projectNames = new[]
        {
            "CitizenCRM", "TradeX Exchange", "SmartSchool", "DocumentFlow",
            "BudgetControl", "HRM System", "InventoryManager", "AssetTracker",
            "NotifyHub", "ReportEngine", "FormBuilder", "DataHub",
            "DigitalVault", "ContractManager", "ServiceDesk", "MeetingHub",
            "AuditTrail", "AppBuilder", "BiometricAuth", "ESignature"
        };

        var types = Enum.GetValues<ContributionType>();
        var users = new[] { "admin", "developer1", "citizen001", "treasurer", "prime-minister" };

        foreach (var user in users)
        {
            for (var date = startDate; date <= endDate; date = date.AddDays(1))
            {
                // Random number of contributions per day (weighted towards fewer)
                var contributionsToday = _random.Next(0, 100) switch
                {
                    < 30 => 0,
                    < 50 => _random.Next(1, 3),
                    < 70 => _random.Next(2, 6),
                    < 85 => _random.Next(4, 10),
                    < 95 => _random.Next(8, 15),
                    _ => _random.Next(12, 25)
                };

                for (var i = 0; i < contributionsToday; i++)
                {
                    var project = projectNames[_random.Next(projectNames.Length)];
                    var type = types[_random.Next(types.Length)];
                    var hour = _random.Next(8, 22); // Working hours
                    var minute = _random.Next(0, 60);

                    _contributions.Add(new Contribution
                    {
                        Id = Guid.NewGuid().ToString(),
                        UserId = user,
                        ProjectId = project.ToLower().Replace(" ", "-"),
                        ProjectName = project,
                        Type = type,
                        Title = GenerateContributionTitle(type, project),
                        Description = GenerateContributionDescription(type),
                        Hash = GenerateHash(),
                        LinesAdded = type == ContributionType.Commit ? _random.Next(1, 500) : 0,
                        LinesRemoved = type == ContributionType.Commit ? _random.Next(0, 200) : 0,
                        FilesChanged = type == ContributionType.Commit ? _random.Next(1, 20) : 0,
                        Timestamp = new DateTime(date.Year, date.Month, date.Day, hour, minute, _random.Next(60)),
                        Branch = "main"
                    });
                }
            }
        }

        // Initialize projects
        foreach (var name in projectNames)
        {
            _projects.Add(new ProjectActivity
            {
                ProjectId = name.ToLower().Replace(" ", "-"),
                ProjectName = name,
                Description = $"Ierahkwa Government Platform Module - {name}",
                Language = GetRandomLanguage(),
                LanguageColor = GetLanguageColor(GetRandomLanguage()),
                TotalContributions = _contributions.Count(c => c.ProjectName == name),
                Stars = _random.Next(10, 500),
                Forks = _random.Next(1, 50),
                LastActivity = DateTime.UtcNow.AddHours(-_random.Next(1, 72)),
                IsPrivate = _random.Next(0, 10) < 3,
                Contributors = users.Take(_random.Next(1, users.Length)).ToList()
            });
        }
    }

    private string GenerateContributionTitle(ContributionType type, string project) => type switch
    {
        ContributionType.Commit => $"feat({project}): {GetRandomCommitMessage()}",
        ContributionType.PullRequest => $"PR: {GetRandomPRTitle()}",
        ContributionType.Issue => $"Issue: {GetRandomIssueTitle()}",
        ContributionType.Review => "Code review completed",
        ContributionType.Deployment => $"Deploy {project} to production",
        ContributionType.Transaction => "Blockchain transaction confirmed",
        ContributionType.Vote => "Governance vote submitted",
        ContributionType.Stake => "Tokens staked in farm",
        ContributionType.Swap => "Token swap executed",
        ContributionType.Farm => "Farm rewards harvested",
        ContributionType.Document => "Document uploaded",
        ContributionType.Meeting => "Meeting scheduled",
        ContributionType.Task => "Task completed",
        _ => "Activity recorded"
    };

    private static string GetRandomCommitMessage() => _random.Next(10) switch
    {
        0 => "add new feature implementation",
        1 => "fix authentication bug",
        2 => "update UI components",
        3 => "improve performance",
        4 => "add unit tests",
        5 => "refactor database queries",
        6 => "update dependencies",
        7 => "add documentation",
        8 => "fix responsive layout",
        _ => "improve error handling"
    };

    private static string GetRandomPRTitle() => _random.Next(5) switch
    {
        0 => "Feature: User dashboard improvements",
        1 => "Fix: API endpoint optimization",
        2 => "Update: Security patches",
        3 => "Add: New reporting module",
        _ => "Refactor: Code cleanup"
    };

    private static string GetRandomIssueTitle() => _random.Next(5) switch
    {
        0 => "Bug in login flow",
        1 => "Feature request: Dark mode",
        2 => "Performance issue on dashboard",
        3 => "API timeout errors",
        _ => "Documentation update needed"
    };

    private string GenerateContributionDescription(ContributionType type) =>
        $"Automated {type.ToString().ToLower()} activity on Ierahkwa Platform";

    private static string GenerateHash() =>
        string.Concat(Enumerable.Range(0, 7).Select(_ => "0123456789abcdef"[_random.Next(16)]));

    private static string GetRandomLanguage() => _random.Next(6) switch
    {
        0 => "C#",
        1 => "TypeScript",
        2 => "JavaScript",
        3 => "HTML",
        4 => "CSS",
        _ => "Rust"
    };

    private static string GetLanguageColor(string lang) => lang switch
    {
        "C#" => "#178600",
        "TypeScript" => "#3178c6",
        "JavaScript" => "#f1e05a",
        "HTML" => "#e34c26",
        "CSS" => "#563d7c",
        "Rust" => "#dea584",
        _ => "#6e7681"
    };

    // ========================================
    // IContributionService Implementation
    // ========================================

    public Task<ContributionGraph> GetContributionGraphAsync(string userId, int year)
    {
        var startDate = new DateTime(year, 1, 1);
        var endDate = year == DateTime.UtcNow.Year 
            ? DateTime.UtcNow 
            : new DateTime(year, 12, 31);

        var userContributions = _contributions
            .Where(c => c.UserId == userId && c.Timestamp.Year == year)
            .ToList();

        var dailyContributions = new List<DailyContribution>();
        var weeks = new List<WeekContribution>();
        var currentWeek = new WeekContribution { WeekNumber = 1, Days = new List<DailyContribution>() };

        for (var date = startDate; date <= endDate; date = date.AddDays(1))
        {
            var dayContributions = userContributions
                .Where(c => c.Timestamp.Date == date.Date)
                .ToList();

            var daily = new DailyContribution
            {
                Date = date,
                Count = dayContributions.Count,
                Level = CalculateLevel(dayContributions.Count),
                Contributions = dayContributions
            };

            dailyContributions.Add(daily);
            currentWeek.Days.Add(daily);

            // Start new week on Sunday
            if (date.DayOfWeek == DayOfWeek.Saturday || date == endDate)
            {
                weeks.Add(currentWeek);
                currentWeek = new WeekContribution 
                { 
                    WeekNumber = weeks.Count + 1, 
                    Days = new List<DailyContribution>() 
                };
            }
        }

        var stats = CalculateStats(userId, userContributions);

        var graph = new ContributionGraph
        {
            UserId = userId,
            Username = userId,
            Year = year,
            Days = dailyContributions,
            Stats = stats,
            Weeks = weeks,
            MonthLabels = CultureInfo.CurrentCulture.DateTimeFormat.AbbreviatedMonthNames
                .Where(m => !string.IsNullOrEmpty(m))
                .ToArray()
        };

        return Task.FromResult(graph);
    }

    public Task<ContributionGraph> GetContributionGraphAsync(string userId) =>
        GetContributionGraphAsync(userId, DateTime.UtcNow.Year);

    public Task<ContributionStats> GetContributionStatsAsync(string userId)
    {
        var userContributions = _contributions.Where(c => c.UserId == userId).ToList();
        return Task.FromResult(CalculateStats(userId, userContributions));
    }

    private ContributionStats CalculateStats(string userId, List<Contribution> contributions)
    {
        var now = DateTime.UtcNow;
        var orderedByDate = contributions.OrderBy(c => c.Timestamp).ToList();

        // Calculate streaks
        var currentStreak = 0;
        var longestStreak = 0;
        var tempStreak = 0;
        var lastDate = DateTime.MinValue;

        foreach (var group in contributions.GroupBy(c => c.Timestamp.Date).OrderBy(g => g.Key))
        {
            if (lastDate == DateTime.MinValue || (group.Key - lastDate).Days == 1)
            {
                tempStreak++;
            }
            else
            {
                tempStreak = 1;
            }

            longestStreak = Math.Max(longestStreak, tempStreak);
            lastDate = group.Key;
        }

        // Current streak (counting back from today)
        for (var date = now.Date; date >= now.AddDays(-365).Date; date = date.AddDays(-1))
        {
            if (contributions.Any(c => c.Timestamp.Date == date))
                currentStreak++;
            else if (date != now.Date) // Allow today to have no contributions yet
                break;
        }

        // Most active day and hour
        var byDay = contributions.GroupBy(c => c.Timestamp.DayOfWeek)
            .OrderByDescending(g => g.Count())
            .FirstOrDefault();
        var byHour = contributions.GroupBy(c => c.Timestamp.Hour)
            .OrderByDescending(g => g.Count())
            .FirstOrDefault();

        return new ContributionStats
        {
            UserId = userId,
            Username = userId,
            TotalContributions = contributions.Count,
            CurrentStreak = currentStreak,
            LongestStreak = longestStreak,
            TotalProjects = contributions.Select(c => c.ProjectId).Distinct().Count(),
            FirstContribution = orderedByDate.FirstOrDefault()?.Timestamp ?? DateTime.MinValue,
            LastContribution = orderedByDate.LastOrDefault()?.Timestamp ?? DateTime.MinValue,
            ContributionsThisYear = contributions.Count(c => c.Timestamp.Year == now.Year),
            ContributionsThisMonth = contributions.Count(c => c.Timestamp.Year == now.Year && c.Timestamp.Month == now.Month),
            ContributionsThisWeek = contributions.Count(c => c.Timestamp >= now.AddDays(-7)),
            ContributionsToday = contributions.Count(c => c.Timestamp.Date == now.Date),
            ByType = contributions.GroupBy(c => c.Type.ToString())
                .ToDictionary(g => g.Key, g => g.Count()),
            ByProject = contributions.GroupBy(c => c.ProjectName)
                .OrderByDescending(g => g.Count())
                .Take(10)
                .ToDictionary(g => g.Key, g => g.Count()),
            MostActiveDay = byDay?.Key ?? DayOfWeek.Monday,
            MostActiveHour = byHour?.Key ?? 10
        };
    }

    private static int CalculateLevel(int count) => count switch
    {
        0 => 0,
        < 3 => 1,
        < 6 => 2,
        < 10 => 3,
        _ => 4
    };

    public Task<List<DailyContribution>> GetDailyContributionsAsync(string userId, DateTime startDate, DateTime endDate)
    {
        var userContributions = _contributions
            .Where(c => c.UserId == userId && c.Timestamp >= startDate && c.Timestamp <= endDate)
            .ToList();

        var result = new List<DailyContribution>();
        for (var date = startDate; date <= endDate; date = date.AddDays(1))
        {
            var dayContributions = userContributions.Where(c => c.Timestamp.Date == date.Date).ToList();
            result.Add(new DailyContribution
            {
                Date = date,
                Count = dayContributions.Count,
                Level = CalculateLevel(dayContributions.Count),
                Contributions = dayContributions
            });
        }

        return Task.FromResult(result);
    }

    public Task<DailyContribution> GetContributionsForDateAsync(string userId, DateTime date)
    {
        var dayContributions = _contributions
            .Where(c => c.UserId == userId && c.Timestamp.Date == date.Date)
            .ToList();

        return Task.FromResult(new DailyContribution
        {
            Date = date,
            Count = dayContributions.Count,
            Level = CalculateLevel(dayContributions.Count),
            Contributions = dayContributions
        });
    }

    public Task<List<Contribution>> GetUserContributionsAsync(string userId, int limit = 100) =>
        Task.FromResult(_contributions
            .Where(c => c.UserId == userId)
            .OrderByDescending(c => c.Timestamp)
            .Take(limit)
            .ToList());

    public Task<List<Contribution>> GetProjectContributionsAsync(string projectId, int limit = 100) =>
        Task.FromResult(_contributions
            .Where(c => c.ProjectId == projectId)
            .OrderByDescending(c => c.Timestamp)
            .Take(limit)
            .ToList());

    public Task<Contribution?> GetContributionByIdAsync(string contributionId) =>
        Task.FromResult(_contributions.FirstOrDefault(c => c.Id == contributionId));

    public Task<Contribution> AddContributionAsync(Contribution contribution)
    {
        contribution.Id = Guid.NewGuid().ToString();
        contribution.Timestamp = contribution.Timestamp == default ? DateTime.UtcNow : contribution.Timestamp;
        _contributions.Add(contribution);
        return Task.FromResult(contribution);
    }

    public Task<List<Contribution>> AddContributionsBatchAsync(List<Contribution> contributions)
    {
        foreach (var c in contributions)
        {
            c.Id = Guid.NewGuid().ToString();
            c.Timestamp = c.Timestamp == default ? DateTime.UtcNow : c.Timestamp;
            _contributions.Add(c);
        }
        return Task.FromResult(contributions);
    }

    public Task<bool> DeleteContributionAsync(string contributionId)
    {
        var contribution = _contributions.FirstOrDefault(c => c.Id == contributionId);
        if (contribution == null) return Task.FromResult(false);
        _contributions.Remove(contribution);
        return Task.FromResult(true);
    }

    public Task<List<ProjectActivity>> GetUserProjectsAsync(string userId)
    {
        var userProjectIds = _contributions
            .Where(c => c.UserId == userId)
            .Select(c => c.ProjectId)
            .Distinct()
            .ToList();

        return Task.FromResult(_projects.Where(p => userProjectIds.Contains(p.ProjectId)).ToList());
    }

    public Task<ProjectActivity?> GetProjectActivityAsync(string projectId) =>
        Task.FromResult(_projects.FirstOrDefault(p => p.ProjectId == projectId));

    public Task<List<ContributionStats>> GetTopContributorsAsync(int limit = 10)
    {
        var stats = _contributions
            .GroupBy(c => c.UserId)
            .Select(g => CalculateStats(g.Key, g.ToList()))
            .OrderByDescending(s => s.TotalContributions)
            .Take(limit)
            .ToList();

        return Task.FromResult(stats);
    }

    public Task<List<ContributionStats>> GetTopContributorsThisMonthAsync(int limit = 10)
    {
        var now = DateTime.UtcNow;
        var stats = _contributions
            .Where(c => c.Timestamp.Year == now.Year && c.Timestamp.Month == now.Month)
            .GroupBy(c => c.UserId)
            .Select(g => CalculateStats(g.Key, g.ToList()))
            .OrderByDescending(s => s.TotalContributions)
            .Take(limit)
            .ToList();

        return Task.FromResult(stats);
    }

    public Task<List<ContributionStats>> GetTopContributorsThisWeekAsync(int limit = 10)
    {
        var now = DateTime.UtcNow;
        var weekStart = now.AddDays(-7);
        var stats = _contributions
            .Where(c => c.Timestamp >= weekStart)
            .GroupBy(c => c.UserId)
            .Select(g => CalculateStats(g.Key, g.ToList()))
            .OrderByDescending(s => s.TotalContributions)
            .Take(limit)
            .ToList();

        return Task.FromResult(stats);
    }
}
