using Microsoft.EntityFrameworkCore;
using Mamey.Portal.Tenant.Domain.Entities;
using Mamey.Portal.Tenant.Domain.Repositories;
using Mamey.Portal.Tenant.Domain.ValueObjects;

namespace Mamey.Portal.Tenant.Infrastructure.Persistence.Repositories;

public sealed class PostgresDocumentNamingRepository : IDocumentNamingRepository
{
    private readonly TenantDbContext _db;

    public PostgresDocumentNamingRepository(TenantDbContext db)
    {
        _db = db;
    }

    public async Task<TenantDocumentNaming?> GetAsync(string tenantId, CancellationToken ct = default)
    {
        var row = await _db.DocumentNaming.AsNoTracking()
            .SingleOrDefaultAsync(x => x.TenantId == tenantId, ct);

        return row is null
            ? null
            : new TenantDocumentNaming(new TenantId(row.TenantId), row.PatternJson, row.UpdatedAt);
    }

    public async Task SaveAsync(TenantDocumentNaming naming, CancellationToken ct = default)
    {
        var row = await _db.DocumentNaming
            .SingleOrDefaultAsync(x => x.TenantId == naming.TenantId.Value, ct);

        if (row is null)
        {
            _db.DocumentNaming.Add(new TenantDocumentNamingRow
            {
                TenantId = naming.TenantId.Value,
                PatternJson = naming.PatternJson,
                UpdatedAt = naming.UpdatedAt
            });
        }
        else
        {
            row.PatternJson = naming.PatternJson;
            row.UpdatedAt = naming.UpdatedAt;
        }

        await _db.SaveChangesAsync(ct);
    }
}
