using Mamey.Portal.Citizenship.Application.Models;
using Mamey.Portal.Shared.Tenancy;

namespace Mamey.Portal.Citizenship.Application.Services;

public sealed class CitizenshipStatusService : ICitizenshipStatusService
{
    private readonly ITenantContext _tenant;
    private readonly ICitizenshipStatusStore _store;

    public CitizenshipStatusService(
        ITenantContext tenant,
        ICitizenshipStatusStore store)
    {
        _tenant = tenant;
        _store = store;
    }

    public async Task<CitizenshipStatus?> GetStatusByEmailAsync(string email, CancellationToken ct = default)
    {
        var status = await _store.GetLatestStatusAsync(_tenant.TenantId, email, ct);
        return status is null ? null : ParseStatus(status.Status);
    }

    public async Task<CitizenshipStatus?> GetStatusByApplicationIdAsync(Guid applicationId, CancellationToken ct = default)
    {
        var status = await _store.GetStatusByApplicationAsync(_tenant.TenantId, applicationId, ct);
        return status is null ? null : ParseStatus(status.Status);
    }

    public async Task CreateOrUpdateStatusAsync(Guid applicationId, string email, CancellationToken ct = default)
    {
        var tenantId = _tenant.TenantId;
        var app = await _store.GetApplicationStatusAsync(tenantId, applicationId, ct);

        if (app is null || (app.Status != "Approved" && app.Status != "Completed" && app.Status != "PassportIssued"))
        {
            throw new InvalidOperationException("Application must be approved before creating citizenship status.");
        }

        var existing = await _store.GetExistingStatusAsync(tenantId, email, ct);
        var now = DateTimeOffset.UtcNow;

        if (existing is null)
        {
            await _store.CreateStatusAsync(
                tenantId,
                applicationId,
                email,
                "Probationary",
                now,
                now.AddYears(2),
                now,
                ct);
        }
        else
        {
            await _store.TouchStatusAsync(existing.Id, now, ct);
        }
    }

    public async Task<bool> IsEligibleForStatusProgressionAsync(string email, CitizenshipStatus targetStatus, CancellationToken ct = default)
    {
        var status = await _store.GetLatestStatusAsync(_tenant.TenantId, email, ct);
        if (status is null)
        {
            return false;
        }

        var currentStatus = ParseStatus(status.Status);
        if (currentStatus is null)
        {
            return false;
        }

        var now = DateTimeOffset.UtcNow;

        return targetStatus switch
        {
            CitizenshipStatus.Resident =>
                currentStatus == CitizenshipStatus.Probationary &&
                (status.StatusExpiresAt is null || now >= status.StatusExpiresAt.Value) &&
                status.YearsCompleted >= 2,

            CitizenshipStatus.Citizen =>
                currentStatus == CitizenshipStatus.Resident &&
                (status.StatusExpiresAt is null || now >= status.StatusExpiresAt.Value) &&
                status.YearsCompleted >= 5,

            _ => false
        };
    }

    public async Task<CitizenshipStatusDetailsDto?> GetStatusDetailsAsync(string email, CancellationToken ct = default)
    {
        var status = await _store.GetLatestStatusAsync(_tenant.TenantId, email, ct);
        if (status is null)
        {
            return null;
        }

        var parsedStatus = ParseStatus(status.Status);
        if (parsedStatus is null)
        {
            return null;
        }

        return new CitizenshipStatusDetailsDto(
            parsedStatus.Value,
            status.YearsCompleted,
            status.StatusGrantedAt,
            status.StatusExpiresAt);
    }

    private static CitizenshipStatus? ParseStatus(string status)
    {
        return status switch
        {
            "Probationary" => CitizenshipStatus.Probationary,
            "Resident" => CitizenshipStatus.Resident,
            "Citizen" => CitizenshipStatus.Citizen,
            _ => null
        };
    }
}
