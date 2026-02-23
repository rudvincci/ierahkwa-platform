using Microsoft.EntityFrameworkCore;
using Mamey.Portal.Citizenship.Application.Services;
using Mamey.Portal.Citizenship.Infrastructure.Persistence;

namespace Mamey.Portal.Citizenship.Infrastructure.Services;

public sealed class PaymentPlanStore : IPaymentPlanStore
{
    private readonly CitizenshipDbContext _db;

    public PaymentPlanStore(CitizenshipDbContext db)
    {
        _db = db;
    }

    public async Task<PaymentApplicationSnapshot?> GetApplicationAsync(string tenantId, Guid applicationId, CancellationToken ct = default)
    {
        var app = await _db.Applications
            .AsNoTracking()
            .Where(a => a.Id == applicationId && a.TenantId == tenantId)
            .Select(a => new { a.Status, a.ApplicationNumber })
            .FirstOrDefaultAsync(ct);

        return app is null ? null : new PaymentApplicationSnapshot(app.Status, app.ApplicationNumber);
    }

    public async Task<PaymentPlanSnapshot?> GetPaymentPlanSnapshotAsync(string tenantId, Guid applicationId, CancellationToken ct = default)
    {
        var plan = await _db.PaymentPlans
            .AsNoTracking()
            .Where(p => p.ApplicationId == applicationId && p.TenantId == tenantId)
            .Select(p => new { p.Id, p.Status })
            .FirstOrDefaultAsync(ct);

        return plan is null ? null : new PaymentPlanSnapshot(plan.Id, plan.Status);
    }

    public async Task<PaymentPlanDto?> GetPaymentPlanAsync(string tenantId, Guid applicationId, CancellationToken ct = default)
    {
        var plan = await _db.PaymentPlans
            .AsNoTracking()
            .Where(p => p.ApplicationId == applicationId && p.TenantId == tenantId)
            .FirstOrDefaultAsync(ct);

        if (plan == null)
        {
            return null;
        }

        return new PaymentPlanDto(
            plan.Id,
            plan.ApplicationId,
            plan.ApplicationNumber,
            plan.Amount,
            plan.Currency,
            plan.Status,
            plan.PaymentReference,
            plan.CreatedAt,
            plan.PaidAt);
    }

    public async Task<string> CreatePaymentPlanAsync(
        string tenantId,
        Guid applicationId,
        string applicationNumber,
        decimal amount,
        string currency,
        DateTimeOffset now,
        CancellationToken ct = default)
    {
        var existingPlan = await _db.PaymentPlans
            .AsNoTracking()
            .Where(p => p.ApplicationId == applicationId && p.TenantId == tenantId)
            .FirstOrDefaultAsync(ct);

        if (existingPlan != null)
        {
            return existingPlan.Id.ToString();
        }

        var plan = new PaymentPlanRow
        {
            Id = Guid.NewGuid(),
            TenantId = tenantId,
            ApplicationId = applicationId,
            ApplicationNumber = applicationNumber,
            Amount = amount,
            Currency = currency,
            Status = "Pending",
            CreatedAt = now,
            UpdatedAt = now
        };

        _db.PaymentPlans.Add(plan);

        var app = await _db.Applications
            .Where(a => a.Id == applicationId && a.TenantId == tenantId)
            .FirstOrDefaultAsync(ct);

        if (app != null)
        {
            app.Status = "PaymentPending";
            app.UpdatedAt = now;
        }

        await _db.SaveChangesAsync(ct);
        return plan.Id.ToString();
    }

    public async Task<bool> MarkPaymentPaidAsync(
        string tenantId,
        Guid applicationId,
        string paymentReference,
        DateTimeOffset now,
        CancellationToken ct = default)
    {
        var plan = await _db.PaymentPlans
            .Where(p => p.ApplicationId == applicationId && p.TenantId == tenantId)
            .FirstOrDefaultAsync(ct);

        if (plan == null)
        {
            return false;
        }

        plan.Status = "Paid";
        plan.PaymentReference = paymentReference;
        plan.PaidAt = now;
        plan.UpdatedAt = now;

        var app = await _db.Applications
            .Where(a => a.Id == applicationId && a.TenantId == tenantId)
            .FirstOrDefaultAsync(ct);

        if (app != null)
        {
            app.Status = "CitizenCreating";
            app.UpdatedAt = now;
        }

        await _db.SaveChangesAsync(ct);
        return true;
    }
}
