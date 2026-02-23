using Microsoft.EntityFrameworkCore;
using Mamey.Portal.Citizenship.Domain.Entities;
using Mamey.Portal.Citizenship.Domain.Repositories;
using Mamey.Portal.Citizenship.Infrastructure.Persistence.Mapping;

namespace Mamey.Portal.Citizenship.Infrastructure.Persistence.Repositories;

public sealed class PostgresStatusProgressionRepository : IStatusProgressionRepository
{
    private readonly CitizenshipDbContext _db;

    public PostgresStatusProgressionRepository(CitizenshipDbContext db)
    {
        _db = db;
    }

    public async Task<StatusProgressionApplication?> GetByIdAsync(Guid id, CancellationToken ct = default)
    {
        var row = await _db.StatusProgressionApplications.AsNoTracking()
            .SingleOrDefaultAsync(x => x.Id == id, ct);

        return row?.ToDomainEntity();
    }

    public async Task<IReadOnlyList<StatusProgressionApplication>> GetByStatusIdAsync(Guid citizenshipStatusId, CancellationToken ct = default)
    {
        var rows = await _db.StatusProgressionApplications.AsNoTracking()
            .Where(x => x.CitizenshipStatusId == citizenshipStatusId)
            .OrderByDescending(x => x.CreatedAt)
            .ToListAsync(ct);

        return rows.Select(x => x.ToDomainEntity()).ToList();
    }

    public async Task SaveAsync(StatusProgressionApplication application, CancellationToken ct = default)
    {
        var row = await _db.StatusProgressionApplications
            .SingleOrDefaultAsync(x => x.Id == application.Id, ct);

        if (row is null)
        {
            _db.StatusProgressionApplications.Add(application.ToRow());
        }
        else
        {
            row.UpdateFromDomain(application);
        }

        await _db.SaveChangesAsync(ct);
    }
}
