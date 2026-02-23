using Microsoft.AspNetCore.Mvc;
using NET10.Core.Interfaces;
using NET10.Core.Models.ERP;

namespace NET10.API.Controllers.ERP;

/// <summary>
/// Accounting Controller - Chart of Accounts, Journal Entries, Financial Reports
/// </summary>
[ApiController]
[Route("api/erp/[controller]")]
[Produces("application/json")]
public class AccountingController : ControllerBase
{
    private readonly IAccountingService _accountingService;
    
    public AccountingController(IAccountingService accountingService)
    {
        _accountingService = accountingService;
    }
    
    // ═══════════════════════════════════════════════════════════════
    // CHART OF ACCOUNTS
    // ═══════════════════════════════════════════════════════════════
    
    /// <summary>
    /// Get Chart of Accounts for a company
    /// </summary>
    [HttpGet("accounts/{companyId}")]
    public async Task<ActionResult<List<Account>>> GetChartOfAccounts(Guid companyId)
    {
        var accounts = await _accountingService.GetChartOfAccountsAsync(companyId);
        return Ok(accounts);
    }
    
    /// <summary>
    /// Get single account by ID
    /// </summary>
    [HttpGet("accounts/detail/{id}")]
    public async Task<ActionResult<Account>> GetAccount(Guid id)
    {
        var account = await _accountingService.GetAccountByIdAsync(id);
        if (account == null) return NotFound();
        return Ok(account);
    }
    
    /// <summary>
    /// Create new account
    /// </summary>
    [HttpPost("accounts")]
    public async Task<ActionResult<Account>> CreateAccount([FromBody] Account account)
    {
        var created = await _accountingService.CreateAccountAsync(account);
        return CreatedAtAction(nameof(GetAccount), new { id = created.Id }, created);
    }
    
    /// <summary>
    /// Update account
    /// </summary>
    [HttpPut("accounts/{id}")]
    public async Task<ActionResult<Account>> UpdateAccount(Guid id, [FromBody] Account account)
    {
        account.Id = id;
        var updated = await _accountingService.UpdateAccountAsync(account);
        return Ok(updated);
    }
    
    // ═══════════════════════════════════════════════════════════════
    // JOURNAL ENTRIES
    // ═══════════════════════════════════════════════════════════════
    
    /// <summary>
    /// Get journal entries for date range
    /// </summary>
    [HttpGet("journal/{companyId}")]
    public async Task<ActionResult<List<JournalEntry>>> GetJournalEntries(
        Guid companyId, 
        [FromQuery] DateTime fromDate, 
        [FromQuery] DateTime toDate)
    {
        var entries = await _accountingService.GetJournalEntriesAsync(companyId, fromDate, toDate);
        return Ok(entries);
    }
    
    /// <summary>
    /// Create journal entry
    /// </summary>
    [HttpPost("journal")]
    public async Task<ActionResult<JournalEntry>> CreateJournalEntry([FromBody] JournalEntry entry)
    {
        // Validate debits = credits
        var totalDebit = entry.Lines.Sum(l => l.Debit);
        var totalCredit = entry.Lines.Sum(l => l.Credit);
        
        if (totalDebit != totalCredit)
        {
            return BadRequest(new { error = "Debits must equal Credits", debit = totalDebit, credit = totalCredit });
        }
        
        var created = await _accountingService.CreateJournalEntryAsync(entry);
        return Ok(created);
    }
    
    /// <summary>
    /// Post journal entry to ledger
    /// </summary>
    [HttpPost("journal/{id}/post")]
    public async Task<ActionResult<JournalEntry>> PostJournalEntry(Guid id)
    {
        var posted = await _accountingService.PostJournalEntryAsync(id);
        return Ok(posted);
    }
    
    // ═══════════════════════════════════════════════════════════════
    // TAX RATES
    // ═══════════════════════════════════════════════════════════════
    
    /// <summary>
    /// Get tax rates for company
    /// </summary>
    [HttpGet("taxes/{companyId}")]
    public async Task<ActionResult<List<TaxRate>>> GetTaxRates(Guid companyId)
    {
        var taxes = await _accountingService.GetTaxRatesAsync(companyId);
        return Ok(taxes);
    }
    
    /// <summary>
    /// Create tax rate
    /// </summary>
    [HttpPost("taxes")]
    public async Task<ActionResult<TaxRate>> CreateTaxRate([FromBody] TaxRate taxRate)
    {
        var created = await _accountingService.CreateTaxRateAsync(taxRate);
        return Ok(created);
    }
    
    // ═══════════════════════════════════════════════════════════════
    // FINANCIAL REPORTS
    // ═══════════════════════════════════════════════════════════════
    
    /// <summary>
    /// Get Trial Balance report
    /// </summary>
    [HttpGet("reports/trial-balance/{companyId}")]
    public async Task<ActionResult<TrialBalance>> GetTrialBalance(Guid companyId, [FromQuery] DateTime asOfDate)
    {
        var report = await _accountingService.GetTrialBalanceAsync(companyId, asOfDate);
        return Ok(report);
    }
    
    /// <summary>
    /// Get Profit & Loss report
    /// </summary>
    [HttpGet("reports/profit-loss/{companyId}")]
    public async Task<ActionResult<ProfitAndLoss>> GetProfitAndLoss(
        Guid companyId, 
        [FromQuery] DateTime fromDate, 
        [FromQuery] DateTime toDate)
    {
        var report = await _accountingService.GetProfitAndLossAsync(companyId, fromDate, toDate);
        return Ok(report);
    }
    
    /// <summary>
    /// Get Balance Sheet report
    /// </summary>
    [HttpGet("reports/balance-sheet/{companyId}")]
    public async Task<ActionResult<BalanceSheet>> GetBalanceSheet(Guid companyId, [FromQuery] DateTime asOfDate)
    {
        var report = await _accountingService.GetBalanceSheetAsync(companyId, asOfDate);
        return Ok(report);
    }
}
