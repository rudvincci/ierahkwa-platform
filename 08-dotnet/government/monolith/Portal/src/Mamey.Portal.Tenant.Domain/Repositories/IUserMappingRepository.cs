using Mamey.Portal.Tenant.Domain.Entities;

namespace Mamey.Portal.Tenant.Domain.Repositories;

public interface IUserMappingRepository
{
    Task<TenantUserMapping?> GetAsync(string issuer, string subject, CancellationToken ct = default);
    Task<IReadOnlyList<TenantUserMapping>> GetByTenantAsync(string tenantId, CancellationToken ct = default);
    Task SaveAsync(TenantUserMapping mapping, CancellationToken ct = default);
}
