using Mamey.Portal.Tenant.Domain.Entities;

namespace Mamey.Portal.Tenant.Domain.Repositories;

public interface IDocumentNamingRepository
{
    Task<TenantDocumentNaming?> GetAsync(string tenantId, CancellationToken ct = default);
    Task SaveAsync(TenantDocumentNaming naming, CancellationToken ct = default);
}
