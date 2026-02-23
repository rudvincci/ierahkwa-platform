namespace IERAHKWA.Platform.Models;

// ============= BANK ACCOUNT MODELS =============

public class BankAccount
{
    public string Id { get; set; } = Guid.NewGuid().ToString();
    public string AccountNumber { get; set; } = "";
    public string IBAN { get; set; } = "";
    public string UserId { get; set; } = "";
    public string Name { get; set; } = "";
    public string Type { get; set; } = "checking"; // checking, savings, business, treasury
    public string Currency { get; set; } = "USD";
    public decimal Balance { get; set; }
    public decimal AvailableBalance { get; set; }
    public decimal PendingBalance { get; set; }
    public string Status { get; set; } = "active"; // active, frozen, closed
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime? LastActivityAt { get; set; }
    public BankAccountLimits Limits { get; set; } = new();
}

public class BankAccountLimits
{
    public decimal DailyTransferLimit { get; set; } = 100000;
    public decimal MonthlyTransferLimit { get; set; } = 1000000;
    public decimal SingleTransactionLimit { get; set; } = 50000;
    public decimal DailyWithdrawalLimit { get; set; } = 10000;
}

public class CreateAccountRequest
{
    public string UserId { get; set; } = "";
    public string Name { get; set; } = "";
    public string Type { get; set; } = "checking";
    public string Currency { get; set; } = "USD";
    public decimal InitialDeposit { get; set; }
}

public class InternalTransferRequest
{
    public string FromAccountId { get; set; } = "";
    public string ToAccountId { get; set; } = "";
    public decimal Amount { get; set; }
    public string Currency { get; set; } = "USD";
    public string? Reference { get; set; }
    public string? Description { get; set; }
}

public class SwiftTransferRequest
{
    public string FromAccountId { get; set; } = "";
    public string BeneficiaryName { get; set; } = "";
    public string BeneficiaryIBAN { get; set; } = "";
    public string BeneficiaryBankSwift { get; set; } = "";
    public string BeneficiaryBankName { get; set; } = "";
    public string BeneficiaryBankAddress { get; set; } = "";
    public string BeneficiaryCountry { get; set; } = "";
    public decimal Amount { get; set; }
    public string Currency { get; set; } = "USD";
    public string? Reference { get; set; }
    public string? Purpose { get; set; }
    public string ChargeType { get; set; } = "SHA"; // SHA, OUR, BEN
}

public class BankTransfer
{
    public string Id { get; set; } = Guid.NewGuid().ToString();
    public string TransferNumber { get; set; } = "";
    public string FromAccountId { get; set; } = "";
    public string ToAccountId { get; set; } = "";
    public string? ToIBAN { get; set; }
    public string? ToSwift { get; set; }
    public string BeneficiaryName { get; set; } = "";
    public decimal Amount { get; set; }
    public decimal Fee { get; set; }
    public string Currency { get; set; } = "USD";
    public string Type { get; set; } = "internal"; // internal, swift, sepa, wire
    public string Status { get; set; } = "pending"; // pending, processing, completed, failed, cancelled
    public string? Reference { get; set; }
    public string? Description { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime? CompletedAt { get; set; }
    public string? MT103Reference { get; set; }
}

public class ExchangeRequest
{
    public string AccountId { get; set; } = "";
    public decimal Amount { get; set; }
    public string FromCurrency { get; set; } = "USD";
    public string ToCurrency { get; set; } = "EUR";
}

public class ExchangeResult
{
    public string Id { get; set; } = Guid.NewGuid().ToString();
    public decimal FromAmount { get; set; }
    public string FromCurrency { get; set; } = "";
    public decimal ToAmount { get; set; }
    public string ToCurrency { get; set; } = "";
    public decimal Rate { get; set; }
    public decimal Fee { get; set; }
    public DateTime ExecutedAt { get; set; } = DateTime.UtcNow;
}

public class ExchangeRate
{
    public string FromCurrency { get; set; } = "";
    public string ToCurrency { get; set; } = "";
    public decimal Rate { get; set; }
    public decimal Spread { get; set; }
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
}

public class BankMovement
{
    public string Id { get; set; } = Guid.NewGuid().ToString();
    public string AccountId { get; set; } = "";
    public string Type { get; set; } = ""; // credit, debit
    public decimal Amount { get; set; }
    public string Currency { get; set; } = "USD";
    public decimal BalanceAfter { get; set; }
    public string Description { get; set; } = "";
    public string? Reference { get; set; }
    public string? TransferId { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}

public class AccountStatement
{
    public string AccountId { get; set; } = "";
    public string AccountNumber { get; set; } = "";
    public string AccountName { get; set; } = "";
    public DateTime FromDate { get; set; }
    public DateTime ToDate { get; set; }
    public decimal OpeningBalance { get; set; }
    public decimal ClosingBalance { get; set; }
    public decimal TotalCredits { get; set; }
    public decimal TotalDebits { get; set; }
    public List<BankMovement> Movements { get; set; } = new();
    public DateTime GeneratedAt { get; set; } = DateTime.UtcNow;
}

public class BankInfo
{
    public string Name { get; set; } = "BDET Central Bank";
    public string FullName { get; set; } = "Banco de Desarrollo Economico Territorial";
    public string SwiftCode { get; set; } = "IERBDETXXX";
    public string Country { get; set; } = "Ierahkwa Ne Kanienke";
    public string[] SupportedCurrencies { get; set; } = { "USD", "EUR", "GBP", "IGT", "BTC", "ETH", "USDT" };
    public BankFees Fees { get; set; } = new();
    public BankLimits Limits { get; set; } = new();
}

public class BankFees
{
    public decimal InternalTransfer { get; set; } = 0;
    public decimal SwiftTransfer { get; set; } = 25;
    public decimal SepaTransfer { get; set; } = 5;
    public decimal WireTransfer { get; set; } = 35;
    public decimal ExchangeSpread { get; set; } = 0.005m;
    public decimal CryptoWithdrawal { get; set; } = 0.001m;
}

public class BankLimits
{
    public decimal MinTransfer { get; set; } = 1;
    public decimal MaxSingleTransfer { get; set; } = 10000000;
    public decimal DailyLimit { get; set; } = 50000000;
}

// ============= CHECKOUT MODELS =============

public class CheckoutSession
{
    public string Id { get; set; } = Guid.NewGuid().ToString();
    public string MerchantId { get; set; } = "";
    public decimal Amount { get; set; }
    public string Currency { get; set; } = "USD";
    public string? Description { get; set; }
    public string Status { get; set; } = "pending"; // pending, processing, completed, expired, cancelled
    public List<CheckoutItem>? Items { get; set; }
    public string? CustomerEmail { get; set; }
    public string? CustomerName { get; set; }
    public string? SuccessUrl { get; set; }
    public string? CancelUrl { get; set; }
    public string? WebhookUrl { get; set; }
    public string? PaymentMethod { get; set; }
    public string? TransactionId { get; set; }
    public string PaymentUrl { get; set; } = "";
    public string EmbedUrl { get; set; } = "";
    public Dictionary<string, string>? Metadata { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime ExpiresAt { get; set; }
    public DateTime? PaidAt { get; set; }
}

public class CheckoutItem
{
    public string Name { get; set; } = "";
    public string? Description { get; set; }
    public int Quantity { get; set; } = 1;
    public decimal UnitPrice { get; set; }
    public string? ImageUrl { get; set; }
}

public class CheckoutSessionRequest
{
    public decimal Amount { get; set; }
    public string Currency { get; set; } = "USD";
    public string? Description { get; set; }
    public List<CheckoutItem>? Items { get; set; }
    public string? CustomerEmail { get; set; }
    public string? CustomerName { get; set; }
    public string? SuccessUrl { get; set; }
    public string? CancelUrl { get; set; }
    public string? WebhookUrl { get; set; }
    public Dictionary<string, string>? Metadata { get; set; }
    public int ExpiresInMinutes { get; set; } = 30;
}

public class CheckoutPaymentRequest
{
    public string PaymentMethod { get; set; } = "crypto"; // crypto, card, bank
    public string? CardNumber { get; set; }
    public string? CardExpiry { get; set; }
    public string? CardCvc { get; set; }
    public string? WalletAddress { get; set; }
    public string? BankAccountId { get; set; }
}

public class CheckoutResult
{
    public string SessionId { get; set; } = "";
    public string Status { get; set; } = "";
    public string? TransactionId { get; set; }
    public string? TxHash { get; set; }
    public decimal AmountPaid { get; set; }
    public string Currency { get; set; } = "";
    public DateTime? PaidAt { get; set; }
    public string? Receipt { get; set; }
}

// ============= MERCHANT MODELS =============

public class Merchant
{
    public string Id { get; set; } = Guid.NewGuid().ToString();
    public string Name { get; set; } = "";
    public string Email { get; set; } = "";
    public string ApiKey { get; set; } = "";
    public string SecretKey { get; set; } = "";
    public string WebhookSecret { get; set; } = "";
    public string? WebhookUrl { get; set; }
    public string Status { get; set; } = "active";
    public MerchantSettings Settings { get; set; } = new();
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}

public class MerchantSettings
{
    public string[] AcceptedCurrencies { get; set; } = { "USD", "EUR", "IGT", "USDT" };
    public string[] AcceptedPaymentMethods { get; set; } = { "crypto", "card", "bank" };
    public decimal FeePercent { get; set; } = 2.9m;
    public decimal FixedFee { get; set; } = 0.30m;
    public bool AutoPayout { get; set; } = true;
    public string PayoutCurrency { get; set; } = "USD";
    public string? PayoutAccountId { get; set; }
}

// ============= SUBSCRIPTION MODELS =============

public class Subscription
{
    public string Id { get; set; } = Guid.NewGuid().ToString();
    public string CustomerId { get; set; } = "";
    public string PlanId { get; set; } = "";
    public string Status { get; set; } = "active"; // active, paused, cancelled, expired
    public decimal Amount { get; set; }
    public string Currency { get; set; } = "USD";
    public string Interval { get; set; } = "monthly"; // daily, weekly, monthly, yearly
    public DateTime CurrentPeriodStart { get; set; }
    public DateTime CurrentPeriodEnd { get; set; }
    public DateTime? CancelledAt { get; set; }
    public int PaymentCount { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}

public class SubscriptionPlan
{
    public string Id { get; set; } = Guid.NewGuid().ToString();
    public string Name { get; set; } = "";
    public string? Description { get; set; }
    public decimal Amount { get; set; }
    public string Currency { get; set; } = "USD";
    public string Interval { get; set; } = "monthly";
    public int? TrialDays { get; set; }
    public bool IsActive { get; set; } = true;
}

// ============= COUPON MODELS =============

public class Coupon
{
    public string Id { get; set; } = Guid.NewGuid().ToString();
    public string Code { get; set; } = "";
    public string Type { get; set; } = "percent"; // percent, fixed
    public decimal Value { get; set; }
    public string? Currency { get; set; }
    public decimal? MinAmount { get; set; }
    public decimal? MaxDiscount { get; set; }
    public int? MaxUses { get; set; }
    public int UsedCount { get; set; }
    public DateTime? ExpiresAt { get; set; }
    public bool IsActive { get; set; } = true;
}
