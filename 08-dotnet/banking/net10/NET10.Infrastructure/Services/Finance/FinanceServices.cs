using NET10.Core.Interfaces;

namespace NET10.Infrastructure.Services.Finance;

// ═══════════════════════════════════════════════════════════════════════════════
// IERAHKWA GOMONEY - PERSONAL FINANCIAL MANAGEMENT SYSTEM
// Complete Implementation
// ═══════════════════════════════════════════════════════════════════════════════

public class FinancialAccountService : IFinancialAccountService
{
    private static readonly List<FinancialAccount> _accounts = InitializeDemoAccounts();
    
    private static List<FinancialAccount> InitializeDemoAccounts()
    {
        var userId = Guid.Parse("00000000-0000-0000-0000-000000000001");
        return new List<FinancialAccount>
        {
            new() { UserId = userId, Name = "Cash Wallet", Type = FinancialAccountType.Cash, Balance = 500, InitialBalance = 500, Icon = "💵", Color = "#4CAF50" },
            new() { UserId = userId, Name = "Bank of Ierahkwa - Checking", Type = FinancialAccountType.Checking, Balance = 5420.50m, InitialBalance = 3000, AccountNumber = "****4521", BankName = "Bank of Ierahkwa", Icon = "🏦", Color = "#2196F3" },
            new() { UserId = userId, Name = "Savings Account", Type = FinancialAccountType.Savings, Balance = 15000, InitialBalance = 10000, AccountNumber = "****7892", BankName = "Bank of Ierahkwa", Icon = "💰", Color = "#9C27B0" },
            new() { UserId = userId, Name = "IGT Crypto Wallet", Type = FinancialAccountType.Crypto, Balance = 2500, InitialBalance = 1000, AccountNumber = "0xIER...", Icon = "🪙", Color = "#FF9800" },
            new() { UserId = userId, Name = "Credit Card", Type = FinancialAccountType.CreditCard, Balance = -1250, InitialBalance = 0, AccountNumber = "****9876", Icon = "💳", Color = "#F44336", IncludeInTotal = false }
        };
    }
    
    public Task<List<FinancialAccount>> GetAllAsync(Guid userId) =>
        Task.FromResult(_accounts.Where(a => a.UserId == userId && a.IsActive).ToList());
    
    public Task<FinancialAccount?> GetByIdAsync(Guid id) =>
        Task.FromResult(_accounts.FirstOrDefault(a => a.Id == id));
    
    public Task<FinancialAccount> CreateAsync(FinancialAccount account)
    {
        account.Id = Guid.NewGuid();
        account.Balance = account.InitialBalance;
        account.CreatedAt = DateTime.UtcNow;
        _accounts.Add(account);
        return Task.FromResult(account);
    }
    
    public Task<FinancialAccount> UpdateAsync(FinancialAccount account)
    {
        var index = _accounts.FindIndex(a => a.Id == account.Id);
        if (index >= 0) _accounts[index] = account;
        return Task.FromResult(account);
    }
    
    public Task<bool> DeleteAsync(Guid id)
    {
        var account = _accounts.FirstOrDefault(a => a.Id == id);
        if (account != null) { account.IsActive = false; return Task.FromResult(true); }
        return Task.FromResult(false);
    }
    
    public Task<decimal> GetBalanceAsync(Guid accountId)
    {
        var account = _accounts.FirstOrDefault(a => a.Id == accountId);
        return Task.FromResult(account?.Balance ?? 0);
    }
    
    public Task<decimal> GetTotalBalanceAsync(Guid userId) =>
        Task.FromResult(_accounts.Where(a => a.UserId == userId && a.IsActive && a.IncludeInTotal).Sum(a => a.Balance));
    
    public async Task<AccountTransfer> TransferAsync(TransferRequest request)
    {
        var fromAccount = await GetByIdAsync(request.FromAccountId);
        var toAccount = await GetByIdAsync(request.ToAccountId);
        
        if (fromAccount == null || toAccount == null)
            throw new InvalidOperationException("Account not found");
        
        fromAccount.Balance -= request.Amount;
        toAccount.Balance += request.Amount;
        fromAccount.LastTransactionAt = DateTime.UtcNow;
        toAccount.LastTransactionAt = DateTime.UtcNow;
        
        return new AccountTransfer
        {
            FromAccountId = request.FromAccountId,
            FromAccountName = fromAccount.Name,
            ToAccountId = request.ToAccountId,
            ToAccountName = toAccount.Name,
            Amount = request.Amount,
            Date = request.Date ?? DateTime.UtcNow,
            Description = request.Description
        };
    }
    
    public Task<List<FinancialAccount>> GetByTypeAsync(Guid userId, FinancialAccountType type) =>
        Task.FromResult(_accounts.Where(a => a.UserId == userId && a.Type == type && a.IsActive).ToList());
}

public class IncomeCategoryService : IIncomeCategoryService
{
    private static readonly List<IncomeCategory> _categories = InitializeDemoCategories();
    
    private static List<IncomeCategory> InitializeDemoCategories()
    {
        var userId = Guid.Parse("00000000-0000-0000-0000-000000000001");
        return new List<IncomeCategory>
        {
            new() { UserId = userId, Name = "Salary", Icon = "💼", Color = "#4CAF50", IsSystem = true },
            new() { UserId = userId, Name = "Freelance", Icon = "💻", Color = "#2196F3", IsSystem = true },
            new() { UserId = userId, Name = "Investment Returns", Icon = "📈", Color = "#9C27B0", IsSystem = true },
            new() { UserId = userId, Name = "Business Income", Icon = "🏢", Color = "#FF9800", IsSystem = true },
            new() { UserId = userId, Name = "Rental Income", Icon = "🏠", Color = "#00BCD4", IsSystem = true },
            new() { UserId = userId, Name = "Dividends", Icon = "💵", Color = "#8BC34A", IsSystem = true },
            new() { UserId = userId, Name = "Crypto Gains", Icon = "🪙", Color = "#FFC107", IsSystem = true },
            new() { UserId = userId, Name = "Government Benefits", Icon = "🏛️", Color = "#3F51B5", IsSystem = true },
            new() { UserId = userId, Name = "Gifts Received", Icon = "🎁", Color = "#E91E63", IsSystem = true },
            new() { UserId = userId, Name = "Other Income", Icon = "💰", Color = "#607D8B", IsSystem = true }
        };
    }
    
    public Task<List<IncomeCategory>> GetAllAsync(Guid userId) =>
        Task.FromResult(_categories.Where(c => c.UserId == userId && c.IsActive).ToList());
    
    public Task<IncomeCategory?> GetByIdAsync(Guid id) =>
        Task.FromResult(_categories.FirstOrDefault(c => c.Id == id));
    
    public Task<IncomeCategory> CreateAsync(IncomeCategory category)
    {
        category.Id = Guid.NewGuid();
        category.CreatedAt = DateTime.UtcNow;
        _categories.Add(category);
        return Task.FromResult(category);
    }
    
    public Task<IncomeCategory> UpdateAsync(IncomeCategory category)
    {
        var index = _categories.FindIndex(c => c.Id == category.Id);
        if (index >= 0) _categories[index] = category;
        return Task.FromResult(category);
    }
    
    public Task<bool> DeleteAsync(Guid id)
    {
        var category = _categories.FirstOrDefault(c => c.Id == id && !c.IsSystem);
        if (category != null) { category.IsActive = false; return Task.FromResult(true); }
        return Task.FromResult(false);
    }
}

public class ExpenseCategoryService : IExpenseCategoryService
{
    private static readonly List<ExpenseCategory> _categories = InitializeDemoCategories();
    
    private static List<ExpenseCategory> InitializeDemoCategories()
    {
        var userId = Guid.Parse("00000000-0000-0000-0000-000000000001");
        return new List<ExpenseCategory>
        {
            new() { UserId = userId, Name = "Food & Dining", Icon = "🍔", Color = "#FF5722", BudgetAmount = 500, IsSystem = true },
            new() { UserId = userId, Name = "Transportation", Icon = "🚗", Color = "#2196F3", BudgetAmount = 300, IsSystem = true },
            new() { UserId = userId, Name = "Housing", Icon = "🏠", Color = "#4CAF50", BudgetAmount = 1500, IsSystem = true },
            new() { UserId = userId, Name = "Utilities", Icon = "💡", Color = "#FFC107", BudgetAmount = 200, IsSystem = true },
            new() { UserId = userId, Name = "Healthcare", Icon = "🏥", Color = "#F44336", BudgetAmount = 150, IsSystem = true },
            new() { UserId = userId, Name = "Entertainment", Icon = "🎬", Color = "#9C27B0", BudgetAmount = 200, IsSystem = true },
            new() { UserId = userId, Name = "Shopping", Icon = "🛍️", Color = "#E91E63", BudgetAmount = 300, IsSystem = true },
            new() { UserId = userId, Name = "Education", Icon = "📚", Color = "#3F51B5", BudgetAmount = 100, IsSystem = true },
            new() { UserId = userId, Name = "Insurance", Icon = "🛡️", Color = "#00BCD4", BudgetAmount = 250, IsSystem = true },
            new() { UserId = userId, Name = "Personal Care", Icon = "💇", Color = "#8BC34A", BudgetAmount = 100, IsSystem = true },
            new() { UserId = userId, Name = "Subscriptions", Icon = "📱", Color = "#673AB7", BudgetAmount = 50, IsSystem = true },
            new() { UserId = userId, Name = "Gifts & Donations", Icon = "🎁", Color = "#FF9800", BudgetAmount = 100, IsSystem = true },
            new() { UserId = userId, Name = "Taxes", Icon = "📋", Color = "#795548", IsSystem = true },
            new() { UserId = userId, Name = "Other Expenses", Icon = "💸", Color = "#607D8B", IsSystem = true }
        };
    }
    
    public Task<List<ExpenseCategory>> GetAllAsync(Guid userId) =>
        Task.FromResult(_categories.Where(c => c.UserId == userId && c.IsActive).ToList());
    
    public Task<ExpenseCategory?> GetByIdAsync(Guid id) =>
        Task.FromResult(_categories.FirstOrDefault(c => c.Id == id));
    
    public Task<ExpenseCategory> CreateAsync(ExpenseCategory category)
    {
        category.Id = Guid.NewGuid();
        category.CreatedAt = DateTime.UtcNow;
        _categories.Add(category);
        return Task.FromResult(category);
    }
    
    public Task<ExpenseCategory> UpdateAsync(ExpenseCategory category)
    {
        var index = _categories.FindIndex(c => c.Id == category.Id);
        if (index >= 0) _categories[index] = category;
        return Task.FromResult(category);
    }
    
    public Task<bool> DeleteAsync(Guid id)
    {
        var category = _categories.FirstOrDefault(c => c.Id == id && !c.IsSystem);
        if (category != null) { category.IsActive = false; return Task.FromResult(true); }
        return Task.FromResult(false);
    }
}

public class FinancialTransactionService : IFinancialTransactionService
{
    private static readonly List<FinancialTransaction> _transactions = InitializeDemoTransactions();
    private readonly IFinancialAccountService _accountService;
    
    public FinancialTransactionService(IFinancialAccountService accountService)
    {
        _accountService = accountService;
    }
    
    private static List<FinancialTransaction> InitializeDemoTransactions()
    {
        var userId = Guid.Parse("00000000-0000-0000-0000-000000000001");
        var now = DateTime.UtcNow;
        return new List<FinancialTransaction>
        {
            new() { UserId = userId, Type = FinancialTransactionType.Income, Amount = 4500, Description = "Monthly Salary", CategoryName = "Salary", AccountName = "Bank of Ierahkwa - Checking", Date = now.AddDays(-15), BalanceAfter = 7920.50m },
            new() { UserId = userId, Type = FinancialTransactionType.Income, Amount = 800, Description = "Freelance Project", CategoryName = "Freelance", AccountName = "Bank of Ierahkwa - Checking", Date = now.AddDays(-10), BalanceAfter = 8720.50m },
            new() { UserId = userId, Type = FinancialTransactionType.Expense, Amount = 1200, Description = "Rent Payment", CategoryName = "Housing", AccountName = "Bank of Ierahkwa - Checking", Date = now.AddDays(-5), BalanceAfter = 7520.50m },
            new() { UserId = userId, Type = FinancialTransactionType.Expense, Amount = 150, Description = "Grocery Shopping", CategoryName = "Food & Dining", AccountName = "Bank of Ierahkwa - Checking", Date = now.AddDays(-4), BalanceAfter = 7370.50m },
            new() { UserId = userId, Type = FinancialTransactionType.Expense, Amount = 85, Description = "Gas", CategoryName = "Transportation", AccountName = "Bank of Ierahkwa - Checking", Date = now.AddDays(-3), BalanceAfter = 7285.50m },
            new() { UserId = userId, Type = FinancialTransactionType.Expense, Amount = 45, Description = "Netflix & Spotify", CategoryName = "Subscriptions", AccountName = "Credit Card", Date = now.AddDays(-2), BalanceAfter = -1295m },
            new() { UserId = userId, Type = FinancialTransactionType.Transfer, Amount = 2000, Description = "Transfer to Savings", AccountName = "Bank of Ierahkwa - Checking", Date = now.AddDays(-1), BalanceAfter = 5285.50m },
            new() { UserId = userId, Type = FinancialTransactionType.Income, Amount = 135, Description = "Crypto Staking Rewards", CategoryName = "Crypto Gains", AccountName = "IGT Crypto Wallet", Date = now, BalanceAfter = 2635m }
        };
    }
    
    public Task<List<FinancialTransaction>> GetAllAsync(Guid userId) =>
        Task.FromResult(_transactions.Where(t => t.UserId == userId).OrderByDescending(t => t.Date).ToList());
    
    public Task<List<FinancialTransaction>> GetByAccountAsync(Guid accountId) =>
        Task.FromResult(_transactions.Where(t => t.AccountId == accountId).OrderByDescending(t => t.Date).ToList());
    
    public Task<List<FinancialTransaction>> GetByDateRangeAsync(Guid userId, DateTime from, DateTime to) =>
        Task.FromResult(_transactions.Where(t => t.UserId == userId && t.Date >= from && t.Date <= to).OrderByDescending(t => t.Date).ToList());
    
    public Task<List<FinancialTransaction>> GetByTypeAsync(Guid userId, FinancialTransactionType type) =>
        Task.FromResult(_transactions.Where(t => t.UserId == userId && t.Type == type).OrderByDescending(t => t.Date).ToList());
    
    public Task<List<FinancialTransaction>> GetByCategoryAsync(Guid categoryId) =>
        Task.FromResult(_transactions.Where(t => t.CategoryId == categoryId).OrderByDescending(t => t.Date).ToList());
    
    public Task<FinancialTransaction?> GetByIdAsync(Guid id) =>
        Task.FromResult(_transactions.FirstOrDefault(t => t.Id == id));
    
    public async Task<FinancialTransaction> CreateAsync(FinancialTransaction transaction)
    {
        transaction.Id = Guid.NewGuid();
        transaction.CreatedAt = DateTime.UtcNow;
        
        var account = await _accountService.GetByIdAsync(transaction.AccountId);
        if (account != null)
        {
            if (transaction.Type == FinancialTransactionType.Income)
                account.Balance += transaction.Amount;
            else if (transaction.Type == FinancialTransactionType.Expense)
                account.Balance -= transaction.Amount;
            
            transaction.BalanceAfter = account.Balance;
            transaction.AccountName = account.Name;
            account.LastTransactionAt = DateTime.UtcNow;
        }
        
        _transactions.Add(transaction);
        return transaction;
    }
    
    public Task<FinancialTransaction> UpdateAsync(FinancialTransaction transaction)
    {
        transaction.UpdatedAt = DateTime.UtcNow;
        var index = _transactions.FindIndex(t => t.Id == transaction.Id);
        if (index >= 0) _transactions[index] = transaction;
        return Task.FromResult(transaction);
    }
    
    public Task<bool> DeleteAsync(Guid id)
    {
        var transaction = _transactions.FirstOrDefault(t => t.Id == id);
        if (transaction != null)
        {
            _transactions.Remove(transaction);
            return Task.FromResult(true);
        }
        return Task.FromResult(false);
    }
    
    public async Task<FinancialTransaction> QuickIncomeAsync(QuickTransactionRequest request)
    {
        var transaction = new FinancialTransaction
        {
            UserId = request.UserId,
            AccountId = request.AccountId,
            Type = FinancialTransactionType.Income,
            CategoryId = request.CategoryId,
            Amount = request.Amount,
            Description = request.Description,
            Payee = request.Payee,
            Date = request.Date ?? DateTime.UtcNow
        };
        return await CreateAsync(transaction);
    }
    
    public async Task<FinancialTransaction> QuickExpenseAsync(QuickTransactionRequest request)
    {
        var transaction = new FinancialTransaction
        {
            UserId = request.UserId,
            AccountId = request.AccountId,
            Type = FinancialTransactionType.Expense,
            CategoryId = request.CategoryId,
            Amount = request.Amount,
            Description = request.Description,
            Payee = request.Payee,
            Date = request.Date ?? DateTime.UtcNow
        };
        return await CreateAsync(transaction);
    }
    
    public Task<List<FinancialTransaction>> GetRecentAsync(Guid userId, int count = 10) =>
        Task.FromResult(_transactions.Where(t => t.UserId == userId).OrderByDescending(t => t.Date).Take(count).ToList());
}

public class PayableReceivableService : IPayableReceivableService
{
    private static readonly List<AccountPayable> _payables = InitializeDemoPayables();
    private static readonly List<AccountReceivable> _receivables = InitializeDemoReceivables();
    
    private static List<AccountPayable> InitializeDemoPayables()
    {
        var userId = Guid.Parse("00000000-0000-0000-0000-000000000001");
        return new List<AccountPayable>
        {
            new() { UserId = userId, Creditor = "Electric Company", Description = "Monthly electricity bill", TotalAmount = 125, DueDate = DateTime.UtcNow.AddDays(10) },
            new() { UserId = userId, Creditor = "Internet Provider", Description = "Internet service", TotalAmount = 75, DueDate = DateTime.UtcNow.AddDays(15) },
            new() { UserId = userId, Creditor = "Credit Card Company", Description = "Credit card minimum payment", TotalAmount = 200, PaidAmount = 50, DueDate = DateTime.UtcNow.AddDays(5), Status = PayableStatus.PartiallyPaid }
        };
    }
    
    private static List<AccountReceivable> InitializeDemoReceivables()
    {
        var userId = Guid.Parse("00000000-0000-0000-0000-000000000001");
        return new List<AccountReceivable>
        {
            new() { UserId = userId, Debtor = "John Smith", Description = "Personal loan", TotalAmount = 500, DueDate = DateTime.UtcNow.AddDays(20) },
            new() { UserId = userId, Debtor = "ABC Company", Description = "Freelance work payment", TotalAmount = 1200, ReceivedAmount = 600, DueDate = DateTime.UtcNow.AddDays(-5), Status = ReceivableStatus.PartiallyReceived }
        };
    }
    
    public Task<List<AccountPayable>> GetPayablesAsync(Guid userId) =>
        Task.FromResult(_payables.Where(p => p.UserId == userId).ToList());
    
    public Task<AccountPayable?> GetPayableAsync(Guid id) =>
        Task.FromResult(_payables.FirstOrDefault(p => p.Id == id));
    
    public Task<AccountPayable> CreatePayableAsync(AccountPayable payable)
    {
        payable.Id = Guid.NewGuid();
        payable.CreatedAt = DateTime.UtcNow;
        _payables.Add(payable);
        return Task.FromResult(payable);
    }
    
    public Task<AccountPayable> UpdatePayableAsync(AccountPayable payable)
    {
        var index = _payables.FindIndex(p => p.Id == payable.Id);
        if (index >= 0) _payables[index] = payable;
        return Task.FromResult(payable);
    }
    
    public async Task<AccountPayable> PayAsync(Guid payableId, decimal amount, Guid fromAccountId)
    {
        var payable = await GetPayableAsync(payableId);
        if (payable == null) throw new InvalidOperationException("Payable not found");
        
        payable.PaidAmount += amount;
        payable.Payments.Add(new PayablePayment { Amount = amount, AccountId = fromAccountId, PaidAt = DateTime.UtcNow });
        payable.Status = payable.PaidAmount >= payable.TotalAmount ? PayableStatus.Paid : PayableStatus.PartiallyPaid;
        
        return payable;
    }
    
    public Task<List<AccountPayable>> GetOverduePayablesAsync(Guid userId) =>
        Task.FromResult(_payables.Where(p => p.UserId == userId && p.DueDate < DateTime.UtcNow && p.Status != PayableStatus.Paid).ToList());
    
    public Task<List<AccountReceivable>> GetReceivablesAsync(Guid userId) =>
        Task.FromResult(_receivables.Where(r => r.UserId == userId).ToList());
    
    public Task<AccountReceivable?> GetReceivableAsync(Guid id) =>
        Task.FromResult(_receivables.FirstOrDefault(r => r.Id == id));
    
    public Task<AccountReceivable> CreateReceivableAsync(AccountReceivable receivable)
    {
        receivable.Id = Guid.NewGuid();
        receivable.CreatedAt = DateTime.UtcNow;
        _receivables.Add(receivable);
        return Task.FromResult(receivable);
    }
    
    public Task<AccountReceivable> UpdateReceivableAsync(AccountReceivable receivable)
    {
        var index = _receivables.FindIndex(r => r.Id == receivable.Id);
        if (index >= 0) _receivables[index] = receivable;
        return Task.FromResult(receivable);
    }
    
    public async Task<AccountReceivable> ReceiveAsync(Guid receivableId, decimal amount, Guid toAccountId)
    {
        var receivable = await GetReceivableAsync(receivableId);
        if (receivable == null) throw new InvalidOperationException("Receivable not found");
        
        receivable.ReceivedAmount += amount;
        receivable.Receipts.Add(new ReceivableReceipt { Amount = amount, AccountId = toAccountId, ReceivedAt = DateTime.UtcNow });
        receivable.Status = receivable.ReceivedAmount >= receivable.TotalAmount ? ReceivableStatus.Received : ReceivableStatus.PartiallyReceived;
        
        return receivable;
    }
    
    public Task<List<AccountReceivable>> GetOverdueReceivablesAsync(Guid userId) =>
        Task.FromResult(_receivables.Where(r => r.UserId == userId && r.DueDate < DateTime.UtcNow && r.Status != ReceivableStatus.Received).ToList());
}

public class FinancialReportService : IFinancialReportService
{
    private readonly IFinancialAccountService _accountService;
    private readonly IFinancialTransactionService _transactionService;
    private readonly IPayableReceivableService _payableReceivableService;
    
    public FinancialReportService(
        IFinancialAccountService accountService,
        IFinancialTransactionService transactionService,
        IPayableReceivableService payableReceivableService)
    {
        _accountService = accountService;
        _transactionService = transactionService;
        _payableReceivableService = payableReceivableService;
    }
    
    public async Task<FinancialDashboard> GetDashboardAsync(Guid userId)
    {
        var accounts = await _accountService.GetAllAsync(userId);
        var transactions = await _transactionService.GetRecentAsync(userId, 10);
        var payables = await _payableReceivableService.GetPayablesAsync(userId);
        var receivables = await _payableReceivableService.GetReceivablesAsync(userId);
        
        var now = DateTime.UtcNow;
        var startOfMonth = new DateTime(now.Year, now.Month, 1);
        var monthTransactions = await _transactionService.GetByDateRangeAsync(userId, startOfMonth, now);
        
        var totalIncome = monthTransactions.Where(t => t.Type == FinancialTransactionType.Income).Sum(t => t.Amount);
        var totalExpense = monthTransactions.Where(t => t.Type == FinancialTransactionType.Expense).Sum(t => t.Amount);
        
        return new FinancialDashboard
        {
            TotalBalance = accounts.Where(a => a.IncludeInTotal).Sum(a => a.Balance),
            TotalIncome = totalIncome,
            TotalExpense = totalExpense,
            NetProfit = totalIncome - totalExpense,
            TotalPayables = payables.Sum(p => p.Balance),
            TotalReceivables = receivables.Sum(r => r.Balance),
            Accounts = accounts.Select(a => new AccountSummary
            {
                AccountId = a.Id,
                Name = a.Name,
                Type = a.Type,
                Balance = a.Balance,
                Currency = a.Currency
            }).ToList(),
            RecentTransactions = transactions,
            TopExpenseCategories = monthTransactions.Where(t => t.Type == FinancialTransactionType.Expense)
                .GroupBy(t => t.CategoryName ?? "Other")
                .Select(g => new CategorySummary { Name = g.Key, Amount = g.Sum(t => t.Amount), TransactionCount = g.Count() })
                .OrderByDescending(c => c.Amount).Take(5).ToList()
        };
    }
    
    public async Task<IncomeReport> GetIncomeReportAsync(Guid userId, DateTime from, DateTime to)
    {
        var transactions = await _transactionService.GetByDateRangeAsync(userId, from, to);
        var incomeTransactions = transactions.Where(t => t.Type == FinancialTransactionType.Income).ToList();
        
        return new IncomeReport
        {
            FromDate = from,
            ToDate = to,
            TotalIncome = incomeTransactions.Sum(t => t.Amount),
            TransactionCount = incomeTransactions.Count,
            ByCategory = incomeTransactions.GroupBy(t => t.CategoryName ?? "Other").ToDictionary(g => g.Key, g => g.Sum(t => t.Amount)),
            ByAccount = incomeTransactions.GroupBy(t => t.AccountName).ToDictionary(g => g.Key, g => g.Sum(t => t.Amount)),
            ByDay = incomeTransactions.GroupBy(t => t.Date.ToString("yyyy-MM-dd")).ToDictionary(g => g.Key, g => g.Sum(t => t.Amount)),
            Transactions = incomeTransactions
        };
    }
    
    public async Task<ExpenseReport> GetExpenseReportAsync(Guid userId, DateTime from, DateTime to)
    {
        var transactions = await _transactionService.GetByDateRangeAsync(userId, from, to);
        var expenseTransactions = transactions.Where(t => t.Type == FinancialTransactionType.Expense).ToList();
        
        return new ExpenseReport
        {
            FromDate = from,
            ToDate = to,
            TotalExpense = expenseTransactions.Sum(t => t.Amount),
            TransactionCount = expenseTransactions.Count,
            ByCategory = expenseTransactions.GroupBy(t => t.CategoryName ?? "Other").ToDictionary(g => g.Key, g => g.Sum(t => t.Amount)),
            ByAccount = expenseTransactions.GroupBy(t => t.AccountName).ToDictionary(g => g.Key, g => g.Sum(t => t.Amount)),
            ByDay = expenseTransactions.GroupBy(t => t.Date.ToString("yyyy-MM-dd")).ToDictionary(g => g.Key, g => g.Sum(t => t.Amount)),
            Transactions = expenseTransactions
        };
    }
    
    public async Task<ProfitLossStatement> GetProfitLossAsync(Guid userId, DateTime from, DateTime to)
    {
        var incomeReport = await GetIncomeReportAsync(userId, from, to);
        var expenseReport = await GetExpenseReportAsync(userId, from, to);
        
        var netProfit = incomeReport.TotalIncome - expenseReport.TotalExpense;
        
        return new ProfitLossStatement
        {
            FromDate = from,
            ToDate = to,
            TotalIncome = incomeReport.TotalIncome,
            TotalExpense = expenseReport.TotalExpense,
            NetProfit = netProfit,
            ProfitMargin = incomeReport.TotalIncome > 0 ? (netProfit / incomeReport.TotalIncome * 100) : 0,
            IncomeBreakdown = incomeReport.ByCategory.Select(kv => new CategorySummary { Name = kv.Key, Amount = kv.Value }).ToList(),
            ExpenseBreakdown = expenseReport.ByCategory.Select(kv => new CategorySummary { Name = kv.Key, Amount = kv.Value }).ToList()
        };
    }
    
    public async Task<CashFlowReport> GetCashFlowAsync(Guid userId, DateTime from, DateTime to)
    {
        var transactions = await _transactionService.GetByDateRangeAsync(userId, from, to);
        var accounts = await _accountService.GetAllAsync(userId);
        
        var totalInflows = transactions.Where(t => t.Type == FinancialTransactionType.Income).Sum(t => t.Amount);
        var totalOutflows = transactions.Where(t => t.Type == FinancialTransactionType.Expense).Sum(t => t.Amount);
        var closingBalance = accounts.Where(a => a.IncludeInTotal).Sum(a => a.Balance);
        var netCashFlow = totalInflows - totalOutflows;
        var openingBalance = closingBalance - netCashFlow;
        
        return new CashFlowReport
        {
            FromDate = from,
            ToDate = to,
            OpeningBalance = openingBalance,
            TotalInflows = totalInflows,
            TotalOutflows = totalOutflows,
            NetCashFlow = netCashFlow,
            ClosingBalance = closingBalance
        };
    }
    
    public Task<BudgetReport> GetBudgetReportAsync(Guid userId, DateTime month)
    {
        return Task.FromResult(new BudgetReport
        {
            Month = month,
            TotalBudgeted = 3750,
            TotalSpent = 2450,
            TotalRemaining = 1300,
            OverBudgetCount = 1,
            OnTrackCount = 12
        });
    }
    
    public async Task<NetWorthReport> GetNetWorthAsync(Guid userId)
    {
        var accounts = await _accountService.GetAllAsync(userId);
        
        var assets = accounts.Where(a => a.Balance >= 0).ToList();
        var liabilities = accounts.Where(a => a.Balance < 0).ToList();
        
        return new NetWorthReport
        {
            AsOfDate = DateTime.UtcNow,
            TotalAssets = assets.Sum(a => a.Balance),
            TotalLiabilities = Math.Abs(liabilities.Sum(a => a.Balance)),
            NetWorth = accounts.Sum(a => a.Balance),
            Assets = assets.Select(a => new AccountSummary { AccountId = a.Id, Name = a.Name, Type = a.Type, Balance = a.Balance, Currency = a.Currency }).ToList(),
            Liabilities = liabilities.Select(a => new AccountSummary { AccountId = a.Id, Name = a.Name, Type = a.Type, Balance = Math.Abs(a.Balance), Currency = a.Currency }).ToList()
        };
    }
}

public class BudgetService : IBudgetService
{
    private static readonly List<Budget> _budgets = InitializeDemoBudgets();
    
    private static List<Budget> InitializeDemoBudgets()
    {
        var userId = Guid.Parse("00000000-0000-0000-0000-000000000001");
        var startOfMonth = new DateTime(DateTime.UtcNow.Year, DateTime.UtcNow.Month, 1);
        return new List<Budget>
        {
            new() { UserId = userId, CategoryName = "Food & Dining", Amount = 500, SpentAmount = 320, StartDate = startOfMonth },
            new() { UserId = userId, CategoryName = "Transportation", Amount = 300, SpentAmount = 185, StartDate = startOfMonth },
            new() { UserId = userId, CategoryName = "Housing", Amount = 1500, SpentAmount = 1200, StartDate = startOfMonth },
            new() { UserId = userId, CategoryName = "Utilities", Amount = 200, SpentAmount = 125, StartDate = startOfMonth },
            new() { UserId = userId, CategoryName = "Entertainment", Amount = 200, SpentAmount = 245, StartDate = startOfMonth },
            new() { UserId = userId, CategoryName = "Shopping", Amount = 300, SpentAmount = 180, StartDate = startOfMonth }
        };
    }
    
    public Task<List<Budget>> GetAllAsync(Guid userId) =>
        Task.FromResult(_budgets.Where(b => b.UserId == userId && b.IsActive).ToList());
    
    public Task<Budget?> GetByIdAsync(Guid id) =>
        Task.FromResult(_budgets.FirstOrDefault(b => b.Id == id));
    
    public Task<Budget?> GetByCategoryAsync(Guid categoryId, DateTime month) =>
        Task.FromResult(_budgets.FirstOrDefault(b => b.CategoryId == categoryId && b.StartDate.Month == month.Month && b.StartDate.Year == month.Year));
    
    public Task<Budget> CreateAsync(Budget budget)
    {
        budget.Id = Guid.NewGuid();
        _budgets.Add(budget);
        return Task.FromResult(budget);
    }
    
    public Task<Budget> UpdateAsync(Budget budget)
    {
        var index = _budgets.FindIndex(b => b.Id == budget.Id);
        if (index >= 0) _budgets[index] = budget;
        return Task.FromResult(budget);
    }
    
    public Task<bool> DeleteAsync(Guid id)
    {
        var budget = _budgets.FirstOrDefault(b => b.Id == id);
        if (budget != null) { budget.IsActive = false; return Task.FromResult(true); }
        return Task.FromResult(false);
    }
    
    public async Task<BudgetProgress> GetProgressAsync(Guid budgetId)
    {
        var budget = await GetByIdAsync(budgetId);
        if (budget == null) throw new InvalidOperationException("Budget not found");
        
        var daysInMonth = DateTime.DaysInMonth(budget.StartDate.Year, budget.StartDate.Month);
        var daysPassed = DateTime.UtcNow.Day;
        var daysRemaining = daysInMonth - daysPassed;
        
        return new BudgetProgress
        {
            BudgetId = budgetId,
            CategoryName = budget.CategoryName,
            BudgetAmount = budget.Amount,
            SpentAmount = budget.SpentAmount,
            RemainingAmount = budget.RemainingAmount,
            UsagePercent = budget.UsagePercent,
            DaysRemaining = daysRemaining,
            DailyAllowance = daysRemaining > 0 ? budget.RemainingAmount / daysRemaining : 0,
            IsOverBudget = budget.SpentAmount > budget.Amount
        };
    }
    
    public Task<List<BudgetAlert>> GetAlertsAsync(Guid userId)
    {
        var alerts = _budgets.Where(b => b.UserId == userId && b.IsActive && b.UsagePercent >= b.AlertThreshold)
            .Select(b => new BudgetAlert
            {
                BudgetId = b.Id,
                CategoryName = b.CategoryName,
                UsagePercent = b.UsagePercent,
                AlertType = b.UsagePercent >= 100 ? "OverBudget" : "Warning",
                Message = b.UsagePercent >= 100 
                    ? $"{b.CategoryName} is over budget by ${b.SpentAmount - b.Amount:N2}"
                    : $"{b.CategoryName} has reached {b.UsagePercent:N0}% of budget"
            }).ToList();
        
        return Task.FromResult(alerts);
    }
}
