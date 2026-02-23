namespace Mamey.Portal.Citizenship.Infrastructure.Persistence;

public sealed class PaymentPlanRow
{
    public Guid Id { get; set; }
    public string TenantId { get; set; } = string.Empty;
    public Guid ApplicationId { get; set; }
    public string ApplicationNumber { get; set; } = string.Empty;
    
    public decimal Amount { get; set; }
    public string Currency { get; set; } = "USD";
    public string Status { get; set; } = "Pending"; // Pending, Paid, Failed, Cancelled
    
    public string? PaymentReference { get; set; } // External payment gateway reference
    public string? PaymentMethod { get; set; } // CreditCard, BankTransfer, etc.
    public string? PaymentGateway { get; set; } // Stripe, PayPal, etc.
    
    public DateTimeOffset CreatedAt { get; set; }
    public DateTimeOffset UpdatedAt { get; set; }
    public DateTimeOffset? PaidAt { get; set; }
    
    // Navigation
    public CitizenshipApplicationRow Application { get; set; } = null!;
}

