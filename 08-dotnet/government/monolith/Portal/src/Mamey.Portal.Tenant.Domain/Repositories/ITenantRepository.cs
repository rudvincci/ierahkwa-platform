using Mamey.Portal.Tenant.Domain.Entities;

namespace Mamey.Portal.Tenant.Domain.Repositories;

public interface ITenantRepository
{
    Task<Entities.Tenant?> GetAsync(string tenantId, CancellationToken ct = default);
    Task SaveAsync(Entities.Tenant tenant, CancellationToken ct = default);
}
