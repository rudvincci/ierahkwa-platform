using Mamey.Portal.Citizenship.Application.Models;
using Mamey.Portal.Shared.Tenancy;

namespace Mamey.Portal.Citizenship.Application.Services;

public sealed class ApplicationStatusService : IApplicationStatusService
{
    private readonly ITenantContext _tenant;
    private readonly IApplicationStatusStore _store;

    public ApplicationStatusService(
        ITenantContext tenant,
        IApplicationStatusStore store)
    {
        _tenant = tenant;
        _store = store;
    }

    public async Task<ApplicationStatusDto?> GetApplicationStatusByNumberAsync(string applicationNumber, CancellationToken ct = default)
    {
        var tenantId = _tenant.TenantId;
        return await _store.GetByNumberAsync(tenantId, applicationNumber, ct);
    }

    public async Task<ApplicationStatusDto?> GetApplicationStatusByIdAsync(Guid applicationId, CancellationToken ct = default)
    {
        var tenantId = _tenant.TenantId;
        return await _store.GetByIdAsync(tenantId, applicationId, ct);
    }

    public async Task<ApplicationStatusDto?> GetApplicationStatusByEmailAsync(string email, CancellationToken ct = default)
    {
        var tenantId = _tenant.TenantId;
        return await _store.GetByEmailAsync(tenantId, email, ct);
    }
}

