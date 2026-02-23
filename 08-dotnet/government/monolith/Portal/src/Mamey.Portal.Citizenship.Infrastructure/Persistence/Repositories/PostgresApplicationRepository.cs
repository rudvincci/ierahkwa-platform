using Microsoft.EntityFrameworkCore;
using Mamey.Portal.Citizenship.Domain.Entities;
using Mamey.Portal.Citizenship.Domain.Repositories;
using Mamey.Portal.Citizenship.Domain.ValueObjects;
using Mamey.Portal.Citizenship.Infrastructure.Persistence.Mapping;

namespace Mamey.Portal.Citizenship.Infrastructure.Persistence.Repositories;

public sealed class PostgresApplicationRepository : IApplicationRepository
{
    private readonly CitizenshipDbContext _db;

    public PostgresApplicationRepository(CitizenshipDbContext db)
    {
        _db = db;
    }

    public async Task<CitizenshipApplication?> GetByIdAsync(Guid id, CancellationToken ct = default)
    {
        var row = await _db.Applications.AsNoTracking()
            .Include(x => x.Uploads)
            .Include(x => x.IssuedDocuments)
            .SingleOrDefaultAsync(x => x.Id == id, ct);

        return row?.ToDomainEntity();
    }

    public async Task<CitizenshipApplication?> GetByApplicationNumberAsync(ApplicationNumber applicationNumber, CancellationToken ct = default)
    {
        var row = await _db.Applications.AsNoTracking()
            .Include(x => x.Uploads)
            .Include(x => x.IssuedDocuments)
            .SingleOrDefaultAsync(x => x.ApplicationNumber == applicationNumber.Value, ct);

        return row?.ToDomainEntity();
    }

    public async Task<IReadOnlyList<CitizenshipApplication>> GetByTenantAsync(string tenantId, CancellationToken ct = default)
    {
        var rows = await _db.Applications.AsNoTracking()
            .Where(x => x.TenantId == tenantId)
            .OrderByDescending(x => x.CreatedAt)
            .ToListAsync(ct);

        return rows.Select(x => x.ToDomainEntity()).ToList();
    }

    public async Task<IReadOnlyList<CitizenshipApplication>> GetByStatusAsync(ApplicationStatus status, CancellationToken ct = default)
    {
        var statusValue = status.ToString();
        var rows = await _db.Applications.AsNoTracking()
            .Where(x => x.Status == statusValue)
            .OrderByDescending(x => x.CreatedAt)
            .ToListAsync(ct);

        return rows.Select(x => x.ToDomainEntity()).ToList();
    }

    public async Task SaveAsync(CitizenshipApplication application, CancellationToken ct = default)
    {
        var row = await _db.Applications
            .SingleOrDefaultAsync(x => x.Id == application.Id, ct);

        if (row is null)
        {
            _db.Applications.Add(application.ToRow());
        }
        else
        {
            row.UpdateFromDomain(application);
        }

        await _db.SaveChangesAsync(ct);
    }

    public async Task DeleteAsync(Guid id, CancellationToken ct = default)
    {
        var row = await _db.Applications.SingleOrDefaultAsync(x => x.Id == id, ct);
        if (row is null)
        {
            return;
        }

        _db.Applications.Remove(row);
        await _db.SaveChangesAsync(ct);
    }
}
