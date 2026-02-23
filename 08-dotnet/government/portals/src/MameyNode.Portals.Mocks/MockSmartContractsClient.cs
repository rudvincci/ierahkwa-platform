using Bogus;

namespace MameyNode.Portals.Mocks;

public interface IMameySmartContractsClient
{
    // Smart contract operations will be added when SDK is available
}

public class MockMameySmartContractsClient : IMameySmartContractsClient
{
    private readonly Faker _faker = new();

    public MockMameySmartContractsClient()
    {
        // Initialize mock smart contract data
    }
}
