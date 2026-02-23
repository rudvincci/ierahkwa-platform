using Microsoft.EntityFrameworkCore;
using Mamey.Portal.Citizenship.Application.Services;
using Mamey.Portal.Citizenship.Infrastructure.Persistence;

namespace Mamey.Portal.Citizenship.Infrastructure.Services;

public sealed class StatusProgressionStore : IStatusProgressionStore
{
    private readonly CitizenshipDbContext _db;

    public StatusProgressionStore(CitizenshipDbContext db)
    {
        _db = db;
    }

    public async Task<StatusRecordSnapshot?> GetLatestStatusAsync(string tenantId, string email, CancellationToken ct = default)
    {
        var status = await _db.CitizenshipStatuses
            .AsNoTracking()
            .Where(s => s.TenantId == tenantId && s.Email == email)
            .OrderByDescending(s => s.StatusGrantedAt)
            .Select(s => new
            {
                s.Id,
                s.Email,
                s.Status,
                s.YearsCompleted,
                s.StatusGrantedAt,
                s.StatusExpiresAt,
                s.ApplicationId
            })
            .FirstOrDefaultAsync(ct);

        return status is null
            ? null
            : new StatusRecordSnapshot(
                status.Id,
                status.Email,
                status.Status,
                status.YearsCompleted,
                status.StatusGrantedAt,
                status.StatusExpiresAt,
                status.ApplicationId);
    }

    public async Task<StatusRecordSnapshot?> GetStatusByIdAsync(string tenantId, Guid statusId, CancellationToken ct = default)
    {
        var status = await _db.CitizenshipStatuses
            .AsNoTracking()
            .Where(s => s.TenantId == tenantId && s.Id == statusId)
            .Select(s => new
            {
                s.Id,
                s.Email,
                s.Status,
                s.YearsCompleted,
                s.StatusGrantedAt,
                s.StatusExpiresAt,
                s.ApplicationId
            })
            .FirstOrDefaultAsync(ct);

        return status is null
            ? null
            : new StatusRecordSnapshot(
                status.Id,
                status.Email,
                status.Status,
                status.YearsCompleted,
                status.StatusGrantedAt,
                status.StatusExpiresAt,
                status.ApplicationId);
    }

    public async Task<StatusProgressionAppSnapshot?> GetProgressionApplicationAsync(string tenantId, Guid progressionApplicationId, CancellationToken ct = default)
    {
        var app = await _db.StatusProgressionApplications
            .AsNoTracking()
            .Where(a => a.TenantId == tenantId && a.Id == progressionApplicationId)
            .Select(a => new
            {
                a.Id,
                a.CitizenshipStatusId,
                a.ApplicationNumber,
                a.TargetStatus,
                a.Status,
                a.YearsCompletedAtApplication,
                a.CreatedAt,
                a.UpdatedAt
            })
            .FirstOrDefaultAsync(ct);

        return app is null
            ? null
            : new StatusProgressionAppSnapshot(
                app.Id,
                app.CitizenshipStatusId,
                app.ApplicationNumber,
                app.TargetStatus,
                app.Status,
                app.YearsCompletedAtApplication,
                app.CreatedAt,
                app.UpdatedAt);
    }

    public async Task<IReadOnlyList<StatusProgressionAppSnapshot>> GetProgressionApplicationsAsync(string tenantId, Guid statusId, CancellationToken ct = default)
    {
        return await _db.StatusProgressionApplications
            .AsNoTracking()
            .Where(a => a.TenantId == tenantId && a.CitizenshipStatusId == statusId)
            .OrderBy(a => a.CreatedAt)
            .Select(a => new StatusProgressionAppSnapshot(
                a.Id,
                a.CitizenshipStatusId,
                a.ApplicationNumber,
                a.TargetStatus,
                a.Status,
                a.YearsCompletedAtApplication,
                a.CreatedAt,
                a.UpdatedAt))
            .ToListAsync(ct);
    }

    public async Task<ApplicationSummarySnapshot?> GetApplicationByIdAsync(string tenantId, Guid applicationId, CancellationToken ct = default)
    {
        var app = await _db.Applications
            .AsNoTracking()
            .Where(a => a.TenantId == tenantId && a.Id == applicationId)
            .Select(a => new
            {
                a.Id,
                a.ApplicationNumber,
                a.Status,
                a.CreatedAt,
                a.FirstName,
                a.LastName
            })
            .FirstOrDefaultAsync(ct);

        return app is null
            ? null
            : new ApplicationSummarySnapshot(
                app.Id,
                app.ApplicationNumber,
                app.Status,
                app.CreatedAt,
                app.FirstName,
                app.LastName);
    }

    public async Task<ApplicationSummarySnapshot?> GetLatestApplicationByEmailAsync(string tenantId, string email, CancellationToken ct = default)
    {
        var app = await _db.Applications
            .AsNoTracking()
            .Where(a => a.TenantId == tenantId && a.Email == email)
            .OrderByDescending(a => a.CreatedAt)
            .Select(a => new
            {
                a.Id,
                a.ApplicationNumber,
                a.Status,
                a.CreatedAt,
                a.FirstName,
                a.LastName
            })
            .FirstOrDefaultAsync(ct);

        return app is null
            ? null
            : new ApplicationSummarySnapshot(
                app.Id,
                app.ApplicationNumber,
                app.Status,
                app.CreatedAt,
                app.FirstName,
                app.LastName);
    }

    public async Task CreateProgressionApplicationAsync(
        string tenantId,
        Guid statusId,
        string applicationNumber,
        string targetStatus,
        string status,
        int yearsCompletedAtApplication,
        DateTimeOffset now,
        CancellationToken ct = default)
    {
        _db.StatusProgressionApplications.Add(new StatusProgressionApplicationRow
        {
            Id = Guid.NewGuid(),
            TenantId = tenantId,
            CitizenshipStatusId = statusId,
            ApplicationNumber = applicationNumber,
            TargetStatus = targetStatus,
            Status = status,
            YearsCompletedAtApplication = yearsCompletedAtApplication,
            CreatedAt = now,
            UpdatedAt = now
        });

        await _db.SaveChangesAsync(ct);
    }

    public async Task UpdateProgressionApprovalAsync(
        string tenantId,
        Guid progressionApplicationId,
        Guid statusId,
        string newStatus,
        int yearsCompleted,
        DateTimeOffset statusGrantedAt,
        DateTimeOffset? statusExpiresAt,
        DateTimeOffset now,
        CancellationToken ct = default)
    {
        var progressionApp = await _db.StatusProgressionApplications
            .Where(a => a.TenantId == tenantId && a.Id == progressionApplicationId)
            .FirstOrDefaultAsync(ct);

        var status = await _db.CitizenshipStatuses
            .Where(s => s.TenantId == tenantId && s.Id == statusId)
            .FirstOrDefaultAsync(ct);

        if (progressionApp == null || status == null)
        {
            return;
        }

        status.Status = newStatus;
        status.StatusGrantedAt = statusGrantedAt;
        status.StatusExpiresAt = statusExpiresAt;
        status.YearsCompleted = yearsCompleted;
        status.UpdatedAt = now;

        progressionApp.Status = "Approved";
        progressionApp.UpdatedAt = now;

        await _db.SaveChangesAsync(ct);
    }
}
