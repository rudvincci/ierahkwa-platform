using Microsoft.EntityFrameworkCore;
using Mamey.Portal.Tenant.Application.Models;
using Mamey.Portal.Tenant.Application.Services;
using Mamey.Portal.Tenant.Infrastructure.Persistence;

namespace Mamey.Portal.Tenant.Infrastructure.Services;

public sealed class TenantUserInviteStore : ITenantUserInviteStore
{
    private readonly TenantDbContext _db;

    public TenantUserInviteStore(TenantDbContext db)
    {
        _db = db;
    }

    public async Task<TenantUserInvite?> GetAsync(string issuer, string email, CancellationToken ct = default)
    {
        var row = await _db.UserInvites.AsNoTracking()
            .SingleOrDefaultAsync(x => x.Issuer == issuer && x.Email == email, ct);

        return row is null ? null : Map(row);
    }

    public async Task<IReadOnlyList<TenantUserInvite>> GetByTenantAsync(string tenantId, CancellationToken ct = default)
    {
        var rows = await _db.UserInvites.AsNoTracking()
            .Where(x => x.TenantId == tenantId)
            .OrderByDescending(x => x.CreatedAt)
            .ThenBy(x => x.Email)
            .ToListAsync(ct);

        return rows.Select(Map).ToList();
    }

    public async Task<TenantUserInvite> UpsertAsync(
        string issuer,
        string email,
        string tenantId,
        DateTimeOffset now,
        CancellationToken ct = default)
    {
        var row = await _db.UserInvites
            .SingleOrDefaultAsync(x => x.Issuer == issuer && x.Email == email, ct);

        if (row is null)
        {
            row = new TenantUserInviteRow
            {
                Issuer = issuer,
                Email = email,
                TenantId = tenantId,
                CreatedAt = now,
                UpdatedAt = now,
                UsedAt = null
            };
            _db.UserInvites.Add(row);
        }
        else
        {
            row.TenantId = tenantId;
            row.UpdatedAt = now;
            row.UsedAt = null;
        }

        await _db.SaveChangesAsync(ct);
        return Map(row);
    }

    public async Task DeleteAsync(string issuer, string email, CancellationToken ct = default)
    {
        var row = await _db.UserInvites
            .SingleOrDefaultAsync(x => x.Issuer == issuer && x.Email == email, ct);

        if (row is null) return;

        _db.UserInvites.Remove(row);
        await _db.SaveChangesAsync(ct);
    }

    private static TenantUserInvite Map(TenantUserInviteRow row)
        => new(
            row.Issuer,
            row.Email,
            row.TenantId,
            row.CreatedAt,
            row.UpdatedAt,
            row.UsedAt);
}
