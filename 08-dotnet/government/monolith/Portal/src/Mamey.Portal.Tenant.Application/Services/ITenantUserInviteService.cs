using Mamey.Portal.Tenant.Application.Models;

namespace Mamey.Portal.Tenant.Application.Services;

public interface ITenantUserInviteService
{
    Task<IReadOnlyList<TenantUserInvite>> GetByTenantAsync(string tenantId, CancellationToken ct = default);
    Task<TenantUserInvite?> GetAsync(string issuer, string email, CancellationToken ct = default);
    Task<TenantUserInvite> CreateOrUpdateAsync(string issuer, string email, string tenantId, CancellationToken ct = default);
    Task RevokeAsync(string issuer, string email, CancellationToken ct = default);
}
