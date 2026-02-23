using Mamey.Government.Modules.Tenant.Core.Domain.Entities;
using Mamey.Types;

namespace Mamey.Government.Modules.Tenant.Core.Domain.Repositories;

internal interface ITenantUserMappingRepository
{
    Task<TenantUserMapping?> GetAsync(string issuer, string subject, CancellationToken cancellationToken = default);
    Task AddAsync(TenantUserMapping mapping, CancellationToken cancellationToken = default);
    Task UpdateAsync(TenantUserMapping mapping, CancellationToken cancellationToken = default);
    Task DeleteAsync(string issuer, string subject, CancellationToken cancellationToken = default);
    Task<bool> ExistsAsync(string issuer, string subject, CancellationToken cancellationToken = default);
    
    // Lookup by tenant
    Task<IReadOnlyList<TenantUserMapping>> GetByTenantAsync(TenantId tenantId, CancellationToken cancellationToken = default);
}
