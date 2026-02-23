namespace Mamey.Portal.Citizenship.Application.Services;

public interface IStatusProgressionStore
{
    Task<StatusRecordSnapshot?> GetLatestStatusAsync(string tenantId, string email, CancellationToken ct = default);
    Task<StatusRecordSnapshot?> GetStatusByIdAsync(string tenantId, Guid statusId, CancellationToken ct = default);
    Task<StatusProgressionAppSnapshot?> GetProgressionApplicationAsync(string tenantId, Guid progressionApplicationId, CancellationToken ct = default);
    Task<IReadOnlyList<StatusProgressionAppSnapshot>> GetProgressionApplicationsAsync(string tenantId, Guid statusId, CancellationToken ct = default);
    Task<ApplicationSummarySnapshot?> GetApplicationByIdAsync(string tenantId, Guid applicationId, CancellationToken ct = default);
    Task<ApplicationSummarySnapshot?> GetLatestApplicationByEmailAsync(string tenantId, string email, CancellationToken ct = default);
    Task CreateProgressionApplicationAsync(
        string tenantId,
        Guid statusId,
        string applicationNumber,
        string targetStatus,
        string status,
        int yearsCompletedAtApplication,
        DateTimeOffset now,
        CancellationToken ct = default);
    Task UpdateProgressionApprovalAsync(
        string tenantId,
        Guid progressionApplicationId,
        Guid statusId,
        string newStatus,
        int yearsCompleted,
        DateTimeOffset statusGrantedAt,
        DateTimeOffset? statusExpiresAt,
        DateTimeOffset now,
        CancellationToken ct = default);
}

public sealed record StatusRecordSnapshot(
    Guid Id,
    string Email,
    string Status,
    int YearsCompleted,
    DateTimeOffset StatusGrantedAt,
    DateTimeOffset? StatusExpiresAt,
    Guid ApplicationId);

public sealed record StatusProgressionAppSnapshot(
    Guid Id,
    Guid CitizenshipStatusId,
    string ApplicationNumber,
    string TargetStatus,
    string Status,
    int YearsCompletedAtApplication,
    DateTimeOffset CreatedAt,
    DateTimeOffset UpdatedAt);

public sealed record ApplicationSummarySnapshot(
    Guid Id,
    string ApplicationNumber,
    string Status,
    DateTimeOffset CreatedAt,
    string FirstName,
    string LastName);
