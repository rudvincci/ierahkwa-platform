using Microsoft.Extensions.Configuration;

namespace Pupitre.FeatureFlags;

/// <summary>
/// Configuration-based feature flag service.
/// </summary>
public class FeatureFlagService : IFeatureFlagService
{
    private readonly IConfiguration _configuration;

    public FeatureFlagService(IConfiguration configuration)
    {
        _configuration = configuration;
    }

    public Task<bool> IsEnabledAsync(string featureName, CancellationToken cancellationToken = default)
    {
        var key = $"FeatureFlags:{featureName.Replace(".", ":")}";
        var value = _configuration.GetValue<bool?>(key);
        return Task.FromResult(value ?? false);
    }

    public Task<bool> IsEnabledForUserAsync(string featureName, Guid userId, CancellationToken cancellationToken = default)
    {
        // Check if feature is enabled globally first
        var isEnabled = _configuration.GetValue<bool?>($"FeatureFlags:{featureName.Replace(".", ":")}");
        if (isEnabled != true)
            return Task.FromResult(false);

        // Check user-specific override
        var userOverride = _configuration.GetValue<bool?>($"FeatureFlags:{featureName.Replace(".", ":")}:Users:{userId}");
        if (userOverride.HasValue)
            return Task.FromResult(userOverride.Value);

        return Task.FromResult(true);
    }

    public Task<bool> IsEnabledForPercentageAsync(string featureName, Guid userId, CancellationToken cancellationToken = default)
    {
        var percentage = _configuration.GetValue<int?>($"FeatureFlags:{featureName.Replace(".", ":")}:Percentage") ?? 0;
        if (percentage <= 0)
            return Task.FromResult(false);
        if (percentage >= 100)
            return Task.FromResult(true);

        // Use consistent hashing based on userId to determine if user is in the percentage
        var hash = userId.GetHashCode();
        var userPercentile = Math.Abs(hash % 100);
        return Task.FromResult(userPercentile < percentage);
    }
}
