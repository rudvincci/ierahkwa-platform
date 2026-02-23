using Mamey.Portal.Tenant.Application.Models;

namespace Mamey.Portal.Tenant.Application.Services;

public interface ITenantUserMappingStore
{
    Task<IReadOnlyList<TenantUserMapping>> GetAllAsync(int take, CancellationToken ct = default);
    Task<TenantUserMapping?> GetAsync(string issuer, string subject, CancellationToken ct = default);
    Task<TenantUserMapping> UpsertAsync(
        string issuer,
        string subject,
        string tenantId,
        string? email,
        DateTimeOffset now,
        CancellationToken ct = default);
    Task DeleteAsync(string issuer, string subject, CancellationToken ct = default);
}
