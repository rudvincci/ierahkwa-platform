namespace NET10.Core.Interfaces;

// ═══════════════════════════════════════════════════════════════════════════════
// IERAHKWA SOVEREIGN BANK - CENTRAL PAYMENT SYSTEM
// The unified banking infrastructure for all platforms, services and departments
// ═══════════════════════════════════════════════════════════════════════════════

/// <summary>
/// Core Banking Service - Account Management
/// </summary>
public interface IBankAccountService
{
    Task<List<BankAccount>> GetAllAsync();
    Task<BankAccount?> GetByIdAsync(Guid id);
    Task<BankAccount?> GetByAccountNumberAsync(string accountNumber);
    Task<List<BankAccount>> GetByCustomerAsync(Guid customerId);
    Task<List<BankAccount>> GetByDepartmentAsync(string department);
    Task<BankAccount> CreateAsync(BankAccount account);
    Task<BankAccount> UpdateAsync(BankAccount account);
    Task<bool> FreezeAsync(Guid id, string reason);
    Task<bool> UnfreezeAsync(Guid id);
    Task<AccountBalance> GetBalanceAsync(Guid accountId);
    Task<List<AccountBalance>> GetAllBalancesAsync(Guid accountId);
}

/// <summary>
/// Payment Processing Service - All Payment Types
/// </summary>
public interface IPaymentProcessingService
{
    // Transfers
    Task<PaymentTransaction> TransferAsync(TransferPaymentRequest request);
    Task<PaymentTransaction> InternalTransferAsync(InternalTransferRequest request);
    Task<PaymentTransaction> ExternalTransferAsync(ExternalTransferRequest request);
    
    // Payments
    Task<PaymentTransaction> ProcessPaymentAsync(PaymentRequest request);
    Task<PaymentTransaction> ProcessBulkPaymentAsync(BulkPaymentRequest request);
    Task<PaymentTransaction> ProcessRecurringPaymentAsync(RecurringPaymentRequest request);
    
    // Refunds
    Task<PaymentTransaction> RefundAsync(Guid transactionId, decimal? amount = null);
    Task<PaymentTransaction> PartialRefundAsync(Guid transactionId, decimal amount);
    
    // Status
    Task<PaymentTransaction?> GetTransactionAsync(Guid id);
    Task<PaymentTransaction?> GetByReferenceAsync(string reference);
    Task<List<PaymentTransaction>> GetTransactionsAsync(TransactionFilter filter);
}

/// <summary>
/// Department Payment Service - Handles payments between departments
/// </summary>
public interface IDepartmentPaymentService
{
    Task<DepartmentPayment> RequestBudgetAsync(BudgetRequest request);
    Task<DepartmentPayment> ApproveBudgetAsync(Guid requestId, string approver);
    Task<DepartmentPayment> RejectBudgetAsync(Guid requestId, string reason);
    Task<DepartmentPayment> DisburseAsync(Guid requestId);
    Task<List<DepartmentPayment>> GetPendingApprovalsAsync();
    Task<List<DepartmentPayment>> GetDepartmentHistoryAsync(string department);
    Task<DepartmentBudget> GetBudgetStatusAsync(string department);
}

/// <summary>
/// Platform Integration Service - Connect all platforms to bank
/// </summary>
public interface IPlatformPaymentService
{
    // ERP Payments
    Task<PaymentTransaction> ProcessInvoicePaymentAsync(Guid invoiceId, PaymentMethod method);
    Task<PaymentTransaction> ProcessSupplierPaymentAsync(Guid supplierId, decimal amount);
    Task<PaymentTransaction> ProcessPayrollAsync(PayrollRequest request);
    
    // College Payments
    Task<PaymentTransaction> ProcessTuitionPaymentAsync(Guid studentId, decimal amount);
    Task<PaymentTransaction> ProcessScholarshipDisbursementAsync(Guid studentId, decimal amount);
    Task<PaymentTransaction> ProcessTeacherSalaryAsync(Guid teacherId);
    
    // Hotel/Property Payments
    Task<PaymentTransaction> ProcessBookingPaymentAsync(Guid bookingId, decimal amount);
    Task<PaymentTransaction> ProcessRefundAsync(Guid bookingId);
    Task<PaymentTransaction> ProcessPropertyRentAsync(Guid leaseId);
    
    // DeFi Integration
    Task<PaymentTransaction> ProcessCryptoDepositAsync(CryptoDepositRequest request);
    Task<PaymentTransaction> ProcessCryptoWithdrawalAsync(CryptoWithdrawalRequest request);
    Task<PaymentTransaction> ProcessSwapSettlementAsync(Guid swapId);
    
    // Government Services
    Task<PaymentTransaction> ProcessTaxPaymentAsync(TaxPaymentRequest request);
    Task<PaymentTransaction> ProcessLicenseFeeAsync(Guid licenseId, decimal amount);
    Task<PaymentTransaction> ProcessFinePaymentAsync(Guid fineId);
    Task<PaymentTransaction> ProcessBenefitDisbursementAsync(BenefitRequest request);
}

/// <summary>
/// Treasury Service - Central Treasury Management
/// </summary>
public interface ITreasuryService
{
    Task<TreasuryOverview> GetOverviewAsync();
    Task<List<TreasuryAccount>> GetAccountsAsync();
    Task<TreasuryAccount?> GetAccountAsync(string accountType);
    Task<TreasuryTransaction> AllocateFundsAsync(AllocationRequest request);
    Task<TreasuryTransaction> RecallFundsAsync(RecallRequest request);
    Task<List<TreasuryTransaction>> GetTransactionsAsync(DateTime from, DateTime to);
    Task<TreasuryReport> GenerateReportAsync(ReportPeriod period);
    Task<List<BudgetAllocation>> GetBudgetAllocationsAsync(int fiscalYear);
}

/// <summary>
/// Reconciliation Service - Balance verification and auditing
/// </summary>
public interface IReconciliationService
{
    Task<ReconciliationResult> ReconcileAccountAsync(Guid accountId, DateTime date);
    Task<ReconciliationResult> ReconcileDepartmentAsync(string department, DateTime date);
    Task<ReconciliationResult> ReconcilePlatformAsync(string platform, DateTime date);
    Task<List<ReconciliationDiscrepancy>> GetDiscrepanciesAsync();
    Task<bool> ResolveDiscrepancyAsync(Guid discrepancyId, string resolution);
    Task<AuditTrail> GetAuditTrailAsync(Guid transactionId);
}

/// <summary>
/// Multi-Currency Service
/// </summary>
public interface IMultiCurrencyService
{
    Task<List<Currency>> GetSupportedCurrenciesAsync();
    Task<ExchangeRate> GetExchangeRateAsync(string fromCurrency, string toCurrency);
    Task<List<ExchangeRate>> GetAllRatesAsync(string baseCurrency);
    Task<CurrencyConversion> ConvertAsync(decimal amount, string from, string to);
    Task<bool> UpdateRatesAsync(); // Fetch latest rates
}

// ═══════════════════════════════════════════════════════════════════════════════
// BANKING MODELS
// ═══════════════════════════════════════════════════════════════════════════════

public class BankAccount
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public string AccountNumber { get; set; } = string.Empty;
    public string AccountName { get; set; } = string.Empty;
    public AccountType Type { get; set; }
    public AccountCategory Category { get; set; }
    public Guid? CustomerId { get; set; }
    public string? Department { get; set; }
    public string? Platform { get; set; }
    public string PrimaryCurrency { get; set; } = "USD";
    public List<AccountBalance> Balances { get; set; } = new();
    public AccountStatus Status { get; set; } = AccountStatus.Active;
    public DateTime OpenedAt { get; set; } = DateTime.UtcNow;
    public DateTime? ClosedAt { get; set; }
    public string? WalletAddress { get; set; }
    public bool IsMultiCurrency { get; set; } = true;
    public decimal DailyLimit { get; set; } = 100_000;
    public decimal MonthlyLimit { get; set; } = 1_000_000;
}

public enum AccountType
{
    Checking,
    Savings,
    Treasury,
    Escrow,
    Operating,
    Payroll,
    Revenue,
    Reserve,
    Investment
}

public enum AccountCategory
{
    Personal,
    Business,
    Department,
    Platform,
    Treasury,
    External
}

public enum AccountStatus
{
    Active,
    Frozen,
    Suspended,
    Closed,
    PendingApproval
}

public class AccountBalance
{
    public string Currency { get; set; } = "USD";
    public decimal Available { get; set; }
    public decimal Pending { get; set; }
    public decimal Locked { get; set; }
    public decimal Total => Available + Pending + Locked;
    public DateTime LastUpdated { get; set; } = DateTime.UtcNow;
}

public class PaymentTransaction
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public string Reference { get; set; } = string.Empty;
    public TransactionType Type { get; set; }
    public PaymentMethod Method { get; set; }
    
    // Accounts
    public Guid? SourceAccountId { get; set; }
    public string? SourceAccountNumber { get; set; }
    public Guid? DestinationAccountId { get; set; }
    public string? DestinationAccountNumber { get; set; }
    
    // Amount
    public decimal Amount { get; set; }
    public string Currency { get; set; } = "USD";
    public decimal? ExchangeRate { get; set; }
    public decimal? ConvertedAmount { get; set; }
    public string? DestinationCurrency { get; set; }
    
    // Fees
    public decimal Fee { get; set; }
    public decimal NetAmount => Amount - Fee;
    
    // Status
    public TransactionStatus Status { get; set; } = TransactionStatus.Pending;
    public string? StatusMessage { get; set; }
    
    // Metadata
    public string? Description { get; set; }
    public string? Category { get; set; }
    public string? Department { get; set; }
    public string? Platform { get; set; }
    public Guid? RelatedEntityId { get; set; }
    public string? RelatedEntityType { get; set; }
    
    // Audit
    public string? InitiatedBy { get; set; }
    public string? ApprovedBy { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime? ProcessedAt { get; set; }
    public DateTime? CompletedAt { get; set; }
    
    // External
    public string? ExternalReference { get; set; }
    public string? ExternalProvider { get; set; }
}

public enum TransactionType
{
    Deposit,
    Withdrawal,
    Transfer,
    Payment,
    Refund,
    Fee,
    Interest,
    Adjustment,
    Payroll,
    TaxPayment,
    BudgetAllocation,
    DeFiSettlement
}

public enum PaymentMethod
{
    BankTransfer,
    InternalTransfer,
    ACH,
    Wire,
    Crypto,
    Card,
    Cash,
    Check,
    DirectDebit,
    StandingOrder
}

public enum TransactionStatus
{
    Pending,
    Processing,
    Completed,
    Failed,
    Cancelled,
    Reversed,
    OnHold,
    RequiresApproval
}

// Payment Requests
public class TransferPaymentRequest
{
    public Guid SourceAccountId { get; set; }
    public Guid DestinationAccountId { get; set; }
    public decimal Amount { get; set; }
    public string Currency { get; set; } = "USD";
    public string? Description { get; set; }
    public string? Reference { get; set; }
    public string InitiatedBy { get; set; } = string.Empty;
}

public class InternalTransferRequest : TransferPaymentRequest
{
    public string? Department { get; set; }
    public string? Category { get; set; }
}

public class ExternalTransferRequest : TransferPaymentRequest
{
    public string BankCode { get; set; } = string.Empty;
    public string RoutingNumber { get; set; } = string.Empty;
    public string AccountHolder { get; set; } = string.Empty;
    public TransferSpeed Speed { get; set; } = TransferSpeed.Standard;
}

public enum TransferSpeed
{
    Standard,  // 1-3 days
    Express,   // Same day
    Instant    // Immediate
}

public class PaymentRequest
{
    public Guid PayerAccountId { get; set; }
    public string PayeeIdentifier { get; set; } = string.Empty;
    public decimal Amount { get; set; }
    public string Currency { get; set; } = "USD";
    public PaymentMethod Method { get; set; }
    public string? Description { get; set; }
    public Guid? RelatedEntityId { get; set; }
    public string? RelatedEntityType { get; set; }
}

public class BulkPaymentRequest
{
    public Guid SourceAccountId { get; set; }
    public List<BulkPaymentItem> Payments { get; set; } = new();
    public string Description { get; set; } = string.Empty;
    public DateTime? ScheduledDate { get; set; }
}

public class BulkPaymentItem
{
    public string DestinationAccount { get; set; } = string.Empty;
    public decimal Amount { get; set; }
    public string? Reference { get; set; }
}

public class RecurringPaymentRequest
{
    public Guid SourceAccountId { get; set; }
    public Guid DestinationAccountId { get; set; }
    public decimal Amount { get; set; }
    public RecurringFrequency Frequency { get; set; }
    public DateTime StartDate { get; set; }
    public DateTime? EndDate { get; set; }
    public string Description { get; set; } = string.Empty;
}

public enum RecurringFrequency
{
    Daily,
    Weekly,
    BiWeekly,
    Monthly,
    Quarterly,
    Annually
}

public class TransactionFilter
{
    public Guid? AccountId { get; set; }
    public string? Department { get; set; }
    public string? Platform { get; set; }
    public TransactionType? Type { get; set; }
    public TransactionStatus? Status { get; set; }
    public DateTime? FromDate { get; set; }
    public DateTime? ToDate { get; set; }
    public decimal? MinAmount { get; set; }
    public decimal? MaxAmount { get; set; }
    public int Page { get; set; } = 1;
    public int PageSize { get; set; } = 50;
}

// Department Payments
public class DepartmentPayment
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public string Department { get; set; } = string.Empty;
    public string RequestedBy { get; set; } = string.Empty;
    public decimal Amount { get; set; }
    public string Purpose { get; set; } = string.Empty;
    public string? BudgetCode { get; set; }
    public DepartmentPaymentStatus Status { get; set; } = DepartmentPaymentStatus.Pending;
    public string? ApprovedBy { get; set; }
    public DateTime? ApprovedAt { get; set; }
    public string? RejectionReason { get; set; }
    public Guid? TransactionId { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}

public enum DepartmentPaymentStatus
{
    Pending,
    Approved,
    Rejected,
    Disbursed,
    Cancelled
}

public class BudgetRequest
{
    public string Department { get; set; } = string.Empty;
    public decimal Amount { get; set; }
    public string Purpose { get; set; } = string.Empty;
    public string? BudgetCode { get; set; }
    public string RequestedBy { get; set; } = string.Empty;
    public List<string>? SupportingDocuments { get; set; }
}

public class DepartmentBudget
{
    public string Department { get; set; } = string.Empty;
    public int FiscalYear { get; set; }
    public decimal AllocatedBudget { get; set; }
    public decimal UsedBudget { get; set; }
    public decimal RemainingBudget => AllocatedBudget - UsedBudget;
    public decimal PendingRequests { get; set; }
    public decimal AvailableBudget => RemainingBudget - PendingRequests;
    public List<BudgetLineItem> LineItems { get; set; } = new();
}

public class BudgetLineItem
{
    public string Code { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public decimal Allocated { get; set; }
    public decimal Used { get; set; }
    public decimal Remaining => Allocated - Used;
}

// Platform Payment Requests
public class PayrollRequest
{
    public string Department { get; set; } = string.Empty;
    public string PayPeriod { get; set; } = string.Empty;
    public List<PayrollItem> Items { get; set; } = new();
    public decimal TotalAmount => Items.Sum(i => i.NetPay);
}

public class PayrollItem
{
    public Guid EmployeeId { get; set; }
    public string EmployeeName { get; set; } = string.Empty;
    public Guid BankAccountId { get; set; }
    public decimal GrossPay { get; set; }
    public decimal Deductions { get; set; }
    public decimal NetPay => GrossPay - Deductions;
}

public class CryptoDepositRequest
{
    public Guid AccountId { get; set; }
    public string WalletAddress { get; set; } = string.Empty;
    public string TokenSymbol { get; set; } = string.Empty;
    public decimal Amount { get; set; }
    public string TransactionHash { get; set; } = string.Empty;
    public int ChainId { get; set; }
}

public class CryptoWithdrawalRequest
{
    public Guid AccountId { get; set; }
    public string DestinationWallet { get; set; } = string.Empty;
    public string TokenSymbol { get; set; } = string.Empty;
    public decimal Amount { get; set; }
    public int ChainId { get; set; }
}

public class TaxPaymentRequest
{
    public Guid PayerAccountId { get; set; }
    public string TaxType { get; set; } = string.Empty;
    public string TaxPeriod { get; set; } = string.Empty;
    public decimal Amount { get; set; }
    public string? TaxId { get; set; }
}

public class BenefitRequest
{
    public Guid BeneficiaryId { get; set; }
    public string BenefitType { get; set; } = string.Empty;
    public decimal Amount { get; set; }
    public string Period { get; set; } = string.Empty;
}

// Treasury Models
public class TreasuryOverview
{
    public decimal TotalAssets { get; set; }
    public decimal TotalLiabilities { get; set; }
    public decimal NetPosition => TotalAssets - TotalLiabilities;
    public decimal CashOnHand { get; set; }
    public decimal Investments { get; set; }
    public decimal Receivables { get; set; }
    public decimal Payables { get; set; }
    public List<TreasuryAccount> Accounts { get; set; } = new();
    public DateTime AsOf { get; set; } = DateTime.UtcNow;
}

public class TreasuryAccount
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public string Name { get; set; } = string.Empty;
    public string Type { get; set; } = string.Empty;
    public decimal Balance { get; set; }
    public string Currency { get; set; } = "USD";
    public string Purpose { get; set; } = string.Empty;
}

public class TreasuryTransaction
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public string Type { get; set; } = string.Empty;
    public Guid FromAccountId { get; set; }
    public Guid ToAccountId { get; set; }
    public decimal Amount { get; set; }
    public string Purpose { get; set; } = string.Empty;
    public string ApprovedBy { get; set; } = string.Empty;
    public DateTime ProcessedAt { get; set; } = DateTime.UtcNow;
}

public class AllocationRequest
{
    public string SourceAccount { get; set; } = string.Empty;
    public string DestinationAccount { get; set; } = string.Empty;
    public decimal Amount { get; set; }
    public string Purpose { get; set; } = string.Empty;
    public string AuthorizedBy { get; set; } = string.Empty;
}

public class RecallRequest
{
    public Guid AllocationId { get; set; }
    public decimal Amount { get; set; }
    public string Reason { get; set; } = string.Empty;
    public string AuthorizedBy { get; set; } = string.Empty;
}

public class TreasuryReport
{
    public ReportPeriod Period { get; set; }
    public DateTime GeneratedAt { get; set; } = DateTime.UtcNow;
    public decimal OpeningBalance { get; set; }
    public decimal ClosingBalance { get; set; }
    public decimal TotalInflows { get; set; }
    public decimal TotalOutflows { get; set; }
    public List<TreasuryTransaction> Transactions { get; set; } = new();
    public Dictionary<string, decimal> ByDepartment { get; set; } = new();
    public Dictionary<string, decimal> ByPlatform { get; set; } = new();
}

public enum ReportPeriod
{
    Daily,
    Weekly,
    Monthly,
    Quarterly,
    Yearly
}

public class BudgetAllocation
{
    public string Department { get; set; } = string.Empty;
    public int FiscalYear { get; set; }
    public decimal Allocated { get; set; }
    public decimal Spent { get; set; }
    public decimal Remaining => Allocated - Spent;
    public decimal PercentUsed => Allocated > 0 ? (Spent / Allocated) * 100 : 0;
}

// Reconciliation Models
public class ReconciliationResult
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public string EntityType { get; set; } = string.Empty;
    public string EntityId { get; set; } = string.Empty;
    public DateTime ReconciliationDate { get; set; }
    public decimal ExpectedBalance { get; set; }
    public decimal ActualBalance { get; set; }
    public decimal Difference => ActualBalance - ExpectedBalance;
    public bool IsBalanced => Math.Abs(Difference) < 0.01m;
    public List<ReconciliationDiscrepancy> Discrepancies { get; set; } = new();
    public DateTime PerformedAt { get; set; } = DateTime.UtcNow;
}

public class ReconciliationDiscrepancy
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public Guid? TransactionId { get; set; }
    public string Description { get; set; } = string.Empty;
    public decimal Amount { get; set; }
    public DiscrepancyType Type { get; set; }
    public DiscrepancyStatus Status { get; set; } = DiscrepancyStatus.Open;
    public string? Resolution { get; set; }
    public DateTime? ResolvedAt { get; set; }
}

public enum DiscrepancyType
{
    MissingTransaction,
    DuplicateTransaction,
    AmountMismatch,
    TimingDifference,
    UnauthorizedTransaction,
    Other
}

public enum DiscrepancyStatus
{
    Open,
    UnderReview,
    Resolved,
    WrittenOff
}

public class AuditTrail
{
    public Guid TransactionId { get; set; }
    public List<AuditEntry> Entries { get; set; } = new();
}

public class AuditEntry
{
    public DateTime Timestamp { get; set; }
    public string Action { get; set; } = string.Empty;
    public string PerformedBy { get; set; } = string.Empty;
    public string? Details { get; set; }
    public string? IpAddress { get; set; }
}

// Currency Models
public class Currency
{
    public string Code { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public string Symbol { get; set; } = string.Empty;
    public int Decimals { get; set; } = 2;
    public bool IsCrypto { get; set; }
    public bool IsActive { get; set; } = true;
}

public class ExchangeRate
{
    public string FromCurrency { get; set; } = string.Empty;
    public string ToCurrency { get; set; } = string.Empty;
    public decimal Rate { get; set; }
    public decimal InverseRate => Rate > 0 ? 1 / Rate : 0;
    public DateTime ValidFrom { get; set; }
    public DateTime? ValidTo { get; set; }
    public string Source { get; set; } = "Internal";
}

public class CurrencyConversion
{
    public decimal OriginalAmount { get; set; }
    public string OriginalCurrency { get; set; } = string.Empty;
    public decimal ConvertedAmount { get; set; }
    public string TargetCurrency { get; set; } = string.Empty;
    public decimal ExchangeRate { get; set; }
    public DateTime ConvertedAt { get; set; } = DateTime.UtcNow;
}
