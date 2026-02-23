using Mamey.Portal.Tenant.Domain.Entities;

namespace Mamey.Portal.Tenant.Domain.Repositories;

public interface IDocumentTemplateRepository
{
    Task<TenantDocumentTemplate?> GetAsync(string tenantId, string kind, CancellationToken ct = default);
    Task<IReadOnlyList<TenantDocumentTemplate>> GetByTenantAsync(string tenantId, CancellationToken ct = default);
    Task SaveAsync(TenantDocumentTemplate template, CancellationToken ct = default);
}
