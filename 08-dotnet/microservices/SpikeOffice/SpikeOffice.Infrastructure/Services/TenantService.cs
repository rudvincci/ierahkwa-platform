using Microsoft.EntityFrameworkCore;
using SpikeOffice.Core.Entities;
using SpikeOffice.Core.Interfaces;
using SpikeOffice.Infrastructure.Data;

namespace SpikeOffice.Infrastructure.Services;

public class TenantService : ITenantService
{
    private readonly SpikeOfficeDbContext _db;
    private readonly ITenantContext _tenantContext;

    public TenantService(SpikeOfficeDbContext db, ITenantContext tenantContext)
    {
        _db = db;
        _tenantContext = tenantContext;
    }

    public async Task<Tenant?> GetByUrlPrefixAsync(string urlPrefix, CancellationToken ct = default)
    {
        return await _db.Tenants.IgnoreQueryFilters()
            .FirstOrDefaultAsync(t => t.UrlPrefix == urlPrefix && t.IsActive, ct);
    }

    public async Task<Tenant?> GetByIdAsync(Guid id, CancellationToken ct = default)
    {
        return await _db.Tenants.IgnoreQueryFilters()
            .FirstOrDefaultAsync(t => t.Id == id && t.IsActive, ct);
    }
}
