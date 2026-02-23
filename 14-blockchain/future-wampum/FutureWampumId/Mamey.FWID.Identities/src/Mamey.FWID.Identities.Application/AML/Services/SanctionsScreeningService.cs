using System.Diagnostics;
using Mamey.FWID.Identities.Application.AML.Models;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;

namespace Mamey.FWID.Identities.Application.AML.Services;

/// <summary>
/// Sanctions screening service implementation.
/// Uses Jaro-Winkler distance for fuzzy name matching.
/// </summary>
public class SanctionsScreeningService : ISanctionsScreeningService
{
    private readonly ILogger<SanctionsScreeningService> _logger;
    private readonly IMemoryCache _cache;
    
    // In-memory sanctions data (in production, use database)
    private readonly Dictionary<string, SanctionsList> _sanctionsLists = new();
    private readonly List<SanctionsEntry> _sanctionsEntries = new();
    private readonly Dictionary<Guid, ScreeningResult> _screeningResults = new();
    private readonly object _lock = new();
    
    private const double MatchThreshold = 0.85;
    private static readonly TimeSpan CacheExpiration = TimeSpan.FromHours(24);
    
    public SanctionsScreeningService(
        ILogger<SanctionsScreeningService> logger,
        IMemoryCache cache)
    {
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        _cache = cache ?? throw new ArgumentNullException(nameof(cache));
        
        InitializeSanctionsLists();
    }
    
    /// <inheritdoc />
    public async Task<ScreeningResult> ScreenIdentityAsync(
        SanctionsScreeningRequest request,
        CancellationToken cancellationToken = default)
    {
        var stopwatch = Stopwatch.StartNew();
        
        _logger.LogInformation("Screening identity {IdentityId} against sanctions lists",
            request.IdentityId);
        
        // Check cache first
        if (request.UseCache)
        {
            var cached = await GetCachedResultAsync(request.IdentityId, cancellationToken);
            if (cached != null && cached.CacheExpiresAt > DateTime.UtcNow)
            {
                _logger.LogDebug("Returning cached screening result for {IdentityId}", request.IdentityId);
                return cached;
            }
        }
        
        var result = new ScreeningResult
        {
            IdentityId = request.IdentityId,
            ScreeningType = ScreeningType.Sanctions,
            SourcesChecked = _sanctionsLists.Keys.ToList(),
            CacheExpiresAt = DateTime.UtcNow.Add(CacheExpiration)
        };
        
        try
        {
            // Screen against all entries
            var namesToCheck = new List<string> { $"{request.FirstName} {request.LastName}" };
            if (!string.IsNullOrEmpty(request.MiddleName))
            {
                namesToCheck.Add($"{request.FirstName} {request.MiddleName} {request.LastName}");
            }
            if (request.Aliases?.Any() == true)
            {
                namesToCheck.AddRange(request.Aliases);
            }
            
            foreach (var entry in _sanctionsEntries)
            {
                // Skip if specific lists requested and entry not in them
                if (request.SpecificLists?.Any() == true &&
                    !request.SpecificLists.Contains(entry.ListId))
                {
                    continue;
                }
                
                var match = CheckForMatch(namesToCheck, request.DateOfBirth, entry);
                if (match != null)
                {
                    result.Matches.Add(match);
                }
            }
            
            // Determine status
            if (!result.HasMatches)
            {
                result.Status = ScreeningStatus.Clear;
            }
            else if (result.HighestMatchScore >= 95)
            {
                result.Status = ScreeningStatus.PotentialMatch;
                result.RequiresManualReview = true;
            }
            else
            {
                result.Status = ScreeningStatus.PotentialMatch;
                result.RequiresManualReview = result.HighestMatchScore >= 90;
            }
            
            stopwatch.Stop();
            result.ProcessingTime = stopwatch.Elapsed;
            
            // Cache result
            _cache.Set($"sanctions:{request.IdentityId}", result, CacheExpiration);
            
            lock (_lock)
            {
                _screeningResults[result.ScreeningId] = result;
            }
            
            _logger.LogInformation(
                "Sanctions screening completed for {IdentityId}. Status: {Status}, Matches: {MatchCount}, Time: {Time}ms",
                request.IdentityId, result.Status, result.Matches.Count, stopwatch.ElapsedMilliseconds);
            
            return result;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error screening identity {IdentityId}", request.IdentityId);
            
            result.Status = ScreeningStatus.Error;
            stopwatch.Stop();
            result.ProcessingTime = stopwatch.Elapsed;
            
            return result;
        }
    }
    
    /// <inheritdoc />
    public Task<ScreeningResult> ScreenNameAsync(
        string firstName,
        string lastName,
        DateTime? dateOfBirth = null,
        string? nationality = null,
        CancellationToken cancellationToken = default)
    {
        return ScreenIdentityAsync(new SanctionsScreeningRequest
        {
            IdentityId = Guid.Empty,
            FirstName = firstName,
            LastName = lastName,
            DateOfBirth = dateOfBirth,
            Nationalities = nationality != null ? new List<string> { nationality } : new(),
            UseCache = false
        }, cancellationToken);
    }
    
    /// <inheritdoc />
    public Task<ScreeningResult?> GetCachedResultAsync(Guid identityId, CancellationToken cancellationToken = default)
    {
        if (_cache.TryGetValue($"sanctions:{identityId}", out ScreeningResult? cached))
        {
            return Task.FromResult(cached);
        }
        return Task.FromResult<ScreeningResult?>(null);
    }
    
    /// <inheritdoc />
    public Task InvalidateCacheAsync(Guid identityId, CancellationToken cancellationToken = default)
    {
        _cache.Remove($"sanctions:{identityId}");
        return Task.CompletedTask;
    }
    
    /// <inheritdoc />
    public Task<IReadOnlyList<SanctionsList>> GetSanctionsListsAsync(CancellationToken cancellationToken = default)
    {
        return Task.FromResult<IReadOnlyList<SanctionsList>>(_sanctionsLists.Values.ToList());
    }
    
    /// <inheritdoc />
    public Task<int> UpdateSanctionsListsAsync(CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Updating sanctions lists (mock)");
        // In production, fetch from OFAC, UN, EU APIs
        return Task.FromResult(0);
    }
    
    /// <inheritdoc />
    public Task<bool> MarkAsFalsePositiveAsync(
        Guid screeningId, Guid matchId, Guid reviewerId, string notes,
        CancellationToken cancellationToken = default)
    {
        lock (_lock)
        {
            if (!_screeningResults.TryGetValue(screeningId, out var result))
                return Task.FromResult(false);
            
            var match = result.Matches.FirstOrDefault(m => m.MatchId == matchId);
            if (match == null)
                return Task.FromResult(false);
            
            match.Status = MatchStatus.FalsePositive;
            result.ReviewedBy = reviewerId;
            result.ReviewedAt = DateTime.UtcNow;
            result.ReviewNotes = notes;
            
            if (result.Matches.All(m => m.Status == MatchStatus.FalsePositive))
            {
                result.Status = ScreeningStatus.FalsePositive;
                result.RequiresManualReview = false;
            }
            
            return Task.FromResult(true);
        }
    }
    
    /// <inheritdoc />
    public Task<bool> ConfirmMatchAsync(
        Guid screeningId, Guid matchId, Guid reviewerId, string notes,
        CancellationToken cancellationToken = default)
    {
        lock (_lock)
        {
            if (!_screeningResults.TryGetValue(screeningId, out var result))
                return Task.FromResult(false);
            
            var match = result.Matches.FirstOrDefault(m => m.MatchId == matchId);
            if (match == null)
                return Task.FromResult(false);
            
            match.Status = MatchStatus.ConfirmedMatch;
            result.Status = ScreeningStatus.ConfirmedMatch;
            result.ReviewedBy = reviewerId;
            result.ReviewedAt = DateTime.UtcNow;
            result.ReviewNotes = notes;
            
            return Task.FromResult(true);
        }
    }
    
    #region Private Methods
    
    private void InitializeSanctionsLists()
    {
        // Initialize sanctions lists
        _sanctionsLists["OFAC_SDN"] = new SanctionsList
        {
            ListId = "OFAC_SDN",
            Source = "OFAC",
            Name = "Specially Designated Nationals And Blocked Persons List",
            UpdateUrl = "https://sanctionssearch.ofac.treas.gov/",
            UpdateFrequencyHours = 24,
            LastUpdated = DateTime.UtcNow.AddHours(-6),
            ExpiresAt = DateTime.UtcNow.AddDays(1)
        };
        
        _sanctionsLists["UN_SC"] = new SanctionsList
        {
            ListId = "UN_SC",
            Source = "UN",
            Name = "UN Security Council Consolidated List",
            UpdateUrl = "https://scsanctions.un.org/",
            UpdateFrequencyHours = 24,
            LastUpdated = DateTime.UtcNow.AddHours(-12),
            ExpiresAt = DateTime.UtcNow.AddDays(1)
        };
        
        _sanctionsLists["EU_CONS"] = new SanctionsList
        {
            ListId = "EU_CONS",
            Source = "EU",
            Name = "EU Consolidated Financial Sanctions List",
            UpdateUrl = "https://webgate.ec.europa.eu/fsd/",
            UpdateFrequencyHours = 24,
            LastUpdated = DateTime.UtcNow.AddHours(-8),
            ExpiresAt = DateTime.UtcNow.AddDays(1)
        };
        
        // Add sample entries (in production, load from database)
        AddSampleEntries();
        
        _logger.LogInformation("Initialized {Count} sanctions lists with {EntryCount} entries",
            _sanctionsLists.Count, _sanctionsEntries.Count);
    }
    
    private void AddSampleEntries()
    {
        // Sample sanctioned individuals for testing
        // Note: These are fictional entries for testing purposes
        _sanctionsEntries.Add(new SanctionsEntry
        {
            ListId = "OFAC_SDN",
            SourceId = "SAMPLE001",
            PrimaryName = "John Test Sanctioned",
            Aliases = new List<string> { "J. Sanctioned", "Johnny Sanctioned" },
            EntityType = SanctionsEntityType.Individual,
            DateOfBirth = new DateTime(1970, 5, 15),
            Nationalities = new List<string> { "XX" },
            Programs = new List<string> { "SDGT" },
            SearchTokens = new List<string> { "john", "test", "sanctioned" }
        });
        
        _sanctionsEntries.Add(new SanctionsEntry
        {
            ListId = "UN_SC",
            SourceId = "SAMPLE002",
            PrimaryName = "Sample Entity Corp",
            EntityType = SanctionsEntityType.Entity,
            Programs = new List<string> { "UN-1267" },
            SearchTokens = new List<string> { "sample", "entity", "corp" }
        });
    }
    
    private ScreeningMatch? CheckForMatch(
        List<string> namesToCheck,
        DateTime? dateOfBirth,
        SanctionsEntry entry)
    {
        double highestScore = 0;
        string? bestMatchedName = null;
        var matchCriteria = new List<MatchCriteria>();
        
        // Check primary name and aliases
        var entryNames = new List<string> { entry.PrimaryName };
        entryNames.AddRange(entry.Aliases);
        
        foreach (var inputName in namesToCheck)
        {
            foreach (var entryName in entryNames)
            {
                var score = CalculateJaroWinkler(
                    NormalizeName(inputName),
                    NormalizeName(entryName));
                
                if (score > highestScore)
                {
                    highestScore = score;
                    bestMatchedName = entryName;
                    matchCriteria.Add(new MatchCriteria
                    {
                        Field = "Name",
                        InputValue = inputName,
                        MatchedValue = entryName,
                        Score = score * 100,
                        Algorithm = "Jaro-Winkler"
                    });
                }
            }
        }
        
        // Date of birth matching (if available)
        if (dateOfBirth.HasValue && entry.DateOfBirth.HasValue)
        {
            var dobMatch = dateOfBirth.Value.Date == entry.DateOfBirth.Value.Date;
            if (dobMatch)
            {
                highestScore = Math.Min(1.0, highestScore + 0.1); // Boost score
                matchCriteria.Add(new MatchCriteria
                {
                    Field = "DateOfBirth",
                    InputValue = dateOfBirth.Value.ToString("yyyy-MM-dd"),
                    MatchedValue = entry.DateOfBirth.Value.ToString("yyyy-MM-dd"),
                    Score = 100,
                    Algorithm = "ExactMatch"
                });
            }
        }
        
        if (highestScore >= MatchThreshold)
        {
            return new ScreeningMatch
            {
                Source = entry.ListId,
                MatchedName = entry.PrimaryName,
                Aliases = entry.Aliases,
                MatchScore = highestScore * 100,
                Category = entry.EntityType.ToString(),
                MatchingCriteria = matchCriteria,
                LikelyFalsePositive = highestScore < 0.90,
                Details = new Dictionary<string, string>
                {
                    ["SourceId"] = entry.SourceId,
                    ["Programs"] = string.Join(", ", entry.Programs),
                    ["BestMatch"] = bestMatchedName ?? entry.PrimaryName
                }
            };
        }
        
        return null;
    }
    
    private static string NormalizeName(string name)
    {
        return name.ToLowerInvariant()
            .Replace("-", " ")
            .Replace("'", "")
            .Replace(".", "")
            .Trim();
    }
    
    /// <summary>
    /// Calculates Jaro-Winkler similarity between two strings.
    /// </summary>
    private static double CalculateJaroWinkler(string s1, string s2)
    {
        if (string.IsNullOrEmpty(s1) || string.IsNullOrEmpty(s2))
            return 0;
        
        if (s1 == s2)
            return 1.0;
        
        int matchWindow = Math.Max(s1.Length, s2.Length) / 2 - 1;
        if (matchWindow < 0) matchWindow = 0;
        
        var s1Matches = new bool[s1.Length];
        var s2Matches = new bool[s2.Length];
        
        int matches = 0;
        int transpositions = 0;
        
        // Find matches
        for (int i = 0; i < s1.Length; i++)
        {
            int start = Math.Max(0, i - matchWindow);
            int end = Math.Min(i + matchWindow + 1, s2.Length);
            
            for (int j = start; j < end; j++)
            {
                if (s2Matches[j] || s1[i] != s2[j]) continue;
                s1Matches[i] = true;
                s2Matches[j] = true;
                matches++;
                break;
            }
        }
        
        if (matches == 0) return 0;
        
        // Count transpositions
        int k = 0;
        for (int i = 0; i < s1.Length; i++)
        {
            if (!s1Matches[i]) continue;
            while (!s2Matches[k]) k++;
            if (s1[i] != s2[k]) transpositions++;
            k++;
        }
        
        double jaro = ((double)matches / s1.Length +
                       (double)matches / s2.Length +
                       (matches - transpositions / 2.0) / matches) / 3;
        
        // Winkler modification
        int prefix = 0;
        for (int i = 0; i < Math.Min(4, Math.Min(s1.Length, s2.Length)); i++)
        {
            if (s1[i] == s2[i]) prefix++;
            else break;
        }
        
        return jaro + prefix * 0.1 * (1 - jaro);
    }
    
    #endregion
}
