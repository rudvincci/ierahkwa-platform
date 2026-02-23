namespace Mamey.Portal.Citizenship.Application.Services;

public interface IApplicationWorkflowStore
{
    Task<WorkflowApplicationSnapshot?> GetApplicationAsync(string tenantId, Guid applicationId, CancellationToken ct = default);
    Task<IntakeReviewSnapshot?> GetLatestIntakeReviewAsync(string tenantId, Guid applicationId, CancellationToken ct = default);
    Task<bool> UpdateStatusAsync(string tenantId, Guid applicationId, string status, string? rejectionReason, DateTimeOffset updatedAt, CancellationToken ct = default);
    Task<bool> CreatePaymentPlanIfMissingAsync(string tenantId, Guid applicationId, string applicationNumber, decimal amount, string currency, DateTimeOffset now, CancellationToken ct = default);
}

public sealed record WorkflowApplicationSnapshot(
    Guid Id,
    string TenantId,
    string Status,
    string? Email,
    DateOnly DateOfBirth,
    string FirstName,
    string LastName,
    bool AcknowledgeTreaty,
    bool SwearAllegiance,
    bool ConsentToVerification,
    bool ConsentToDataProcessing,
    IReadOnlyList<string> UploadKinds,
    string ApplicationNumber);

public sealed record IntakeReviewSnapshot(string Recommendation);
