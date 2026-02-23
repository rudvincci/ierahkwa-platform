using Bogus;
using MameyNode.Portals.Mocks.Interfaces;
using MameyNode.Portals.Mocks.Models;

namespace MameyNode.Portals.Mocks;

public class MockFutureWampumLedgerClient : IFutureWampumLedgerClient
{
    private readonly Faker _faker = new();
    private readonly List<TransactionInfo> _transactions = new();
    private readonly List<LedgerBlockInfo> _blocks = new();
    private readonly List<TransactionLogInfo> _transactionLogs = new();
    private readonly List<TransactionFlagInfo> _transactionFlags = new();
    private readonly List<CurrencyInfo> _currencies = new();
    private readonly List<CreditEntryInfo> _creditHistory = new();

    public MockFutureWampumLedgerClient()
    {
        InitializeMockData();
    }

    private void InitializeMockData()
    {
        var transactionFaker = new Faker<TransactionInfo>()
            .RuleFor(t => t.TransactionId, f => $"TXN-{f.Random.AlphaNumeric(12).ToUpper()}")
            .RuleFor(t => t.FromAccount, f => $"ACC-{f.Random.AlphaNumeric(8)}")
            .RuleFor(t => t.ToAccount, f => $"ACC-{f.Random.AlphaNumeric(8)}")
            .RuleFor(t => t.Amount, f => f.Finance.Amount(10, 100000, 2).ToString("F2"))
            .RuleFor(t => t.Currency, f => f.PickRandom("USD", "EUR", "GBP"))
            .RuleFor(t => t.Status, f => f.PickRandom<TransactionStatus>())
            .RuleFor(t => t.BlockHash, f => $"0x{f.Random.AlphaNumeric(64)}")
            .RuleFor(t => t.Timestamp, f => f.Date.Recent(30))
            .RuleFor(t => t.TransactionType, f => f.PickRandom("Payment", "Transfer", "Settlement", "Mint", "Burn"))
            .RuleFor(t => t.Metadata, f => new Dictionary<string, string> { { "memo", f.Lorem.Sentence() } });

        _transactions.AddRange(transactionFaker.Generate(500));

        var blockFaker = new Faker<LedgerBlockInfo>()
            .RuleFor(b => b.BlockHash, f => $"0x{f.Random.AlphaNumeric(64)}")
            .RuleFor(b => b.Previous, f => $"0x{f.Random.AlphaNumeric(64)}")
            .RuleFor(b => b.Account, f => $"ACC-{f.Random.AlphaNumeric(8)}")
            .RuleFor(b => b.Timestamp, f => f.Date.Recent(7))
            .RuleFor(b => b.BlockHeight, f => (ulong)f.Random.Long(1000000, 5000000))
            .RuleFor(b => b.IsConfirmed, f => f.Random.Bool(0.95f))
            .RuleFor(b => b.BlockType, f => f.PickRandom("Send", "Receive", "Open", "Change"))
            .RuleFor(b => b.Transactions, f => _transactions.Take(f.Random.Int(1, 10)).Select(t => t.TransactionId).ToList());

        _blocks.AddRange(blockFaker.Generate(200));

        var logFaker = new Faker<TransactionLogInfo>()
            .RuleFor(l => l.LogId, f => $"LOG-{f.Random.AlphaNumeric(10).ToUpper()}")
            .RuleFor(l => l.TransactionId, f => _transactions[f.Random.Int(0, _transactions.Count - 1)].TransactionId)
            .RuleFor(l => l.BlockchainHash, f => $"0x{f.Random.AlphaNumeric(64)}")
            .RuleFor(l => l.EventType, f => f.PickRandom<TransactionEventType>())
            .RuleFor(l => l.FromAccount, f => $"ACC-{f.Random.AlphaNumeric(8)}")
            .RuleFor(l => l.ToAccount, f => $"ACC-{f.Random.AlphaNumeric(8)}")
            .RuleFor(l => l.Amount, f => f.Finance.Amount(10, 100000, 2).ToString("F2"))
            .RuleFor(l => l.Currency, "USD")
            .RuleFor(l => l.Category, f => f.PickRandom<TransactionCategory>())
            .RuleFor(l => l.Metadata, f => new Dictionary<string, string>())
            .RuleFor(l => l.Description, f => f.Lorem.Sentence())
            .RuleFor(l => l.CreatedAt, f => f.Date.Recent(30))
            .RuleFor(l => l.ConfirmedAt, (f, l) => f.Random.Bool(0.9f) ? f.Date.Between(l.CreatedAt, DateTime.Now) : null)
            .RuleFor(l => l.ComplianceStatus, f => f.PickRandom<ComplianceStatus>())
            .RuleFor(l => l.ComplianceReason, f => f.Lorem.Sentence());

        _transactionLogs.AddRange(logFaker.Generate(400));

        var flagFaker = new Faker<TransactionFlagInfo>()
            .RuleFor(f => f.FlagId, f => $"FLAG-{f.Random.AlphaNumeric(10).ToUpper()}")
            .RuleFor(f => f.TransactionId, f => _transactions[f.Random.Int(0, _transactions.Count - 1)].TransactionId)
            .RuleFor(f => f.FlagType, f => f.PickRandom("AML", "Fraud", "Compliance", "Risk"))
            .RuleFor(f => f.Severity, f => f.PickRandom("Low", "Medium", "High", "Critical"))
            .RuleFor(f => f.Reason, f => f.Lorem.Sentence())
            .RuleFor(f => f.Status, f => f.PickRandom<FlagStatus>())
            .RuleFor(f => f.CreatedAt, f => f.Date.Recent(30));

        _transactionFlags.AddRange(flagFaker.Generate(50));

        var currencyFaker = new Faker<CurrencyInfo>()
            .RuleFor(c => c.CurrencyId, f => $"CURR-{f.Random.AlphaNumeric(8).ToUpper()}")
            .RuleFor(c => c.CurrencyCode, f => f.PickRandom("USD", "EUR", "GBP", "CAD", "AUD", "JPY"))
            .RuleFor(c => c.CurrencyName, (f, c) => c.CurrencyCode == "USD" ? "US Dollar" : c.CurrencyCode == "EUR" ? "Euro" : c.CurrencyCode == "GBP" ? "British Pound" : c.CurrencyCode)
            .RuleFor(c => c.Symbol, f => f.PickRandom("$", "€", "£", "¥"))
            .RuleFor(c => c.Decimals, 2)
            .RuleFor(c => c.Issuer, f => f.Company.CompanyName())
            .RuleFor(c => c.RegisteredAt, f => f.Date.Past(2))
            .RuleFor(c => c.Status, f => f.PickRandom<CurrencyStatus>())
            .RuleFor(c => c.Metadata, f => new Dictionary<string, string>());

        _currencies.AddRange(currencyFaker.Generate(10));

        var creditFaker = new Faker<CreditEntryInfo>()
            .RuleFor(c => c.CreditId, f => $"CREDIT-{f.Random.AlphaNumeric(10).ToUpper()}")
            .RuleFor(c => c.AccountId, f => $"ACC-{f.Random.AlphaNumeric(8)}")
            .RuleFor(c => c.CreditType, f => f.PickRandom("Loan", "CreditLine", "Overdraft"))
            .RuleFor(c => c.Amount, f => f.Finance.Amount(1000, 100000, 2).ToString("F2"))
            .RuleFor(c => c.Currency, "USD")
            .RuleFor(c => c.Description, f => f.Lorem.Sentence())
            .RuleFor(c => c.Timestamp, f => f.Date.Recent(90));

        _creditHistory.AddRange(creditFaker.Generate(200));
    }

    public Task<List<TransactionInfo>> GetTransactionsAsync(int limit = 50) => Task.FromResult(_transactions.Take(limit).ToList());
    public Task<TransactionInfo?> GetTransactionAsync(string transactionId) => Task.FromResult(_transactions.FirstOrDefault(t => t.TransactionId == transactionId));
    public Task<List<LedgerBlockInfo>> GetBlocksAsync(int limit = 50) => Task.FromResult(_blocks.Take(limit).ToList());
    public Task<LedgerBlockInfo?> GetBlockAsync(string blockHash) => Task.FromResult(_blocks.FirstOrDefault(b => b.BlockHash == blockHash));
    public Task<TransparencyDashboardInfo> GetTransparencyDashboardAsync()
    {
        var dashboard = new TransparencyDashboardInfo
        {
            TotalTransactions = _transactions.Count.ToString(),
            TotalVolume = _transactions.Sum(t => decimal.Parse(t.Amount)).ToString("F2"),
            ActiveAccounts = _transactions.Select(t => t.FromAccount).Union(_transactions.Select(t => t.ToAccount)).Distinct().Count(),
            CurrencyVolumes = _transactions.GroupBy(t => t.Currency).Select(g => new CurrencyVolumeInfo
            {
                Currency = g.Key,
                Volume = g.Sum(t => decimal.Parse(t.Amount)).ToString("F2"),
                TransactionCount = g.Count()
            }).ToList(),
            LastUpdated = DateTime.Now
        };
        return Task.FromResult(dashboard);
    }
    public Task<List<AuditTrailInfo>> GetAuditTrailAsync(string entityType, string entityId)
    {
        var entries = new List<AuditTrailInfo>();
        for (int i = 0; i < 20; i++)
        {
            entries.Add(new AuditTrailInfo
            {
                EntryId = $"AUDIT-{_faker.Random.AlphaNumeric(10).ToUpper()}",
                EntityType = entityType,
                EntityId = entityId,
                Action = _faker.PickRandom("Create", "Update", "Delete", "View"),
                Actor = $"USER-{_faker.Random.AlphaNumeric(6)}",
                Timestamp = _faker.Date.Recent(90),
                Details = new Dictionary<string, string> { { "ip", _faker.Internet.Ip() } }
            });
        }
        return Task.FromResult(entries);
    }
    public Task<List<SynchronizationInfo>> GetSynchronizationsAsync()
    {
        var syncs = new List<SynchronizationInfo>();
        for (int i = 0; i < 10; i++)
        {
            syncs.Add(new SynchronizationInfo
            {
                SyncId = $"SYNC-{_faker.Random.AlphaNumeric(10).ToUpper()}",
                SourceSystem = _faker.PickRandom("External", "Legacy", "Partner"),
                TargetSystem = "MameyNode",
                Status = _faker.PickRandom("Pending", "Processing", "Completed", "Failed"),
                StartedAt = _faker.Date.Recent(7),
                CompletedAt = _faker.Random.Bool(0.7f) ? _faker.Date.Recent(1) : null,
                TotalRecords = _faker.Random.Int(100, 10000),
                SyncedRecords = _faker.Random.Int(50, 10000),
                FailedRecords = _faker.Random.Int(0, 10)
            });
        }
        return Task.FromResult(syncs);
    }
    public Task<List<TransactionLogInfo>> GetTransactionLogsAsync() => Task.FromResult(_transactionLogs);
    public Task<List<TransactionFlagInfo>> GetTransactionFlagsAsync() => Task.FromResult(_transactionFlags);
    public Task<List<CurrencyInfo>> GetCurrenciesAsync() => Task.FromResult(_currencies);
    public Task<List<CreditEntryInfo>> GetCreditHistoryAsync(string accountId) => Task.FromResult(_creditHistory.Where(c => c.AccountId == accountId).ToList());
    public Task<CreditSummaryInfo?> GetCreditSummaryAsync(string accountId)
    {
        var accountCredits = _creditHistory.Where(c => c.AccountId == accountId).ToList();
        if (!accountCredits.Any()) return Task.FromResult<CreditSummaryInfo?>(null);
        
        return Task.FromResult<CreditSummaryInfo?>(new CreditSummaryInfo
        {
            AccountId = accountId,
            TotalCredit = accountCredits.Sum(c => decimal.Parse(c.Amount)).ToString("F2"),
            AvailableCredit = (accountCredits.Sum(c => decimal.Parse(c.Amount)) * 0.5m).ToString("F2"),
            UsedCredit = (accountCredits.Sum(c => decimal.Parse(c.Amount)) * 0.5m).ToString("F2"),
            Currency = "USD",
            LastUpdated = DateTime.Now
        });
    }
}

