using Microsoft.EntityFrameworkCore;
using Mamey.Portal.Citizenship.Domain.Entities;
using Mamey.Portal.Citizenship.Domain.Repositories;
using Mamey.Portal.Citizenship.Infrastructure.Persistence.Mapping;

namespace Mamey.Portal.Citizenship.Infrastructure.Persistence.Repositories;

public sealed class PostgresUploadRepository : IUploadRepository
{
    private readonly CitizenshipDbContext _db;

    public PostgresUploadRepository(CitizenshipDbContext db)
    {
        _db = db;
    }

    public async Task<CitizenshipUpload?> GetByIdAsync(Guid id, CancellationToken ct = default)
    {
        var row = await _db.Uploads.AsNoTracking()
            .SingleOrDefaultAsync(x => x.Id == id, ct);

        return row?.ToDomainEntity();
    }

    public async Task<IReadOnlyList<CitizenshipUpload>> GetByApplicationAsync(Guid applicationId, CancellationToken ct = default)
    {
        var rows = await _db.Uploads.AsNoTracking()
            .Where(x => x.ApplicationId == applicationId)
            .OrderByDescending(x => x.UploadedAt)
            .ToListAsync(ct);

        return rows.Select(x => x.ToDomainEntity()).ToList();
    }

    public async Task SaveAsync(CitizenshipUpload upload, CancellationToken ct = default)
    {
        var row = await _db.Uploads
            .SingleOrDefaultAsync(x => x.Id == upload.Id, ct);

        if (row is null)
        {
            _db.Uploads.Add(upload.ToRow());
        }
        else
        {
            row.UpdateFromDomain(upload);
        }

        await _db.SaveChangesAsync(ct);
    }
}
