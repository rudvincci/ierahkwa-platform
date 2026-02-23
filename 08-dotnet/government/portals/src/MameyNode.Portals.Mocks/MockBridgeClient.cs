using Bogus;

namespace MameyNode.Portals.Mocks;

public interface IMameyBridgeClient
{
    // Bridge operations will be added when SDK is available
}

public class MockMameyBridgeClient : IMameyBridgeClient
{
    private readonly Faker _faker = new();

    public MockMameyBridgeClient()
    {
        // Initialize mock bridge data
    }
}
