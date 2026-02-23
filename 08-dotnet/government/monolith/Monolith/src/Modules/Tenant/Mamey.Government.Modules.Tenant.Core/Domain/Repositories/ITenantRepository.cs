using Mamey.Government.Modules.Tenant.Core.Domain.Entities;
using Mamey.Types;

namespace Mamey.Government.Modules.Tenant.Core.Domain.Repositories;

internal interface ITenantRepository
{
    Task<TenantEntity?> GetAsync(TenantId id, CancellationToken cancellationToken = default);
    Task AddAsync(TenantEntity tenant, CancellationToken cancellationToken = default);
    Task UpdateAsync(TenantEntity tenant, CancellationToken cancellationToken = default);
    Task DeleteAsync(TenantId id, CancellationToken cancellationToken = default);
    Task<bool> ExistsAsync(TenantId id, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<TenantEntity>> BrowseAsync(CancellationToken cancellationToken = default);
    
    // Lookup by domain
    Task<TenantEntity?> GetByDomainAsync(string domain, CancellationToken cancellationToken = default);
}
