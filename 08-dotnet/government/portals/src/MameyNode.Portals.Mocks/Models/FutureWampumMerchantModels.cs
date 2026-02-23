namespace MameyNode.Portals.Mocks.Models;

// Models based on payments.proto and general.proto for FutureWampumMerchant

public class MerchantOnboardingInfo
{
    public string MerchantId { get; set; } = string.Empty;
    public string BusinessName { get; set; } = string.Empty;
    public string BusinessType { get; set; } = string.Empty;
    public string Status { get; set; } = "Pending";
    public DateTime SubmittedAt { get; set; }
    public DateTime? ApprovedAt { get; set; }
    public string ContactEmail { get; set; } = string.Empty;
    public string ContactPhone { get; set; } = string.Empty;
    public string Address { get; set; } = string.Empty;
}

// MerchantPaymentInfo is already defined in PaymentModels.cs

public class MerchantSettlementPaymentInfo
{
    public string PaymentId { get; set; } = string.Empty;
    public string MerchantId { get; set; } = string.Empty;
    public string CustomerAccount { get; set; } = string.Empty;
    public string Amount { get; set; } = "0";
    public string Currency { get; set; } = "USD";
    public string OrderId { get; set; } = string.Empty;
    public PaymentStatus Status { get; set; }
    public DateTime CreatedAt { get; set; }
    public string TerminalId { get; set; } = string.Empty;
}

public class SettlementInfo
{
    public string SettlementId { get; set; } = string.Empty;
    public string MerchantId { get; set; } = string.Empty;
    public string SettlementPeriod { get; set; } = string.Empty;
    public string TotalAmount { get; set; } = "0";
    public string Currency { get; set; } = "USD";
    public int TransactionCount { get; set; }
    public SettlementStatus Status { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? ProcessedAt { get; set; }
    public string FeeAmount { get; set; } = "0";
    public string NetAmount { get; set; } = "0";
}

public class InvoiceInfo
{
    public string InvoiceId { get; set; } = string.Empty;
    public string MerchantId { get; set; } = string.Empty;
    public string CustomerId { get; set; } = string.Empty;
    public string Amount { get; set; } = "0";
    public string Currency { get; set; } = "USD";
    public string Status { get; set; } = "Pending";
    public DateTime CreatedAt { get; set; }
    public DateTime? DueDate { get; set; }
    public DateTime? PaidAt { get; set; }
    public string Description { get; set; } = string.Empty;
    public List<string> LineItems { get; set; } = new();
}

public class QRCodeInfo
{
    public string QRCodeId { get; set; } = string.Empty;
    public string MerchantId { get; set; } = string.Empty;
    public string Amount { get; set; } = "0";
    public string Currency { get; set; } = "USD";
    public string QRCodeData { get; set; } = string.Empty;
    public string QRCodeImageUrl { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; }
    public DateTime? ExpiresAt { get; set; }
    public bool IsUsed { get; set; }
    public string Status { get; set; } = "Active";
}

public class MerchantAnalyticsInfo
{
    public string MerchantId { get; set; } = string.Empty;
    public string Period { get; set; } = string.Empty; // Daily, Weekly, Monthly
    public string TotalRevenue { get; set; } = "0";
    public string TotalTransactions { get; set; } = "0";
    public string AverageTransaction { get; set; } = "0";
    public int TransactionCount { get; set; }
    public Dictionary<string, int> TransactionsByStatus { get; set; } = new();
    public Dictionary<string, string> RevenueByCurrency { get; set; } = new();
    public DateTime PeriodStart { get; set; }
    public DateTime PeriodEnd { get; set; }
}

public class MerchantComplianceInfo
{
    public string ComplianceId { get; set; } = string.Empty;
    public string MerchantId { get; set; } = string.Empty;
    public string ComplianceType { get; set; } = string.Empty; // KYC, AML, etc.
    public string Status { get; set; } = "Pending";
    public DateTime CheckedAt { get; set; }
    public bool IsCompliant { get; set; }
    public List<string> Violations { get; set; } = new();
    public DateTime? ExpiresAt { get; set; }
}

