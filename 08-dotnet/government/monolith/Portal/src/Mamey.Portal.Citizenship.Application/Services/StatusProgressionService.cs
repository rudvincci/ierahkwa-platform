using Mamey.Portal.Citizenship.Application.Models;
using Mamey.Portal.Shared.Tenancy;

namespace Mamey.Portal.Citizenship.Application.Services;

public sealed class StatusProgressionService : IStatusProgressionService
{
    private readonly ITenantContext _tenant;
    private readonly ICitizenshipStatusService _statusService;
    private readonly ICitizenshipBackofficeService _backofficeService;
    private readonly IStatusProgressionStore _store;

    public StatusProgressionService(
        ITenantContext tenant,
        ICitizenshipStatusService statusService,
        ICitizenshipBackofficeService backofficeService,
        IStatusProgressionStore store)
    {
        _tenant = tenant;
        _statusService = statusService;
        _backofficeService = backofficeService;
        _store = store;
    }

    public async Task<StatusProgressionEligibilityDto?> CheckEligibilityAsync(string email, CitizenshipStatus targetStatus, CancellationToken ct = default)
    {
        var tenantId = _tenant.TenantId;
        var status = await _store.GetLatestStatusAsync(tenantId, email, ct);

        if (status is null)
        {
            return new StatusProgressionEligibilityDto(
                IsEligible: false,
                CurrentStatus: CitizenshipStatus.Probationary,
                TargetStatus: targetStatus,
                YearsCompleted: 0,
                YearsRequired: targetStatus == CitizenshipStatus.Resident ? 2 : 5,
                StatusExpiresAt: null,
                ReasonNotEligible: "No citizenship status record found. You must have an approved application first."
            );
        }

        var currentStatus = ParseStatus(status.Status);
        if (currentStatus is null)
        {
            return new StatusProgressionEligibilityDto(
                IsEligible: false,
                CurrentStatus: CitizenshipStatus.Probationary,
                TargetStatus: targetStatus,
                YearsCompleted: status.YearsCompleted,
                YearsRequired: targetStatus == CitizenshipStatus.Resident ? 2 : 5,
                StatusExpiresAt: status.StatusExpiresAt,
                ReasonNotEligible: "Invalid citizenship status."
            );
        }

        var now = DateTimeOffset.UtcNow;
        var yearsRequired = targetStatus == CitizenshipStatus.Resident ? 2 : 5;
        var isEligible = false;
        string? reasonNotEligible = null;

        if (targetStatus == CitizenshipStatus.Resident)
        {
            if (currentStatus != CitizenshipStatus.Probationary)
            {
                reasonNotEligible = $"You must be in Probationary status to apply for Resident status. Current status: {status.Status}";
            }
            else if (status.YearsCompleted < 2)
            {
                reasonNotEligible = $"You must complete 2 years as Probationary. Years completed: {status.YearsCompleted}";
            }
            else if (status.StatusExpiresAt.HasValue && now < status.StatusExpiresAt.Value)
            {
                reasonNotEligible = $"Your Probationary status expires on {status.StatusExpiresAt.Value:yyyy-MM-dd}. You can apply after that date.";
            }
            else
            {
                isEligible = true;
            }
        }
        else if (targetStatus == CitizenshipStatus.Citizen)
        {
            if (currentStatus != CitizenshipStatus.Resident)
            {
                reasonNotEligible = $"You must be in Resident status to apply for Citizen status. Current status: {status.Status}";
            }
            else if (status.YearsCompleted < 5)
            {
                reasonNotEligible = $"You must complete 5 total years to apply for Citizen status. Years completed: {status.YearsCompleted}";
            }
            else if (status.StatusExpiresAt.HasValue && now < status.StatusExpiresAt.Value)
            {
                reasonNotEligible = $"Your Resident status expires on {status.StatusExpiresAt.Value:yyyy-MM-dd}. You can apply after that date.";
            }
            else
            {
                isEligible = true;
            }
        }
        else
        {
            reasonNotEligible = $"Invalid target status: {targetStatus}";
        }

        return new StatusProgressionEligibilityDto(
            IsEligible: isEligible,
            CurrentStatus: currentStatus.Value,
            TargetStatus: targetStatus,
            YearsCompleted: status.YearsCompleted,
            YearsRequired: yearsRequired,
            StatusExpiresAt: status.StatusExpiresAt,
            ReasonNotEligible: reasonNotEligible
        );
    }

    public async Task<string> SubmitProgressionApplicationAsync(string email, CitizenshipStatus targetStatus, CancellationToken ct = default)
    {
        var eligibility = await CheckEligibilityAsync(email, targetStatus, ct);
        if (eligibility is null || !eligibility.IsEligible)
        {
            throw new InvalidOperationException(eligibility?.ReasonNotEligible ?? "Not eligible for status progression");
        }

        var tenantId = _tenant.TenantId;
        var status = await _store.GetLatestStatusAsync(tenantId, email, ct);

        if (status is null)
        {
            throw new InvalidOperationException("Citizenship status not found");
        }

        var now = DateTimeOffset.UtcNow;
        var appNumber = $"PROG-{tenantId.ToUpperInvariant()}-{now:yyyyMMdd}-{Guid.NewGuid():N[..8]}";

        await _store.CreateProgressionApplicationAsync(
            tenantId,
            status.Id,
            appNumber,
            targetStatus.ToString(),
            "Submitted",
            status.YearsCompleted,
            now,
            ct);

        return appNumber;
    }

    public async Task<IReadOnlyList<StatusProgressionApplicationDto>> GetProgressionApplicationsAsync(string email, CancellationToken ct = default)
    {
        var tenantId = _tenant.TenantId;
        var status = await _store.GetLatestStatusAsync(tenantId, email, ct);

        if (status is null)
        {
            return Array.Empty<StatusProgressionApplicationDto>();
        }

        var apps = await _store.GetProgressionApplicationsAsync(tenantId, status.Id, ct);
        return apps
            .Select(a => new StatusProgressionApplicationDto(
                a.Id,
                a.ApplicationNumber,
                a.TargetStatus,
                a.Status,
                a.YearsCompletedAtApplication,
                a.CreatedAt,
                a.UpdatedAt))
            .ToList();
    }

    public async Task ApproveProgressionApplicationAsync(Guid progressionApplicationId, CancellationToken ct = default)
    {
        var tenantId = _tenant.TenantId;
        var progressionApp = await _store.GetProgressionApplicationAsync(tenantId, progressionApplicationId, ct);

        if (progressionApp is null)
        {
            throw new InvalidOperationException("Progression application not found");
        }

        if (progressionApp.Status != "Submitted")
        {
            throw new InvalidOperationException($"Application is already {progressionApp.Status}");
        }

        var status = await _store.GetStatusByIdAsync(tenantId, progressionApp.CitizenshipStatusId, ct);
        if (status is null)
        {
            throw new InvalidOperationException("Citizenship status not found");
        }

        var targetStatus = ParseStatus(progressionApp.TargetStatus);
        if (targetStatus is null)
        {
            throw new InvalidOperationException($"Invalid target status: {progressionApp.TargetStatus}");
        }

        var now = DateTimeOffset.UtcNow;
        var yearsCompleted = targetStatus.Value == CitizenshipStatus.Resident ? 2 : 5;
        DateTimeOffset? expiresAt = targetStatus.Value switch
        {
            CitizenshipStatus.Resident => now.AddYears(3),
            CitizenshipStatus.Citizen => null,
            _ => status.StatusExpiresAt
        };

        await _store.UpdateProgressionApprovalAsync(
            tenantId,
            progressionApp.Id,
            status.Id,
            targetStatus.Value.ToString(),
            yearsCompleted,
            now,
            expiresAt,
            now,
            ct);

        try
        {
            await _backofficeService.ReissueDocumentsForStatusProgressionAsync(status.Email, targetStatus.Value, ct);
        }
        catch (Exception)
        {
            // Best-effort: status already updated, documents can be reissued later.
        }
    }

    public async Task<StatusProgressionTimelineDto> GetStatusProgressionTimelineAsync(string email, CancellationToken ct = default)
    {
        var tenantId = _tenant.TenantId;
        var entries = new List<StatusProgressionTimelineEntryDto>();

        var status = await _store.GetLatestStatusAsync(tenantId, email, ct);
        if (status is not null)
        {
            var initialApp = await _store.GetApplicationByIdAsync(tenantId, status.ApplicationId, ct);

            if (initialApp is not null)
            {
                entries.Add(new StatusProgressionTimelineEntryDto(
                    initialApp.CreatedAt,
                    "Application Submitted",
                    $"Citizenship application {initialApp.ApplicationNumber} was submitted.",
                    "Submitted",
                    $"Application for {initialApp.FirstName} {initialApp.LastName}"));

                if (initialApp.Status == "Approved")
                {
                    entries.Add(new StatusProgressionTimelineEntryDto(
                        status.StatusGrantedAt,
                        "Application Approved",
                        $"Application approved and {status.Status} status granted.",
                        status.Status,
                        $"Status granted on {status.StatusGrantedAt:yyyy-MM-dd}"));
                }
            }
            else
            {
                entries.Add(new StatusProgressionTimelineEntryDto(
                    status.StatusGrantedAt,
                    "Status Granted",
                    $"{status.Status} status was granted.",
                    status.Status,
                    $"Status granted on {status.StatusGrantedAt:yyyy-MM-dd}"));
            }

            var progressionApps = await _store.GetProgressionApplicationsAsync(tenantId, status.Id, ct);
            foreach (var app in progressionApps.OrderBy(a => a.CreatedAt))
            {
                entries.Add(new StatusProgressionTimelineEntryDto(
                    app.CreatedAt,
                    "Progression Application Submitted",
                    $"Application {app.ApplicationNumber} submitted to progress from {status.Status} to {app.TargetStatus}.",
                    app.Status,
                    $"Years completed at application: {app.YearsCompletedAtApplication}"));

                if (app.Status == "Approved")
                {
                    entries.Add(new StatusProgressionTimelineEntryDto(
                        app.UpdatedAt,
                        "Progression Application Approved",
                        $"Status progressed from {status.Status} to {app.TargetStatus}.",
                        app.TargetStatus,
                        $"Approved on {app.UpdatedAt:yyyy-MM-dd}"));
                }
                else if (app.Status == "Rejected")
                {
                    entries.Add(new StatusProgressionTimelineEntryDto(
                        app.UpdatedAt,
                        "Progression Application Rejected",
                        $"Application {app.ApplicationNumber} was rejected.",
                        app.Status,
                        $"Rejected on {app.UpdatedAt:yyyy-MM-dd}"));
                }
            }

            entries.Add(new StatusProgressionTimelineEntryDto(
                DateTimeOffset.UtcNow,
                "Current Status",
                $"Current citizenship status: {status.Status}",
                status.Status,
                status.StatusExpiresAt.HasValue
                    ? $"Status expires on {status.StatusExpiresAt.Value:yyyy-MM-dd}"
                    : "Permanent status"));
        }
        else
        {
            var pendingApp = await _store.GetLatestApplicationByEmailAsync(tenantId, email, ct);
            if (pendingApp is not null)
            {
                entries.Add(new StatusProgressionTimelineEntryDto(
                    pendingApp.CreatedAt,
                    "Application Submitted",
                    $"Citizenship application {pendingApp.ApplicationNumber} was submitted.",
                    pendingApp.Status,
                    $"Status: {pendingApp.Status}"));
            }
        }

        return new StatusProgressionTimelineDto(entries.OrderBy(e => e.Timestamp).ToList());
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
