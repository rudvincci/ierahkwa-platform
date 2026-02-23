using Mamey.Government.Modules.Tenant.Core.Domain.Entities;
using Mamey.Types;

namespace Mamey.Government.Modules.Tenant.Core.Domain.Repositories;

internal interface ITenantSettingsRepository
{
    Task<TenantSettings?> GetAsync(TenantId tenantId, CancellationToken cancellationToken = default);
    Task AddAsync(TenantSettings settings, CancellationToken cancellationToken = default);
    Task UpdateAsync(TenantSettings settings, CancellationToken cancellationToken = default);
    Task<bool> ExistsAsync(TenantId tenantId, CancellationToken cancellationToken = default);
}
