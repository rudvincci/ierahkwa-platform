namespace IERAHKWA.Platform.Models;

// ============= TRANSACTION MODELS =============

public class TransactionRequest
{
    public string FromWallet { get; set; } = "";
    public string ToWallet { get; set; } = "";
    public decimal Amount { get; set; }
    public string Currency { get; set; } = "IGT";
    public string? TokenAddress { get; set; }
    public string? Memo { get; set; }
    public string Type { get; set; } = "transfer"; // transfer, payment, swap, stake
}

public class Transaction
{
    public string Id { get; set; } = Guid.NewGuid().ToString();
    public string TxHash { get; set; } = "";
    public string FromWallet { get; set; } = "";
    public string ToWallet { get; set; } = "";
    public decimal Amount { get; set; }
    public string Currency { get; set; } = "IGT";
    public decimal Fee { get; set; }
    public string Status { get; set; } = "pending"; // pending, confirmed, failed, cancelled
    public string Type { get; set; } = "transfer";
    public string? Memo { get; set; }
    public int Confirmations { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime? ConfirmedAt { get; set; }
    public string? BlockNumber { get; set; }
}

public class TransferRequest
{
    public string FromWalletId { get; set; } = "";
    public string ToAddress { get; set; } = "";
    public decimal Amount { get; set; }
    public string Currency { get; set; } = "IGT";
    public string? TokenAddress { get; set; }
    public string? Pin { get; set; }
    public string? Memo { get; set; }
}

public class TransactionStats
{
    public int TotalTransactions { get; set; }
    public decimal TotalVolume { get; set; }
    public int TodayTransactions { get; set; }
    public decimal TodayVolume { get; set; }
    public decimal AverageAmount { get; set; }
    public int PendingCount { get; set; }
    public int ConfirmedCount { get; set; }
    public int FailedCount { get; set; }
}

public class TransactionStatus
{
    public string TxHash { get; set; } = "";
    public string Status { get; set; } = "";
    public int Confirmations { get; set; }
    public int RequiredConfirmations { get; set; } = 12;
    public DateTime? ConfirmedAt { get; set; }
    public string? BlockNumber { get; set; }
}

// ============= WALLET MODELS =============

public class Wallet
{
    public string Id { get; set; } = Guid.NewGuid().ToString();
    public string Address { get; set; } = "";
    public string UserId { get; set; } = "";
    public string Name { get; set; } = "Main Wallet";
    public string Type { get; set; } = "standard"; // standard, multisig, hardware
    public List<TokenBalance> Balances { get; set; } = new();
    public bool IsActive { get; set; } = true;
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime? LastActivityAt { get; set; }
}

public class TokenBalance
{
    public string Symbol { get; set; } = "";
    public string Name { get; set; } = "";
    public decimal Balance { get; set; }
    public decimal LockedBalance { get; set; }
    public string? ContractAddress { get; set; }
    public int Decimals { get; set; } = 18;
    public decimal UsdValue { get; set; }
}

public class CreateWalletRequest
{
    public string UserId { get; set; } = "";
    public string Name { get; set; } = "Main Wallet";
    public string Type { get; set; } = "standard";
    public string? Password { get; set; }
}

public class DepositRequest
{
    public decimal Amount { get; set; }
    public string Currency { get; set; } = "IGT";
    public string? FromAddress { get; set; }
    public string? TxHash { get; set; }
}

public class WithdrawRequest
{
    public decimal Amount { get; set; }
    public string Currency { get; set; } = "IGT";
    public string ToAddress { get; set; } = "";
    public string? Pin { get; set; }
}

public class AddTokenRequest
{
    public string ContractAddress { get; set; } = "";
    public string Symbol { get; set; } = "";
    public string Name { get; set; } = "";
    public int Decimals { get; set; } = 18;
}

public class ExportRequest
{
    public string Password { get; set; } = "";
}

public class ImportWalletRequest
{
    public string EncryptedData { get; set; } = "";
    public string Password { get; set; } = "";
    public string UserId { get; set; } = "";
}

public class WalletBalance
{
    public string WalletId { get; set; } = "";
    public string Address { get; set; } = "";
    public decimal TotalUsdValue { get; set; }
    public List<TokenBalance> Tokens { get; set; } = new();
}

// ============= PAYMENT MODELS =============

public class PaymentRequest
{
    public string WalletId { get; set; } = "";
    public decimal Amount { get; set; }
    public string Currency { get; set; } = "IGT";
    public string? Description { get; set; }
    public string? CallbackUrl { get; set; }
    public string? ReturnUrl { get; set; }
    public Dictionary<string, string>? Metadata { get; set; }
    public int ExpiresInMinutes { get; set; } = 30;
}

public class Payment
{
    public string Id { get; set; } = Guid.NewGuid().ToString();
    public string WalletId { get; set; } = "";
    public decimal Amount { get; set; }
    public string Currency { get; set; } = "IGT";
    public string Status { get; set; } = "pending"; // pending, processing, completed, failed, expired, refunded
    public string? TxHash { get; set; }
    public string? Description { get; set; }
    public string? PaymentAddress { get; set; }
    public string? QrCode { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime ExpiresAt { get; set; }
    public DateTime? PaidAt { get; set; }
    public Dictionary<string, string>? Metadata { get; set; }
}

public class ConfirmPaymentRequest
{
    public string TxHash { get; set; } = "";
    public string? Pin { get; set; }
}

public class RefundRequest
{
    public decimal? Amount { get; set; } // null = full refund
    public string Reason { get; set; } = "";
}

public class WebhookPayload
{
    public string Event { get; set; } = "";
    public string PaymentId { get; set; } = "";
    public string Status { get; set; } = "";
    public string? TxHash { get; set; }
    public DateTime Timestamp { get; set; } = DateTime.UtcNow;
    public string Signature { get; set; } = "";
}

public class PaymentMethod
{
    public string Id { get; set; } = "";
    public string Name { get; set; } = "";
    public string Type { get; set; } = ""; // crypto, fiat, card
    public string[] Currencies { get; set; } = Array.Empty<string>();
    public decimal MinAmount { get; set; }
    public decimal MaxAmount { get; set; }
    public decimal Fee { get; set; }
    public bool IsActive { get; set; } = true;
}

public class InvoiceRequest
{
    public string CustomerEmail { get; set; } = "";
    public string? CustomerName { get; set; }
    public decimal Amount { get; set; }
    public string Currency { get; set; } = "IGT";
    public string Description { get; set; } = "";
    public List<InvoiceItem>? Items { get; set; }
    public DateTime? DueDate { get; set; }
}

public class InvoiceItem
{
    public string Description { get; set; } = "";
    public int Quantity { get; set; } = 1;
    public decimal UnitPrice { get; set; }
    public decimal Total => Quantity * UnitPrice;
}

public class Invoice
{
    public string Id { get; set; } = Guid.NewGuid().ToString();
    public string InvoiceNumber { get; set; } = "";
    public string CustomerEmail { get; set; } = "";
    public string? CustomerName { get; set; }
    public decimal Amount { get; set; }
    public string Currency { get; set; } = "IGT";
    public string Status { get; set; } = "draft"; // draft, sent, paid, overdue, cancelled
    public string Description { get; set; } = "";
    public List<InvoiceItem>? Items { get; set; }
    public string PaymentUrl { get; set; } = "";
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime? DueDate { get; set; }
    public DateTime? PaidAt { get; set; }
}

public class RecurringPaymentRequest
{
    public string WalletId { get; set; } = "";
    public decimal Amount { get; set; }
    public string Currency { get; set; } = "IGT";
    public string Interval { get; set; } = "monthly"; // daily, weekly, monthly, yearly
    public string ToAddress { get; set; } = "";
    public string? Description { get; set; }
    public DateTime? StartDate { get; set; }
    public DateTime? EndDate { get; set; }
}

public class RecurringPayment
{
    public string Id { get; set; } = Guid.NewGuid().ToString();
    public string WalletId { get; set; } = "";
    public decimal Amount { get; set; }
    public string Currency { get; set; } = "IGT";
    public string Interval { get; set; } = "monthly";
    public string ToAddress { get; set; } = "";
    public string Status { get; set; } = "active"; // active, paused, cancelled
    public DateTime NextPaymentDate { get; set; }
    public DateTime? LastPaymentDate { get; set; }
    public int PaymentCount { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}

public class QRCodeData
{
    public string PaymentId { get; set; } = "";
    public string Address { get; set; } = "";
    public decimal Amount { get; set; }
    public string Currency { get; set; } = "";
    public string QrImage { get; set; } = ""; // Base64
    public string DeepLink { get; set; } = "";
}

// ============= EXCEPTIONS =============

public class InsufficientFundsException : Exception
{
    public InsufficientFundsException() : base("Insufficient funds") { }
    public InsufficientFundsException(string message) : base(message) { }
}

public class WalletNotFoundException : Exception
{
    public WalletNotFoundException() : base("Wallet not found") { }
    public WalletNotFoundException(string message) : base(message) { }
}

public class TransactionFailedException : Exception
{
    public TransactionFailedException() : base("Transaction failed") { }
    public TransactionFailedException(string message) : base(message) { }
}
