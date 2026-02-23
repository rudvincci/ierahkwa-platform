using NET10.Core.Interfaces;

namespace NET10.Infrastructure.Services.Banking;

// ═══════════════════════════════════════════════════════════════════════════════
// IERAHKWA SOVEREIGN BANK - SERVICES IMPLEMENTATION
// Central Payment System for All Platforms
// ═══════════════════════════════════════════════════════════════════════════════

public class BankAccountService : IBankAccountService
{
    private readonly List<BankAccount> _accounts = InitializeAccounts();
    
    public Task<List<BankAccount>> GetAllAsync() => Task.FromResult(_accounts.ToList());
    
    public Task<BankAccount?> GetByIdAsync(Guid id) =>
        Task.FromResult(_accounts.FirstOrDefault(a => a.Id == id));
    
    public Task<BankAccount?> GetByAccountNumberAsync(string accountNumber) =>
        Task.FromResult(_accounts.FirstOrDefault(a => a.AccountNumber == accountNumber));
    
    public Task<List<BankAccount>> GetByCustomerAsync(Guid customerId) =>
        Task.FromResult(_accounts.Where(a => a.CustomerId == customerId).ToList());
    
    public Task<List<BankAccount>> GetByDepartmentAsync(string department) =>
        Task.FromResult(_accounts.Where(a => a.Department == department).ToList());
    
    public Task<BankAccount> CreateAsync(BankAccount account)
    {
        account.Id = Guid.NewGuid();
        account.AccountNumber = GenerateAccountNumber(account.Type);
        account.OpenedAt = DateTime.UtcNow;
        account.Balances.Add(new AccountBalance { Currency = account.PrimaryCurrency });
        _accounts.Add(account);
        return Task.FromResult(account);
    }
    
    public Task<BankAccount> UpdateAsync(BankAccount account)
    {
        var index = _accounts.FindIndex(a => a.Id == account.Id);
        if (index >= 0) _accounts[index] = account;
        return Task.FromResult(account);
    }
    
    public Task<bool> FreezeAsync(Guid id, string reason)
    {
        var account = _accounts.FirstOrDefault(a => a.Id == id);
        if (account != null)
        {
            account.Status = AccountStatus.Frozen;
            return Task.FromResult(true);
        }
        return Task.FromResult(false);
    }
    
    public Task<bool> UnfreezeAsync(Guid id)
    {
        var account = _accounts.FirstOrDefault(a => a.Id == id);
        if (account != null && account.Status == AccountStatus.Frozen)
        {
            account.Status = AccountStatus.Active;
            return Task.FromResult(true);
        }
        return Task.FromResult(false);
    }
    
    public Task<AccountBalance> GetBalanceAsync(Guid accountId)
    {
        var account = _accounts.FirstOrDefault(a => a.Id == accountId);
        return Task.FromResult(account?.Balances.FirstOrDefault() ?? new AccountBalance());
    }
    
    public Task<List<AccountBalance>> GetAllBalancesAsync(Guid accountId)
    {
        var account = _accounts.FirstOrDefault(a => a.Id == accountId);
        return Task.FromResult(account?.Balances ?? new List<AccountBalance>());
    }
    
    private static string GenerateAccountNumber(AccountType type)
    {
        var prefix = type switch
        {
            AccountType.Treasury => "TSY",
            AccountType.Operating => "OPR",
            AccountType.Payroll => "PAY",
            AccountType.Revenue => "REV",
            AccountType.Reserve => "RSV",
            AccountType.Escrow => "ESC",
            _ => "ACC"
        };
        return $"{prefix}-{DateTime.UtcNow:yyyyMM}-{new Random().Next(100000, 999999)}";
    }
    
    private static List<BankAccount> InitializeAccounts()
    {
        return new List<BankAccount>
        {
            // Treasury Accounts
            new()
            {
                AccountNumber = "TSY-MAIN-000001",
                AccountName = "Ierahkwa Sovereign Treasury - Main",
                Type = AccountType.Treasury,
                Category = AccountCategory.Treasury,
                Department = "Treasury",
                Balances = new() { new() { Currency = "USD", Available = 500_000_000, Pending = 5_000_000 } }
            },
            new()
            {
                AccountNumber = "TSY-RSV-000001",
                AccountName = "Sovereign Reserve Fund",
                Type = AccountType.Reserve,
                Category = AccountCategory.Treasury,
                Department = "Treasury",
                Balances = new() { new() { Currency = "USD", Available = 250_000_000 } }
            },
            // Department Operating Accounts
            new()
            {
                AccountNumber = "OPR-EDU-000001",
                AccountName = "Education Department Operating",
                Type = AccountType.Operating,
                Category = AccountCategory.Department,
                Department = "Education",
                Balances = new() { new() { Currency = "USD", Available = 15_000_000, Pending = 500_000 } }
            },
            new()
            {
                AccountNumber = "OPR-HLT-000001",
                AccountName = "Health Services Operating",
                Type = AccountType.Operating,
                Category = AccountCategory.Department,
                Department = "Health",
                Balances = new() { new() { Currency = "USD", Available = 25_000_000, Pending = 1_000_000 } }
            },
            new()
            {
                AccountNumber = "OPR-INF-000001",
                AccountName = "Infrastructure Department Operating",
                Type = AccountType.Operating,
                Category = AccountCategory.Department,
                Department = "Infrastructure",
                Balances = new() { new() { Currency = "USD", Available = 50_000_000, Pending = 10_000_000 } }
            },
            // Platform Accounts
            new()
            {
                AccountNumber = "PLT-ERP-000001",
                AccountName = "NAGADAN ERP Platform Revenue",
                Type = AccountType.Revenue,
                Category = AccountCategory.Platform,
                Platform = "ERP",
                Balances = new() { new() { Currency = "USD", Available = 5_000_000 } }
            },
            new()
            {
                AccountNumber = "PLT-DEFI-000001",
                AccountName = "DeFi Platform Operations",
                Type = AccountType.Operating,
                Category = AccountCategory.Platform,
                Platform = "DeFi",
                Balances = new() { 
                    new() { Currency = "USD", Available = 10_000_000 },
                    new() { Currency = "IGT", Available = 50_000_000 }
                }
            },
            new()
            {
                AccountNumber = "PLT-HTL-000001",
                AccountName = "Hotel & Property Revenue",
                Type = AccountType.Revenue,
                Category = AccountCategory.Platform,
                Platform = "Hotel",
                Balances = new() { new() { Currency = "USD", Available = 2_500_000 } }
            },
            new()
            {
                AccountNumber = "PLT-COL-000001",
                AccountName = "College Tuition & Fees",
                Type = AccountType.Revenue,
                Category = AccountCategory.Platform,
                Platform = "College",
                Balances = new() { new() { Currency = "USD", Available = 3_500_000 } }
            },
            // Payroll Accounts
            new()
            {
                AccountNumber = "PAY-GOV-000001",
                AccountName = "Government Payroll Account",
                Type = AccountType.Payroll,
                Category = AccountCategory.Department,
                Department = "HR",
                Balances = new() { new() { Currency = "USD", Available = 8_000_000 } }
            },
            // Escrow
            new()
            {
                AccountNumber = "ESC-BRG-000001",
                AccountName = "Bridge Escrow Account",
                Type = AccountType.Escrow,
                Category = AccountCategory.Platform,
                Platform = "Bridge",
                Balances = new() { 
                    new() { Currency = "USD", Available = 25_000_000, Locked = 5_000_000 },
                    new() { Currency = "ETH", Available = 5_000, Locked = 1_000 }
                }
            }
        };
    }
}

public class PaymentProcessingService : IPaymentProcessingService
{
    private readonly List<PaymentTransaction> _transactions = new();
    private readonly IBankAccountService _accountService;
    private static long _transactionCounter = 1000000;
    
    public PaymentProcessingService(IBankAccountService accountService)
    {
        _accountService = accountService;
    }
    
    public async Task<PaymentTransaction> TransferAsync(TransferPaymentRequest request)
    {
        var tx = CreateTransaction(request.Amount, request.Currency, TransactionType.Transfer, PaymentMethod.InternalTransfer);
        tx.SourceAccountId = request.SourceAccountId;
        tx.DestinationAccountId = request.DestinationAccountId;
        tx.Description = request.Description;
        tx.InitiatedBy = request.InitiatedBy;
        
        // Process transfer
        tx.Status = TransactionStatus.Completed;
        tx.ProcessedAt = DateTime.UtcNow;
        tx.CompletedAt = DateTime.UtcNow;
        
        _transactions.Add(tx);
        return tx;
    }
    
    public Task<PaymentTransaction> InternalTransferAsync(InternalTransferRequest request)
    {
        var tx = CreateTransaction(request.Amount, request.Currency, TransactionType.Transfer, PaymentMethod.InternalTransfer);
        tx.SourceAccountId = request.SourceAccountId;
        tx.DestinationAccountId = request.DestinationAccountId;
        tx.Department = request.Department;
        tx.Category = request.Category;
        tx.Status = TransactionStatus.Completed;
        tx.ProcessedAt = DateTime.UtcNow;
        tx.CompletedAt = DateTime.UtcNow;
        
        _transactions.Add(tx);
        return Task.FromResult(tx);
    }
    
    public Task<PaymentTransaction> ExternalTransferAsync(ExternalTransferRequest request)
    {
        var tx = CreateTransaction(request.Amount, request.Currency, TransactionType.Transfer, PaymentMethod.Wire);
        tx.SourceAccountId = request.SourceAccountId;
        tx.ExternalReference = $"EXT-{Guid.NewGuid():N}"[..16];
        tx.Fee = request.Speed switch
        {
            TransferSpeed.Instant => request.Amount * 0.01m,
            TransferSpeed.Express => request.Amount * 0.005m,
            _ => request.Amount * 0.001m
        };
        tx.Status = TransactionStatus.Processing;
        tx.ProcessedAt = DateTime.UtcNow;
        
        _transactions.Add(tx);
        return Task.FromResult(tx);
    }
    
    public Task<PaymentTransaction> ProcessPaymentAsync(PaymentRequest request)
    {
        var tx = CreateTransaction(request.Amount, request.Currency, TransactionType.Payment, request.Method);
        tx.SourceAccountId = request.PayerAccountId;
        tx.Description = request.Description;
        tx.RelatedEntityId = request.RelatedEntityId;
        tx.RelatedEntityType = request.RelatedEntityType;
        tx.Status = TransactionStatus.Completed;
        tx.ProcessedAt = DateTime.UtcNow;
        tx.CompletedAt = DateTime.UtcNow;
        
        _transactions.Add(tx);
        return Task.FromResult(tx);
    }
    
    public Task<PaymentTransaction> ProcessBulkPaymentAsync(BulkPaymentRequest request)
    {
        var totalAmount = request.Payments.Sum(p => p.Amount);
        var tx = CreateTransaction(totalAmount, "USD", TransactionType.Payment, PaymentMethod.BankTransfer);
        tx.SourceAccountId = request.SourceAccountId;
        tx.Description = $"Bulk payment: {request.Payments.Count} recipients - {request.Description}";
        tx.Status = TransactionStatus.Completed;
        tx.ProcessedAt = DateTime.UtcNow;
        tx.CompletedAt = DateTime.UtcNow;
        
        _transactions.Add(tx);
        return Task.FromResult(tx);
    }
    
    public Task<PaymentTransaction> ProcessRecurringPaymentAsync(RecurringPaymentRequest request)
    {
        var tx = CreateTransaction(request.Amount, "USD", TransactionType.Payment, PaymentMethod.StandingOrder);
        tx.SourceAccountId = request.SourceAccountId;
        tx.DestinationAccountId = request.DestinationAccountId;
        tx.Description = $"Recurring: {request.Frequency} - {request.Description}";
        tx.Status = TransactionStatus.Completed;
        tx.ProcessedAt = DateTime.UtcNow;
        tx.CompletedAt = DateTime.UtcNow;
        
        _transactions.Add(tx);
        return Task.FromResult(tx);
    }
    
    public Task<PaymentTransaction> RefundAsync(Guid transactionId, decimal? amount = null)
    {
        var original = _transactions.FirstOrDefault(t => t.Id == transactionId);
        if (original == null) throw new Exception("Transaction not found");
        
        var refundAmount = amount ?? original.Amount;
        var tx = CreateTransaction(refundAmount, original.Currency, TransactionType.Refund, original.Method);
        tx.SourceAccountId = original.DestinationAccountId;
        tx.DestinationAccountId = original.SourceAccountId;
        tx.Description = $"Refund for {original.Reference}";
        tx.RelatedEntityId = transactionId;
        tx.Status = TransactionStatus.Completed;
        tx.ProcessedAt = DateTime.UtcNow;
        tx.CompletedAt = DateTime.UtcNow;
        
        _transactions.Add(tx);
        return Task.FromResult(tx);
    }
    
    public Task<PaymentTransaction> PartialRefundAsync(Guid transactionId, decimal amount) =>
        RefundAsync(transactionId, amount);
    
    public Task<PaymentTransaction?> GetTransactionAsync(Guid id) =>
        Task.FromResult(_transactions.FirstOrDefault(t => t.Id == id));
    
    public Task<PaymentTransaction?> GetByReferenceAsync(string reference) =>
        Task.FromResult(_transactions.FirstOrDefault(t => t.Reference == reference));
    
    public Task<List<PaymentTransaction>> GetTransactionsAsync(TransactionFilter filter)
    {
        var query = _transactions.AsQueryable();
        
        if (filter.AccountId.HasValue)
            query = query.Where(t => t.SourceAccountId == filter.AccountId || t.DestinationAccountId == filter.AccountId);
        if (!string.IsNullOrEmpty(filter.Department))
            query = query.Where(t => t.Department == filter.Department);
        if (!string.IsNullOrEmpty(filter.Platform))
            query = query.Where(t => t.Platform == filter.Platform);
        if (filter.Type.HasValue)
            query = query.Where(t => t.Type == filter.Type);
        if (filter.Status.HasValue)
            query = query.Where(t => t.Status == filter.Status);
        if (filter.FromDate.HasValue)
            query = query.Where(t => t.CreatedAt >= filter.FromDate);
        if (filter.ToDate.HasValue)
            query = query.Where(t => t.CreatedAt <= filter.ToDate);
        if (filter.MinAmount.HasValue)
            query = query.Where(t => t.Amount >= filter.MinAmount);
        if (filter.MaxAmount.HasValue)
            query = query.Where(t => t.Amount <= filter.MaxAmount);
        
        return Task.FromResult(query
            .OrderByDescending(t => t.CreatedAt)
            .Skip((filter.Page - 1) * filter.PageSize)
            .Take(filter.PageSize)
            .ToList());
    }
    
    private PaymentTransaction CreateTransaction(decimal amount, string currency, TransactionType type, PaymentMethod method)
    {
        return new PaymentTransaction
        {
            Reference = $"TXN-{Interlocked.Increment(ref _transactionCounter)}",
            Amount = amount,
            Currency = currency,
            Type = type,
            Method = method,
            CreatedAt = DateTime.UtcNow
        };
    }
}

public class DepartmentPaymentService : IDepartmentPaymentService
{
    private readonly List<DepartmentPayment> _payments = new();
    private readonly List<DepartmentBudget> _budgets = InitializeBudgets();
    
    public Task<DepartmentPayment> RequestBudgetAsync(BudgetRequest request)
    {
        var payment = new DepartmentPayment
        {
            Department = request.Department,
            Amount = request.Amount,
            Purpose = request.Purpose,
            BudgetCode = request.BudgetCode,
            RequestedBy = request.RequestedBy,
            Status = DepartmentPaymentStatus.Pending
        };
        _payments.Add(payment);
        return Task.FromResult(payment);
    }
    
    public Task<DepartmentPayment> ApproveBudgetAsync(Guid requestId, string approver)
    {
        var payment = _payments.FirstOrDefault(p => p.Id == requestId);
        if (payment != null)
        {
            payment.Status = DepartmentPaymentStatus.Approved;
            payment.ApprovedBy = approver;
            payment.ApprovedAt = DateTime.UtcNow;
        }
        return Task.FromResult(payment!);
    }
    
    public Task<DepartmentPayment> RejectBudgetAsync(Guid requestId, string reason)
    {
        var payment = _payments.FirstOrDefault(p => p.Id == requestId);
        if (payment != null)
        {
            payment.Status = DepartmentPaymentStatus.Rejected;
            payment.RejectionReason = reason;
        }
        return Task.FromResult(payment!);
    }
    
    public Task<DepartmentPayment> DisburseAsync(Guid requestId)
    {
        var payment = _payments.FirstOrDefault(p => p.Id == requestId);
        if (payment != null && payment.Status == DepartmentPaymentStatus.Approved)
        {
            payment.Status = DepartmentPaymentStatus.Disbursed;
            payment.TransactionId = Guid.NewGuid();
        }
        return Task.FromResult(payment!);
    }
    
    public Task<List<DepartmentPayment>> GetPendingApprovalsAsync() =>
        Task.FromResult(_payments.Where(p => p.Status == DepartmentPaymentStatus.Pending).ToList());
    
    public Task<List<DepartmentPayment>> GetDepartmentHistoryAsync(string department) =>
        Task.FromResult(_payments.Where(p => p.Department == department).OrderByDescending(p => p.CreatedAt).ToList());
    
    public Task<DepartmentBudget> GetBudgetStatusAsync(string department) =>
        Task.FromResult(_budgets.FirstOrDefault(b => b.Department == department) ?? new DepartmentBudget { Department = department });
    
    private static List<DepartmentBudget> InitializeBudgets()
    {
        return new List<DepartmentBudget>
        {
            new() { Department = "Education", FiscalYear = 2024, AllocatedBudget = 50_000_000, UsedBudget = 35_000_000 },
            new() { Department = "Health", FiscalYear = 2024, AllocatedBudget = 75_000_000, UsedBudget = 55_000_000 },
            new() { Department = "Infrastructure", FiscalYear = 2024, AllocatedBudget = 100_000_000, UsedBudget = 60_000_000 },
            new() { Department = "Social Services", FiscalYear = 2024, AllocatedBudget = 30_000_000, UsedBudget = 22_000_000 },
            new() { Department = "Public Safety", FiscalYear = 2024, AllocatedBudget = 25_000_000, UsedBudget = 18_000_000 }
        };
    }
}

public class TreasuryService : ITreasuryService
{
    private readonly List<TreasuryAccount> _accounts = InitializeTreasuryAccounts();
    private readonly List<TreasuryTransaction> _transactions = new();
    
    public Task<TreasuryOverview> GetOverviewAsync()
    {
        return Task.FromResult(new TreasuryOverview
        {
            TotalAssets = 1_500_000_000,
            TotalLiabilities = 250_000_000,
            CashOnHand = 500_000_000,
            Investments = 750_000_000,
            Receivables = 150_000_000,
            Payables = 100_000_000,
            Accounts = _accounts
        });
    }
    
    public Task<List<TreasuryAccount>> GetAccountsAsync() => Task.FromResult(_accounts.ToList());
    
    public Task<TreasuryAccount?> GetAccountAsync(string accountType) =>
        Task.FromResult(_accounts.FirstOrDefault(a => a.Type == accountType));
    
    public Task<TreasuryTransaction> AllocateFundsAsync(AllocationRequest request)
    {
        var tx = new TreasuryTransaction
        {
            Type = "Allocation",
            Amount = request.Amount,
            Purpose = request.Purpose,
            ApprovedBy = request.AuthorizedBy
        };
        _transactions.Add(tx);
        return Task.FromResult(tx);
    }
    
    public Task<TreasuryTransaction> RecallFundsAsync(RecallRequest request)
    {
        var tx = new TreasuryTransaction
        {
            Type = "Recall",
            Amount = request.Amount,
            Purpose = request.Reason,
            ApprovedBy = request.AuthorizedBy
        };
        _transactions.Add(tx);
        return Task.FromResult(tx);
    }
    
    public Task<List<TreasuryTransaction>> GetTransactionsAsync(DateTime from, DateTime to) =>
        Task.FromResult(_transactions.Where(t => t.ProcessedAt >= from && t.ProcessedAt <= to).ToList());
    
    public Task<TreasuryReport> GenerateReportAsync(ReportPeriod period)
    {
        return Task.FromResult(new TreasuryReport
        {
            Period = period,
            OpeningBalance = 1_450_000_000,
            ClosingBalance = 1_500_000_000,
            TotalInflows = 150_000_000,
            TotalOutflows = 100_000_000,
            ByDepartment = new Dictionary<string, decimal>
            {
                ["Education"] = 15_000_000,
                ["Health"] = 20_000_000,
                ["Infrastructure"] = 30_000_000,
                ["Social Services"] = 10_000_000
            },
            ByPlatform = new Dictionary<string, decimal>
            {
                ["ERP"] = 5_000_000,
                ["DeFi"] = 15_000_000,
                ["Hotel"] = 3_000_000,
                ["College"] = 2_000_000
            }
        });
    }
    
    public Task<List<BudgetAllocation>> GetBudgetAllocationsAsync(int fiscalYear)
    {
        return Task.FromResult(new List<BudgetAllocation>
        {
            new() { Department = "Education", FiscalYear = fiscalYear, Allocated = 50_000_000, Spent = 35_000_000 },
            new() { Department = "Health", FiscalYear = fiscalYear, Allocated = 75_000_000, Spent = 55_000_000 },
            new() { Department = "Infrastructure", FiscalYear = fiscalYear, Allocated = 100_000_000, Spent = 60_000_000 },
            new() { Department = "Social Services", FiscalYear = fiscalYear, Allocated = 30_000_000, Spent = 22_000_000 },
            new() { Department = "Public Safety", FiscalYear = fiscalYear, Allocated = 25_000_000, Spent = 18_000_000 }
        });
    }
    
    private static List<TreasuryAccount> InitializeTreasuryAccounts()
    {
        return new List<TreasuryAccount>
        {
            new() { Name = "General Fund", Type = "General", Balance = 500_000_000, Purpose = "Primary operating fund" },
            new() { Name = "Reserve Fund", Type = "Reserve", Balance = 250_000_000, Purpose = "Emergency reserves" },
            new() { Name = "Investment Fund", Type = "Investment", Balance = 750_000_000, Purpose = "Long-term investments" },
            new() { Name = "Capital Projects", Type = "Capital", Balance = 150_000_000, Purpose = "Infrastructure projects" },
            new() { Name = "Debt Service", Type = "Debt", Balance = 50_000_000, Purpose = "Debt payments" }
        };
    }
}

public class MultiCurrencyService : IMultiCurrencyService
{
    private readonly List<Currency> _currencies = InitializeCurrencies();
    private readonly List<ExchangeRate> _rates = InitializeRates();
    
    public Task<List<Currency>> GetSupportedCurrenciesAsync() => Task.FromResult(_currencies.ToList());
    
    public Task<ExchangeRate> GetExchangeRateAsync(string fromCurrency, string toCurrency)
    {
        var rate = _rates.FirstOrDefault(r => r.FromCurrency == fromCurrency && r.ToCurrency == toCurrency);
        return Task.FromResult(rate ?? new ExchangeRate { FromCurrency = fromCurrency, ToCurrency = toCurrency, Rate = 1 });
    }
    
    public Task<List<ExchangeRate>> GetAllRatesAsync(string baseCurrency) =>
        Task.FromResult(_rates.Where(r => r.FromCurrency == baseCurrency).ToList());
    
    public async Task<CurrencyConversion> ConvertAsync(decimal amount, string from, string to)
    {
        var rate = await GetExchangeRateAsync(from, to);
        return new CurrencyConversion
        {
            OriginalAmount = amount,
            OriginalCurrency = from,
            ConvertedAmount = amount * rate.Rate,
            TargetCurrency = to,
            ExchangeRate = rate.Rate
        };
    }
    
    public Task<bool> UpdateRatesAsync()
    {
        // In production, this would fetch from external APIs
        return Task.FromResult(true);
    }
    
    private static List<Currency> InitializeCurrencies()
    {
        return new List<Currency>
        {
            new() { Code = "USD", Name = "US Dollar", Symbol = "$", Decimals = 2 },
            new() { Code = "CAD", Name = "Canadian Dollar", Symbol = "C$", Decimals = 2 },
            new() { Code = "EUR", Name = "Euro", Symbol = "€", Decimals = 2 },
            new() { Code = "MXN", Name = "Mexican Peso", Symbol = "MX$", Decimals = 2 },
            new() { Code = "IGT", Name = "Ierahkwa Governance Token", Symbol = "IGT", Decimals = 18, IsCrypto = true },
            new() { Code = "ETH", Name = "Ethereum", Symbol = "Ξ", Decimals = 18, IsCrypto = true },
            new() { Code = "USDT", Name = "Tether USD", Symbol = "₮", Decimals = 6, IsCrypto = true },
            new() { Code = "BTC", Name = "Bitcoin", Symbol = "₿", Decimals = 8, IsCrypto = true }
        };
    }
    
    private static List<ExchangeRate> InitializeRates()
    {
        return new List<ExchangeRate>
        {
            new() { FromCurrency = "USD", ToCurrency = "CAD", Rate = 1.36m },
            new() { FromCurrency = "USD", ToCurrency = "EUR", Rate = 0.92m },
            new() { FromCurrency = "USD", ToCurrency = "MXN", Rate = 17.15m },
            new() { FromCurrency = "USD", ToCurrency = "IGT", Rate = 0.40m },
            new() { FromCurrency = "USD", ToCurrency = "ETH", Rate = 0.0004m },
            new() { FromCurrency = "IGT", ToCurrency = "USD", Rate = 2.50m },
            new() { FromCurrency = "ETH", ToCurrency = "USD", Rate = 2500m }
        };
    }
}
