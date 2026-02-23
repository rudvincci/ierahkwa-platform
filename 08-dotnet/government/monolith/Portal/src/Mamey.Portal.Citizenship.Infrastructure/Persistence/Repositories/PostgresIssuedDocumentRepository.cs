using Microsoft.EntityFrameworkCore;
using Mamey.Portal.Citizenship.Domain.Entities;
using Mamey.Portal.Citizenship.Domain.Repositories;
using Mamey.Portal.Citizenship.Infrastructure.Persistence.Mapping;

namespace Mamey.Portal.Citizenship.Infrastructure.Persistence.Repositories;

public sealed class PostgresIssuedDocumentRepository : IIssuedDocumentRepository
{
    private readonly CitizenshipDbContext _db;

    public PostgresIssuedDocumentRepository(CitizenshipDbContext db)
    {
        _db = db;
    }

    public async Task<IssuedDocument?> GetByIdAsync(Guid id, CancellationToken ct = default)
    {
        var row = await _db.IssuedDocuments.AsNoTracking()
            .SingleOrDefaultAsync(x => x.Id == id, ct);

        return row?.ToDomainEntity();
    }

    public async Task<IReadOnlyList<IssuedDocument>> GetByApplicationAsync(Guid applicationId, CancellationToken ct = default)
    {
        var rows = await _db.IssuedDocuments.AsNoTracking()
            .Where(x => x.ApplicationId == applicationId)
            .OrderByDescending(x => x.CreatedAt)
            .ToListAsync(ct);

        return rows.Select(x => x.ToDomainEntity()).ToList();
    }

    public async Task SaveAsync(IssuedDocument document, CancellationToken ct = default)
    {
        var row = await _db.IssuedDocuments
            .SingleOrDefaultAsync(x => x.Id == document.Id, ct);

        if (row is null)
        {
            _db.IssuedDocuments.Add(document.ToRow());
        }
        else
        {
            row.UpdateFromDomain(document);
        }

        await _db.SaveChangesAsync(ct);
    }
}
