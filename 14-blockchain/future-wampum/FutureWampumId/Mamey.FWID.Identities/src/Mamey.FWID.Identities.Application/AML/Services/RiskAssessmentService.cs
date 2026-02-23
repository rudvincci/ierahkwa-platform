using Mamey.FWID.Identities.Application.AML.Models;
using Mamey.MessageBrokers;
using Microsoft.Extensions.Logging;

namespace Mamey.FWID.Identities.Application.AML.Services;

/// <summary>
/// Risk assessment service implementation.
/// </summary>
public class RiskAssessmentService : IRiskAssessmentService
{
    private readonly ILogger<RiskAssessmentService> _logger;
    private readonly IBusPublisher _publisher;
    
    // In-memory storage
    private readonly Dictionary<Guid, RiskProfile> _riskProfiles = new();
    private readonly object _lock = new();
    
    // Risk factor weights
    private static readonly Dictionary<string, double> FactorWeights = new()
    {
        ["geography"] = 0.20,
        ["transaction_volume"] = 0.15,
        ["pep_status"] = 0.25,
        ["sanctions_proximity"] = 0.20,
        ["behavioral_anomaly"] = 0.20
    };
    
    // High-risk zones (example)
    private static readonly HashSet<string> HighRiskZones = new() { "external", "unverified" };
    
    public RiskAssessmentService(
        ILogger<RiskAssessmentService> logger,
        IBusPublisher publisher)
    {
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        _publisher = publisher ?? throw new ArgumentNullException(nameof(publisher));
    }
    
    /// <inheritdoc />
    public async Task<RiskProfile> CalculateRiskProfileAsync(
        RiskAssessmentRequest request,
        CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Calculating risk profile for identity {IdentityId}", request.IdentityId);
        
        var profile = new RiskProfile
        {
            IdentityId = request.IdentityId
        };
        
        // Calculate each risk factor
        
        // 1. Geography risk
        var geoRisk = CalculateGeographyRisk(request.Zone, request.HasHighRiskCountryConnections);
        profile.RiskFactors.Add(geoRisk);
        
        // 2. Transaction volume risk
        var txRisk = CalculateTransactionRisk(request.TransactionCount30Days, request.TransactionVolume30Days);
        profile.RiskFactors.Add(txRisk);
        
        // 3. PEP status risk
        var pepRisk = CalculatePEPRisk(request.PEPScreening);
        profile.RiskFactors.Add(pepRisk);
        profile.IsPEP = pepRisk.Score > 0;
        
        // 4. Sanctions proximity risk
        var sanctionsRisk = CalculateSanctionsRisk(request.SanctionsScreening);
        profile.RiskFactors.Add(sanctionsRisk);
        profile.HasSanctionsMatches = sanctionsRisk.Score > 50;
        
        // 5. Behavioral anomaly risk
        var behavioralRisk = CalculateBehavioralRisk(request.BehavioralAnomalyScore);
        profile.RiskFactors.Add(behavioralRisk);
        
        // Calculate overall score
        profile.RiskScore = profile.RiskFactors.Sum(f => f.WeightedScore);
        profile.RiskLevel = DetermineRiskLevel(profile.RiskScore);
        
        // Set required due diligence and monitoring
        profile.RequiredDueDiligence = DetermineDueDiligence(profile);
        profile.RecommendedAuthLevel = DetermineAuthLevel(profile);
        profile.MonitoringFrequency = DetermineMonitoringFrequency(profile);
        profile.NextReviewDate = CalculateNextReviewDate(profile);
        
        // Add to history
        profile.History.Add(new RiskScoreHistoryEntry
        {
            Timestamp = DateTime.UtcNow,
            Score = profile.RiskScore,
            Level = profile.RiskLevel,
            ChangeReason = "Initial assessment"
        });
        
        // Store profile
        lock (_lock)
        {
            _riskProfiles[request.IdentityId] = profile;
        }
        
        // Publish event if high risk
        if (profile.RiskLevel is RiskLevel.High or RiskLevel.Critical)
        {
            await _publisher.PublishAsync(new RiskLevelChangedEvent
            {
                IdentityId = request.IdentityId,
                PreviousLevel = RiskLevel.Low,
                NewLevel = profile.RiskLevel,
                RiskScore = profile.RiskScore,
                ChangedAt = DateTime.UtcNow
            });
        }
        
        _logger.LogInformation(
            "Risk profile calculated for {IdentityId}: Score={Score}, Level={Level}",
            request.IdentityId, profile.RiskScore, profile.RiskLevel);
        
        return profile;
    }
    
    /// <inheritdoc />
    public Task<RiskProfile?> GetRiskProfileAsync(Guid identityId, CancellationToken cancellationToken = default)
    {
        lock (_lock)
        {
            _riskProfiles.TryGetValue(identityId, out var profile);
            return Task.FromResult(profile);
        }
    }
    
    /// <inheritdoc />
    public async Task<RiskProfile> UpdateRiskFactorAsync(
        Guid identityId,
        RiskFactor factor,
        CancellationToken cancellationToken = default)
    {
        var profile = await GetRiskProfileAsync(identityId, cancellationToken)
            ?? throw new InvalidOperationException($"Risk profile not found for {identityId}");
        
        var existing = profile.RiskFactors.FirstOrDefault(f => f.FactorId == factor.FactorId);
        if (existing != null)
        {
            profile.RiskFactors.Remove(existing);
        }
        profile.RiskFactors.Add(factor);
        
        return await RecalculateRiskScoreAsync(identityId, $"Updated factor: {factor.Name}", cancellationToken);
    }
    
    /// <inheritdoc />
    public async Task<RiskProfile> RecalculateRiskScoreAsync(
        Guid identityId,
        string? reason = null,
        CancellationToken cancellationToken = default)
    {
        var profile = await GetRiskProfileAsync(identityId, cancellationToken)
            ?? throw new InvalidOperationException($"Risk profile not found for {identityId}");
        
        var previousLevel = profile.RiskLevel;
        var previousScore = profile.RiskScore;
        
        // Recalculate
        profile.RiskScore = profile.RiskFactors.Sum(f => f.WeightedScore);
        profile.RiskLevel = DetermineRiskLevel(profile.RiskScore);
        profile.UpdatedAt = DateTime.UtcNow;
        
        // Add to history
        profile.History.Add(new RiskScoreHistoryEntry
        {
            Timestamp = DateTime.UtcNow,
            Score = profile.RiskScore,
            Level = profile.RiskLevel,
            ChangeReason = reason ?? "Recalculated"
        });
        
        // Publish event if level changed
        if (previousLevel != profile.RiskLevel)
        {
            await _publisher.PublishAsync(new RiskLevelChangedEvent
            {
                IdentityId = identityId,
                PreviousLevel = previousLevel,
                NewLevel = profile.RiskLevel,
                RiskScore = profile.RiskScore,
                ChangedAt = DateTime.UtcNow
            });
        }
        
        return profile;
    }
    
    /// <inheritdoc />
    public Task<IReadOnlyList<RiskProfile>> GetIdentitiesRequiringReviewAsync(CancellationToken cancellationToken = default)
    {
        lock (_lock)
        {
            var profiles = _riskProfiles.Values
                .Where(p => p.NextReviewDate <= DateTime.UtcNow)
                .OrderByDescending(p => p.RiskScore)
                .ToList();
            return Task.FromResult<IReadOnlyList<RiskProfile>>(profiles);
        }
    }
    
    /// <inheritdoc />
    public Task<IReadOnlyList<RiskProfile>> GetHighRiskIdentitiesAsync(int limit = 100, CancellationToken cancellationToken = default)
    {
        lock (_lock)
        {
            var profiles = _riskProfiles.Values
                .Where(p => p.RiskLevel is RiskLevel.High or RiskLevel.Critical)
                .OrderByDescending(p => p.RiskScore)
                .Take(limit)
                .ToList();
            return Task.FromResult<IReadOnlyList<RiskProfile>>(profiles);
        }
    }
    
    /// <inheritdoc />
    public Task AddReviewNoteAsync(
        Guid identityId,
        Guid reviewerId,
        string reviewerName,
        string note,
        string? action = null,
        CancellationToken cancellationToken = default)
    {
        lock (_lock)
        {
            if (_riskProfiles.TryGetValue(identityId, out var profile))
            {
                profile.ReviewNotes.Add(new RiskReviewNote
                {
                    ReviewerId = reviewerId,
                    ReviewerName = reviewerName,
                    Timestamp = DateTime.UtcNow,
                    Note = note,
                    Action = action
                });
            }
        }
        return Task.CompletedTask;
    }
    
    #region Private Methods
    
    private RiskFactor CalculateGeographyRisk(string zone, bool? hasHighRiskCountryConnections)
    {
        double score = 0;
        string rationale;
        
        if (HighRiskZones.Contains(zone.ToLowerInvariant()))
        {
            score = 80;
            rationale = $"Zone '{zone}' is classified as high-risk";
        }
        else if (hasHighRiskCountryConnections == true)
        {
            score = 60;
            rationale = "Has connections to high-risk countries";
        }
        else
        {
            score = 20;
            rationale = "Standard zone with no high-risk connections";
        }
        
        return new RiskFactor
        {
            FactorId = "geography",
            Name = "Geographic Risk",
            Category = "Location",
            Weight = FactorWeights["geography"],
            Score = score,
            Source = "Zone Registry",
            Rationale = rationale
        };
    }
    
    private RiskFactor CalculateTransactionRisk(int? transactionCount, decimal? transactionVolume)
    {
        double score = 0;
        string rationale;
        
        if (transactionVolume > 100000)
        {
            score = 80;
            rationale = "High transaction volume";
        }
        else if (transactionVolume > 50000 || transactionCount > 100)
        {
            score = 50;
            rationale = "Moderate transaction activity";
        }
        else
        {
            score = 20;
            rationale = "Normal transaction activity";
        }
        
        return new RiskFactor
        {
            FactorId = "transaction_volume",
            Name = "Transaction Volume Risk",
            Category = "Activity",
            Weight = FactorWeights["transaction_volume"],
            Score = score,
            Source = "FutureWampumLedger",
            Rationale = rationale
        };
    }
    
    private RiskFactor CalculatePEPRisk(ScreeningResult? pepScreening)
    {
        double score = 0;
        string rationale;
        
        if (pepScreening?.HasMatches == true)
        {
            if (pepScreening.HighestMatchScore >= 95)
            {
                score = 100;
                rationale = "Confirmed PEP match";
            }
            else
            {
                score = 70;
                rationale = "Potential PEP match requiring review";
            }
        }
        else
        {
            score = 0;
            rationale = "No PEP matches found";
        }
        
        return new RiskFactor
        {
            FactorId = "pep_status",
            Name = "PEP Status Risk",
            Category = "PEP",
            Weight = FactorWeights["pep_status"],
            Score = score,
            Source = "PEP Screening",
            Rationale = rationale
        };
    }
    
    private RiskFactor CalculateSanctionsRisk(ScreeningResult? sanctionsScreening)
    {
        double score = 0;
        string rationale;
        
        if (sanctionsScreening?.Status == ScreeningStatus.ConfirmedMatch)
        {
            score = 100;
            rationale = "Confirmed sanctions match";
        }
        else if (sanctionsScreening?.HasMatches == true)
        {
            score = 80;
            rationale = "Potential sanctions match requiring review";
        }
        else
        {
            score = 0;
            rationale = "No sanctions matches found";
        }
        
        return new RiskFactor
        {
            FactorId = "sanctions_proximity",
            Name = "Sanctions Proximity Risk",
            Category = "Sanctions",
            Weight = FactorWeights["sanctions_proximity"],
            Score = score,
            Source = "Sanctions Screening",
            Rationale = rationale
        };
    }
    
    private RiskFactor CalculateBehavioralRisk(double? anomalyScore)
    {
        double score = anomalyScore ?? 0;
        string rationale = anomalyScore switch
        {
            > 70 => "High behavioral anomaly detected",
            > 40 => "Moderate behavioral patterns of concern",
            _ => "Normal behavioral patterns"
        };
        
        return new RiskFactor
        {
            FactorId = "behavioral_anomaly",
            Name = "Behavioral Anomaly Risk",
            Category = "Behavior",
            Weight = FactorWeights["behavioral_anomaly"],
            Score = score,
            Source = "AI Behavioral Analysis",
            Rationale = rationale
        };
    }
    
    private static RiskLevel DetermineRiskLevel(double score) => score switch
    {
        >= 81 => RiskLevel.Critical,
        >= 61 => RiskLevel.High,
        >= 31 => RiskLevel.Medium,
        _ => RiskLevel.Low
    };
    
    private static DueDiligenceLevel DetermineDueDiligence(RiskProfile profile) => profile.RiskLevel switch
    {
        RiskLevel.Critical => DueDiligenceLevel.Comprehensive,
        RiskLevel.High => DueDiligenceLevel.Enhanced,
        _ => DueDiligenceLevel.Standard
    };
    
    private static AuthenticationLevel DetermineAuthLevel(RiskProfile profile) => profile.RiskLevel switch
    {
        RiskLevel.Critical => AuthenticationLevel.ManualReview,
        RiskLevel.High => AuthenticationLevel.StepUp,
        RiskLevel.Medium => AuthenticationLevel.Enhanced,
        _ => AuthenticationLevel.Standard
    };
    
    private static MonitoringFrequency DetermineMonitoringFrequency(RiskProfile profile) => profile.RiskLevel switch
    {
        RiskLevel.Critical => MonitoringFrequency.Continuous,
        RiskLevel.High => MonitoringFrequency.Monthly,
        RiskLevel.Medium => MonitoringFrequency.Quarterly,
        _ => MonitoringFrequency.Annual
    };
    
    private static DateTime CalculateNextReviewDate(RiskProfile profile) => profile.MonitoringFrequency switch
    {
        MonitoringFrequency.Continuous => DateTime.UtcNow.AddDays(1),
        MonitoringFrequency.Monthly => DateTime.UtcNow.AddMonths(1),
        MonitoringFrequency.Quarterly => DateTime.UtcNow.AddMonths(3),
        MonitoringFrequency.SemiAnnual => DateTime.UtcNow.AddMonths(6),
        _ => DateTime.UtcNow.AddYears(1)
    };
    
    #endregion
}

/// <summary>
/// Event published when risk level changes.
/// </summary>
public record RiskLevelChangedEvent
{
    public Guid IdentityId { get; init; }
    public RiskLevel PreviousLevel { get; init; }
    public RiskLevel NewLevel { get; init; }
    public double RiskScore { get; init; }
    public DateTime ChangedAt { get; init; }
}
