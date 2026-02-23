using Microsoft.EntityFrameworkCore;
using Mamey.Portal.Tenant.Domain.Entities;
using Mamey.Portal.Tenant.Domain.Repositories;
using Mamey.Portal.Tenant.Domain.ValueObjects;

namespace Mamey.Portal.Tenant.Infrastructure.Persistence.Repositories;

public sealed class PostgresUserInviteRepository : IUserInviteRepository
{
    private readonly TenantDbContext _db;

    public PostgresUserInviteRepository(TenantDbContext db)
    {
        _db = db;
    }

    public async Task<TenantUserInvite?> GetAsync(string issuer, string email, CancellationToken ct = default)
    {
        var row = await _db.UserInvites.AsNoTracking()
            .SingleOrDefaultAsync(x => x.Issuer == issuer && x.Email == email, ct);

        return row is null
            ? null
            : new TenantUserInvite(row.Issuer, row.Email, new TenantId(row.TenantId), row.CreatedAt, row.UpdatedAt, row.UsedAt);
    }

    public async Task<IReadOnlyList<TenantUserInvite>> GetByTenantAsync(string tenantId, CancellationToken ct = default)
    {
        var rows = await _db.UserInvites.AsNoTracking()
            .Where(x => x.TenantId == tenantId)
            .OrderByDescending(x => x.CreatedAt)
            .ToListAsync(ct);

        return rows.Select(x => new TenantUserInvite(x.Issuer, x.Email, new TenantId(x.TenantId), x.CreatedAt, x.UpdatedAt, x.UsedAt)).ToList();
    }

    public async Task SaveAsync(TenantUserInvite invite, CancellationToken ct = default)
    {
        var row = await _db.UserInvites
            .SingleOrDefaultAsync(x => x.Issuer == invite.Issuer && x.Email == invite.Email, ct);

        if (row is null)
        {
            _db.UserInvites.Add(new TenantUserInviteRow
            {
                Issuer = invite.Issuer,
                Email = invite.Email,
                TenantId = invite.TenantId.Value,
                CreatedAt = invite.CreatedAt,
                UpdatedAt = invite.UpdatedAt,
                UsedAt = invite.UsedAt
            });
        }
        else
        {
            row.TenantId = invite.TenantId.Value;
            row.CreatedAt = invite.CreatedAt;
            row.UpdatedAt = invite.UpdatedAt;
            row.UsedAt = invite.UsedAt;
        }

        await _db.SaveChangesAsync(ct);
    }

    public async Task DeleteAsync(string issuer, string email, CancellationToken ct = default)
    {
        var row = await _db.UserInvites
            .SingleOrDefaultAsync(x => x.Issuer == issuer && x.Email == email, ct);

        if (row is null) return;

        _db.UserInvites.Remove(row);
        await _db.SaveChangesAsync(ct);
    }
}
