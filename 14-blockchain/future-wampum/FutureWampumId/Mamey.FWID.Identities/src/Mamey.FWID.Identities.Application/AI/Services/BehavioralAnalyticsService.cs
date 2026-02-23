using Mamey.FWID.Identities.Application.AI.Models;
using Microsoft.Extensions.Logging;

namespace Mamey.FWID.Identities.Application.AI.Services;

/// <summary>
/// AI-powered behavioral analytics service implementation.
/// Builds behavioral baselines and detects deviations for adaptive auth.
/// </summary>
public class BehavioralAnalyticsService : IBehavioralAnalyticsService
{
    private readonly ILogger<BehavioralAnalyticsService> _logger;
    
    private readonly Dictionary<Guid, BehavioralProfile> _profiles = new();
    private readonly Dictionary<Guid, List<BehavioralEvent>> _eventHistory = new();
    private readonly object _lock = new();
    
    private const int MinDataPointsForBaseline = 10;
    private const double AnomalyThreshold = 0.7;
    
    public BehavioralAnalyticsService(ILogger<BehavioralAnalyticsService> logger)
    {
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }
    
    /// <inheritdoc />
    public Task<BehavioralProfile> GetOrCreateProfileAsync(
        Guid identityId,
        CancellationToken cancellationToken = default)
    {
        lock (_lock)
        {
            if (!_profiles.TryGetValue(identityId, out var profile))
            {
                profile = new BehavioralProfile
                {
                    IdentityId = identityId
                };
                _profiles[identityId] = profile;
                _eventHistory[identityId] = new List<BehavioralEvent>();
                
                _logger.LogDebug("Created behavioral profile for {IdentityId}", identityId);
            }
            return Task.FromResult(profile);
        }
    }
    
    /// <inheritdoc />
    public async Task RecordBehaviorAsync(
        BehavioralEvent behaviorEvent,
        CancellationToken cancellationToken = default)
    {
        var profile = await GetOrCreateProfileAsync(behaviorEvent.IdentityId, cancellationToken);
        
        lock (_lock)
        {
            // Store event
            if (!_eventHistory.TryGetValue(behaviorEvent.IdentityId, out var events))
            {
                events = new List<BehavioralEvent>();
                _eventHistory[behaviorEvent.IdentityId] = events;
            }
            events.Add(behaviorEvent);
            
            // Keep only last 1000 events
            if (events.Count > 1000)
                events.RemoveAt(0);
            
            // Update profile based on event
            UpdateProfile(profile, behaviorEvent, events);
            profile.DataPointCount++;
            profile.UpdatedAt = DateTime.UtcNow;
        }
        
        _logger.LogDebug("Recorded {EventType} for {IdentityId}", 
            behaviorEvent.EventType, behaviorEvent.IdentityId);
    }
    
    /// <inheritdoc />
    public async Task<BehavioralAnalysisResult> AnalyzeBehaviorAsync(
        Guid identityId,
        AuthenticationContext context,
        CancellationToken cancellationToken = default)
    {
        var profile = await GetOrCreateProfileAsync(identityId, cancellationToken);
        
        var result = new BehavioralAnalysisResult();
        
        if (!profile.IsEstablished)
        {
            result.Confidence = 0.3; // Low confidence without established baseline
            result.DeviationScore = 0;
            return result;
        }
        
        // Check login time deviation
        var hourDeviation = CheckTimeDeviation(profile, context.Timestamp);
        if (hourDeviation != null)
            result.Deviations.Add(hourDeviation);
        
        // Check location deviation
        var locationDeviation = CheckLocationDeviation(profile, context);
        if (locationDeviation != null)
            result.Deviations.Add(locationDeviation);
        
        // Check device deviation
        var deviceDeviation = CheckDeviceDeviation(profile, context);
        if (deviceDeviation != null)
            result.Deviations.Add(deviceDeviation);
        
        // Calculate overall deviation score
        result.DeviationScore = result.Deviations.Any() 
            ? result.Deviations.Average(d => d.DeviationScore) 
            : 0;
        
        result.IsAnomalous = result.DeviationScore > AnomalyThreshold;
        result.Confidence = profile.BaselineConfidence;
        
        return result;
    }
    
    /// <inheritdoc />
    public async Task<AdaptiveAuthResult> EvaluateForAdaptiveAuthAsync(
        Guid identityId,
        AuthenticationContext context,
        CancellationToken cancellationToken = default)
    {
        var analysis = await AnalyzeBehaviorAsync(identityId, context, cancellationToken);
        
        var result = new AdaptiveAuthResult
        {
            IdentityId = identityId,
            RiskScore = analysis.DeviationScore * 100,
            Deviations = analysis.Deviations,
            Confidence = analysis.Confidence
        };
        
        // Determine required action based on risk
        result.RequiredAction = DetermineAction(result.RiskScore, analysis);
        
        // Add risk factors
        foreach (var deviation in analysis.Deviations)
        {
            result.RiskFactors.Add($"{deviation.DeviationType}: {deviation.Description}");
        }
        
        _logger.LogInformation(
            "Adaptive auth evaluation for {IdentityId}: Risk={Risk}, Action={Action}",
            identityId, result.RiskScore, result.RequiredAction);
        
        return result;
    }
    
    /// <inheritdoc />
    public async Task<List<BehavioralAnomaly>> DetectAnomaliesAsync(
        Guid identityId,
        CancellationToken cancellationToken = default)
    {
        var anomalies = new List<BehavioralAnomaly>();
        var profile = await GetOrCreateProfileAsync(identityId, cancellationToken);
        
        List<BehavioralEvent> events;
        lock (_lock)
        {
            if (!_eventHistory.TryGetValue(identityId, out events!))
                return anomalies;
            events = events.ToList(); // Copy for thread safety
        }
        
        // Check for rapid login attempts
        var recentLogins = events
            .Where(e => e.EventType == BehavioralEventType.Login)
            .Where(e => e.Timestamp > DateTime.UtcNow.AddHours(-1))
            .ToList();
        
        if (recentLogins.Count > 10)
        {
            anomalies.Add(new BehavioralAnomaly
            {
                IdentityId = identityId,
                AnomalyType = "RapidLoginAttempts",
                Description = $"{recentLogins.Count} login attempts in the last hour",
                Severity = 0.8,
                Evidence = new Dictionary<string, string>
                {
                    ["LoginCount"] = recentLogins.Count.ToString(),
                    ["TimeWindow"] = "1 hour"
                }
            });
        }
        
        // Check for multiple locations
        var recentLocations = events
            .Where(e => e.Timestamp > DateTime.UtcNow.AddHours(-4))
            .Where(e => !string.IsNullOrEmpty(e.Location))
            .Select(e => e.Location)
            .Distinct()
            .ToList();
        
        if (recentLocations.Count > 3)
        {
            anomalies.Add(new BehavioralAnomaly
            {
                IdentityId = identityId,
                AnomalyType = "MultipleLocations",
                Description = "Activity from multiple locations in short timeframe",
                Severity = 0.9,
                Evidence = new Dictionary<string, string>
                {
                    ["LocationCount"] = recentLocations.Count.ToString(),
                    ["Locations"] = string.Join(", ", recentLocations.Take(5)!)
                }
            });
        }
        
        // Check for unusual time activity
        var nightActivity = events
            .Where(e => e.Timestamp > DateTime.UtcNow.AddDays(-1))
            .Where(e => e.Timestamp.Hour is >= 0 and < 6)
            .ToList();
        
        if (nightActivity.Count > 20 && !profile.LoginPattern.PreferredHoursOfDay.Any(h => h is >= 0 and < 6))
        {
            anomalies.Add(new BehavioralAnomaly
            {
                IdentityId = identityId,
                AnomalyType = "UnusualTimeActivity",
                Description = "Significant activity during unusual hours",
                Severity = 0.6,
                Evidence = new Dictionary<string, string>
                {
                    ["NightActivityCount"] = nightActivity.Count.ToString(),
                    ["TimeRange"] = "00:00 - 06:00"
                }
            });
        }
        
        return anomalies;
    }
    
    #region Private Methods
    
    private void UpdateProfile(BehavioralProfile profile, BehavioralEvent ev, List<BehavioralEvent> history)
    {
        var loginEvents = history.Where(e => e.EventType == BehavioralEventType.Login).ToList();
        
        if (loginEvents.Any())
        {
            // Update preferred hours
            var hourGroups = loginEvents
                .GroupBy(e => e.Timestamp.Hour)
                .OrderByDescending(g => g.Count())
                .Take(5);
            profile.LoginPattern.PreferredHoursOfDay = hourGroups.Select(g => g.Key).ToList();
            
            // Update preferred days
            var dayGroups = loginEvents
                .GroupBy(e => e.Timestamp.DayOfWeek)
                .OrderByDescending(g => g.Count())
                .Take(5);
            profile.LoginPattern.PreferredDaysOfWeek = dayGroups.Select(g => g.Key).ToList();
            
            // Update common locations
            var locations = loginEvents
                .Where(e => !string.IsNullOrEmpty(e.Location))
                .GroupBy(e => e.Location)
                .OrderByDescending(g => g.Count())
                .Take(5)
                .Select(g => g.Key!);
            profile.LoginPattern.CommonLocations = locations.ToList();
        }
        
        // Update device profile
        if (!string.IsNullOrEmpty(ev.DeviceId) && 
            !profile.DeviceProfile.KnownDevices.Contains(ev.DeviceId))
        {
            profile.DeviceProfile.KnownDevices.Add(ev.DeviceId);
            if (profile.DeviceProfile.KnownDevices.Count == 1)
                profile.DeviceProfile.PrimaryDeviceId = ev.DeviceId;
        }
        
        // Recalculate baseline confidence
        profile.BaselineConfidence = Math.Min(0.95, profile.DataPointCount / 100.0);
    }
    
    private BehavioralDeviation? CheckTimeDeviation(BehavioralProfile profile, DateTime timestamp)
    {
        if (!profile.LoginPattern.PreferredHoursOfDay.Any())
            return null;
        
        var currentHour = timestamp.Hour;
        if (!profile.LoginPattern.PreferredHoursOfDay.Contains(currentHour))
        {
            var nearestPreferred = profile.LoginPattern.PreferredHoursOfDay
                .OrderBy(h => Math.Min(Math.Abs(h - currentHour), 24 - Math.Abs(h - currentHour)))
                .First();
            
            var hourDiff = Math.Min(Math.Abs(nearestPreferred - currentHour), 24 - Math.Abs(nearestPreferred - currentHour));
            
            if (hourDiff > 4)
            {
                return new BehavioralDeviation
                {
                    DeviationType = "LoginTime",
                    Description = "Login at unusual time",
                    DeviationScore = Math.Min(1.0, hourDiff / 12.0),
                    ExpectedValue = string.Join(", ", profile.LoginPattern.PreferredHoursOfDay.Select(h => $"{h}:00")),
                    ActualValue = $"{currentHour}:00"
                };
            }
        }
        
        return null;
    }
    
    private BehavioralDeviation? CheckLocationDeviation(BehavioralProfile profile, AuthenticationContext context)
    {
        if (string.IsNullOrEmpty(context.Country) || !profile.LoginPattern.CommonLocations.Any())
            return null;
        
        var currentLocation = $"{context.City}, {context.Country}";
        
        if (!profile.LoginPattern.CommonLocations.Any(l => 
            l.Contains(context.Country, StringComparison.OrdinalIgnoreCase)))
        {
            return new BehavioralDeviation
            {
                DeviationType = "Location",
                Description = "Login from unusual location",
                DeviationScore = 0.8,
                ExpectedValue = string.Join("; ", profile.LoginPattern.CommonLocations.Take(3)),
                ActualValue = currentLocation
            };
        }
        
        return null;
    }
    
    private BehavioralDeviation? CheckDeviceDeviation(BehavioralProfile profile, AuthenticationContext context)
    {
        if (string.IsNullOrEmpty(context.DeviceId) || !profile.DeviceProfile.KnownDevices.Any())
            return null;
        
        if (!profile.DeviceProfile.KnownDevices.Contains(context.DeviceId))
        {
            return new BehavioralDeviation
            {
                DeviationType = "Device",
                Description = "Login from unknown device",
                DeviationScore = 0.6,
                ExpectedValue = $"Known device ({profile.DeviceProfile.KnownDevices.Count} registered)",
                ActualValue = "New device"
            };
        }
        
        return null;
    }
    
    private AdaptiveAuthAction DetermineAction(double riskScore, BehavioralAnalysisResult analysis)
    {
        if (riskScore >= 90)
            return AdaptiveAuthAction.Block;
        
        if (riskScore >= 70)
            return AdaptiveAuthAction.ManualReview;
        
        if (riskScore >= 50)
            return AdaptiveAuthAction.StepUp;
        
        if (riskScore >= 30)
            return AdaptiveAuthAction.Challenge;
        
        return AdaptiveAuthAction.Allow;
    }
    
    #endregion
}
