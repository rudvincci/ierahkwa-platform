namespace Mamey.Portal.Citizenship.Application.Services;

/// <summary>
/// Service for managing payment plans for citizenship applications
/// </summary>
public interface IPaymentPlanService
{
    /// <summary>
    /// Creates a payment plan for an approved application
    /// </summary>
    Task<string> CreatePaymentPlanAsync(Guid applicationId, CancellationToken ct = default);

    /// <summary>
    /// Gets the payment plan for an application
    /// </summary>
    Task<PaymentPlanDto?> GetPaymentPlanAsync(Guid applicationId, CancellationToken ct = default);

    /// <summary>
    /// Confirms payment completion and moves application to next step
    /// </summary>
    Task<bool> ConfirmPaymentAsync(Guid applicationId, string paymentReference, CancellationToken ct = default);
}

public sealed record PaymentPlanDto(
    Guid Id,
    Guid ApplicationId,
    string ApplicationNumber,
    decimal Amount,
    string Currency,
    string Status,
    string? PaymentReference,
    DateTimeOffset CreatedAt,
    DateTimeOffset? PaidAt);

