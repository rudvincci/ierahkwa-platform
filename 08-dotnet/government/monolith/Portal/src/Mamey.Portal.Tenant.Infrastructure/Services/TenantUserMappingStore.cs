using Microsoft.EntityFrameworkCore;
using Mamey.Portal.Tenant.Application.Models;
using Mamey.Portal.Tenant.Application.Services;
using Mamey.Portal.Tenant.Infrastructure.Persistence;

namespace Mamey.Portal.Tenant.Infrastructure.Services;

public sealed class TenantUserMappingStore : ITenantUserMappingStore
{
    private readonly TenantDbContext _db;

    public TenantUserMappingStore(TenantDbContext db)
    {
        _db = db;
    }

    public async Task<IReadOnlyList<TenantUserMapping>> GetAllAsync(int take, CancellationToken ct = default)
    {
        var rows = await _db.UserMappings.AsNoTracking()
            .OrderBy(x => x.TenantId)
            .ThenBy(x => x.Email)
            .ThenBy(x => x.Subject)
            .Take(take)
            .ToListAsync(ct);

        return rows.Select(Map).ToList();
    }

    public async Task<TenantUserMapping?> GetAsync(string issuer, string subject, CancellationToken ct = default)
    {
        var row = await _db.UserMappings.AsNoTracking()
            .SingleOrDefaultAsync(x => x.Issuer == issuer && x.Subject == subject, ct);

        return row is null ? null : Map(row);
    }

    public async Task<TenantUserMapping> UpsertAsync(
        string issuer,
        string subject,
        string tenantId,
        string? email,
        DateTimeOffset now,
        CancellationToken ct = default)
    {
        var row = await _db.UserMappings
            .SingleOrDefaultAsync(x => x.Issuer == issuer && x.Subject == subject, ct);

        if (row is null)
        {
            row = new TenantUserMappingRow
            {
                Issuer = issuer,
                Subject = subject,
                TenantId = tenantId,
                Email = email,
                CreatedAt = now,
                UpdatedAt = now,
            };
            _db.UserMappings.Add(row);
        }
        else
        {
            row.TenantId = tenantId;
            row.Email = email;
            row.UpdatedAt = now;
        }

        await _db.SaveChangesAsync(ct);
        return Map(row);
    }

    public async Task DeleteAsync(string issuer, string subject, CancellationToken ct = default)
    {
        var row = await _db.UserMappings
            .SingleOrDefaultAsync(x => x.Issuer == issuer && x.Subject == subject, ct);

        if (row is null) return;

        _db.UserMappings.Remove(row);
        await _db.SaveChangesAsync(ct);
    }

    private static TenantUserMapping Map(TenantUserMappingRow x)
        => new(
            x.Issuer,
            x.Subject,
            x.TenantId,
            x.Email,
            x.CreatedAt,
            x.UpdatedAt);
}

