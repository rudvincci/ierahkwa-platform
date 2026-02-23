namespace Mamey.Portal.Citizenship.Application.Services;

public interface IPaymentPlanStore
{
    Task<PaymentApplicationSnapshot?> GetApplicationAsync(string tenantId, Guid applicationId, CancellationToken ct = default);
    Task<PaymentPlanSnapshot?> GetPaymentPlanSnapshotAsync(string tenantId, Guid applicationId, CancellationToken ct = default);
    Task<PaymentPlanDto?> GetPaymentPlanAsync(string tenantId, Guid applicationId, CancellationToken ct = default);
    Task<string> CreatePaymentPlanAsync(
        string tenantId,
        Guid applicationId,
        string applicationNumber,
        decimal amount,
        string currency,
        DateTimeOffset now,
        CancellationToken ct = default);
    Task<bool> MarkPaymentPaidAsync(
        string tenantId,
        Guid applicationId,
        string paymentReference,
        DateTimeOffset now,
        CancellationToken ct = default);
}

public sealed record PaymentApplicationSnapshot(string Status, string ApplicationNumber);

public sealed record PaymentPlanSnapshot(Guid Id, string Status);
