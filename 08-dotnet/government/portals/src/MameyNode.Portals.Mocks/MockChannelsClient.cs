using Bogus;

namespace MameyNode.Portals.Mocks;

public interface IMameyChannelsClient
{
    // Channels operations will be added when SDK is available
}

public class MockMameyChannelsClient : IMameyChannelsClient
{
    private readonly Faker _faker = new();

    public MockMameyChannelsClient()
    {
        // Initialize mock channels data
    }
}
