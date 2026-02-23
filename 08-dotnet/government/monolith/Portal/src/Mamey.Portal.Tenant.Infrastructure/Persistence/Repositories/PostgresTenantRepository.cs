using Microsoft.EntityFrameworkCore;
using TenantEntity = Mamey.Portal.Tenant.Domain.Entities.Tenant;
using Mamey.Portal.Tenant.Domain.Repositories;
using Mamey.Portal.Tenant.Domain.ValueObjects;

namespace Mamey.Portal.Tenant.Infrastructure.Persistence.Repositories;

public sealed class PostgresTenantRepository : ITenantRepository
{
    private readonly TenantDbContext _db;

    public PostgresTenantRepository(TenantDbContext db)
    {
        _db = db;
    }

    public async Task<TenantEntity?> GetAsync(string tenantId, CancellationToken ct = default)
    {
        var row = await _db.Tenants.AsNoTracking()
            .SingleOrDefaultAsync(x => x.TenantId == tenantId, ct);

        return row is null
            ? null
            : new TenantEntity(new TenantId(row.TenantId), row.DisplayName, row.CreatedAt, row.UpdatedAt);
    }

    public async Task SaveAsync(TenantEntity tenant, CancellationToken ct = default)
    {
        var row = await _db.Tenants
            .SingleOrDefaultAsync(x => x.TenantId == tenant.Id, ct);

        if (row is null)
        {
            _db.Tenants.Add(new TenantRow
            {
                TenantId = tenant.Id,
                DisplayName = tenant.DisplayName,
                CreatedAt = tenant.CreatedAt,
                UpdatedAt = tenant.UpdatedAt
            });
        }
        else
        {
            row.DisplayName = tenant.DisplayName;
            row.CreatedAt = tenant.CreatedAt;
            row.UpdatedAt = tenant.UpdatedAt;
        }

        await _db.SaveChangesAsync(ct);
    }
}
