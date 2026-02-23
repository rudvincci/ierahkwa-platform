using Mamey.Portal.Tenant.Domain.Entities;

namespace Mamey.Portal.Tenant.Domain.Repositories;

public interface IUserInviteRepository
{
    Task<TenantUserInvite?> GetAsync(string issuer, string email, CancellationToken ct = default);
    Task<IReadOnlyList<TenantUserInvite>> GetByTenantAsync(string tenantId, CancellationToken ct = default);
    Task SaveAsync(TenantUserInvite invite, CancellationToken ct = default);
    Task DeleteAsync(string issuer, string email, CancellationToken ct = default);
}
