using Microsoft.EntityFrameworkCore;
using Mamey.Portal.Citizenship.Application.Services;
using Mamey.Portal.Citizenship.Infrastructure.Persistence;

namespace Mamey.Portal.Citizenship.Infrastructure.Services;

public sealed class PublicDocumentValidationStore : IPublicDocumentValidationStore
{
    private readonly CitizenshipDbContext _db;

    public PublicDocumentValidationStore(CitizenshipDbContext db)
    {
        _db = db;
    }

    public async Task<IssuedDocumentSnapshot?> GetLatestIssuedDocumentAsync(string tenantId, string documentNumber, CancellationToken ct = default)
    {
        var doc = await (
                from d in _db.IssuedDocuments.AsNoTracking()
                join a in _db.Applications.AsNoTracking() on d.ApplicationId equals a.Id
                where a.TenantId == tenantId && d.DocumentNumber == documentNumber
                orderby d.CreatedAt descending
                select new
                {
                    d.Kind,
                    d.DocumentNumber,
                    d.CreatedAt,
                    d.ExpiresAt
                })
            .FirstOrDefaultAsync(ct);

        return doc is null
            ? null
            : new IssuedDocumentSnapshot(
                doc.Kind,
                doc.DocumentNumber,
                doc.CreatedAt,
                doc.ExpiresAt);
    }
}
