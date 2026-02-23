using Microsoft.EntityFrameworkCore;
using Mamey.Portal.Citizenship.Domain.Entities;
using Mamey.Portal.Citizenship.Domain.Repositories;
using Mamey.Portal.Citizenship.Infrastructure.Persistence.Mapping;

namespace Mamey.Portal.Citizenship.Infrastructure.Persistence.Repositories;

public sealed class PostgresIntakeReviewRepository : IIntakeReviewRepository
{
    private readonly CitizenshipDbContext _db;

    public PostgresIntakeReviewRepository(CitizenshipDbContext db)
    {
        _db = db;
    }

    public async Task<IntakeReview?> GetByApplicationAsync(Guid applicationId, CancellationToken ct = default)
    {
        var row = await _db.IntakeReviews.AsNoTracking()
            .SingleOrDefaultAsync(x => x.ApplicationId == applicationId, ct);

        return row?.ToDomainEntity();
    }

    public async Task SaveAsync(IntakeReview review, CancellationToken ct = default)
    {
        var row = await _db.IntakeReviews
            .SingleOrDefaultAsync(x => x.Id == review.Id, ct);

        if (row is null)
        {
            _db.IntakeReviews.Add(review.ToRow());
        }
        else
        {
            row.UpdateFromDomain(review);
        }

        await _db.SaveChangesAsync(ct);
    }
}
