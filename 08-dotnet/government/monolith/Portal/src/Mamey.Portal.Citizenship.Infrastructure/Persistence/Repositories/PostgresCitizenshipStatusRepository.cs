using Microsoft.EntityFrameworkCore;
using Mamey.Portal.Citizenship.Domain.Entities;
using Mamey.Portal.Citizenship.Domain.Repositories;
using Mamey.Portal.Citizenship.Infrastructure.Persistence.Mapping;

namespace Mamey.Portal.Citizenship.Infrastructure.Persistence.Repositories;

public sealed class PostgresCitizenshipStatusRepository : ICitizenshipStatusRepository
{
    private readonly CitizenshipDbContext _db;

    public PostgresCitizenshipStatusRepository(CitizenshipDbContext db)
    {
        _db = db;
    }

    public async Task<CitizenshipStatus?> GetByIdAsync(Guid id, CancellationToken ct = default)
    {
        var row = await _db.CitizenshipStatuses.AsNoTracking()
            .Include(x => x.ProgressionApplications)
            .SingleOrDefaultAsync(x => x.Id == id, ct);

        return row?.ToDomainEntity();
    }

    public async Task<CitizenshipStatus?> GetByEmailAsync(string tenantId, string email, CancellationToken ct = default)
    {
        var row = await _db.CitizenshipStatuses.AsNoTracking()
            .Include(x => x.ProgressionApplications)
            .SingleOrDefaultAsync(x => x.TenantId == tenantId && x.Email == email, ct);

        return row?.ToDomainEntity();
    }

    public async Task SaveAsync(CitizenshipStatus status, CancellationToken ct = default)
    {
        var row = await _db.CitizenshipStatuses
            .SingleOrDefaultAsync(x => x.Id == status.Id, ct);

        if (row is null)
        {
            _db.CitizenshipStatuses.Add(status.ToRow());
        }
        else
        {
            row.UpdateFromDomain(status);
        }

        await _db.SaveChangesAsync(ct);
    }
}
