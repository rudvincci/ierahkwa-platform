using Bogus;

namespace MameyNode.Portals.Mocks;

public interface IMameyNodeManagementClient
{
    // Node management operations will be added when SDK is available
}

public class MockMameyNodeManagementClient : IMameyNodeManagementClient
{
    private readonly Faker _faker = new();

    public MockMameyNodeManagementClient()
    {
        // Initialize mock node management data
    }
}
