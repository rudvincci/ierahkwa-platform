namespace AppBuilder.Core.Models;

/// <summary>Invoice - Appy: Automatic Invoice Generation, PDF. PayPal / Bank Transfer.</summary>
public class Invoice
{
    public string Id { get; set; } = Guid.NewGuid().ToString();
    public string Number { get; set; } = string.Empty;     // INV-2026-00001
    public string UserId { get; set; } = string.Empty;
    public string? SubscriptionId { get; set; }
    public decimal Amount { get; set; }
    public string Currency { get; set; } = "USD";
    public PaymentMethod PaymentMethod { get; set; }
    public PaymentStatus Status { get; set; } = PaymentStatus.Pending;
    public string? Description { get; set; }
    public string? PayPalTransactionId { get; set; }
    public string? BankReference { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime? PaidAt { get; set; }
    public DateTime? DueDate { get; set; }
}

public enum PaymentMethod
{
    PayPal,
    BankTransfer
}

public enum PaymentStatus
{
    Pending,
    Paid,
    Failed,
    Refunded,
    Cancelled
}
