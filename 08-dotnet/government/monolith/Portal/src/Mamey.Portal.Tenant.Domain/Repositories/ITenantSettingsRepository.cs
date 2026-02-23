using Mamey.Portal.Tenant.Domain.Entities;

namespace Mamey.Portal.Tenant.Domain.Repositories;

public interface ITenantSettingsRepository
{
    Task<TenantSettings?> GetAsync(string tenantId, CancellationToken ct = default);
    Task SaveAsync(TenantSettings settings, CancellationToken ct = default);
}
