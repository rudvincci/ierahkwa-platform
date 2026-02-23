using Mamey.Portal.Shared.Storage.DocumentNaming;
using Mamey.Portal.Tenant.Application.Models;

namespace Mamey.Portal.Tenant.Application.Services;

public interface ITenantOnboardingService
{
    Task<IReadOnlyList<TenantSummary>> GetTenantsAsync(CancellationToken ct = default);
    Task<TenantSettings?> GetSettingsAsync(string tenantId, CancellationToken ct = default);

    Task<TenantSettings> CreateTenantAsync(
        string tenantId,
        string displayName,
        TenantBranding? branding = null,
        DocumentNamingPattern? namingPattern = null,
        TenantActivationChecklist? activationChecklist = null,
        CancellationToken ct = default);

    Task<TenantSettings> UpdateSettingsAsync(
        string tenantId,
        TenantBranding branding,
        DocumentNamingPattern namingPattern,
        TenantActivationChecklist? activationChecklist = null,
        CancellationToken ct = default);
}



