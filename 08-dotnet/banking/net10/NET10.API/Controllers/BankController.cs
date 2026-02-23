using Microsoft.AspNetCore.Mvc;
using NET10.Core.Interfaces;

namespace NET10.API.Controllers;

/// <summary>
/// Bank Controller - Ierahkwa Sovereign Bank Central Payment System
/// The unified banking infrastructure for all platforms, services and departments
/// </summary>
[ApiController]
[Route("api/[controller]")]
[Produces("application/json")]
public class BankController : ControllerBase
{
    private readonly IBankAccountService _accountService;
    private readonly IPaymentProcessingService _paymentService;
    private readonly IDepartmentPaymentService _departmentService;
    private readonly ITreasuryService _treasuryService;
    private readonly IMultiCurrencyService _currencyService;
    
    public BankController(
        IBankAccountService accountService,
        IPaymentProcessingService paymentService,
        IDepartmentPaymentService departmentService,
        ITreasuryService treasuryService,
        IMultiCurrencyService currencyService)
    {
        _accountService = accountService;
        _paymentService = paymentService;
        _departmentService = departmentService;
        _treasuryService = treasuryService;
        _currencyService = currencyService;
    }
    
    // ═══════════════════════════════════════════════════════════════
    // BANK OVERVIEW
    // ═══════════════════════════════════════════════════════════════
    
    /// <summary>
    /// Get bank overview - total assets, liabilities, accounts
    /// </summary>
    [HttpGet]
    public async Task<ActionResult<BankOverview>> GetOverview()
    {
        var treasury = await _treasuryService.GetOverviewAsync();
        var accounts = await _accountService.GetAllAsync();
        
        return Ok(new BankOverview
        {
            BankName = "Ierahkwa Sovereign Bank",
            Currency = "Multi-Currency (USD, CAD, EUR, MXN, IGT, ETH, BTC)",
            TotalAssets = treasury.TotalAssets,
            TotalLiabilities = treasury.TotalLiabilities,
            NetPosition = treasury.NetPosition,
            TotalAccounts = accounts.Count,
            ActiveAccounts = accounts.Count(a => a.Status == AccountStatus.Active),
            Departments = accounts.Where(a => a.Category == AccountCategory.Department).Select(a => a.Department).Distinct().Count(),
            Platforms = accounts.Where(a => a.Category == AccountCategory.Platform).Select(a => a.Platform).Distinct().Count(),
            LastUpdated = DateTime.UtcNow
        });
    }
    
    // ═══════════════════════════════════════════════════════════════
    // ACCOUNT MANAGEMENT
    // ═══════════════════════════════════════════════════════════════
    
    /// <summary>
    /// Get all bank accounts
    /// </summary>
    [HttpGet("accounts")]
    public async Task<ActionResult<List<BankAccount>>> GetAccounts()
    {
        var accounts = await _accountService.GetAllAsync();
        return Ok(accounts);
    }
    
    /// <summary>
    /// Get account by ID
    /// </summary>
    [HttpGet("accounts/{id}")]
    public async Task<ActionResult<BankAccount>> GetAccount(Guid id)
    {
        var account = await _accountService.GetByIdAsync(id);
        if (account == null) return NotFound();
        return Ok(account);
    }
    
    /// <summary>
    /// Get account by account number
    /// </summary>
    [HttpGet("accounts/number/{accountNumber}")]
    public async Task<ActionResult<BankAccount>> GetAccountByNumber(string accountNumber)
    {
        var account = await _accountService.GetByAccountNumberAsync(accountNumber);
        if (account == null) return NotFound();
        return Ok(account);
    }
    
    /// <summary>
    /// Get accounts by department
    /// </summary>
    [HttpGet("accounts/department/{department}")]
    public async Task<ActionResult<List<BankAccount>>> GetDepartmentAccounts(string department)
    {
        var accounts = await _accountService.GetByDepartmentAsync(department);
        return Ok(accounts);
    }
    
    /// <summary>
    /// Create new bank account
    /// </summary>
    [HttpPost("accounts")]
    public async Task<ActionResult<BankAccount>> CreateAccount([FromBody] CreateBankAccountRequest request)
    {
        var account = new BankAccount
        {
            AccountName = request.AccountName,
            Type = request.Type,
            Category = request.Category,
            Department = request.Department,
            Platform = request.Platform,
            PrimaryCurrency = request.Currency,
            DailyLimit = request.DailyLimit,
            MonthlyLimit = request.MonthlyLimit
        };
        
        var created = await _accountService.CreateAsync(account);
        return CreatedAtAction(nameof(GetAccount), new { id = created.Id }, created);
    }
    
    /// <summary>
    /// Get account balance
    /// </summary>
    [HttpGet("accounts/{id}/balance")]
    public async Task<ActionResult<AccountBalance>> GetBalance(Guid id)
    {
        var balance = await _accountService.GetBalanceAsync(id);
        return Ok(balance);
    }
    
    /// <summary>
    /// Freeze account
    /// </summary>
    [HttpPost("accounts/{id}/freeze")]
    public async Task<ActionResult> FreezeAccount(Guid id, [FromBody] FreezeRequest request)
    {
        var result = await _accountService.FreezeAsync(id, request.Reason);
        if (!result) return NotFound();
        return Ok(new { message = "Account frozen successfully" });
    }
    
    /// <summary>
    /// Unfreeze account
    /// </summary>
    [HttpPost("accounts/{id}/unfreeze")]
    public async Task<ActionResult> UnfreezeAccount(Guid id)
    {
        var result = await _accountService.UnfreezeAsync(id);
        if (!result) return NotFound();
        return Ok(new { message = "Account unfrozen successfully" });
    }
    
    // ═══════════════════════════════════════════════════════════════
    // PAYMENT PROCESSING
    // ═══════════════════════════════════════════════════════════════
    
    /// <summary>
    /// Process internal transfer
    /// </summary>
    [HttpPost("transfer")]
    public async Task<ActionResult<PaymentTransaction>> Transfer([FromBody] TransferPaymentRequest request)
    {
        var tx = await _paymentService.TransferAsync(request);
        return Ok(tx);
    }
    
    /// <summary>
    /// Process internal department transfer
    /// </summary>
    [HttpPost("transfer/internal")]
    public async Task<ActionResult<PaymentTransaction>> InternalTransfer([FromBody] InternalTransferRequest request)
    {
        var tx = await _paymentService.InternalTransferAsync(request);
        return Ok(tx);
    }
    
    /// <summary>
    /// Process external wire transfer
    /// </summary>
    [HttpPost("transfer/external")]
    public async Task<ActionResult<PaymentTransaction>> ExternalTransfer([FromBody] ExternalTransferRequest request)
    {
        var tx = await _paymentService.ExternalTransferAsync(request);
        return Ok(tx);
    }
    
    /// <summary>
    /// Process payment
    /// </summary>
    [HttpPost("payment")]
    public async Task<ActionResult<PaymentTransaction>> ProcessPayment([FromBody] PaymentRequest request)
    {
        var tx = await _paymentService.ProcessPaymentAsync(request);
        return Ok(tx);
    }
    
    /// <summary>
    /// Process bulk payment (payroll, mass disbursement)
    /// </summary>
    [HttpPost("payment/bulk")]
    public async Task<ActionResult<PaymentTransaction>> ProcessBulkPayment([FromBody] BulkPaymentRequest request)
    {
        var tx = await _paymentService.ProcessBulkPaymentAsync(request);
        return Ok(tx);
    }
    
    /// <summary>
    /// Setup recurring payment
    /// </summary>
    [HttpPost("payment/recurring")]
    public async Task<ActionResult<PaymentTransaction>> SetupRecurringPayment([FromBody] RecurringPaymentRequest request)
    {
        var tx = await _paymentService.ProcessRecurringPaymentAsync(request);
        return Ok(tx);
    }
    
    /// <summary>
    /// Process refund
    /// </summary>
    [HttpPost("refund/{transactionId}")]
    public async Task<ActionResult<PaymentTransaction>> Refund(Guid transactionId, [FromQuery] decimal? amount = null)
    {
        var tx = await _paymentService.RefundAsync(transactionId, amount);
        return Ok(tx);
    }
    
    /// <summary>
    /// Get transaction by ID
    /// </summary>
    [HttpGet("transactions/{id}")]
    public async Task<ActionResult<PaymentTransaction>> GetTransaction(Guid id)
    {
        var tx = await _paymentService.GetTransactionAsync(id);
        if (tx == null) return NotFound();
        return Ok(tx);
    }
    
    /// <summary>
    /// Get transaction by reference
    /// </summary>
    [HttpGet("transactions/reference/{reference}")]
    public async Task<ActionResult<PaymentTransaction>> GetTransactionByReference(string reference)
    {
        var tx = await _paymentService.GetByReferenceAsync(reference);
        if (tx == null) return NotFound();
        return Ok(tx);
    }
    
    /// <summary>
    /// Get transactions with filter
    /// </summary>
    [HttpGet("transactions")]
    public async Task<ActionResult<List<PaymentTransaction>>> GetTransactions([FromQuery] TransactionFilter filter)
    {
        var transactions = await _paymentService.GetTransactionsAsync(filter);
        return Ok(transactions);
    }
    
    // ═══════════════════════════════════════════════════════════════
    // DEPARTMENT BUDGET & PAYMENTS
    // ═══════════════════════════════════════════════════════════════
    
    /// <summary>
    /// Request budget allocation
    /// </summary>
    [HttpPost("budget/request")]
    public async Task<ActionResult<DepartmentPayment>> RequestBudget([FromBody] BudgetRequest request)
    {
        var payment = await _departmentService.RequestBudgetAsync(request);
        return Ok(payment);
    }
    
    /// <summary>
    /// Approve budget request
    /// </summary>
    [HttpPost("budget/{id}/approve")]
    public async Task<ActionResult<DepartmentPayment>> ApproveBudget(Guid id, [FromBody] ApprovalRequest request)
    {
        var payment = await _departmentService.ApproveBudgetAsync(id, request.ApprovedBy);
        return Ok(payment);
    }
    
    /// <summary>
    /// Reject budget request
    /// </summary>
    [HttpPost("budget/{id}/reject")]
    public async Task<ActionResult<DepartmentPayment>> RejectBudget(Guid id, [FromBody] RejectionRequest request)
    {
        var payment = await _departmentService.RejectBudgetAsync(id, request.Reason);
        return Ok(payment);
    }
    
    /// <summary>
    /// Disburse approved budget
    /// </summary>
    [HttpPost("budget/{id}/disburse")]
    public async Task<ActionResult<DepartmentPayment>> DisburseBudget(Guid id)
    {
        var payment = await _departmentService.DisburseAsync(id);
        return Ok(payment);
    }
    
    /// <summary>
    /// Get pending budget approvals
    /// </summary>
    [HttpGet("budget/pending")]
    public async Task<ActionResult<List<DepartmentPayment>>> GetPendingBudgets()
    {
        var payments = await _departmentService.GetPendingApprovalsAsync();
        return Ok(payments);
    }
    
    /// <summary>
    /// Get department budget status
    /// </summary>
    [HttpGet("budget/status/{department}")]
    public async Task<ActionResult<DepartmentBudget>> GetBudgetStatus(string department)
    {
        var budget = await _departmentService.GetBudgetStatusAsync(department);
        return Ok(budget);
    }
    
    /// <summary>
    /// Get department payment history
    /// </summary>
    [HttpGet("budget/history/{department}")]
    public async Task<ActionResult<List<DepartmentPayment>>> GetDepartmentHistory(string department)
    {
        var history = await _departmentService.GetDepartmentHistoryAsync(department);
        return Ok(history);
    }
    
    // ═══════════════════════════════════════════════════════════════
    // TREASURY MANAGEMENT
    // ═══════════════════════════════════════════════════════════════
    
    /// <summary>
    /// Get treasury overview
    /// </summary>
    [HttpGet("treasury")]
    public async Task<ActionResult<TreasuryOverview>> GetTreasuryOverview()
    {
        var overview = await _treasuryService.GetOverviewAsync();
        return Ok(overview);
    }
    
    /// <summary>
    /// Get treasury accounts
    /// </summary>
    [HttpGet("treasury/accounts")]
    public async Task<ActionResult<List<TreasuryAccount>>> GetTreasuryAccounts()
    {
        var accounts = await _treasuryService.GetAccountsAsync();
        return Ok(accounts);
    }
    
    /// <summary>
    /// Allocate treasury funds
    /// </summary>
    [HttpPost("treasury/allocate")]
    public async Task<ActionResult<TreasuryTransaction>> AllocateFunds([FromBody] AllocationRequest request)
    {
        var tx = await _treasuryService.AllocateFundsAsync(request);
        return Ok(tx);
    }
    
    /// <summary>
    /// Recall treasury funds
    /// </summary>
    [HttpPost("treasury/recall")]
    public async Task<ActionResult<TreasuryTransaction>> RecallFunds([FromBody] RecallRequest request)
    {
        var tx = await _treasuryService.RecallFundsAsync(request);
        return Ok(tx);
    }
    
    /// <summary>
    /// Get treasury report
    /// </summary>
    [HttpGet("treasury/report")]
    public async Task<ActionResult<TreasuryReport>> GetTreasuryReport([FromQuery] ReportPeriod period = ReportPeriod.Monthly)
    {
        var report = await _treasuryService.GenerateReportAsync(period);
        return Ok(report);
    }
    
    /// <summary>
    /// Get budget allocations by fiscal year
    /// </summary>
    [HttpGet("treasury/allocations/{year}")]
    public async Task<ActionResult<List<BudgetAllocation>>> GetBudgetAllocations(int year)
    {
        var allocations = await _treasuryService.GetBudgetAllocationsAsync(year);
        return Ok(allocations);
    }
    
    // ═══════════════════════════════════════════════════════════════
    // MULTI-CURRENCY
    // ═══════════════════════════════════════════════════════════════
    
    /// <summary>
    /// Get supported currencies
    /// </summary>
    [HttpGet("currencies")]
    public async Task<ActionResult<List<Currency>>> GetCurrencies()
    {
        var currencies = await _currencyService.GetSupportedCurrenciesAsync();
        return Ok(currencies);
    }
    
    /// <summary>
    /// Get exchange rate
    /// </summary>
    [HttpGet("exchange-rate")]
    public async Task<ActionResult<ExchangeRate>> GetExchangeRate([FromQuery] string from, [FromQuery] string to)
    {
        var rate = await _currencyService.GetExchangeRateAsync(from, to);
        return Ok(rate);
    }
    
    /// <summary>
    /// Get all exchange rates for base currency
    /// </summary>
    [HttpGet("exchange-rates/{baseCurrency}")]
    public async Task<ActionResult<List<ExchangeRate>>> GetExchangeRates(string baseCurrency)
    {
        var rates = await _currencyService.GetAllRatesAsync(baseCurrency);
        return Ok(rates);
    }
    
    /// <summary>
    /// Convert currency
    /// </summary>
    [HttpPost("convert")]
    public async Task<ActionResult<CurrencyConversion>> ConvertCurrency([FromBody] ConversionRequest request)
    {
        var result = await _currencyService.ConvertAsync(request.Amount, request.From, request.To);
        return Ok(result);
    }
    
    // ═══════════════════════════════════════════════════════════════
    // PLATFORM INTEGRATIONS
    // ═══════════════════════════════════════════════════════════════
    
    /// <summary>
    /// Get platform payment summary
    /// </summary>
    [HttpGet("platforms")]
    public ActionResult<List<PlatformPaymentSummary>> GetPlatformSummaries()
    {
        var summaries = new List<PlatformPaymentSummary>
        {
            new() { Platform = "ERP (NAGADAN)", TotalTransactions = 15678, TotalVolume = 25_000_000, TodayTransactions = 234, TodayVolume = 450_000 },
            new() { Platform = "DeFi", TotalTransactions = 125000, TotalVolume = 500_000_000, TodayTransactions = 5678, TodayVolume = 15_000_000 },
            new() { Platform = "Hotel & Real Estate", TotalTransactions = 8500, TotalVolume = 12_000_000, TodayTransactions = 45, TodayVolume = 125_000 },
            new() { Platform = "College", TotalTransactions = 3200, TotalVolume = 8_500_000, TodayTransactions = 12, TodayVolume = 35_000 },
            new() { Platform = "Government Services", TotalTransactions = 45000, TotalVolume = 75_000_000, TodayTransactions = 123, TodayVolume = 250_000 },
            new() { Platform = "Bridge", TotalTransactions = 25000, TotalVolume = 150_000_000, TodayTransactions = 890, TodayVolume = 5_000_000 }
        };
        return Ok(summaries);
    }
    
    /// <summary>
    /// Get department payment summary
    /// </summary>
    [HttpGet("departments")]
    public ActionResult<List<DepartmentPaymentSummary>> GetDepartmentSummaries()
    {
        var summaries = new List<DepartmentPaymentSummary>
        {
            new() { Department = "Education", AllocatedBudget = 50_000_000, SpentBudget = 35_000_000, PendingPayments = 2_500_000 },
            new() { Department = "Health", AllocatedBudget = 75_000_000, SpentBudget = 55_000_000, PendingPayments = 5_000_000 },
            new() { Department = "Infrastructure", AllocatedBudget = 100_000_000, SpentBudget = 60_000_000, PendingPayments = 15_000_000 },
            new() { Department = "Social Services", AllocatedBudget = 30_000_000, SpentBudget = 22_000_000, PendingPayments = 1_500_000 },
            new() { Department = "Public Safety", AllocatedBudget = 25_000_000, SpentBudget = 18_000_000, PendingPayments = 2_000_000 },
            new() { Department = "Treasury", AllocatedBudget = 500_000_000, SpentBudget = 200_000_000, PendingPayments = 25_000_000 }
        };
        return Ok(summaries);
    }
}

// ═══════════════════════════════════════════════════════════════
// BANK API MODELS
// ═══════════════════════════════════════════════════════════════

public class BankOverview
{
    public string BankName { get; set; } = string.Empty;
    public string Currency { get; set; } = string.Empty;
    public decimal TotalAssets { get; set; }
    public decimal TotalLiabilities { get; set; }
    public decimal NetPosition { get; set; }
    public int TotalAccounts { get; set; }
    public int ActiveAccounts { get; set; }
    public int Departments { get; set; }
    public int Platforms { get; set; }
    public DateTime LastUpdated { get; set; }
}

public class CreateBankAccountRequest
{
    public string AccountName { get; set; } = string.Empty;
    public AccountType Type { get; set; }
    public AccountCategory Category { get; set; }
    public string? Department { get; set; }
    public string? Platform { get; set; }
    public string Currency { get; set; } = "USD";
    public decimal DailyLimit { get; set; } = 100_000;
    public decimal MonthlyLimit { get; set; } = 1_000_000;
}

public class FreezeRequest
{
    public string Reason { get; set; } = string.Empty;
}

public class ApprovalRequest
{
    public string ApprovedBy { get; set; } = string.Empty;
}

public class RejectionRequest
{
    public string Reason { get; set; } = string.Empty;
}

public class ConversionRequest
{
    public decimal Amount { get; set; }
    public string From { get; set; } = string.Empty;
    public string To { get; set; } = string.Empty;
}

public class PlatformPaymentSummary
{
    public string Platform { get; set; } = string.Empty;
    public long TotalTransactions { get; set; }
    public decimal TotalVolume { get; set; }
    public int TodayTransactions { get; set; }
    public decimal TodayVolume { get; set; }
}

public class DepartmentPaymentSummary
{
    public string Department { get; set; } = string.Empty;
    public decimal AllocatedBudget { get; set; }
    public decimal SpentBudget { get; set; }
    public decimal RemainingBudget => AllocatedBudget - SpentBudget;
    public decimal PendingPayments { get; set; }
    public decimal AvailableBudget => RemainingBudget - PendingPayments;
}
