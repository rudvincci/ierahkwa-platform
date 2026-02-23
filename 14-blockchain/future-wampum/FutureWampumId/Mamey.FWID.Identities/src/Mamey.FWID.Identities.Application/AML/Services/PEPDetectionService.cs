using System.Diagnostics;
using Mamey.FWID.Identities.Application.AML.Models;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;

namespace Mamey.FWID.Identities.Application.AML.Services;

/// <summary>
/// PEP detection service implementation.
/// </summary>
public class PEPDetectionService : IPEPDetectionService
{
    private readonly ILogger<PEPDetectionService> _logger;
    private readonly IMemoryCache _cache;
    
    // In-memory PEP database (in production, use database)
    private readonly Dictionary<Guid, PEPRecord> _pepRecords = new();
    private readonly object _lock = new();
    
    private const double MatchThreshold = 0.85;
    private static readonly TimeSpan CacheExpiration = TimeSpan.FromHours(24);
    
    public PEPDetectionService(
        ILogger<PEPDetectionService> logger,
        IMemoryCache cache)
    {
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        _cache = cache ?? throw new ArgumentNullException(nameof(cache));
        
        InitializePEPDatabase();
    }
    
    /// <inheritdoc />
    public async Task<ScreeningResult> ScreenForPEPAsync(
        PEPScreeningRequest request,
        CancellationToken cancellationToken = default)
    {
        var stopwatch = Stopwatch.StartNew();
        
        _logger.LogInformation("Screening identity {IdentityId} for PEP status", request.IdentityId);
        
        // Check cache
        if (request.UseCache)
        {
            var cached = await GetCachedResultAsync(request.IdentityId, cancellationToken);
            if (cached != null && cached.CacheExpiresAt > DateTime.UtcNow)
            {
                return cached;
            }
        }
        
        var result = new ScreeningResult
        {
            IdentityId = request.IdentityId,
            ScreeningType = ScreeningType.PEP,
            SourcesChecked = new List<string> { "Global PEP Database" },
            CacheExpiresAt = DateTime.UtcNow.Add(CacheExpiration)
        };
        
        try
        {
            var fullName = $"{request.FirstName} {request.LastName}";
            
            foreach (var record in _pepRecords.Values)
            {
                var match = CheckForPEPMatch(fullName, request.DateOfBirth, record, request.IncludeRelatives);
                if (match != null)
                {
                    result.Matches.Add(match);
                }
            }
            
            // Set status
            if (!result.HasMatches)
            {
                result.Status = ScreeningStatus.Clear;
            }
            else
            {
                result.Status = ScreeningStatus.PotentialMatch;
                result.RequiresManualReview = result.HighestMatchScore >= 90;
            }
            
            stopwatch.Stop();
            result.ProcessingTime = stopwatch.Elapsed;
            
            // Cache result
            _cache.Set($"pep:{request.IdentityId}", result, CacheExpiration);
            
            _logger.LogInformation(
                "PEP screening completed for {IdentityId}. Status: {Status}, Matches: {MatchCount}",
                request.IdentityId, result.Status, result.Matches.Count);
            
            return result;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error screening identity {IdentityId} for PEP", request.IdentityId);
            result.Status = ScreeningStatus.Error;
            stopwatch.Stop();
            result.ProcessingTime = stopwatch.Elapsed;
            return result;
        }
    }
    
    /// <inheritdoc />
    public Task<PEPRecord?> GetPEPRecordAsync(Guid recordId, CancellationToken cancellationToken = default)
    {
        _pepRecords.TryGetValue(recordId, out var record);
        return Task.FromResult(record);
    }
    
    /// <inheritdoc />
    public Task<IReadOnlyList<PEPRecord>> SearchPEPDatabaseAsync(
        string searchTerm,
        int limit = 20,
        CancellationToken cancellationToken = default)
    {
        var normalized = searchTerm.ToLowerInvariant();
        var results = _pepRecords.Values
            .Where(r => r.PrimaryName.ToLowerInvariant().Contains(normalized) ||
                       r.Aliases.Any(a => a.ToLowerInvariant().Contains(normalized)))
            .Take(limit)
            .ToList();
        
        return Task.FromResult<IReadOnlyList<PEPRecord>>(results);
    }
    
    /// <inheritdoc />
    public Task<ScreeningResult?> GetCachedResultAsync(Guid identityId, CancellationToken cancellationToken = default)
    {
        if (_cache.TryGetValue($"pep:{identityId}", out ScreeningResult? cached))
        {
            return Task.FromResult(cached);
        }
        return Task.FromResult<ScreeningResult?>(null);
    }
    
    /// <inheritdoc />
    public Task<int> UpdatePEPDatabaseAsync(CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Updating PEP database (mock)");
        return Task.FromResult(0);
    }
    
    #region Private Methods
    
    private void InitializePEPDatabase()
    {
        // Add sample PEP records (fictional for testing)
        var record1 = new PEPRecord
        {
            Source = "Global PEP Database",
            SourceId = "PEP001",
            PrimaryName = "Test Political Figure",
            Category = PEPCategory.ForeignPEP,
            Tier = PEPTier.Tier2,
            IsActive = true,
            RiskLevel = PEPRiskLevel.High,
            Nationalities = new List<string> { "XX" },
            Positions = new List<PEPPosition>
            {
                new()
                {
                    Title = "Minister of Example",
                    Country = "XX",
                    IsCurrent = true,
                    StartDate = new DateTime(2020, 1, 1)
                }
            },
            LastUpdated = DateTime.UtcNow
        };
        _pepRecords[record1.Id] = record1;
        
        var record2 = new PEPRecord
        {
            Source = "Global PEP Database",
            SourceId = "PEP002",
            PrimaryName = "Sample PEP Relative",
            Category = PEPCategory.FamilyMember,
            Tier = PEPTier.Tier4,
            IsActive = true,
            RiskLevel = PEPRiskLevel.Medium,
            Relations = new List<PEPRelation>
            {
                new()
                {
                    Name = "Test Political Figure",
                    RelationType = PEPRelationType.Spouse,
                    RelatedPEPId = record1.Id.ToString()
                }
            },
            LastUpdated = DateTime.UtcNow
        };
        _pepRecords[record2.Id] = record2;
        
        _logger.LogInformation("Initialized PEP database with {Count} records", _pepRecords.Count);
    }
    
    private ScreeningMatch? CheckForPEPMatch(
        string fullName,
        DateTime? dateOfBirth,
        PEPRecord record,
        bool includeRelatives)
    {
        // Skip relatives if not requested
        if (!includeRelatives && record.Category is PEPCategory.FamilyMember or PEPCategory.CloseAssociate)
        {
            return null;
        }
        
        var namesToCheck = new List<string> { record.PrimaryName };
        namesToCheck.AddRange(record.Aliases);
        
        double highestScore = 0;
        string? bestMatch = null;
        
        foreach (var pepName in namesToCheck)
        {
            var score = CalculateSimilarity(fullName.ToLowerInvariant(), pepName.ToLowerInvariant());
            if (score > highestScore)
            {
                highestScore = score;
                bestMatch = pepName;
            }
        }
        
        // DOB boost
        if (dateOfBirth.HasValue && record.DateOfBirth.HasValue &&
            dateOfBirth.Value.Date == record.DateOfBirth.Value.Date)
        {
            highestScore = Math.Min(1.0, highestScore + 0.1);
        }
        
        if (highestScore >= MatchThreshold)
        {
            return new ScreeningMatch
            {
                Source = record.Source,
                MatchedName = record.PrimaryName,
                Aliases = record.Aliases,
                MatchScore = highestScore * 100,
                Category = $"{record.Category} ({record.Tier})",
                LikelyFalsePositive = highestScore < 0.90,
                MatchingCriteria = new List<MatchCriteria>
                {
                    new()
                    {
                        Field = "Name",
                        InputValue = fullName,
                        MatchedValue = bestMatch ?? record.PrimaryName,
                        Score = highestScore * 100,
                        Algorithm = "Jaro-Winkler"
                    }
                },
                Details = new Dictionary<string, string>
                {
                    ["SourceId"] = record.SourceId,
                    ["Category"] = record.Category.ToString(),
                    ["Tier"] = record.Tier.ToString(),
                    ["RiskLevel"] = record.RiskLevel.ToString(),
                    ["IsActive"] = record.IsActive.ToString(),
                    ["Position"] = record.Positions.FirstOrDefault()?.Title ?? "N/A"
                }
            };
        }
        
        return null;
    }
    
    private static double CalculateSimilarity(string s1, string s2)
    {
        if (s1 == s2) return 1.0;
        if (string.IsNullOrEmpty(s1) || string.IsNullOrEmpty(s2)) return 0;
        
        // Simplified Jaro-Winkler
        int matches = 0;
        var s1Words = s1.Split(' ');
        var s2Words = s2.Split(' ');
        
        foreach (var w1 in s1Words)
        {
            if (s2Words.Any(w2 => w1 == w2 || LevenshteinDistance(w1, w2) <= 2))
            {
                matches++;
            }
        }
        
        return (double)matches / Math.Max(s1Words.Length, s2Words.Length);
    }
    
    private static int LevenshteinDistance(string s1, string s2)
    {
        var d = new int[s1.Length + 1, s2.Length + 1];
        
        for (int i = 0; i <= s1.Length; i++) d[i, 0] = i;
        for (int j = 0; j <= s2.Length; j++) d[0, j] = j;
        
        for (int i = 1; i <= s1.Length; i++)
        {
            for (int j = 1; j <= s2.Length; j++)
            {
                int cost = s1[i - 1] == s2[j - 1] ? 0 : 1;
                d[i, j] = Math.Min(Math.Min(d[i - 1, j] + 1, d[i, j - 1] + 1), d[i - 1, j - 1] + cost);
            }
        }
        
        return d[s1.Length, s2.Length];
    }
    
    #endregion
}
