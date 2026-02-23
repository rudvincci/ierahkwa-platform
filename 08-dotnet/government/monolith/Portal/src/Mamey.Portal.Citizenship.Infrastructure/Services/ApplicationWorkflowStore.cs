using Microsoft.EntityFrameworkCore;
using Mamey.Portal.Citizenship.Application.Services;
using Mamey.Portal.Citizenship.Infrastructure.Persistence;

namespace Mamey.Portal.Citizenship.Infrastructure.Services;

public sealed class ApplicationWorkflowStore : IApplicationWorkflowStore
{
    private readonly CitizenshipDbContext _db;

    public ApplicationWorkflowStore(CitizenshipDbContext db)
    {
        _db = db;
    }

    public async Task<WorkflowApplicationSnapshot?> GetApplicationAsync(string tenantId, Guid applicationId, CancellationToken ct = default)
    {
        var row = await _db.Applications
            .AsNoTracking()
            .Include(x => x.Uploads)
            .Where(a => a.Id == applicationId && a.TenantId == tenantId)
            .FirstOrDefaultAsync(ct);

        if (row is null)
        {
            return null;
        }

        return new WorkflowApplicationSnapshot(
            row.Id,
            row.TenantId,
            row.Status,
            row.Email,
            row.DateOfBirth,
            row.FirstName,
            row.LastName,
            row.AcknowledgeTreaty,
            row.SwearAllegiance,
            row.ConsentToVerification,
            row.ConsentToDataProcessing,
            row.Uploads.Select(u => u.Kind).ToList(),
            row.ApplicationNumber);
    }

    public async Task<IntakeReviewSnapshot?> GetLatestIntakeReviewAsync(string tenantId, Guid applicationId, CancellationToken ct = default)
    {
        var review = await _db.IntakeReviews
            .AsNoTracking()
            .Where(r => r.ApplicationId == applicationId && r.TenantId == tenantId)
            .OrderByDescending(r => r.ReviewDate)
            .FirstOrDefaultAsync(ct);

        return review is null ? null : new IntakeReviewSnapshot(review.Recommendation);
    }

    public async Task<bool> UpdateStatusAsync(string tenantId, Guid applicationId, string status, string? rejectionReason, DateTimeOffset updatedAt, CancellationToken ct = default)
    {
        var app = await _db.Applications
            .Where(a => a.Id == applicationId && a.TenantId == tenantId)
            .FirstOrDefaultAsync(ct);

        if (app is null)
        {
            return false;
        }

        app.Status = status;
        app.RejectionReason = rejectionReason;
        app.UpdatedAt = updatedAt;

        await _db.SaveChangesAsync(ct);
        return true;
    }

    public async Task<bool> CreatePaymentPlanIfMissingAsync(string tenantId, Guid applicationId, string applicationNumber, decimal amount, string currency, DateTimeOffset now, CancellationToken ct = default)
    {
        var existingPlan = await _db.PaymentPlans
            .AsNoTracking()
            .Where(p => p.ApplicationId == applicationId && p.TenantId == tenantId)
            .FirstOrDefaultAsync(ct);

        if (existingPlan is not null)
        {
            return true;
        }

        var app = await _db.Applications
            .Where(a => a.Id == applicationId && a.TenantId == tenantId)
            .FirstOrDefaultAsync(ct);

        if (app is null)
        {
            return false;
        }

        _db.PaymentPlans.Add(new PaymentPlanRow
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
        });

        app.Status = "PaymentPending";
        app.UpdatedAt = now;

        await _db.SaveChangesAsync(ct);
        return true;
    }
}
