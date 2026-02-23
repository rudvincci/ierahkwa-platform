using Mamey.Portal.Citizenship.Domain.Events;
using Mamey.Portal.Citizenship.Domain.ValueObjects;
using Mamey.Types;

namespace Mamey.Portal.Citizenship.Domain.Entities;

public sealed class CitizenshipStatus : AggregateRoot<Guid>
{
    public string TenantId { get; private set; } = string.Empty;
    public string Email { get; private set; } = string.Empty;
    public CitizenshipStatusType Status { get; private set; }
    public Guid ApplicationId { get; private set; }
    public DateTimeOffset StatusGrantedAt { get; private set; }
    public DateTimeOffset? StatusExpiresAt { get; private set; }
    public int YearsCompleted { get; private set; }
    public DateTimeOffset CreatedAt { get; private set; }
    public DateTimeOffset UpdatedAt { get; private set; }

    public List<StatusProgressionApplication> ProgressionApplications { get; private set; } = new();

    private CitizenshipStatus() { }

    public CitizenshipStatus(
        Guid id,
        string tenantId,
        string email,
        CitizenshipStatusType status,
        Guid applicationId,
        DateTimeOffset statusGrantedAt,
        DateTimeOffset? statusExpiresAt,
        int yearsCompleted,
        DateTimeOffset createdAt,
        DateTimeOffset updatedAt)
        : base(id)
    {
        TenantId = tenantId;
        Email = email;
        Status = status;
        ApplicationId = applicationId;
        StatusGrantedAt = statusGrantedAt;
        StatusExpiresAt = statusExpiresAt;
        YearsCompleted = yearsCompleted;
        CreatedAt = createdAt;
        UpdatedAt = updatedAt;
    }

    public static CitizenshipStatus Rehydrate(
        Guid id,
        string tenantId,
        string email,
        CitizenshipStatusType status,
        Guid applicationId,
        DateTimeOffset statusGrantedAt,
        DateTimeOffset? statusExpiresAt,
        int yearsCompleted,
        DateTimeOffset createdAt,
        DateTimeOffset updatedAt,
        IReadOnlyList<StatusProgressionApplication> progressionApplications)
    {
        var citizenshipStatus = new CitizenshipStatus(
            id,
            tenantId,
            email,
            status,
            applicationId,
            statusGrantedAt,
            statusExpiresAt,
            yearsCompleted,
            createdAt,
            updatedAt)
        {
            ProgressionApplications = progressionApplications.ToList()
        };

        return citizenshipStatus;
    }

    public void ProgressTo(CitizenshipStatusType nextStatus, DateTimeOffset progressedAt)
    {
        if (nextStatus == Status)
        {
            return;
        }

        Status = nextStatus;
        StatusGrantedAt = progressedAt;
        UpdatedAt = progressedAt;
        AddEvent(new StatusProgressed(Id, TenantId, Email, nextStatus.ToString(), progressedAt));
    }
}
