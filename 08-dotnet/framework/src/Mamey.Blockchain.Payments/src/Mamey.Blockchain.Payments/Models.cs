namespace Mamey.Blockchain.Payments;

public class SendP2PPaymentRequest
{
    public string FromAccount { get; set; } = string.Empty;
    public string ToAccount { get; set; } = string.Empty;
    public string Amount { get; set; } = string.Empty;
    public string Currency { get; set; } = string.Empty;
    public string Memo { get; set; } = string.Empty;
}

public class SendP2PPaymentResult
{
    public string PaymentId { get; set; } = string.Empty;
    public string TransactionId { get; set; } = string.Empty;
    public PaymentStatus Status { get; set; }
    public bool Success { get; set; }
    public string ErrorMessage { get; set; } = string.Empty;
}

public enum PaymentStatus
{
    Pending = 0,
    Processing = 1,
    Completed = 2,
    Failed = 3,
    Cancelled = 4
}




