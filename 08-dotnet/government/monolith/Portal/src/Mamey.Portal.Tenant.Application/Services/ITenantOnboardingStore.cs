using Mamey.Portal.Tenant.Application.Models;

namespace Mamey.Portal.Tenant.Application.Services;

public interface ITenantOnboardingStore
{
    Task<IReadOnlyList<TenantSummary>> GetTenantsAsync(int take, CancellationToken ct = default);
    Task<TenantRowSnapshot?> GetTenantAsync(string tenantId, CancellationToken ct = default);
    Task<TenantSettingsSnapshot?> GetSettingsAsync(string tenantId, CancellationToken ct = default);
    Task<TenantDocumentNamingSnapshot?> GetNamingAsync(string tenantId, CancellationToken ct = default);
    Task<bool> TenantExistsAsync(string tenantId, CancellationToken ct = default);
    Task CreateTenantAsync(
        string tenantId,
        string displayName,
        string brandingJson,
        string activationJson,
        string namingPatternJson,
        DateTimeOffset now,
        CancellationToken ct = default);
    Task UpdateTenantAsync(
        string tenantId,
        string displayName,
        string brandingJson,
        string activationJson,
        string namingPatternJson,
        DateTimeOffset now,
        CancellationToken ct = default);
}
