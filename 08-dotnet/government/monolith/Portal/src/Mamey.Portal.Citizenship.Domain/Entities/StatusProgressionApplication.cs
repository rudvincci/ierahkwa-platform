using Mamey.Portal.Citizenship.Domain.ValueObjects;

namespace Mamey.Portal.Citizenship.Domain.Entities;

public sealed class StatusProgressionApplication
{
    public Guid Id { get; private set; }
    public string TenantId { get; private set; } = string.Empty;
    public Guid CitizenshipStatusId { get; private set; }
    public ApplicationNumber ApplicationNumber { get; private set; }
    public CitizenshipStatusType TargetStatus { get; private set; }
    public ApplicationStatus Status { get; private set; }
    public int YearsCompletedAtApplication { get; private set; }
    public DateTimeOffset CreatedAt { get; private set; }
    public DateTimeOffset UpdatedAt { get; private set; }

    private StatusProgressionApplication() { }

    public StatusProgressionApplication(
        Guid id,
        string tenantId,
        Guid citizenshipStatusId,
        ApplicationNumber applicationNumber,
        CitizenshipStatusType targetStatus,
        ApplicationStatus status,
        int yearsCompletedAtApplication,
        DateTimeOffset createdAt,
        DateTimeOffset updatedAt)
    {
        Id = id;
        TenantId = tenantId;
        CitizenshipStatusId = citizenshipStatusId;
        ApplicationNumber = applicationNumber;
        TargetStatus = targetStatus;
        Status = status;
        YearsCompletedAtApplication = yearsCompletedAtApplication;
        CreatedAt = createdAt;
        UpdatedAt = updatedAt;
    }
}
