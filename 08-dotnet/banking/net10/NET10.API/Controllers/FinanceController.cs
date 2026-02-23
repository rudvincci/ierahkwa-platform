using Microsoft.AspNetCore.Mvc;
using NET10.Core.Interfaces;

namespace NET10.API.Controllers;

/// <summary>
/// Finance Controller - Ierahkwa GoMoney Personal Financial Management
/// </summary>
[ApiController]
[Route("api/[controller]")]
[Produces("application/json")]
public class FinanceController : ControllerBase
{
    private readonly IFinancialAccountService _accountService;
    private readonly IIncomeCategoryService _incomeCategoryService;
    private readonly IExpenseCategoryService _expenseCategoryService;
    private readonly IFinancialTransactionService _transactionService;
    private readonly IPayableReceivableService _payableReceivableService;
    private readonly IFinancialReportService _reportService;
    private readonly IBudgetService _budgetService;
    
    // Default user for demo
    private static readonly Guid DemoUserId = Guid.Parse("00000000-0000-0000-0000-000000000001");
    
    public FinanceController(
        IFinancialAccountService accountService,
        IIncomeCategoryService incomeCategoryService,
        IExpenseCategoryService expenseCategoryService,
        IFinancialTransactionService transactionService,
        IPayableReceivableService payableReceivableService,
        IFinancialReportService reportService,
        IBudgetService budgetService)
    {
        _accountService = accountService;
        _incomeCategoryService = incomeCategoryService;
        _expenseCategoryService = expenseCategoryService;
        _transactionService = transactionService;
        _payableReceivableService = payableReceivableService;
        _reportService = reportService;
        _budgetService = budgetService;
    }
    
    // ═══════════════════════════════════════════════════════════════
    // DASHBOARD
    // ═══════════════════════════════════════════════════════════════
    
    /// <summary>
    /// Get financial dashboard
    /// </summary>
    [HttpGet]
    public async Task<ActionResult<FinancialDashboard>> GetDashboard()
    {
        var dashboard = await _reportService.GetDashboardAsync(DemoUserId);
        return Ok(dashboard);
    }
    
    // ═══════════════════════════════════════════════════════════════
    // ACCOUNTS
    // ═══════════════════════════════════════════════════════════════
    
    /// <summary>
    /// Get all accounts
    /// </summary>
    [HttpGet("accounts")]
    public async Task<ActionResult<List<FinancialAccount>>> GetAccounts()
    {
        var accounts = await _accountService.GetAllAsync(DemoUserId);
        return Ok(accounts);
    }
    
    /// <summary>
    /// Get account by ID
    /// </summary>
    [HttpGet("accounts/{id}")]
    public async Task<ActionResult<FinancialAccount>> GetAccount(Guid id)
    {
        var account = await _accountService.GetByIdAsync(id);
        if (account == null) return NotFound();
        return Ok(account);
    }
    
    /// <summary>
    /// Create account
    /// </summary>
    [HttpPost("accounts")]
    public async Task<ActionResult<FinancialAccount>> CreateAccount([FromBody] FinancialAccount account)
    {
        account.UserId = DemoUserId;
        var created = await _accountService.CreateAsync(account);
        return CreatedAtAction(nameof(GetAccount), new { id = created.Id }, created);
    }
    
    /// <summary>
    /// Update account
    /// </summary>
    [HttpPut("accounts/{id}")]
    public async Task<ActionResult<FinancialAccount>> UpdateAccount(Guid id, [FromBody] FinancialAccount account)
    {
        account.Id = id;
        var updated = await _accountService.UpdateAsync(account);
        return Ok(updated);
    }
    
    /// <summary>
    /// Delete account
    /// </summary>
    [HttpDelete("accounts/{id}")]
    public async Task<ActionResult> DeleteAccount(Guid id)
    {
        var result = await _accountService.DeleteAsync(id);
        if (!result) return NotFound();
        return NoContent();
    }
    
    /// <summary>
    /// Get total balance
    /// </summary>
    [HttpGet("accounts/total-balance")]
    public async Task<ActionResult<decimal>> GetTotalBalance()
    {
        var balance = await _accountService.GetTotalBalanceAsync(DemoUserId);
        return Ok(new { totalBalance = balance });
    }
    
    /// <summary>
    /// Transfer between accounts
    /// </summary>
    [HttpPost("accounts/transfer")]
    public async Task<ActionResult<AccountTransfer>> Transfer([FromBody] TransferRequest request)
    {
        try
        {
            request.UserId = DemoUserId;
            var transfer = await _accountService.TransferAsync(request);
            return Ok(transfer);
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(new { error = ex.Message });
        }
    }
    
    // ═══════════════════════════════════════════════════════════════
    // INCOME CATEGORIES
    // ═══════════════════════════════════════════════════════════════
    
    /// <summary>
    /// Get income categories
    /// </summary>
    [HttpGet("categories/income")]
    public async Task<ActionResult<List<IncomeCategory>>> GetIncomeCategories()
    {
        var categories = await _incomeCategoryService.GetAllAsync(DemoUserId);
        return Ok(categories);
    }
    
    /// <summary>
    /// Create income category
    /// </summary>
    [HttpPost("categories/income")]
    public async Task<ActionResult<IncomeCategory>> CreateIncomeCategory([FromBody] IncomeCategory category)
    {
        category.UserId = DemoUserId;
        var created = await _incomeCategoryService.CreateAsync(category);
        return Ok(created);
    }
    
    /// <summary>
    /// Update income category
    /// </summary>
    [HttpPut("categories/income/{id}")]
    public async Task<ActionResult<IncomeCategory>> UpdateIncomeCategory(Guid id, [FromBody] IncomeCategory category)
    {
        category.Id = id;
        var updated = await _incomeCategoryService.UpdateAsync(category);
        return Ok(updated);
    }
    
    /// <summary>
    /// Delete income category
    /// </summary>
    [HttpDelete("categories/income/{id}")]
    public async Task<ActionResult> DeleteIncomeCategory(Guid id)
    {
        var result = await _incomeCategoryService.DeleteAsync(id);
        if (!result) return NotFound();
        return NoContent();
    }
    
    // ═══════════════════════════════════════════════════════════════
    // EXPENSE CATEGORIES
    // ═══════════════════════════════════════════════════════════════
    
    /// <summary>
    /// Get expense categories
    /// </summary>
    [HttpGet("categories/expense")]
    public async Task<ActionResult<List<ExpenseCategory>>> GetExpenseCategories()
    {
        var categories = await _expenseCategoryService.GetAllAsync(DemoUserId);
        return Ok(categories);
    }
    
    /// <summary>
    /// Create expense category
    /// </summary>
    [HttpPost("categories/expense")]
    public async Task<ActionResult<ExpenseCategory>> CreateExpenseCategory([FromBody] ExpenseCategory category)
    {
        category.UserId = DemoUserId;
        var created = await _expenseCategoryService.CreateAsync(category);
        return Ok(created);
    }
    
    /// <summary>
    /// Update expense category
    /// </summary>
    [HttpPut("categories/expense/{id}")]
    public async Task<ActionResult<ExpenseCategory>> UpdateExpenseCategory(Guid id, [FromBody] ExpenseCategory category)
    {
        category.Id = id;
        var updated = await _expenseCategoryService.UpdateAsync(category);
        return Ok(updated);
    }
    
    /// <summary>
    /// Delete expense category
    /// </summary>
    [HttpDelete("categories/expense/{id}")]
    public async Task<ActionResult> DeleteExpenseCategory(Guid id)
    {
        var result = await _expenseCategoryService.DeleteAsync(id);
        if (!result) return NotFound();
        return NoContent();
    }
    
    // ═══════════════════════════════════════════════════════════════
    // TRANSACTIONS
    // ═══════════════════════════════════════════════════════════════
    
    /// <summary>
    /// Get all transactions
    /// </summary>
    [HttpGet("transactions")]
    public async Task<ActionResult<List<FinancialTransaction>>> GetTransactions(
        [FromQuery] DateTime? from = null,
        [FromQuery] DateTime? to = null,
        [FromQuery] FinancialTransactionType? type = null)
    {
        if (from.HasValue && to.HasValue)
        {
            var transactions = await _transactionService.GetByDateRangeAsync(DemoUserId, from.Value, to.Value);
            if (type.HasValue) transactions = transactions.Where(t => t.Type == type.Value).ToList();
            return Ok(transactions);
        }
        
        if (type.HasValue)
        {
            var transactions = await _transactionService.GetByTypeAsync(DemoUserId, type.Value);
            return Ok(transactions);
        }
        
        var all = await _transactionService.GetAllAsync(DemoUserId);
        return Ok(all);
    }
    
    /// <summary>
    /// Get transaction by ID
    /// </summary>
    [HttpGet("transactions/{id}")]
    public async Task<ActionResult<FinancialTransaction>> GetTransaction(Guid id)
    {
        var transaction = await _transactionService.GetByIdAsync(id);
        if (transaction == null) return NotFound();
        return Ok(transaction);
    }
    
    /// <summary>
    /// Create transaction
    /// </summary>
    [HttpPost("transactions")]
    public async Task<ActionResult<FinancialTransaction>> CreateTransaction([FromBody] FinancialTransaction transaction)
    {
        transaction.UserId = DemoUserId;
        var created = await _transactionService.CreateAsync(transaction);
        return Ok(created);
    }
    
    /// <summary>
    /// Quick income transaction
    /// </summary>
    [HttpPost("transactions/quick-income")]
    public async Task<ActionResult<FinancialTransaction>> QuickIncome([FromBody] QuickTransactionRequest request)
    {
        request.UserId = DemoUserId;
        var transaction = await _transactionService.QuickIncomeAsync(request);
        return Ok(transaction);
    }
    
    /// <summary>
    /// Quick expense transaction
    /// </summary>
    [HttpPost("transactions/quick-expense")]
    public async Task<ActionResult<FinancialTransaction>> QuickExpense([FromBody] QuickTransactionRequest request)
    {
        request.UserId = DemoUserId;
        var transaction = await _transactionService.QuickExpenseAsync(request);
        return Ok(transaction);
    }
    
    /// <summary>
    /// Update transaction
    /// </summary>
    [HttpPut("transactions/{id}")]
    public async Task<ActionResult<FinancialTransaction>> UpdateTransaction(Guid id, [FromBody] FinancialTransaction transaction)
    {
        transaction.Id = id;
        var updated = await _transactionService.UpdateAsync(transaction);
        return Ok(updated);
    }
    
    /// <summary>
    /// Delete transaction
    /// </summary>
    [HttpDelete("transactions/{id}")]
    public async Task<ActionResult> DeleteTransaction(Guid id)
    {
        var result = await _transactionService.DeleteAsync(id);
        if (!result) return NotFound();
        return NoContent();
    }
    
    /// <summary>
    /// Get recent transactions
    /// </summary>
    [HttpGet("transactions/recent")]
    public async Task<ActionResult<List<FinancialTransaction>>> GetRecentTransactions([FromQuery] int count = 10)
    {
        var transactions = await _transactionService.GetRecentAsync(DemoUserId, count);
        return Ok(transactions);
    }
    
    // ═══════════════════════════════════════════════════════════════
    // PAYABLES & RECEIVABLES
    // ═══════════════════════════════════════════════════════════════
    
    /// <summary>
    /// Get payables (money you owe)
    /// </summary>
    [HttpGet("payables")]
    public async Task<ActionResult<List<AccountPayable>>> GetPayables()
    {
        var payables = await _payableReceivableService.GetPayablesAsync(DemoUserId);
        return Ok(payables);
    }
    
    /// <summary>
    /// Create payable
    /// </summary>
    [HttpPost("payables")]
    public async Task<ActionResult<AccountPayable>> CreatePayable([FromBody] AccountPayable payable)
    {
        payable.UserId = DemoUserId;
        var created = await _payableReceivableService.CreatePayableAsync(payable);
        return Ok(created);
    }
    
    /// <summary>
    /// Pay a payable
    /// </summary>
    [HttpPost("payables/{id}/pay")]
    public async Task<ActionResult<AccountPayable>> PayPayable(Guid id, [FromBody] FinancePaymentRequest request)
    {
        try
        {
            var payable = await _payableReceivableService.PayAsync(id, request.Amount, request.AccountId);
            return Ok(payable);
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(new { error = ex.Message });
        }
    }
    
    /// <summary>
    /// Get receivables (money owed to you)
    /// </summary>
    [HttpGet("receivables")]
    public async Task<ActionResult<List<AccountReceivable>>> GetReceivables()
    {
        var receivables = await _payableReceivableService.GetReceivablesAsync(DemoUserId);
        return Ok(receivables);
    }
    
    /// <summary>
    /// Create receivable
    /// </summary>
    [HttpPost("receivables")]
    public async Task<ActionResult<AccountReceivable>> CreateReceivable([FromBody] AccountReceivable receivable)
    {
        receivable.UserId = DemoUserId;
        var created = await _payableReceivableService.CreateReceivableAsync(receivable);
        return Ok(created);
    }
    
    /// <summary>
    /// Receive payment for a receivable
    /// </summary>
    [HttpPost("receivables/{id}/receive")]
    public async Task<ActionResult<AccountReceivable>> ReceiveReceivable(Guid id, [FromBody] FinancePaymentRequest request)
    {
        try
        {
            var receivable = await _payableReceivableService.ReceiveAsync(id, request.Amount, request.AccountId);
            return Ok(receivable);
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(new { error = ex.Message });
        }
    }
    
    // ═══════════════════════════════════════════════════════════════
    // BUDGETS
    // ═══════════════════════════════════════════════════════════════
    
    /// <summary>
    /// Get all budgets
    /// </summary>
    [HttpGet("budgets")]
    public async Task<ActionResult<List<Budget>>> GetBudgets()
    {
        var budgets = await _budgetService.GetAllAsync(DemoUserId);
        return Ok(budgets);
    }
    
    /// <summary>
    /// Create budget
    /// </summary>
    [HttpPost("budgets")]
    public async Task<ActionResult<Budget>> CreateBudget([FromBody] Budget budget)
    {
        budget.UserId = DemoUserId;
        var created = await _budgetService.CreateAsync(budget);
        return Ok(created);
    }
    
    /// <summary>
    /// Get budget progress
    /// </summary>
    [HttpGet("budgets/{id}/progress")]
    public async Task<ActionResult<BudgetProgress>> GetBudgetProgress(Guid id)
    {
        try
        {
            var progress = await _budgetService.GetProgressAsync(id);
            return Ok(progress);
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(new { error = ex.Message });
        }
    }
    
    /// <summary>
    /// Get budget alerts
    /// </summary>
    [HttpGet("budgets/alerts")]
    public async Task<ActionResult<List<BudgetAlert>>> GetBudgetAlerts()
    {
        var alerts = await _budgetService.GetAlertsAsync(DemoUserId);
        return Ok(alerts);
    }
    
    // ═══════════════════════════════════════════════════════════════
    // REPORTS
    // ═══════════════════════════════════════════════════════════════
    
    /// <summary>
    /// Get income report
    /// </summary>
    [HttpGet("reports/income")]
    public async Task<ActionResult<IncomeReport>> GetIncomeReport(
        [FromQuery] DateTime? from = null,
        [FromQuery] DateTime? to = null)
    {
        var report = await _reportService.GetIncomeReportAsync(
            DemoUserId,
            from ?? DateTime.UtcNow.AddDays(-30),
            to ?? DateTime.UtcNow);
        return Ok(report);
    }
    
    /// <summary>
    /// Get expense report
    /// </summary>
    [HttpGet("reports/expense")]
    public async Task<ActionResult<ExpenseReport>> GetExpenseReport(
        [FromQuery] DateTime? from = null,
        [FromQuery] DateTime? to = null)
    {
        var report = await _reportService.GetExpenseReportAsync(
            DemoUserId,
            from ?? DateTime.UtcNow.AddDays(-30),
            to ?? DateTime.UtcNow);
        return Ok(report);
    }
    
    /// <summary>
    /// Get profit/loss statement
    /// </summary>
    [HttpGet("reports/profit-loss")]
    public async Task<ActionResult<ProfitLossStatement>> GetProfitLoss(
        [FromQuery] DateTime? from = null,
        [FromQuery] DateTime? to = null)
    {
        var report = await _reportService.GetProfitLossAsync(
            DemoUserId,
            from ?? DateTime.UtcNow.AddDays(-30),
            to ?? DateTime.UtcNow);
        return Ok(report);
    }
    
    /// <summary>
    /// Get cash flow report
    /// </summary>
    [HttpGet("reports/cash-flow")]
    public async Task<ActionResult<CashFlowReport>> GetCashFlow(
        [FromQuery] DateTime? from = null,
        [FromQuery] DateTime? to = null)
    {
        var report = await _reportService.GetCashFlowAsync(
            DemoUserId,
            from ?? DateTime.UtcNow.AddDays(-30),
            to ?? DateTime.UtcNow);
        return Ok(report);
    }
    
    /// <summary>
    /// Get net worth
    /// </summary>
    [HttpGet("reports/net-worth")]
    public async Task<ActionResult<NetWorthReport>> GetNetWorth()
    {
        var report = await _reportService.GetNetWorthAsync(DemoUserId);
        return Ok(report);
    }
}

// ═══════════════════════════════════════════════════════════════
// REQUEST MODELS
// ═══════════════════════════════════════════════════════════════

public class FinancePaymentRequest
{
    public decimal Amount { get; set; }
    public Guid AccountId { get; set; }
}
