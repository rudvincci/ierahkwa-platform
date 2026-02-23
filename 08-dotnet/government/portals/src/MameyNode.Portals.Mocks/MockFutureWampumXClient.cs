using Bogus;
using MameyNode.Portals.Mocks.Interfaces;
using MameyNode.Portals.Mocks.Models;

namespace MameyNode.Portals.Mocks;

public class MockFutureWampumXClient : IFutureWampumXClient
{
    private readonly Faker _faker = new();
    private readonly List<PoolInfo> _pools = new();
    private readonly List<SwapInfo> _swaps = new();
    private readonly List<MultiCurrencyWalletInfo> _wallets = new();
    private readonly List<ExchangeOrderInfo> _orders = new();
    private readonly List<TradingPairInfo> _tradingPairs = new();
    private readonly List<ExchangeRateOracleInfo> _oracles = new();
    private readonly List<CryptoOrderInfo> _cryptoOrders = new();
    private readonly List<CustodyAccountInfo> _custodyAccounts = new();
    private readonly List<StakingInfo> _stakingInfo = new();
    private readonly List<AccountMappingInfo> _accountMappings = new();
    private readonly List<BridgedIdentityInfo> _bridgedIdentities = new();
    private readonly List<BridgedTransactionInfo> _bridgedTransactions = new();
    private readonly List<TravelRuleInfo> _travelRuleTransactions = new();
    private readonly List<VASPDirectoryInfo> _vaspDirectory = new();
    private readonly List<TrustLineInfo> _trustLines = new();

    public MockFutureWampumXClient()
    {
        InitializeMockData();
    }

    private void InitializeMockData()
    {
        var poolFaker = new Faker<PoolInfo>()
            .RuleFor(p => p.PoolId, f => $"POOL-{f.Random.AlphaNumeric(10).ToUpper()}")
            .RuleFor(p => p.TokenA, f => f.PickRandom("USD", "EUR", "GBP"))
            .RuleFor(p => p.TokenB, f => f.PickRandom("USD", "EUR", "GBP", "CAD", "AUD", "JPY"))
            .RuleFor(p => p.ReserveA, f => f.Finance.Amount(100000, 10000000, 2).ToString("F2"))
            .RuleFor(p => p.ReserveB, f => f.Finance.Amount(100000, 10000000, 2).ToString("F2"))
            .RuleFor(p => p.TotalLpSupply, f => f.Finance.Amount(10000, 1000000, 2).ToString("F2"))
            .RuleFor(p => p.Model, f => f.PickRandom<AMMModel>())
            .RuleFor(p => p.FeeRate, f => f.Random.Decimal(0.001m, 0.01m).ToString("F4"))
            .RuleFor(p => p.Status, f => f.PickRandom<PoolStatus>())
            .RuleFor(p => p.CreatedAt, f => f.Date.Past(1))
            .RuleFor(p => p.TotalVolume24h, f => f.Finance.Amount(10000, 1000000, 2).ToString("F2"))
            .RuleFor(p => p.TotalFees24h, (f, p) => (decimal.Parse(p.TotalVolume24h) * decimal.Parse(p.FeeRate)).ToString("F2"));

        _pools.AddRange(poolFaker.Generate(30));

        var swapFaker = new Faker<SwapInfo>()
            .RuleFor(s => s.SwapId, f => $"SWAP-{f.Random.AlphaNumeric(10).ToUpper()}")
            .RuleFor(s => s.PoolId, f => _pools[f.Random.Int(0, _pools.Count - 1)].PoolId)
            .RuleFor(s => s.TokenIn, f => f.PickRandom("USD", "EUR", "GBP"))
            .RuleFor(s => s.TokenOut, f => f.PickRandom("USD", "EUR", "GBP", "CAD"))
            .RuleFor(s => s.AmountIn, f => f.Finance.Amount(100, 10000, 2).ToString("F2"))
            .RuleFor(s => s.AmountOut, (f, s) => (decimal.Parse(s.AmountIn) * f.Random.Decimal(0.8m, 1.2m)).ToString("F2"))
            .RuleFor(s => s.PriceImpact, f => f.Random.Decimal(0.01m, 5.0m).ToString("F2"))
            .RuleFor(s => s.FeePaid, (f, s) => (decimal.Parse(s.AmountIn) * 0.003m).ToString("F2"))
            .RuleFor(s => s.RouteTaken, f => new List<string> { _pools[f.Random.Int(0, _pools.Count - 1)].PoolId })
            .RuleFor(s => s.ExecutedAt, f => f.Date.Recent(7))
            .RuleFor(s => s.Status, "Completed");

        _swaps.AddRange(swapFaker.Generate(200));

        var walletFaker = new Faker<MultiCurrencyWalletInfo>()
            .RuleFor(w => w.WalletId, f => $"WALLET-{f.Random.AlphaNumeric(10).ToUpper()}")
            .RuleFor(w => w.AccountId, f => $"ACC-{f.Random.AlphaNumeric(8)}")
            .RuleFor(w => w.Balances, f => new Dictionary<string, string>
            {
                { "USD", f.Finance.Amount(1000, 50000, 2).ToString("F2") },
                { "EUR", f.Finance.Amount(500, 25000, 2).ToString("F2") },
                { "GBP", f.Finance.Amount(300, 15000, 2).ToString("F2") }
            })
            .RuleFor(w => w.CreatedAt, f => f.Date.Past(1));

        _wallets.AddRange(walletFaker.Generate(150));

        var orderFaker = new Faker<ExchangeOrderInfo>()
            .RuleFor(o => o.OrderId, f => $"ORDER-{f.Random.AlphaNumeric(10).ToUpper()}")
            .RuleFor(o => o.TradingPair, f => $"{f.PickRandom("USD", "EUR")}/{f.PickRandom("EUR", "GBP", "CAD")}")
            .RuleFor(o => o.OrderType, f => f.PickRandom("buy", "sell"))
            .RuleFor(o => o.Price, f => f.Random.Decimal(0.5m, 2.0m).ToString("F4"))
            .RuleFor(o => o.Quantity, f => f.Finance.Amount(100, 10000, 2).ToString("F2"))
            .RuleFor(o => o.FilledQuantity, (f, o) => f.Random.Bool(0.7f) ? o.Quantity : (decimal.Parse(o.Quantity) * f.Random.Decimal(0.1m, 0.9m)).ToString("F2"))
            .RuleFor(o => o.Status, f => f.PickRandom<OrderStatus>())
            .RuleFor(o => o.CreatedAt, f => f.Date.Recent(7))
            .RuleFor(o => o.FilledAt, (f, o) => o.Status == OrderStatus.Filled ? f.Date.Between(o.CreatedAt, DateTime.Now) : null);

        _orders.AddRange(orderFaker.Generate(300));

        var pairFaker = new Faker<TradingPairInfo>()
            .RuleFor(p => p.TradingPair, f => $"{f.PickRandom("USD", "EUR")}/{f.PickRandom("EUR", "GBP", "CAD")}")
            .RuleFor(p => p.BaseCurrency, f => f.PickRandom("USD", "EUR", "GBP"))
            .RuleFor(p => p.QuoteCurrency, f => f.PickRandom("USD", "EUR", "GBP", "CAD"))
            .RuleFor(p => p.LastPrice, f => f.Random.Decimal(0.5m, 2.0m).ToString("F4"))
            .RuleFor(p => p.Volume24h, f => f.Finance.Amount(10000, 1000000, 2).ToString("F2"))
            .RuleFor(p => p.MinTradeAmount, "10")
            .RuleFor(p => p.TickSize, "0.0001");

        _tradingPairs.AddRange(pairFaker.Generate(20));

        var oracleFaker = new Faker<ExchangeRateOracleInfo>()
            .RuleFor(o => o.OracleId, f => $"ORACLE-{f.Random.AlphaNumeric(8).ToUpper()}")
            .RuleFor(o => o.FromCurrency, f => f.PickRandom("USD", "EUR", "GBP"))
            .RuleFor(o => o.ToCurrency, f => f.PickRandom("USD", "EUR", "GBP", "CAD", "AUD"))
            .RuleFor(o => o.ExchangeRate, f => f.Random.Double(0.5, 2.0).ToString("F4"))
            .RuleFor(o => o.UpdatedAt, f => f.Date.Recent(1))
            .RuleFor(o => o.Source, f => f.PickRandom("Chainlink", "Internal", "Aggregated"))
            .RuleFor(o => o.Confidence, f => f.Random.Double(0.8, 1.0));

        _oracles.AddRange(oracleFaker.Generate(15));

        var cryptoOrderFaker = new Faker<CryptoOrderInfo>()
            .RuleFor(o => o.OrderId, f => $"CRYPTO-{f.Random.AlphaNumeric(10).ToUpper()}")
            .RuleFor(o => o.TradingPair, f => $"{f.PickRandom("BTC", "ETH")}/USD")
            .RuleFor(o => o.OrderType, f => f.PickRandom("buy", "sell"))
            .RuleFor(o => o.Price, f => f.Random.Decimal(100, 100000).ToString("F2"))
            .RuleFor(o => o.Quantity, f => f.Random.Decimal(0.001m, 10.0m).ToString("F6"))
            .RuleFor(o => o.FilledQuantity, (f, o) => f.Random.Bool(0.6f) ? o.Quantity : (decimal.Parse(o.Quantity) * f.Random.Decimal(0.1m, 0.9m)).ToString("F6"))
            .RuleFor(o => o.Status, f => f.PickRandom<OrderStatus>())
            .RuleFor(o => o.CreatedAt, f => f.Date.Recent(7))
            .RuleFor(o => o.FilledAt, (f, o) => o.Status == OrderStatus.Filled ? f.Date.Between(o.CreatedAt, DateTime.Now) : null);

        _cryptoOrders.AddRange(cryptoOrderFaker.Generate(150));

        var custodyFaker = new Faker<CustodyAccountInfo>()
            .RuleFor(c => c.CustodyAccountId, f => $"CUSTODY-{f.Random.AlphaNumeric(10).ToUpper()}")
            .RuleFor(c => c.AccountId, f => $"ACC-{f.Random.AlphaNumeric(8)}")
            .RuleFor(c => c.Currency, f => f.PickRandom("USD", "EUR", "GBP", "BTC", "ETH"))
            .RuleFor(c => c.Balance, f => f.Finance.Amount(10000, 1000000, 2).ToString("F2"))
            .RuleFor(c => c.Type, f => f.PickRandom<CustodyAccountType>())
            .RuleFor(c => c.CreatedAt, f => f.Date.Past(1));

        _custodyAccounts.AddRange(custodyFaker.Generate(50));

        var stakingFaker = new Faker<StakingInfo>()
            .RuleFor(s => s.AccountId, f => $"ACC-{f.Random.AlphaNumeric(8)}")
            .RuleFor(s => s.TotalStaked, f => f.Finance.Amount(1000, 100000, 2).ToString("F2"))
            .RuleFor(s => s.Currency, f => f.PickRandom("USD", "EUR", "GBP"))
            .RuleFor(s => s.RewardsEarned, f => f.Finance.Amount(10, 1000, 2).ToString("F2"))
            .RuleFor(s => s.StakingStartDate, f => f.Date.Past(1));

        _stakingInfo.AddRange(stakingFaker.Generate(40));

        var mappingFaker = new Faker<AccountMappingInfo>()
            .RuleFor(m => m.MappingId, f => $"MAP-{f.Random.AlphaNumeric(10).ToUpper()}")
            .RuleFor(m => m.BankingAccountId, f => $"BANK-{f.Random.AlphaNumeric(8)}")
            .RuleFor(m => m.BlockchainAccount, f => $"0x{f.Random.AlphaNumeric(40)}")
            .RuleFor(m => m.AccountType, f => f.PickRandom("Personal", "Business", "Institutional"))
            .RuleFor(m => m.CreatedAt, f => f.Date.Past(1))
            .RuleFor(m => m.Status, f => f.PickRandom<MappingStatus>());

        _accountMappings.AddRange(mappingFaker.Generate(100));

        var bridgedIdentityFaker = new Faker<BridgedIdentityInfo>()
            .RuleFor(b => b.BridgeId, f => $"BRIDGE-{f.Random.AlphaNumeric(10).ToUpper()}")
            .RuleFor(b => b.SourceSystem, f => f.PickRandom("Legacy", "External", "Partner"))
            .RuleFor(b => b.SourceIdentityId, f => $"SRC-{f.Random.AlphaNumeric(8)}")
            .RuleFor(b => b.TargetSystem, "MameyNode")
            .RuleFor(b => b.TargetIdentityId, f => $"TGT-{f.Random.AlphaNumeric(8)}")
            .RuleFor(b => b.IdentityData, f => new Dictionary<string, string> { { "name", f.Person.FullName }, { "email", f.Person.Email } })
            .RuleFor(b => b.BridgedAt, f => f.Date.Past(1))
            .RuleFor(b => b.LastSyncedAt, f => f.Date.Recent(7))
            .RuleFor(b => b.Status, f => f.PickRandom<BridgeStatus>());

        _bridgedIdentities.AddRange(bridgedIdentityFaker.Generate(75));

        var bridgedTxFaker = new Faker<BridgedTransactionInfo>()
            .RuleFor(b => b.BridgeId, f => $"BRIDGE-TX-{f.Random.AlphaNumeric(10).ToUpper()}")
            .RuleFor(b => b.SourceSystem, f => f.PickRandom("Legacy", "External", "Partner"))
            .RuleFor(b => b.SourceTransactionId, f => $"SRC-TX-{f.Random.AlphaNumeric(12)}")
            .RuleFor(b => b.TargetSystem, "MameyNode")
            .RuleFor(b => b.TargetTransactionId, f => $"TGT-TX-{f.Random.AlphaNumeric(12)}")
            .RuleFor(b => b.FromAccount, f => $"ACC-{f.Random.AlphaNumeric(8)}")
            .RuleFor(b => b.ToAccount, f => $"ACC-{f.Random.AlphaNumeric(8)}")
            .RuleFor(b => b.Amount, f => f.Finance.Amount(100, 100000, 2).ToString("F2"))
            .RuleFor(b => b.Currency, "USD")
            .RuleFor(b => b.BridgedAt, f => f.Date.Recent(30))
            .RuleFor(b => b.Status, f => f.PickRandom<BridgeStatus>());

        _bridgedTransactions.AddRange(bridgedTxFaker.Generate(200));

        var travelRuleFaker = new Faker<TravelRuleInfo>()
            .RuleFor(t => t.TravelRuleId, f => $"TRAVEL-{f.Random.AlphaNumeric(10).ToUpper()}")
            .RuleFor(t => t.TransactionId, f => $"TXN-{f.Random.AlphaNumeric(12).ToUpper()}")
            .RuleFor(t => t.SenderVASP, f => $"VASP-{f.Random.AlphaNumeric(8)}")
            .RuleFor(t => t.ReceiverVASP, f => $"VASP-{f.Random.AlphaNumeric(8)}")
            .RuleFor(t => t.IVMS101Data, f => $"{{\"originator\":\"{f.Person.FullName}\",\"beneficiary\":\"{f.Person.FullName}\"}}")
            .RuleFor(t => t.Status, f => f.PickRandom("Pending", "Verified", "Rejected"))
            .RuleFor(t => t.CreatedAt, f => f.Date.Recent(7))
            .RuleFor(t => t.ProcessedAt, (f, t) => t.Status == "Verified" ? f.Date.Between(t.CreatedAt, DateTime.Now) : null);

        _travelRuleTransactions.AddRange(travelRuleFaker.Generate(100));

        var vaspFaker = new Faker<VASPDirectoryInfo>()
            .RuleFor(v => v.VASPId, f => $"VASP-{f.Random.AlphaNumeric(8).ToUpper()}")
            .RuleFor(v => v.VASPName, f => f.Company.CompanyName())
            .RuleFor(v => v.VASPAddress, f => f.Address.FullAddress())
            .RuleFor(v => v.PublicKey, f => $"0x{f.Random.AlphaNumeric(64)}")
            .RuleFor(v => v.IsActive, f => f.Random.Bool(0.9f))
            .RuleFor(v => v.RegisteredAt, f => f.Date.Past(1));

        _vaspDirectory.AddRange(vaspFaker.Generate(25));

        var trustLineFaker = new Faker<TrustLineInfo>()
            .RuleFor(t => t.TrustLineId, f => $"TRUST-{f.Random.AlphaNumeric(10).ToUpper()}")
            .RuleFor(t => t.AccountA, f => $"ACC-{f.Random.AlphaNumeric(8)}")
            .RuleFor(t => t.AccountB, f => $"ACC-{f.Random.AlphaNumeric(8)}")
            .RuleFor(t => t.Currency, f => f.PickRandom("USD", "EUR", "GBP"))
            .RuleFor(t => t.Limit, f => f.Finance.Amount(10000, 1000000, 2).ToString("F2"))
            .RuleFor(t => t.Balance, (f, t) => (decimal.Parse(t.Limit) * f.Random.Decimal(0m, 0.8m)).ToString("F2"))
            .RuleFor(t => t.IsActive, f => f.Random.Bool(0.85f))
            .RuleFor(t => t.CreatedAt, f => f.Date.Past(1));

        _trustLines.AddRange(trustLineFaker.Generate(60));
    }

    public Task<List<PoolInfo>> GetPoolsAsync() => Task.FromResult(_pools);
    public Task<PoolInfo?> GetPoolAsync(string poolId) => Task.FromResult(_pools.FirstOrDefault(p => p.PoolId == poolId));
    public Task<List<SwapInfo>> GetSwapsAsync() => Task.FromResult(_swaps);
    public Task<List<MultiCurrencyWalletInfo>> GetMultiCurrencyWalletsAsync() => Task.FromResult(_wallets);
    public Task<List<ExchangeOrderInfo>> GetExchangeOrdersAsync() => Task.FromResult(_orders);
    public Task<List<TradingPairInfo>> GetTradingPairsAsync() => Task.FromResult(_tradingPairs);
    public Task<List<ExchangeRateOracleInfo>> GetExchangeRateOraclesAsync() => Task.FromResult(_oracles);
    public Task<List<CryptoOrderInfo>> GetCryptoOrdersAsync() => Task.FromResult(_cryptoOrders);
    public Task<List<CustodyAccountInfo>> GetCustodyAccountsAsync() => Task.FromResult(_custodyAccounts);
    public Task<List<StakingInfo>> GetStakingInfoAsync() => Task.FromResult(_stakingInfo);
    public Task<List<AccountMappingInfo>> GetAccountMappingsAsync() => Task.FromResult(_accountMappings);
    public Task<List<BridgedIdentityInfo>> GetBridgedIdentitiesAsync() => Task.FromResult(_bridgedIdentities);
    public Task<List<BridgedTransactionInfo>> GetBridgedTransactionsAsync() => Task.FromResult(_bridgedTransactions);
    public Task<List<TravelRuleInfo>> GetTravelRuleTransactionsAsync() => Task.FromResult(_travelRuleTransactions);
    public Task<List<VASPDirectoryInfo>> GetVASPDirectoryAsync() => Task.FromResult(_vaspDirectory);
    public Task<List<TrustLineInfo>> GetTrustLinesAsync() => Task.FromResult(_trustLines);
}

