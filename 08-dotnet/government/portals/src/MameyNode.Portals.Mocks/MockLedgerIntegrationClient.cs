using Bogus;

namespace MameyNode.Portals.Mocks;

public interface IMameyLedgerIntegrationClient
{
    // Ledger integration operations will be added when SDK is available
}

public class MockMameyLedgerIntegrationClient : IMameyLedgerIntegrationClient
{
    private readonly Faker _faker = new();

    public MockMameyLedgerIntegrationClient()
    {
        // Initialize mock ledger integration data
    }
}
