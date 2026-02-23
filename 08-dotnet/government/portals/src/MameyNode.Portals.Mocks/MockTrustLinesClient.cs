using Bogus;

namespace MameyNode.Portals.Mocks;

public interface IMameyTrustLinesClient
{
    // Trust lines operations will be added when SDK is available
}

public class MockMameyTrustLinesClient : IMameyTrustLinesClient
{
    private readonly Faker _faker = new();

    public MockMameyTrustLinesClient()
    {
        // Initialize mock trust lines data
    }
}
