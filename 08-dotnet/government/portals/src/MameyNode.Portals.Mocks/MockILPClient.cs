using Bogus;

namespace MameyNode.Portals.Mocks;

public interface IMameyILPClient
{
    // ILP operations will be added when SDK is available
}

public class MockMameyILPClient : IMameyILPClient
{
    private readonly Faker _faker = new();

    public MockMameyILPClient()
    {
        // Initialize mock ILP data
    }
}
