using Bogus;
using MameyNode.Portals.Mocks.Interfaces;
using MameyNode.Portals.Mocks.Models;

namespace MameyNode.Portals.Mocks;

public class MockUPGClient : IUPGClient
{
    private readonly Faker _faker = new();
    private readonly List<ProtocolAdapterInfo> _adapters = new();
    private readonly List<POSTransactionInfo> _posTransactions = new();
    private readonly List<MerchantSettlementInfo> _settlements = new();

    public MockUPGClient()
    {
        InitializeMockData();
    }

    private void InitializeMockData()
    {
        var adapterFaker = new Faker<ProtocolAdapterInfo>()
            .RuleFor(a => a.AdapterId, f => $"ADPT-{f.Random.AlphaNumeric(10).ToUpper()}")
            .RuleFor(a => a.ProtocolName, f => f.PickRandom("Visa", "Mastercard", "SWIFT", "ACH", "SEPA"))
            .RuleFor(a => a.AdapterType, f => f.PickRandom("Payment", "Settlement", "FX"))
            .RuleFor(a => a.Configuration, f => new Dictionary<string, string> { { "endpoint", f.Internet.Url() }, { "timeout", "30" } })
            .RuleFor(a => a.RegisteredAt, f => f.Date.Past(1))
            .RuleFor(a => a.Status, f => f.PickRandom<AdapterStatus>());

        _adapters.AddRange(adapterFaker.Generate(15));

        var posFaker = new Faker<POSTransactionInfo>()
            .RuleFor(p => p.TransactionId, f => $"POS-{f.Random.AlphaNumeric(10).ToUpper()}")
            .RuleFor(p => p.MerchantId, f => $"MERCH-{f.Random.AlphaNumeric(8)}")
            .RuleFor(p => p.TerminalId, f => $"TERM-{f.Random.AlphaNumeric(8)}")
            .RuleFor(p => p.Amount, f => f.Finance.Amount(10, 1000, 2).ToString("F2"))
            .RuleFor(p => p.Currency, "USD")
            .RuleFor(p => p.AuthorizationCode, f => f.Random.AlphaNumeric(6).ToUpper())
            .RuleFor(p => p.Status, f => f.PickRandom<POSTransactionStatus>())
            .RuleFor(p => p.CreatedAt, f => f.Date.Recent(7));

        _posTransactions.AddRange(posFaker.Generate(200));

        var settlementFaker = new Faker<MerchantSettlementInfo>()
            .RuleFor(s => s.SettlementId, f => $"SETTLE-{f.Random.AlphaNumeric(10).ToUpper()}")
            .RuleFor(s => s.MerchantId, f => $"MERCH-{f.Random.AlphaNumeric(8)}")
            .RuleFor(s => s.SettlementPeriod, f => f.Date.Recent(30).ToString("yyyy-MM"))
            .RuleFor(s => s.TotalAmount, f => f.Finance.Amount(10000, 500000, 2).ToString("F2"))
            .RuleFor(s => s.Currency, "USD")
            .RuleFor(s => s.TransactionCount, f => f.Random.Int(10, 500))
            .RuleFor(s => s.Status, f => f.PickRandom<SettlementStatus>())
            .RuleFor(s => s.CreatedAt, f => f.Date.Recent(30))
            .RuleFor(s => s.ProcessedAt, (f, s) => s.Status == SettlementStatus.Completed ? f.Date.Between(s.CreatedAt, DateTime.Now) : null);

        _settlements.AddRange(settlementFaker.Generate(50));
    }

    public Task<List<ProtocolAdapterInfo>> GetProtocolAdaptersAsync() => Task.FromResult(_adapters);
    public Task<ProtocolAdapterInfo?> GetProtocolAdapterAsync(string adapterId) => Task.FromResult(_adapters.FirstOrDefault(a => a.AdapterId == adapterId));
    public Task<List<RouteOption>> GetRouteOptionsAsync(string fromAccount, string toAccount, string amount, string currency)
    {
        var options = new List<RouteOption>
        {
            new() { Rail = "Direct", Fee = "0.01", EstimatedTime = "5s", ReliabilityScore = 0.99 },
            new() { Rail = "Hub", Fee = "0.005", EstimatedTime = "10s", ReliabilityScore = 0.95 },
            new() { Rail = "MultiHop", Fee = "0.02", EstimatedTime = "15s", ReliabilityScore = 0.90 }
        };
        return Task.FromResult(options);
    }
    public Task<List<POSTransactionInfo>> GetPOSTransactionsAsync() => Task.FromResult(_posTransactions);
    public Task<List<MerchantSettlementInfo>> GetMerchantSettlementsAsync() => Task.FromResult(_settlements);
}
