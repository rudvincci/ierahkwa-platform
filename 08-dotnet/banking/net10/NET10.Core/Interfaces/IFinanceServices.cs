namespace NET10.Core.Interfaces;

// ═══════════════════════════════════════════════════════════════════════════════
// IERAHKWA GOMONEY - PERSONAL FINANCIAL MANAGEMENT SYSTEM
// Complete Income, Expense & Account Management
// ═══════════════════════════════════════════════════════════════════════════════

/// <summary>
/// Financial Account Management Service
/// </summary>
public interface IFinancialAccountService
{
    Task<List<FinancialAccount>> GetAllAsync(Guid userId);
    Task<FinancialAccount?> GetByIdAsync(Guid id);
    Task<FinancialAccount> CreateAsync(FinancialAccount account);
    Task<FinancialAccount> UpdateAsync(FinancialAccount account);
    Task<bool> DeleteAsync(Guid id);
    Task<decimal> GetBalanceAsync(Guid accountId);
    Task<decimal> GetTotalBalanceAsync(Guid userId);
    Task<AccountTransfer> TransferAsync(TransferRequest request);
    Task<List<FinancialAccount>> GetByTypeAsync(Guid userId, FinancialAccountType type);
}

/// <summary>
/// Income Category Management Service
/// </summary>
public interface IIncomeCategoryService
{
    Task<List<IncomeCategory>> GetAllAsync(Guid userId);
    Task<IncomeCategory?> GetByIdAsync(Guid id);
    Task<IncomeCategory> CreateAsync(IncomeCategory category);
    Task<IncomeCategory> UpdateAsync(IncomeCategory category);
    Task<bool> DeleteAsync(Guid id);
}

/// <summary>
/// Expense Category Management Service
/// </summary>
public interface IExpenseCategoryService
{
    Task<List<ExpenseCategory>> GetAllAsync(Guid userId);
    Task<ExpenseCategory?> GetByIdAsync(Guid id);
    Task<ExpenseCategory> CreateAsync(ExpenseCategory category);
    Task<ExpenseCategory> UpdateAsync(ExpenseCategory category);
    Task<bool> DeleteAsync(Guid id);
}

/// <summary>
/// Transaction Management Service (Income & Expense)
/// </summary>
public interface IFinancialTransactionService
{
    Task<List<FinancialTransaction>> GetAllAsync(Guid userId);
    Task<List<FinancialTransaction>> GetByAccountAsync(Guid accountId);
    Task<List<FinancialTransaction>> GetByDateRangeAsync(Guid userId, DateTime from, DateTime to);
    Task<List<FinancialTransaction>> GetByTypeAsync(Guid userId, FinancialTransactionType type);
    Task<List<FinancialTransaction>> GetByCategoryAsync(Guid categoryId);
    Task<FinancialTransaction?> GetByIdAsync(Guid id);
    Task<FinancialTransaction> CreateAsync(FinancialTransaction transaction);
    Task<FinancialTransaction> UpdateAsync(FinancialTransaction transaction);
    Task<bool> DeleteAsync(Guid id);
    Task<FinancialTransaction> QuickIncomeAsync(QuickTransactionRequest request);
    Task<FinancialTransaction> QuickExpenseAsync(QuickTransactionRequest request);
    Task<List<FinancialTransaction>> GetRecentAsync(Guid userId, int count = 10);
}

/// <summary>
/// Payable & Receivable Management Service
/// </summary>
public interface IPayableReceivableService
{
    // Accounts Payable (money you owe)
    Task<List<AccountPayable>> GetPayablesAsync(Guid userId);
    Task<AccountPayable?> GetPayableAsync(Guid id);
    Task<AccountPayable> CreatePayableAsync(AccountPayable payable);
    Task<AccountPayable> UpdatePayableAsync(AccountPayable payable);
    Task<AccountPayable> PayAsync(Guid payableId, decimal amount, Guid fromAccountId);
    Task<List<AccountPayable>> GetOverduePayablesAsync(Guid userId);
    
    // Accounts Receivable (money owed to you)
    Task<List<AccountReceivable>> GetReceivablesAsync(Guid userId);
    Task<AccountReceivable?> GetReceivableAsync(Guid id);
    Task<AccountReceivable> CreateReceivableAsync(AccountReceivable receivable);
    Task<AccountReceivable> UpdateReceivableAsync(AccountReceivable receivable);
    Task<AccountReceivable> ReceiveAsync(Guid receivableId, decimal amount, Guid toAccountId);
    Task<List<AccountReceivable>> GetOverdueReceivablesAsync(Guid userId);
}

/// <summary>
/// Financial Reports Service
/// </summary>
public interface IFinancialReportService
{
    Task<FinancialDashboard> GetDashboardAsync(Guid userId);
    Task<IncomeReport> GetIncomeReportAsync(Guid userId, DateTime from, DateTime to);
    Task<ExpenseReport> GetExpenseReportAsync(Guid userId, DateTime from, DateTime to);
    Task<ProfitLossStatement> GetProfitLossAsync(Guid userId, DateTime from, DateTime to);
    Task<CashFlowReport> GetCashFlowAsync(Guid userId, DateTime from, DateTime to);
    Task<BudgetReport> GetBudgetReportAsync(Guid userId, DateTime month);
    Task<NetWorthReport> GetNetWorthAsync(Guid userId);
}

/// <summary>
/// Budget Management Service
/// </summary>
public interface IBudgetService
{
    Task<List<Budget>> GetAllAsync(Guid userId);
    Task<Budget?> GetByIdAsync(Guid id);
    Task<Budget?> GetByCategoryAsync(Guid categoryId, DateTime month);
    Task<Budget> CreateAsync(Budget budget);
    Task<Budget> UpdateAsync(Budget budget);
    Task<bool> DeleteAsync(Guid id);
    Task<BudgetProgress> GetProgressAsync(Guid budgetId);
    Task<List<BudgetAlert>> GetAlertsAsync(Guid userId);
}

// ═══════════════════════════════════════════════════════════════════════════════
// FINANCIAL MODELS
// ═══════════════════════════════════════════════════════════════════════════════

public class FinancialAccount
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public Guid UserId { get; set; }
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    public FinancialAccountType Type { get; set; } = FinancialAccountType.Cash;
    public string Currency { get; set; } = "USD";
    public decimal Balance { get; set; }
    public decimal InitialBalance { get; set; }
    public string? AccountNumber { get; set; }
    public string? BankName { get; set; }
    public string? Icon { get; set; }
    public string? Color { get; set; }
    public bool IsActive { get; set; } = true;
    public bool IncludeInTotal { get; set; } = true;
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime? LastTransactionAt { get; set; }
}

public enum FinancialAccountType
{
    Cash,
    Checking,
    Savings,
    CreditCard,
    Investment,
    Loan,
    Crypto,
    EWallet,
    Other
}

public class IncomeCategory
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public Guid UserId { get; set; }
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    public string? Icon { get; set; }
    public string? Color { get; set; }
    public Guid? ParentId { get; set; }
    public bool IsSystem { get; set; }
    public bool IsActive { get; set; } = true;
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}

public class ExpenseCategory
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public Guid UserId { get; set; }
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    public string? Icon { get; set; }
    public string? Color { get; set; }
    public Guid? ParentId { get; set; }
    public decimal? BudgetAmount { get; set; }
    public bool IsSystem { get; set; }
    public bool IsActive { get; set; } = true;
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}

public class FinancialTransaction
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public Guid UserId { get; set; }
    public Guid AccountId { get; set; }
    public string AccountName { get; set; } = string.Empty;
    public FinancialTransactionType Type { get; set; }
    public Guid? CategoryId { get; set; }
    public string? CategoryName { get; set; }
    public decimal Amount { get; set; }
    public string Currency { get; set; } = "USD";
    public DateTime Date { get; set; }
    public string? Description { get; set; }
    public string? Notes { get; set; }
    public string? Reference { get; set; }
    public string? Payee { get; set; }
    public string? Tags { get; set; }
    public bool IsRecurring { get; set; }
    public RecurrencePattern? RecurrencePattern { get; set; }
    public Guid? TransferToAccountId { get; set; }
    public Guid? RelatedTransactionId { get; set; }
    public decimal BalanceAfter { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime? UpdatedAt { get; set; }
}

public enum FinancialTransactionType
{
    Income,
    Expense,
    Transfer,
    Adjustment
}

public enum RecurrencePattern
{
    Daily,
    Weekly,
    BiWeekly,
    Monthly,
    Quarterly,
    Yearly
}

public class QuickTransactionRequest
{
    public Guid UserId { get; set; }
    public Guid AccountId { get; set; }
    public Guid? CategoryId { get; set; }
    public decimal Amount { get; set; }
    public string? Description { get; set; }
    public string? Payee { get; set; }
    public DateTime? Date { get; set; }
}

public class TransferRequest
{
    public Guid UserId { get; set; }
    public Guid FromAccountId { get; set; }
    public Guid ToAccountId { get; set; }
    public decimal Amount { get; set; }
    public string? Description { get; set; }
    public DateTime? Date { get; set; }
}

public class AccountTransfer
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public Guid FromAccountId { get; set; }
    public string FromAccountName { get; set; } = string.Empty;
    public Guid ToAccountId { get; set; }
    public string ToAccountName { get; set; } = string.Empty;
    public decimal Amount { get; set; }
    public DateTime Date { get; set; }
    public string? Description { get; set; }
    public Guid WithdrawalTransactionId { get; set; }
    public Guid DepositTransactionId { get; set; }
}

public class AccountPayable
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public Guid UserId { get; set; }
    public string Creditor { get; set; } = string.Empty;
    public string? Description { get; set; }
    public decimal TotalAmount { get; set; }
    public decimal PaidAmount { get; set; }
    public decimal Balance => TotalAmount - PaidAmount;
    public DateTime DueDate { get; set; }
    public PayableStatus Status { get; set; } = PayableStatus.Pending;
    public Guid? CategoryId { get; set; }
    public string? Notes { get; set; }
    public List<PayablePayment> Payments { get; set; } = new();
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}

public class PayablePayment
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public decimal Amount { get; set; }
    public Guid AccountId { get; set; }
    public DateTime PaidAt { get; set; }
    public string? Reference { get; set; }
}

public enum PayableStatus
{
    Pending,
    PartiallyPaid,
    Paid,
    Overdue,
    Cancelled
}

public class AccountReceivable
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public Guid UserId { get; set; }
    public string Debtor { get; set; } = string.Empty;
    public string? Description { get; set; }
    public decimal TotalAmount { get; set; }
    public decimal ReceivedAmount { get; set; }
    public decimal Balance => TotalAmount - ReceivedAmount;
    public DateTime DueDate { get; set; }
    public ReceivableStatus Status { get; set; } = ReceivableStatus.Pending;
    public Guid? CategoryId { get; set; }
    public string? Notes { get; set; }
    public List<ReceivableReceipt> Receipts { get; set; } = new();
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}

public class ReceivableReceipt
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public decimal Amount { get; set; }
    public Guid AccountId { get; set; }
    public DateTime ReceivedAt { get; set; }
    public string? Reference { get; set; }
}

public enum ReceivableStatus
{
    Pending,
    PartiallyReceived,
    Received,
    Overdue,
    WrittenOff
}

public class Budget
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public Guid UserId { get; set; }
    public Guid CategoryId { get; set; }
    public string CategoryName { get; set; } = string.Empty;
    public decimal Amount { get; set; }
    public BudgetPeriod Period { get; set; } = BudgetPeriod.Monthly;
    public DateTime StartDate { get; set; }
    public DateTime? EndDate { get; set; }
    public decimal SpentAmount { get; set; }
    public decimal RemainingAmount => Amount - SpentAmount;
    public decimal UsagePercent => Amount > 0 ? (SpentAmount / Amount * 100) : 0;
    public bool AlertEnabled { get; set; } = true;
    public decimal AlertThreshold { get; set; } = 80;
    public bool IsActive { get; set; } = true;
}

public enum BudgetPeriod
{
    Weekly,
    Monthly,
    Quarterly,
    Yearly
}

public class BudgetProgress
{
    public Guid BudgetId { get; set; }
    public string CategoryName { get; set; } = string.Empty;
    public decimal BudgetAmount { get; set; }
    public decimal SpentAmount { get; set; }
    public decimal RemainingAmount { get; set; }
    public decimal UsagePercent { get; set; }
    public int DaysRemaining { get; set; }
    public decimal DailyAllowance { get; set; }
    public bool IsOverBudget { get; set; }
    public List<FinancialTransaction> Transactions { get; set; } = new();
}

public class BudgetAlert
{
    public Guid BudgetId { get; set; }
    public string CategoryName { get; set; } = string.Empty;
    public decimal UsagePercent { get; set; }
    public string AlertType { get; set; } = string.Empty;
    public string Message { get; set; } = string.Empty;
}

// ═══════════════════════════════════════════════════════════════════════════════
// FINANCIAL REPORT MODELS
// ═══════════════════════════════════════════════════════════════════════════════

public class FinancialDashboard
{
    public decimal TotalBalance { get; set; }
    public decimal TotalIncome { get; set; }
    public decimal TotalExpense { get; set; }
    public decimal NetProfit { get; set; }
    public decimal TotalPayables { get; set; }
    public decimal TotalReceivables { get; set; }
    public List<AccountSummary> Accounts { get; set; } = new();
    public List<CategorySummary> TopExpenseCategories { get; set; } = new();
    public List<CategorySummary> TopIncomeCategories { get; set; } = new();
    public List<FinancialTransaction> RecentTransactions { get; set; } = new();
    public List<BudgetAlert> BudgetAlerts { get; set; } = new();
    public Dictionary<string, decimal> IncomeByMonth { get; set; } = new();
    public Dictionary<string, decimal> ExpenseByMonth { get; set; } = new();
}

public class AccountSummary
{
    public Guid AccountId { get; set; }
    public string Name { get; set; } = string.Empty;
    public FinancialAccountType Type { get; set; }
    public decimal Balance { get; set; }
    public string Currency { get; set; } = string.Empty;
}

public class CategorySummary
{
    public Guid CategoryId { get; set; }
    public string Name { get; set; } = string.Empty;
    public decimal Amount { get; set; }
    public decimal Percentage { get; set; }
    public int TransactionCount { get; set; }
}

public class IncomeReport
{
    public DateTime FromDate { get; set; }
    public DateTime ToDate { get; set; }
    public decimal TotalIncome { get; set; }
    public int TransactionCount { get; set; }
    public Dictionary<string, decimal> ByCategory { get; set; } = new();
    public Dictionary<string, decimal> ByAccount { get; set; } = new();
    public Dictionary<string, decimal> ByDay { get; set; } = new();
    public List<FinancialTransaction> Transactions { get; set; } = new();
}

public class ExpenseReport
{
    public DateTime FromDate { get; set; }
    public DateTime ToDate { get; set; }
    public decimal TotalExpense { get; set; }
    public int TransactionCount { get; set; }
    public Dictionary<string, decimal> ByCategory { get; set; } = new();
    public Dictionary<string, decimal> ByAccount { get; set; } = new();
    public Dictionary<string, decimal> ByDay { get; set; } = new();
    public List<FinancialTransaction> Transactions { get; set; } = new();
}

public class ProfitLossStatement
{
    public DateTime FromDate { get; set; }
    public DateTime ToDate { get; set; }
    public decimal TotalIncome { get; set; }
    public decimal TotalExpense { get; set; }
    public decimal NetProfit { get; set; }
    public decimal ProfitMargin { get; set; }
    public List<CategorySummary> IncomeBreakdown { get; set; } = new();
    public List<CategorySummary> ExpenseBreakdown { get; set; } = new();
    public Dictionary<string, ProfitByPeriod> ByMonth { get; set; } = new();
}

public class ProfitByPeriod
{
    public decimal Income { get; set; }
    public decimal Expense { get; set; }
    public decimal Profit { get; set; }
}

public class CashFlowReport
{
    public DateTime FromDate { get; set; }
    public DateTime ToDate { get; set; }
    public decimal OpeningBalance { get; set; }
    public decimal TotalInflows { get; set; }
    public decimal TotalOutflows { get; set; }
    public decimal NetCashFlow { get; set; }
    public decimal ClosingBalance { get; set; }
    public Dictionary<string, CashFlowPeriod> ByDay { get; set; } = new();
}

public class CashFlowPeriod
{
    public decimal Inflow { get; set; }
    public decimal Outflow { get; set; }
    public decimal Net { get; set; }
    public decimal RunningBalance { get; set; }
}

public class BudgetReport
{
    public DateTime Month { get; set; }
    public decimal TotalBudgeted { get; set; }
    public decimal TotalSpent { get; set; }
    public decimal TotalRemaining { get; set; }
    public List<BudgetProgress> Budgets { get; set; } = new();
    public int OverBudgetCount { get; set; }
    public int OnTrackCount { get; set; }
}

public class NetWorthReport
{
    public DateTime AsOfDate { get; set; }
    public decimal TotalAssets { get; set; }
    public decimal TotalLiabilities { get; set; }
    public decimal NetWorth { get; set; }
    public List<AccountSummary> Assets { get; set; } = new();
    public List<AccountSummary> Liabilities { get; set; } = new();
    public Dictionary<string, decimal> NetWorthHistory { get; set; } = new();
}
