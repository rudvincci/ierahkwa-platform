using Bogus;

namespace MameyNode.Portals.Mocks;

public interface IMameyAdvancedClient
{
    // Advanced operations will be added when SDK is available
}

public class MockMameyAdvancedClient : IMameyAdvancedClient
{
    private readonly Faker _faker = new();

    public MockMameyAdvancedClient()
    {
        // Initialize mock advanced services data
    }
}
