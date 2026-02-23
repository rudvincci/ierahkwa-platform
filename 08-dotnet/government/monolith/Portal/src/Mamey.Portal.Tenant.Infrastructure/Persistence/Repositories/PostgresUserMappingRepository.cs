using Microsoft.EntityFrameworkCore;
using Mamey.Portal.Tenant.Domain.Entities;
using Mamey.Portal.Tenant.Domain.Repositories;
using Mamey.Portal.Tenant.Domain.ValueObjects;

namespace Mamey.Portal.Tenant.Infrastructure.Persistence.Repositories;

public sealed class PostgresUserMappingRepository : IUserMappingRepository
{
    private readonly TenantDbContext _db;

    public PostgresUserMappingRepository(TenantDbContext db)
    {
        _db = db;
    }

    public async Task<TenantUserMapping?> GetAsync(string issuer, string subject, CancellationToken ct = default)
    {
        var row = await _db.UserMappings.AsNoTracking()
            .SingleOrDefaultAsync(x => x.Issuer == issuer && x.Subject == subject, ct);

        return row is null
            ? null
            : new TenantUserMapping(row.Issuer, row.Subject, new TenantId(row.TenantId), row.Email, row.CreatedAt, row.UpdatedAt);
    }

    public async Task<IReadOnlyList<TenantUserMapping>> GetByTenantAsync(string tenantId, CancellationToken ct = default)
    {
        var rows = await _db.UserMappings.AsNoTracking()
            .Where(x => x.TenantId == tenantId)
            .OrderBy(x => x.Email)
            .ToListAsync(ct);

        return rows.Select(x => new TenantUserMapping(x.Issuer, x.Subject, new TenantId(x.TenantId), x.Email, x.CreatedAt, x.UpdatedAt)).ToList();
    }

    public async Task SaveAsync(TenantUserMapping mapping, CancellationToken ct = default)
    {
        var row = await _db.UserMappings
            .SingleOrDefaultAsync(x => x.Issuer == mapping.Issuer && x.Subject == mapping.Subject, ct);

        if (row is null)
        {
            _db.UserMappings.Add(new TenantUserMappingRow
            {
                Issuer = mapping.Issuer,
                Subject = mapping.Subject,
                TenantId = mapping.TenantId.Value,
                Email = mapping.Email,
                CreatedAt = mapping.CreatedAt,
                UpdatedAt = mapping.UpdatedAt
            });
        }
        else
        {
            row.TenantId = mapping.TenantId.Value;
            row.Email = mapping.Email;
            row.CreatedAt = mapping.CreatedAt;
            row.UpdatedAt = mapping.UpdatedAt;
        }

        await _db.SaveChangesAsync(ct);
    }
}
