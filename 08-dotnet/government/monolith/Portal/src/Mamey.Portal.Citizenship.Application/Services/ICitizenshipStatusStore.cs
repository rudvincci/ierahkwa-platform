namespace Mamey.Portal.Citizenship.Application.Services;

public interface ICitizenshipStatusStore
{
    Task<ApplicationStatusSnapshot?> GetApplicationStatusAsync(string tenantId, Guid applicationId, CancellationToken ct = default);
    Task<CitizenshipStatusSnapshot?> GetLatestStatusAsync(string tenantId, string email, CancellationToken ct = default);
    Task<CitizenshipStatusSnapshot?> GetStatusByApplicationAsync(string tenantId, Guid applicationId, CancellationToken ct = default);
    Task<CitizenshipStatusSnapshot?> GetExistingStatusAsync(string tenantId, string email, CancellationToken ct = default);
    Task<Guid> CreateStatusAsync(
        string tenantId,
        Guid applicationId,
        string email,
        string status,
        DateTimeOffset grantedAt,
        DateTimeOffset? expiresAt,
        DateTimeOffset now,
        CancellationToken ct = default);
    Task<bool> TouchStatusAsync(Guid statusId, DateTimeOffset now, CancellationToken ct = default);
}

public sealed record ApplicationStatusSnapshot(string Status);

public sealed record CitizenshipStatusSnapshot(
    Guid Id,
    string Status,
    int YearsCompleted,
    DateTimeOffset StatusGrantedAt,
    DateTimeOffset? StatusExpiresAt);
