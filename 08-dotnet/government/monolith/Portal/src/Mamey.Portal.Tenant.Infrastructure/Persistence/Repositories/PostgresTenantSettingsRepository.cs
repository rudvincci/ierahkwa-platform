using Microsoft.EntityFrameworkCore;
using Mamey.Portal.Tenant.Domain.Entities;
using Mamey.Portal.Tenant.Domain.Repositories;
using Mamey.Portal.Tenant.Domain.ValueObjects;

namespace Mamey.Portal.Tenant.Infrastructure.Persistence.Repositories;

public sealed class PostgresTenantSettingsRepository : ITenantSettingsRepository
{
    private readonly TenantDbContext _db;

    public PostgresTenantSettingsRepository(TenantDbContext db)
    {
        _db = db;
    }

    public async Task<TenantSettings?> GetAsync(string tenantId, CancellationToken ct = default)
    {
        var row = await _db.TenantSettings.AsNoTracking()
            .SingleOrDefaultAsync(x => x.TenantId == tenantId, ct);

        return row is null
            ? null
            : new TenantSettings(new TenantId(row.TenantId), row.BrandingJson, row.ActivationJson, row.CreatedAt, row.UpdatedAt);
    }

    public async Task SaveAsync(TenantSettings settings, CancellationToken ct = default)
    {
        var row = await _db.TenantSettings
            .SingleOrDefaultAsync(x => x.TenantId == settings.TenantId.Value, ct);

        if (row is null)
        {
            _db.TenantSettings.Add(new TenantSettingsRow
            {
                TenantId = settings.TenantId.Value,
                BrandingJson = settings.BrandingJson,
                ActivationJson = settings.ActivationJson,
                CreatedAt = settings.CreatedAt,
                UpdatedAt = settings.UpdatedAt
            });
        }
        else
        {
            row.BrandingJson = settings.BrandingJson;
            row.ActivationJson = settings.ActivationJson;
            row.CreatedAt = settings.CreatedAt;
            row.UpdatedAt = settings.UpdatedAt;
        }

        await _db.SaveChangesAsync(ct);
    }
}
