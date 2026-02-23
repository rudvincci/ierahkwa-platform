using Mamey.Portal.Tenant.Application.Models;

namespace Mamey.Portal.Tenant.Application.Services;

public interface ITenantUserInviteStore
{
    Task<TenantUserInvite?> GetAsync(string issuer, string email, CancellationToken ct = default);
    Task<IReadOnlyList<TenantUserInvite>> GetByTenantAsync(string tenantId, CancellationToken ct = default);
    Task<TenantUserInvite> UpsertAsync(
        string issuer,
        string email,
        string tenantId,
        DateTimeOffset now,
        CancellationToken ct = default);
    Task DeleteAsync(string issuer, string email, CancellationToken ct = default);
}
