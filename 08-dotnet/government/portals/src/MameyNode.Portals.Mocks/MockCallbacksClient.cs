using Bogus;

namespace MameyNode.Portals.Mocks;

public interface IMameyCallbacksClient
{
    // Callbacks operations will be added when SDK is available
}

public class MockMameyCallbacksClient : IMameyCallbacksClient
{
    private readonly Faker _faker = new();

    public MockMameyCallbacksClient()
    {
        // Initialize mock callbacks data
    }
}
