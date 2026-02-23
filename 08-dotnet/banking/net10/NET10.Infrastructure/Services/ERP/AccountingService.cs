using NET10.Core.Interfaces;
using NET10.Core.Models.ERP;
using AccountType = NET10.Core.Models.ERP.AccountType;
using AccountCategory = NET10.Core.Models.ERP.AccountCategory;

namespace NET10.Infrastructure.Services.ERP;

/// <summary>
/// Accounting Service - Chart of Accounts, Journal Entries, Financial Reports
/// </summary>
public class AccountingService : IAccountingService
{
    private static readonly List<Account> _accounts = [];
    private static readonly List<JournalEntry> _journalEntries = [];
    private static readonly List<TaxRate> _taxRates = [];
    private static readonly Dictionary<Guid, int> _journalCounters = [];
    private static bool _initialized = false;
    
    public AccountingService()
    {
        if (!_initialized)
        {
            InitializeDefaultAccounts();
            InitializeDefaultTaxRates();
            _initialized = true;
        }
    }
    
    // ═══════════════════════════════════════════════════════════════
    // CHART OF ACCOUNTS
    // ═══════════════════════════════════════════════════════════════
    
    public Task<List<Account>> GetChartOfAccountsAsync(Guid companyId)
    {
        var accounts = _accounts.Where(a => a.CompanyId == companyId || a.CompanyId == Guid.Empty)
                                .OrderBy(a => a.Code)
                                .ToList();
        return Task.FromResult(accounts);
    }
    
    public Task<Account?> GetAccountByIdAsync(Guid id)
    {
        var account = _accounts.FirstOrDefault(a => a.Id == id);
        return Task.FromResult(account);
    }
    
    public Task<Account> CreateAccountAsync(Account account)
    {
        account.Id = Guid.NewGuid();
        account.CreatedAt = DateTime.UtcNow;
        _accounts.Add(account);
        return Task.FromResult(account);
    }
    
    public Task<Account> UpdateAccountAsync(Account account)
    {
        var index = _accounts.FindIndex(a => a.Id == account.Id);
        if (index >= 0)
        {
            _accounts[index] = account;
        }
        return Task.FromResult(account);
    }
    
    // ═══════════════════════════════════════════════════════════════
    // JOURNAL ENTRIES
    // ═══════════════════════════════════════════════════════════════
    
    public Task<JournalEntry> CreateJournalEntryAsync(JournalEntry entry)
    {
        entry.Id = Guid.NewGuid();
        entry.CreatedAt = DateTime.UtcNow;
        
        if (string.IsNullOrEmpty(entry.EntryNumber))
        {
            if (!_journalCounters.ContainsKey(entry.CompanyId))
                _journalCounters[entry.CompanyId] = 1000;
            _journalCounters[entry.CompanyId]++;
            entry.EntryNumber = $"JE-{_journalCounters[entry.CompanyId]:D6}";
        }
        
        _journalEntries.Add(entry);
        return Task.FromResult(entry);
    }
    
    public Task<JournalEntry> PostJournalEntryAsync(Guid entryId)
    {
        var entry = _journalEntries.FirstOrDefault(e => e.Id == entryId);
        if (entry != null)
        {
            entry.Status = JournalStatus.Posted;
            entry.IsPosted = true;
            entry.PostedAt = DateTime.UtcNow;
            
            // Update account balances
            foreach (var line in entry.Lines)
            {
                var account = _accounts.FirstOrDefault(a => a.Id == line.AccountId);
                if (account != null)
                {
                    // Debit increases assets/expenses, decreases liabilities/equity/revenue
                    // Credit is opposite
                    if (account.Type == AccountType.Asset || account.Type == AccountType.Expense)
                    {
                        account.CurrentBalance += line.Debit - line.Credit;
                    }
                    else
                    {
                        account.CurrentBalance += line.Credit - line.Debit;
                    }
                }
            }
        }
        return Task.FromResult(entry!);
    }
    
    public Task<List<JournalEntry>> GetJournalEntriesAsync(Guid companyId, DateTime fromDate, DateTime toDate)
    {
        var entries = _journalEntries
            .Where(e => e.CompanyId == companyId && 
                        e.EntryDate >= fromDate && 
                        e.EntryDate <= toDate)
            .OrderByDescending(e => e.EntryDate)
            .ToList();
        return Task.FromResult(entries);
    }
    
    // ═══════════════════════════════════════════════════════════════
    // TAX RATES
    // ═══════════════════════════════════════════════════════════════
    
    public Task<List<TaxRate>> GetTaxRatesAsync(Guid companyId)
    {
        var taxes = _taxRates.Where(t => t.CompanyId == companyId || t.CompanyId == Guid.Empty)
                             .ToList();
        return Task.FromResult(taxes);
    }
    
    public Task<TaxRate> CreateTaxRateAsync(TaxRate taxRate)
    {
        taxRate.Id = Guid.NewGuid();
        _taxRates.Add(taxRate);
        return Task.FromResult(taxRate);
    }
    
    // ═══════════════════════════════════════════════════════════════
    // FINANCIAL REPORTS
    // ═══════════════════════════════════════════════════════════════
    
    public Task<TrialBalance> GetTrialBalanceAsync(Guid companyId, DateTime asOfDate)
    {
        var accounts = _accounts.Where(a => a.CompanyId == companyId || a.CompanyId == Guid.Empty)
                                .OrderBy(a => a.Code)
                                .ToList();
        
        var report = new TrialBalance
        {
            AsOfDate = asOfDate,
            Lines = accounts.Select(a => new TrialBalanceLine
            {
                AccountCode = a.Code,
                AccountName = a.Name,
                Type = a.Type.ToString(),
                Debit = (a.Type == AccountType.Asset || a.Type == AccountType.Expense) && a.CurrentBalance > 0 ? a.CurrentBalance : 0,
                Credit = (a.Type == AccountType.Liability || a.Type == AccountType.Equity || a.Type == AccountType.Revenue) && a.CurrentBalance > 0 ? a.CurrentBalance : 0
            }).ToList()
        };
        
        report.TotalDebit = report.Lines.Sum(l => l.Debit);
        report.TotalCredit = report.Lines.Sum(l => l.Credit);
        
        return Task.FromResult(report);
    }
    
    public Task<ProfitAndLoss> GetProfitAndLossAsync(Guid companyId, DateTime fromDate, DateTime toDate)
    {
        var accounts = _accounts.Where(a => (a.CompanyId == companyId || a.CompanyId == Guid.Empty) && 
                                           (a.Type == AccountType.Revenue || a.Type == AccountType.Expense))
                                .ToList();
        
        var report = new ProfitAndLoss
        {
            FromDate = fromDate,
            ToDate = toDate,
            Revenue = accounts.Where(a => a.Type == AccountType.Revenue)
                              .GroupBy(a => a.Category.ToString())
                              .Select(g => new PLSection
                              {
                                  Category = g.Key,
                                  Amount = g.Sum(a => a.CurrentBalance),
                                  Lines = g.Select(a => new PLLine { AccountName = a.Name, Amount = a.CurrentBalance }).ToList()
                              }).ToList(),
            Expenses = accounts.Where(a => a.Type == AccountType.Expense)
                               .GroupBy(a => a.Category.ToString())
                               .Select(g => new PLSection
                               {
                                   Category = g.Key,
                                   Amount = g.Sum(a => a.CurrentBalance),
                                   Lines = g.Select(a => new PLLine { AccountName = a.Name, Amount = a.CurrentBalance }).ToList()
                               }).ToList()
        };
        
        report.TotalRevenue = report.Revenue.Sum(r => r.Amount);
        report.TotalExpenses = report.Expenses.Sum(e => e.Amount);
        
        return Task.FromResult(report);
    }
    
    public Task<BalanceSheet> GetBalanceSheetAsync(Guid companyId, DateTime asOfDate)
    {
        var accounts = _accounts.Where(a => (a.CompanyId == companyId || a.CompanyId == Guid.Empty) &&
                                           (a.Type == AccountType.Asset || a.Type == AccountType.Liability || a.Type == AccountType.Equity))
                                .ToList();
        
        var report = new BalanceSheet
        {
            AsOfDate = asOfDate,
            Assets = accounts.Where(a => a.Type == AccountType.Asset)
                            .GroupBy(a => a.Category.ToString())
                            .Select(g => new BSSection
                            {
                                Category = g.Key,
                                Total = g.Sum(a => a.CurrentBalance),
                                Lines = g.Select(a => new BSLine { AccountName = a.Name, Balance = a.CurrentBalance }).ToList()
                            }).ToList(),
            Liabilities = accounts.Where(a => a.Type == AccountType.Liability)
                                  .GroupBy(a => a.Category.ToString())
                                  .Select(g => new BSSection
                                  {
                                      Category = g.Key,
                                      Total = g.Sum(a => a.CurrentBalance),
                                      Lines = g.Select(a => new BSLine { AccountName = a.Name, Balance = a.CurrentBalance }).ToList()
                                  }).ToList(),
            Equity = accounts.Where(a => a.Type == AccountType.Equity)
                            .GroupBy(a => a.Category.ToString())
                            .Select(g => new BSSection
                            {
                                Category = g.Key,
                                Total = g.Sum(a => a.CurrentBalance),
                                Lines = g.Select(a => new BSLine { AccountName = a.Name, Balance = a.CurrentBalance }).ToList()
                            }).ToList()
        };
        
        report.TotalAssets = report.Assets.Sum(a => a.Total);
        report.TotalLiabilities = report.Liabilities.Sum(l => l.Total);
        report.TotalEquity = report.Equity.Sum(e => e.Total);
        
        return Task.FromResult(report);
    }
    
    // ═══════════════════════════════════════════════════════════════
    // DEFAULT DATA
    // ═══════════════════════════════════════════════════════════════
    
    private void InitializeDefaultAccounts()
    {
        var defaultAccounts = new List<Account>
        {
            // Assets
            new() { Code = "1000", Name = "Caja", Type = AccountType.Asset, Category = AccountCategory.Cash, CurrentBalance = 15000, IsSystemAccount = true },
            new() { Code = "1100", Name = "Bancos", Type = AccountType.Asset, Category = AccountCategory.Bank, CurrentBalance = 245780, IsSystemAccount = true },
            new() { Code = "1200", Name = "Cuentas por Cobrar", Type = AccountType.Asset, Category = AccountCategory.AccountsReceivable, CurrentBalance = 42350, IsSystemAccount = true },
            new() { Code = "1300", Name = "Inventario", Type = AccountType.Asset, Category = AccountCategory.Inventory, CurrentBalance = 85000, IsSystemAccount = true },
            new() { Code = "1500", Name = "Activos Fijos", Type = AccountType.Asset, Category = AccountCategory.FixedAssets, CurrentBalance = 150000, IsSystemAccount = true },
            
            // Liabilities
            new() { Code = "2000", Name = "Cuentas por Pagar", Type = AccountType.Liability, Category = AccountCategory.AccountsPayable, CurrentBalance = 18500, IsSystemAccount = true },
            new() { Code = "2100", Name = "Tarjetas de Crédito", Type = AccountType.Liability, Category = AccountCategory.CreditCard, CurrentBalance = 5200, IsSystemAccount = true },
            new() { Code = "2200", Name = "IVA por Pagar", Type = AccountType.Liability, Category = AccountCategory.TaxPayable, CurrentBalance = 12400, IsSystemAccount = true },
            new() { Code = "2300", Name = "Préstamos Bancarios", Type = AccountType.Liability, Category = AccountCategory.Loans, CurrentBalance = 75000, IsSystemAccount = true },
            
            // Equity
            new() { Code = "3000", Name = "Capital Social", Type = AccountType.Equity, Category = AccountCategory.OwnersEquity, CurrentBalance = 200000, IsSystemAccount = true },
            new() { Code = "3100", Name = "Utilidades Retenidas", Type = AccountType.Equity, Category = AccountCategory.RetainedEarnings, CurrentBalance = 85000, IsSystemAccount = true },
            
            // Revenue
            new() { Code = "4000", Name = "Ventas", Type = AccountType.Revenue, Category = AccountCategory.Sales, CurrentBalance = 245780, IsSystemAccount = true },
            new() { Code = "4100", Name = "Otros Ingresos", Type = AccountType.Revenue, Category = AccountCategory.OtherIncome, CurrentBalance = 8500, IsSystemAccount = true },
            
            // Expenses
            new() { Code = "5000", Name = "Costo de Ventas", Type = AccountType.Expense, Category = AccountCategory.CostOfGoodsSold, CurrentBalance = 98000, IsSystemAccount = true },
            new() { Code = "6000", Name = "Gastos Operativos", Type = AccountType.Expense, Category = AccountCategory.OperatingExpenses, CurrentBalance = 45000, IsSystemAccount = true },
            new() { Code = "6100", Name = "Nómina", Type = AccountType.Expense, Category = AccountCategory.Payroll, CurrentBalance = 62000, IsSystemAccount = true },
            new() { Code = "6200", Name = "Gastos de Impuestos", Type = AccountType.Expense, Category = AccountCategory.TaxExpense, CurrentBalance = 15000, IsSystemAccount = true }
        };
        
        _accounts.AddRange(defaultAccounts);
    }
    
    private void InitializeDefaultTaxRates()
    {
        var defaultTaxes = new List<TaxRate>
        {
            new() { Code = "IVA16", Name = "IVA 16%", Rate = 16.0m, Type = TaxType.VAT, IsDefault = true },
            new() { Code = "IVA0", Name = "IVA 0%", Rate = 0m, Type = TaxType.VAT },
            new() { Code = "ISR10", Name = "Retención ISR 10%", Rate = 10.0m, Type = TaxType.WithholdingTax },
            new() { Code = "IEPS8", Name = "IEPS 8%", Rate = 8.0m, Type = TaxType.Excise },
            new() { Code = "GST5", Name = "GST 5%", Rate = 5.0m, Type = TaxType.GST }
        };
        
        _taxRates.AddRange(defaultTaxes);
    }
}
