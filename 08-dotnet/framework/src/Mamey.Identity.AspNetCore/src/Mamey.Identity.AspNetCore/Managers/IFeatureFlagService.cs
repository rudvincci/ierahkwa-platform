namespace Mamey.Identity.AspNetCore.Managers;

/// <summary>
/// Enables or disables named features per tenant.
/// </summary>
public interface IFeatureFlagService
{
    Task EnableAsync(string featureName, CancellationToken ct = default);
    Task DisableAsync(string featureName, CancellationToken ct = default);
    Task<bool> IsEnabledAsync( string featureName, CancellationToken ct = default);
}

