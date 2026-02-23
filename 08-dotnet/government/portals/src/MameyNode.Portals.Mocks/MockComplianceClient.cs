using Bogus;

namespace MameyNode.Portals.Mocks;

public interface IMameyComplianceClient
{
    // Compliance operations will be added when SDK is available
}

public class MockMameyComplianceClient : IMameyComplianceClient
{
    private readonly Faker _faker = new();

    public MockMameyComplianceClient()
    {
        // Initialize mock compliance data
    }
}
