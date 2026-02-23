using Mamey.Portal.Tenant.Application.Models;

namespace Mamey.Portal.Tenant.Application.Services;

public interface ITenantUserMappingService
{
    Task<IReadOnlyList<TenantUserMapping>> GetAllAsync(int take = 200, CancellationToken ct = default);

    Task<TenantUserMapping?> GetAsync(string issuer, string subject, CancellationToken ct = default);

    Task<TenantUserMapping> UpsertAsync(
        string issuer,
        string subject,
        string tenantId,
        string? email,
        CancellationToken ct = default);

    Task DeleteAsync(string issuer, string subject, CancellationToken ct = default);
}




