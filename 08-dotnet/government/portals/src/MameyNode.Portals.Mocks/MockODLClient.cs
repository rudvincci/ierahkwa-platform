using Bogus;
using MameyNode.Portals.Mocks.Interfaces;
using MameyNode.Portals.Mocks.Models;

namespace MameyNode.Portals.Mocks;

public class MockODLClient : IODLClient
{
    private readonly Faker _faker = new();
    private readonly List<ODLInfo> _odlOperations = new();

    public MockODLClient()
    {
        InitializeMockData();
    }

    private void InitializeMockData()
    {
        var odlFaker = new Faker<ODLInfo>()
            .RuleFor(o => o.ODLId, f => $"ODL-{f.Random.AlphaNumeric(10).ToUpper()}")
            .RuleFor(o => o.ProviderId, f => $"PROV-{f.Random.AlphaNumeric(8)}")
            .RuleFor(o => o.FromCurrency, f => f.PickRandom("USD", "EUR", "GBP"))
            .RuleFor(o => o.ToCurrency, f => f.PickRandom("USD", "EUR", "GBP", "CAD", "AUD"))
            .RuleFor(o => o.Amount, f => f.Finance.Amount(10000, 1000000, 2).ToString("F2"))
            .RuleFor(o => o.ExchangeRate, f => f.Random.Double(0.5, 2.0).ToString("F4"))
            .RuleFor(o => o.Status, f => f.PickRandom("Pending", "Processing", "Completed", "Failed"))
            .RuleFor(o => o.CreatedAt, f => f.Date.Recent(30))
            .RuleFor(o => o.ExecutedAt, (f, o) => o.Status == "Completed" ? f.Date.Between(o.CreatedAt, DateTime.Now) : null);

        _odlOperations.AddRange(odlFaker.Generate(75));
    }

    public Task<List<ODLInfo>> GetODLOperationsAsync() => Task.FromResult(_odlOperations);
    public Task<ODLInfo?> GetODLOperationAsync(string odlId) => Task.FromResult(_odlOperations.FirstOrDefault(o => o.ODLId == odlId));
}
