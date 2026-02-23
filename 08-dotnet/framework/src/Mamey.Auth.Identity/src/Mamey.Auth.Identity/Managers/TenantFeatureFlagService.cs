// using Mamey.Persistence.Redis;
//
// namespace Mamey.Auth.Identity.Managers;
//
// /// <summary>
// /// Redis‐backed implementation for tenant feature flags.
// /// </summary>
// public class TenantFeatureFlagService : IFeatureFlagService
// {
//     private readonly ICache _cache;
//     private const string Prefix = "tenant_features:";
//
//     public TenantFeatureFlagService(ICache cache) => _cache = cache;
//
//     public async Task EnableAsync(string featureName, CancellationToken ct = default)
//     {
//         var key = Prefix + tenantId;
//         var current = await _cache.GetAsync<HashSet<string>>(key) ?? new();
//         current.Add(featureName);
//         await _cache.SetAsync(key, current, expiry: TimeSpan.FromDays(1));
//     }
//
//     public async Task DisableAsync(Guid tenantId, string featureName, CancellationToken ct = default)
//     {
//         var key = Prefix + tenantId;
//         var current = await _cache.GetAsync<HashSet<string>>(key) ?? new();
//         if (current.Remove(featureName))
//             await _cache.SetAsync(key, current, expiry: TimeSpan.FromDays(1));
//     }
//
//     public async Task<bool> IsEnabledAsync(Guid tenantId, string featureName, CancellationToken ct = default)
//     {
//         var key = Prefix + tenantId;
//         var current = await _cache.GetAsync<HashSet<string>>(key);
//         return current?.Contains(featureName) == true;
//     }
// }