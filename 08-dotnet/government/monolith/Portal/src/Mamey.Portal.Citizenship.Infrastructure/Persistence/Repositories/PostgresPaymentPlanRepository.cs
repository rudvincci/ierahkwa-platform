using Microsoft.EntityFrameworkCore;
using Mamey.Portal.Citizenship.Domain.Entities;
using Mamey.Portal.Citizenship.Domain.Repositories;
using Mamey.Portal.Citizenship.Infrastructure.Persistence.Mapping;

namespace Mamey.Portal.Citizenship.Infrastructure.Persistence.Repositories;

public sealed class PostgresPaymentPlanRepository : IPaymentPlanRepository
{
    private readonly CitizenshipDbContext _db;

    public PostgresPaymentPlanRepository(CitizenshipDbContext db)
    {
        _db = db;
    }

    public async Task<PaymentPlan?> GetByApplicationAsync(Guid applicationId, CancellationToken ct = default)
    {
        var row = await _db.PaymentPlans.AsNoTracking()
            .SingleOrDefaultAsync(x => x.ApplicationId == applicationId, ct);

        return row?.ToDomainEntity();
    }

    public async Task SaveAsync(PaymentPlan plan, CancellationToken ct = default)
    {
        var row = await _db.PaymentPlans
            .SingleOrDefaultAsync(x => x.Id == plan.Id, ct);

        if (row is null)
        {
            _db.PaymentPlans.Add(plan.ToRow());
        }
        else
        {
            row.UpdateFromDomain(plan);
        }

        await _db.SaveChangesAsync(ct);
    }
}
