using Bogus;

namespace MameyNode.Portals.Mocks;

public interface IMameyShardingClient
{
    // Sharding operations will be added when SDK is available
}

public class MockMameyShardingClient : IMameyShardingClient
{
    private readonly Faker _faker = new();

    public MockMameyShardingClient()
    {
        // Initialize mock sharding data
    }
}
