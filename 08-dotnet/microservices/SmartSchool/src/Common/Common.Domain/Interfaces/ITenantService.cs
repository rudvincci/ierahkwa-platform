using Common.Domain.Entities;

namespace Common.Domain.Interfaces;

public interface ITenantService
{
    int? GetCurrentTenantId();
    Task<Tenant?> GetCurrentTenantAsync();
    void SetTenant(int tenantId);
}
