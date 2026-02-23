using Microsoft.EntityFrameworkCore;
using Mamey.Portal.Citizenship.Application.Services;
using Mamey.Portal.Citizenship.Infrastructure.Persistence;

namespace Mamey.Portal.Citizenship.Infrastructure.Services;

public sealed class CitizenshipStatusStore : ICitizenshipStatusStore
{
    private readonly CitizenshipDbContext _db;

    public CitizenshipStatusStore(CitizenshipDbContext db)
    {
        _db = db;
    }

    public async Task<ApplicationStatusSnapshot?> GetApplicationStatusAsync(string tenantId, Guid applicationId, CancellationToken ct = default)
    {
        var app = await _db.Applications
            .AsNoTracking()
            .Where(a => a.TenantId == tenantId && a.Id == applicationId)
            .Select(a => new { a.Status })
            .FirstOrDefaultAsync(ct);

        return app is null ? null : new ApplicationStatusSnapshot(app.Status);
    }

    public async Task<CitizenshipStatusSnapshot?> GetLatestStatusAsync(string tenantId, string email, CancellationToken ct = default)
    {
        var status = await _db.CitizenshipStatuses
            .AsNoTracking()
            .Where(s => s.TenantId == tenantId && s.Email == email)
            .OrderByDescending(s => s.StatusGrantedAt)
            .Select(s => new { s.Id, s.Status, s.YearsCompleted, s.StatusGrantedAt, s.StatusExpiresAt })
            .FirstOrDefaultAsync(ct);

        return status is null ? null : new CitizenshipStatusSnapshot(
            status.Id,
            status.Status,
            status.YearsCompleted,
            status.StatusGrantedAt,
            status.StatusExpiresAt);
    }

    public async Task<CitizenshipStatusSnapshot?> GetStatusByApplicationAsync(string tenantId, Guid applicationId, CancellationToken ct = default)
    {
        var status = await _db.CitizenshipStatuses
            .AsNoTracking()
            .Where(s => s.TenantId == tenantId && s.ApplicationId == applicationId)
            .Select(s => new { s.Id, s.Status, s.YearsCompleted, s.StatusGrantedAt, s.StatusExpiresAt })
            .FirstOrDefaultAsync(ct);

        return status is null ? null : new CitizenshipStatusSnapshot(
            status.Id,
            status.Status,
            status.YearsCompleted,
            status.StatusGrantedAt,
            status.StatusExpiresAt);
    }

    public Task<CitizenshipStatusSnapshot?> GetExistingStatusAsync(string tenantId, string email, CancellationToken ct = default)
    {
        return GetLatestStatusAsync(tenantId, email, ct);
    }

    public async Task<Guid> CreateStatusAsync(
        string tenantId,
        Guid applicationId,
        string email,
        string status,
        DateTimeOffset grantedAt,
        DateTimeOffset? expiresAt,
        DateTimeOffset now,
        CancellationToken ct = default)
    {
        var row = new CitizenshipStatusRow
        {
            Id = Guid.NewGuid(),
            TenantId = tenantId,
            Email = email,
            Status = status,
            ApplicationId = applicationId,
            StatusGrantedAt = grantedAt,
            StatusExpiresAt = expiresAt,
            YearsCompleted = 0,
            CreatedAt = now,
            UpdatedAt = now
        };

        _db.CitizenshipStatuses.Add(row);
        await _db.SaveChangesAsync(ct);
        return row.Id;
    }

    public async Task<bool> TouchStatusAsync(Guid statusId, DateTimeOffset now, CancellationToken ct = default)
    {
        var row = await _db.CitizenshipStatuses
            .Where(s => s.Id == statusId)
            .FirstOrDefaultAsync(ct);

        if (row == null)
        {
            return false;
        }

        row.UpdatedAt = now;
        await _db.SaveChangesAsync(ct);
        return true;
    }
}
