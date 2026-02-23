using Bogus;

namespace MameyNode.Portals.Mocks;

public interface IMameyAccountAbstractionClient
{
    // Account abstraction operations will be added when SDK is available
}

public class MockMameyAccountAbstractionClient : IMameyAccountAbstractionClient
{
    private readonly Faker _faker = new();

    public MockMameyAccountAbstractionClient()
    {
        // Initialize mock account abstraction data
    }
}
