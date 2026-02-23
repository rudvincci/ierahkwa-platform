namespace MameyNode.Portals.Mocks.Models;

// Models based on payments.proto
public enum PaymentStatus
{
    Unspecified = 0,
    Pending = 1,
    Processing = 2,
    Completed = 3,
    Failed = 4,
    Cancelled = 5
}

public enum RecurringPaymentStatus
{
    Unspecified = 0,
    Active = 1,
    Paused = 2,
    Cancelled = 3,
    Completed = 4
}

public enum MultisigPaymentStatus
{
    Unspecified = 0,
    PendingSignatures = 1,
    Completed = 2,
    Executed = 3,
    Cancelled = 4
}

public class PaymentInfo
{
    public string PaymentId { get; set; } = string.Empty;
    public string FromAccount { get; set; } = string.Empty;
    public string ToAccount { get; set; } = string.Empty;
    public string Amount { get; set; } = string.Empty;
    public string Currency { get; set; } = "USD";
    public PaymentStatus Status { get; set; }
    public string TransactionId { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; }
    public DateTime? CompletedAt { get; set; }
    public string Memo { get; set; } = string.Empty;
}

public class MerchantPaymentInfo
{
    public string PaymentId { get; set; } = string.Empty;
    public string MerchantId { get; set; } = string.Empty;
    public string CustomerAccount { get; set; } = string.Empty;
    public string Amount { get; set; } = string.Empty;
    public string Currency { get; set; } = "USD";
    public string OrderId { get; set; } = string.Empty;
    public PaymentStatus Status { get; set; }
    public DateTime CreatedAt { get; set; }
}

public class DisbursementInfo
{
    public string DisbursementId { get; set; } = string.Empty;
    public string ProgramId { get; set; } = string.Empty;
    public string RecipientId { get; set; } = string.Empty;
    public string Amount { get; set; } = string.Empty;
    public string Currency { get; set; } = "USD";
    public string Purpose { get; set; } = string.Empty;
    public PaymentStatus Status { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? ProcessedAt { get; set; }
}

public class RecurringPaymentInfo
{
    public string RecurringPaymentId { get; set; } = string.Empty;
    public string FromAccount { get; set; } = string.Empty;
    public string ToAccount { get; set; } = string.Empty;
    public string Amount { get; set; } = string.Empty;
    public string Currency { get; set; } = "USD";
    public string Frequency { get; set; } = "monthly"; // daily, weekly, monthly, yearly
    public DateTime StartDate { get; set; }
    public DateTime? EndDate { get; set; }
    public DateTime NextPaymentDate { get; set; }
    public RecurringPaymentStatus Status { get; set; }
}

public class MultisigPaymentInfo
{
    public string PaymentId { get; set; } = string.Empty;
    public List<string> Signers { get; set; } = new();
    public int RequiredSignatures { get; set; }
    public int CurrentSignatures { get; set; }
    public List<string> SignedBy { get; set; } = new();
    public string ToAccount { get; set; } = string.Empty;
    public string Amount { get; set; } = string.Empty;
    public string Currency { get; set; } = "USD";
    public MultisigPaymentStatus Status { get; set; }
    public DateTime CreatedAt { get; set; }
}

