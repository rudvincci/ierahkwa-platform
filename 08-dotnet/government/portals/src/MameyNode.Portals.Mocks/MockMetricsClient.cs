using Bogus;

namespace MameyNode.Portals.Mocks;

public interface IMameyMetricsClient
{
    // Metrics operations will be added when SDK is available
}

public class MockMameyMetricsClient : IMameyMetricsClient
{
    private readonly Faker _faker = new();

    public MockMameyMetricsClient()
    {
        // Initialize mock metrics data
    }
}
