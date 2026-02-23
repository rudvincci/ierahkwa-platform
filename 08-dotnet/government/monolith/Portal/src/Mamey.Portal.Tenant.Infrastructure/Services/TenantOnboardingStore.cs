using Microsoft.EntityFrameworkCore;
using Mamey.Portal.Tenant.Application.Models;
using Mamey.Portal.Tenant.Application.Services;
using Mamey.Portal.Tenant.Infrastructure.Persistence;

namespace Mamey.Portal.Tenant.Infrastructure.Services;

public sealed class TenantOnboardingStore : ITenantOnboardingStore
{
    private readonly TenantDbContext _db;

    public TenantOnboardingStore(TenantDbContext db)
    {
        _db = db;
    }

    public async Task<IReadOnlyList<TenantSummary>> GetTenantsAsync(int take, CancellationToken ct = default)
    {
        take = Math.Clamp(take, 1, 500);
        var rows = await _db.Tenants.AsNoTracking()
            .OrderBy(x => x.TenantId)
            .Take(take)
            .ToListAsync(ct);

        return rows.Select(x => new TenantSummary(x.TenantId, x.DisplayName, x.CreatedAt, x.UpdatedAt)).ToList();
    }

    public async Task<TenantRowSnapshot?> GetTenantAsync(string tenantId, CancellationToken ct = default)
    {
        var tenant = await _db.Tenants.AsNoTracking().SingleOrDefaultAsync(x => x.TenantId == tenantId, ct);
        if (tenant is null) return null;

        return new TenantRowSnapshot(
            tenant.TenantId,
            tenant.DisplayName,
            tenant.CreatedAt,
            tenant.UpdatedAt);
    }

    public async Task<TenantSettingsSnapshot?> GetSettingsAsync(string tenantId, CancellationToken ct = default)
    {
        var settings = await _db.TenantSettings.AsNoTracking().SingleOrDefaultAsync(x => x.TenantId == tenantId, ct);
        if (settings is null) return null;

        return new TenantSettingsSnapshot(
            settings.TenantId,
            settings.BrandingJson,
            settings.ActivationJson,
            settings.CreatedAt,
            settings.UpdatedAt);
    }

    public async Task<TenantDocumentNamingSnapshot?> GetNamingAsync(string tenantId, CancellationToken ct = default)
    {
        var naming = await _db.DocumentNaming.AsNoTracking().SingleOrDefaultAsync(x => x.TenantId == tenantId, ct);
        if (naming is null) return null;

        return new TenantDocumentNamingSnapshot(
            naming.TenantId,
            naming.PatternJson,
            naming.UpdatedAt);
    }

    public async Task<bool> TenantExistsAsync(string tenantId, CancellationToken ct = default)
    {
        return await _db.Tenants.AsNoTracking().AnyAsync(x => x.TenantId == tenantId, ct);
    }

    public async Task CreateTenantAsync(
        string tenantId,
        string displayName,
        string brandingJson,
        string activationJson,
        string namingPatternJson,
        DateTimeOffset now,
        CancellationToken ct = default)
    {
        var tenantRow = new TenantRow
        {
            TenantId = tenantId,
            DisplayName = displayName,
            CreatedAt = now,
            UpdatedAt = now
        };

        _db.Tenants.Add(tenantRow);
        _db.TenantSettings.Add(new TenantSettingsRow
        {
            TenantId = tenantId,
            BrandingJson = brandingJson,
            ActivationJson = activationJson,
            CreatedAt = now,
            UpdatedAt = now
        });
        _db.DocumentNaming.Add(new TenantDocumentNamingRow
        {
            TenantId = tenantId,
            PatternJson = namingPatternJson,
            UpdatedAt = now
        });

        await _db.SaveChangesAsync(ct);
    }

    public async Task UpdateTenantAsync(
        string tenantId,
        string displayName,
        string brandingJson,
        string activationJson,
        string namingPatternJson,
        DateTimeOffset now,
        CancellationToken ct = default)
    {
        var tenant = await _db.Tenants.SingleOrDefaultAsync(x => x.TenantId == tenantId, ct)
                     ?? throw new InvalidOperationException("Tenant not found.");

        tenant.DisplayName = displayName;
        tenant.UpdatedAt = now;

        var settings = await _db.TenantSettings.SingleOrDefaultAsync(x => x.TenantId == tenantId, ct);
        if (settings is null)
        {
            _db.TenantSettings.Add(new TenantSettingsRow
            {
                TenantId = tenantId,
                BrandingJson = brandingJson,
                ActivationJson = activationJson,
                CreatedAt = now,
                UpdatedAt = now
            });
        }
        else
        {
            settings.BrandingJson = brandingJson;
            settings.ActivationJson = activationJson;
            settings.UpdatedAt = now;
        }

        var naming = await _db.DocumentNaming.SingleOrDefaultAsync(x => x.TenantId == tenantId, ct);
        if (naming is null)
        {
            _db.DocumentNaming.Add(new TenantDocumentNamingRow
            {
                TenantId = tenantId,
                PatternJson = namingPatternJson,
                UpdatedAt = now
            });
        }
        else
        {
            naming.PatternJson = namingPatternJson;
            naming.UpdatedAt = now;
        }

        await _db.SaveChangesAsync(ct);
    }
}

