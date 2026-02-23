namespace Mamey.Portal.Citizenship.Domain.Entities;

public sealed class PaymentPlan
{
    public Guid Id { get; private set; }
    public string TenantId { get; private set; } = string.Empty;
    public Guid ApplicationId { get; private set; }
    public string ApplicationNumber { get; private set; } = string.Empty;
    public decimal Amount { get; private set; }
    public string Currency { get; private set; } = "USD";
    public string Status { get; private set; } = "Pending";
    public string? PaymentReference { get; private set; }
    public string? PaymentMethod { get; private set; }
    public string? PaymentGateway { get; private set; }
    public DateTimeOffset CreatedAt { get; private set; }
    public DateTimeOffset UpdatedAt { get; private set; }
    public DateTimeOffset? PaidAt { get; private set; }

    private PaymentPlan() { }

    public PaymentPlan(
        Guid id,
        string tenantId,
        Guid applicationId,
        string applicationNumber,
        decimal amount,
        string currency,
        string status,
        string? paymentReference,
        string? paymentMethod,
        string? paymentGateway,
        DateTimeOffset createdAt,
        DateTimeOffset updatedAt,
        DateTimeOffset? paidAt)
    {
        Id = id;
        TenantId = tenantId;
        ApplicationId = applicationId;
        ApplicationNumber = applicationNumber;
        Amount = amount;
        Currency = currency;
        Status = status;
        PaymentReference = paymentReference;
        PaymentMethod = paymentMethod;
        PaymentGateway = paymentGateway;
        CreatedAt = createdAt;
        UpdatedAt = updatedAt;
        PaidAt = paidAt;
    }
}
