using System.Security.Cryptography;
using IERAHKWA.Platform.Models;

namespace IERAHKWA.Platform.Services;

public interface IBankService
{
    Task<BankAccount> CreateAccountAsync(CreateAccountRequest request);
    Task<BankAccount?> GetAccountAsync(string id);
    Task<List<BankAccount>> GetUserAccountsAsync(string userId);
    Task<BankTransfer> InternalTransferAsync(InternalTransferRequest request);
    Task<BankTransfer> SwiftTransferAsync(SwiftTransferRequest request);
    Task<BankInfo> GetBankInfoAsync();
    Task<List<ExchangeRate>> GetExchangeRatesAsync();
    Task<ExchangeResult> ExchangeAsync(ExchangeRequest request);
    Task<List<BankMovement>> GetMovementsAsync(string accountId, int page, int limit);
    Task<AccountStatement> GenerateStatementAsync(string accountId, DateTime? from, DateTime? to);
}

public class BankService : IBankService
{
    private readonly ILogger<BankService> _logger;
    private static readonly List<BankAccount> _accounts = new();
    private static readonly List<BankTransfer> _transfers = new();
    private static readonly List<BankMovement> _movements = new();
    private static readonly object _lock = new();
    private static int _accountCounter = 1000000;
    private static int _transferCounter = 100000;

    public BankService(ILogger<BankService> logger)
    {
        _logger = logger;
        InitializeSampleData();
    }

    private void InitializeSampleData()
    {
        lock (_lock)
        {
            if (_accounts.Count == 0)
            {
                _accounts.AddRange(new[]
                {
                    new BankAccount
                    {
                        Id = "acct_pm_001",
                        AccountNumber = "BDET-1000001",
                        IBAN = "IE89BDET00010000001001",
                        UserId = "pm_chief",
                        Name = "Prime Minister Treasury",
                        Type = "treasury",
                        Currency = "USD",
                        Balance = 50000000,
                        AvailableBalance = 50000000,
                        Status = "active"
                    },
                    new BankAccount
                    {
                        Id = "acct_gov_001",
                        AccountNumber = "BDET-1000002",
                        IBAN = "IE89BDET00010000001002",
                        UserId = "gov_operations",
                        Name = "Government Operations",
                        Type = "business",
                        Currency = "USD",
                        Balance = 10000000,
                        AvailableBalance = 9500000,
                        PendingBalance = 500000,
                        Status = "active"
                    },
                    new BankAccount
                    {
                        Id = "acct_reserve_001",
                        AccountNumber = "BDET-1000003",
                        IBAN = "IE89BDET00010000001003",
                        UserId = "central_bank",
                        Name = "BDET Reserve Account",
                        Type = "treasury",
                        Currency = "USD",
                        Balance = 500000000,
                        AvailableBalance = 500000000,
                        Status = "active"
                    },
                    new BankAccount
                    {
                        Id = "acct_eur_001",
                        AccountNumber = "BDET-2000001",
                        IBAN = "IE89BDET00020000001001",
                        UserId = "pm_chief",
                        Name = "Euro Operations",
                        Type = "business",
                        Currency = "EUR",
                        Balance = 25000000,
                        AvailableBalance = 25000000,
                        Status = "active"
                    }
                });

                // Sample movements
                _movements.AddRange(new[]
                {
                    new BankMovement
                    {
                        AccountId = "acct_pm_001",
                        Type = "credit",
                        Amount = 5000000,
                        Currency = "USD",
                        BalanceAfter = 50000000,
                        Description = "Treasury deposit",
                        CreatedAt = DateTime.UtcNow.AddDays(-5)
                    },
                    new BankMovement
                    {
                        AccountId = "acct_pm_001",
                        Type = "debit",
                        Amount = 100000,
                        Currency = "USD",
                        BalanceAfter = 49900000,
                        Description = "Transfer to Gov Operations",
                        CreatedAt = DateTime.UtcNow.AddDays(-3)
                    }
                });
            }
        }
    }

    public async Task<BankAccount> CreateAccountAsync(CreateAccountRequest request)
    {
        await Task.Delay(100);

        var accountNumber = $"BDET-{Interlocked.Increment(ref _accountCounter)}";
        var ibanSuffix = accountNumber.Replace("BDET-", "").PadLeft(12, '0');
        
        var account = new BankAccount
        {
            Id = $"acct_{Guid.NewGuid().ToString("N")[..8]}",
            AccountNumber = accountNumber,
            IBAN = $"IE89BDET0001{ibanSuffix}",
            UserId = request.UserId,
            Name = request.Name,
            Type = request.Type,
            Currency = request.Currency,
            Balance = request.InitialDeposit,
            AvailableBalance = request.InitialDeposit,
            Status = "active"
        };

        lock (_lock)
        {
            _accounts.Add(account);

            if (request.InitialDeposit > 0)
            {
                _movements.Add(new BankMovement
                {
                    AccountId = account.Id,
                    Type = "credit",
                    Amount = request.InitialDeposit,
                    Currency = request.Currency,
                    BalanceAfter = request.InitialDeposit,
                    Description = "Initial deposit"
                });
            }
        }

        _logger.LogInformation("Bank account created: {AccountNumber} for user {UserId}", accountNumber, request.UserId);
        return account;
    }

    public async Task<BankAccount?> GetAccountAsync(string id)
    {
        await Task.Delay(50);
        lock (_lock)
        {
            return _accounts.FirstOrDefault(a => a.Id == id || a.AccountNumber == id || a.IBAN == id);
        }
    }

    public async Task<List<BankAccount>> GetUserAccountsAsync(string userId)
    {
        await Task.Delay(50);
        lock (_lock)
        {
            return _accounts.Where(a => a.UserId == userId).ToList();
        }
    }

    public async Task<BankTransfer> InternalTransferAsync(InternalTransferRequest request)
    {
        await Task.Delay(150);

        BankAccount? fromAccount, toAccount;
        lock (_lock)
        {
            fromAccount = _accounts.FirstOrDefault(a => a.Id == request.FromAccountId);
            toAccount = _accounts.FirstOrDefault(a => a.Id == request.ToAccountId);
        }

        if (fromAccount == null || toAccount == null)
            throw new ArgumentException("Account not found");

        if (fromAccount.AvailableBalance < request.Amount)
            throw new InsufficientFundsException();

        var transfer = new BankTransfer
        {
            Id = $"txf_{Guid.NewGuid().ToString("N")[..8]}",
            TransferNumber = $"INT{Interlocked.Increment(ref _transferCounter)}",
            FromAccountId = request.FromAccountId,
            ToAccountId = request.ToAccountId,
            BeneficiaryName = toAccount.Name,
            Amount = request.Amount,
            Fee = 0, // Internal transfers are free
            Currency = request.Currency,
            Type = "internal",
            Status = "completed", // Instant for internal
            Reference = request.Reference,
            Description = request.Description,
            CompletedAt = DateTime.UtcNow
        };

        lock (_lock)
        {
            // Debit from source
            fromAccount.Balance -= request.Amount;
            fromAccount.AvailableBalance -= request.Amount;
            fromAccount.LastActivityAt = DateTime.UtcNow;

            _movements.Add(new BankMovement
            {
                AccountId = fromAccount.Id,
                Type = "debit",
                Amount = request.Amount,
                Currency = request.Currency,
                BalanceAfter = fromAccount.Balance,
                Description = $"Transfer to {toAccount.Name}",
                Reference = transfer.TransferNumber,
                TransferId = transfer.Id
            });

            // Credit to destination
            toAccount.Balance += request.Amount;
            toAccount.AvailableBalance += request.Amount;
            toAccount.LastActivityAt = DateTime.UtcNow;

            _movements.Add(new BankMovement
            {
                AccountId = toAccount.Id,
                Type = "credit",
                Amount = request.Amount,
                Currency = request.Currency,
                BalanceAfter = toAccount.Balance,
                Description = $"Transfer from {fromAccount.Name}",
                Reference = transfer.TransferNumber,
                TransferId = transfer.Id
            });

            _transfers.Add(transfer);
        }

        _logger.LogInformation("Internal transfer completed: {TransferNumber} - {Amount} {Currency}",
            transfer.TransferNumber, request.Amount, request.Currency);

        return transfer;
    }

    public async Task<BankTransfer> SwiftTransferAsync(SwiftTransferRequest request)
    {
        await Task.Delay(200);

        BankAccount? fromAccount;
        lock (_lock)
        {
            fromAccount = _accounts.FirstOrDefault(a => a.Id == request.FromAccountId);
        }

        if (fromAccount == null)
            throw new ArgumentException("Account not found");

        decimal fee = 25; // SWIFT fee
        if (request.ChargeType == "OUR") fee += 15;

        var totalAmount = request.Amount + fee;
        if (fromAccount.AvailableBalance < totalAmount)
            throw new InsufficientFundsException();

        // Generate MT103 reference
        var mt103Ref = $"MT103{DateTime.UtcNow:yyyyMMddHHmmss}{RandomNumberGenerator.GetInt32(1000, 9999)}";

        var transfer = new BankTransfer
        {
            Id = $"txf_{Guid.NewGuid().ToString("N")[..8]}",
            TransferNumber = $"SWF{Interlocked.Increment(ref _transferCounter)}",
            FromAccountId = request.FromAccountId,
            ToIBAN = request.BeneficiaryIBAN,
            ToSwift = request.BeneficiaryBankSwift,
            BeneficiaryName = request.BeneficiaryName,
            Amount = request.Amount,
            Fee = fee,
            Currency = request.Currency,
            Type = "swift",
            Status = "processing", // SWIFT takes time
            Reference = request.Reference,
            Description = request.Purpose,
            MT103Reference = mt103Ref
        };

        lock (_lock)
        {
            // Debit from source (including fee)
            fromAccount.Balance -= totalAmount;
            fromAccount.AvailableBalance -= totalAmount;
            fromAccount.LastActivityAt = DateTime.UtcNow;

            _movements.Add(new BankMovement
            {
                AccountId = fromAccount.Id,
                Type = "debit",
                Amount = totalAmount,
                Currency = request.Currency,
                BalanceAfter = fromAccount.Balance,
                Description = $"SWIFT to {request.BeneficiaryName} (incl. fee {fee} {request.Currency})",
                Reference = transfer.TransferNumber,
                TransferId = transfer.Id
            });

            _transfers.Add(transfer);
        }

        // Simulate SWIFT processing in background
        _ = ProcessSwiftTransfer(transfer.Id);

        _logger.LogInformation("SWIFT transfer initiated: {TransferNumber} - {Amount} {Currency} to {Beneficiary}",
            transfer.TransferNumber, request.Amount, request.Currency, request.BeneficiaryName);

        return transfer;
    }

    private async Task ProcessSwiftTransfer(string transferId)
    {
        // Simulate SWIFT processing time (5-10 seconds for demo, real is 1-5 days)
        await Task.Delay(5000);

        lock (_lock)
        {
            var transfer = _transfers.FirstOrDefault(t => t.Id == transferId);
            if (transfer != null && transfer.Status == "processing")
            {
                transfer.Status = "completed";
                transfer.CompletedAt = DateTime.UtcNow;
            }
        }
    }

    public async Task<BankInfo> GetBankInfoAsync()
    {
        await Task.Delay(50);
        return new BankInfo();
    }

    public async Task<List<ExchangeRate>> GetExchangeRatesAsync()
    {
        await Task.Delay(50);
        return new List<ExchangeRate>
        {
            new() { FromCurrency = "USD", ToCurrency = "EUR", Rate = 0.92m, Spread = 0.005m },
            new() { FromCurrency = "USD", ToCurrency = "GBP", Rate = 0.79m, Spread = 0.005m },
            new() { FromCurrency = "USD", ToCurrency = "IGT", Rate = 1.0m, Spread = 0.001m },
            new() { FromCurrency = "EUR", ToCurrency = "USD", Rate = 1.09m, Spread = 0.005m },
            new() { FromCurrency = "EUR", ToCurrency = "GBP", Rate = 0.86m, Spread = 0.005m },
            new() { FromCurrency = "GBP", ToCurrency = "USD", Rate = 1.27m, Spread = 0.005m },
            new() { FromCurrency = "IGT", ToCurrency = "USD", Rate = 1.0m, Spread = 0.001m },
            new() { FromCurrency = "BTC", ToCurrency = "USD", Rate = 43500m, Spread = 0.01m },
            new() { FromCurrency = "ETH", ToCurrency = "USD", Rate = 2650m, Spread = 0.01m },
            new() { FromCurrency = "USDT", ToCurrency = "USD", Rate = 1.0m, Spread = 0.001m }
        };
    }

    public async Task<ExchangeResult> ExchangeAsync(ExchangeRequest request)
    {
        await Task.Delay(100);

        var rates = await GetExchangeRatesAsync();
        var rate = rates.FirstOrDefault(r => r.FromCurrency == request.FromCurrency && r.ToCurrency == request.ToCurrency);
        
        if (rate == null)
            throw new ArgumentException($"Exchange rate not available for {request.FromCurrency}/{request.ToCurrency}");

        BankAccount? account;
        lock (_lock)
        {
            account = _accounts.FirstOrDefault(a => a.Id == request.AccountId);
        }

        if (account == null)
            throw new ArgumentException("Account not found");

        if (account.AvailableBalance < request.Amount)
            throw new InsufficientFundsException();

        var effectiveRate = rate.Rate * (1 - rate.Spread);
        var toAmount = request.Amount * effectiveRate;
        var fee = request.Amount * rate.Spread;

        lock (_lock)
        {
            account.Balance -= request.Amount;
            account.AvailableBalance -= request.Amount;

            _movements.Add(new BankMovement
            {
                AccountId = account.Id,
                Type = "debit",
                Amount = request.Amount,
                Currency = request.FromCurrency,
                BalanceAfter = account.Balance,
                Description = $"Exchange to {request.ToCurrency}"
            });
        }

        _logger.LogInformation("Exchange completed: {Amount} {From} -> {ToAmount} {To} at rate {Rate}",
            request.Amount, request.FromCurrency, toAmount, request.ToCurrency, effectiveRate);

        return new ExchangeResult
        {
            FromAmount = request.Amount,
            FromCurrency = request.FromCurrency,
            ToAmount = toAmount,
            ToCurrency = request.ToCurrency,
            Rate = effectiveRate,
            Fee = fee
        };
    }

    public async Task<List<BankMovement>> GetMovementsAsync(string accountId, int page, int limit)
    {
        await Task.Delay(50);
        lock (_lock)
        {
            return _movements
                .Where(m => m.AccountId == accountId)
                .OrderByDescending(m => m.CreatedAt)
                .Skip((page - 1) * limit)
                .Take(limit)
                .ToList();
        }
    }

    public async Task<AccountStatement> GenerateStatementAsync(string accountId, DateTime? from, DateTime? to)
    {
        await Task.Delay(100);

        from ??= DateTime.UtcNow.AddMonths(-1);
        to ??= DateTime.UtcNow;

        BankAccount? account;
        List<BankMovement> movements;

        lock (_lock)
        {
            account = _accounts.FirstOrDefault(a => a.Id == accountId);
            movements = _movements
                .Where(m => m.AccountId == accountId && m.CreatedAt >= from && m.CreatedAt <= to)
                .OrderBy(m => m.CreatedAt)
                .ToList();
        }

        if (account == null)
            throw new ArgumentException("Account not found");

        var credits = movements.Where(m => m.Type == "credit").Sum(m => m.Amount);
        var debits = movements.Where(m => m.Type == "debit").Sum(m => m.Amount);

        return new AccountStatement
        {
            AccountId = account.Id,
            AccountNumber = account.AccountNumber,
            AccountName = account.Name,
            FromDate = from.Value,
            ToDate = to.Value,
            OpeningBalance = account.Balance - credits + debits,
            ClosingBalance = account.Balance,
            TotalCredits = credits,
            TotalDebits = debits,
            Movements = movements
        };
    }
}
