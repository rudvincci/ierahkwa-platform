using Bogus;

namespace MameyNode.Portals.Mocks;

public interface IMameyWebhooksClient
{
    // Webhooks operations will be added when SDK is available
}

public class MockMameyWebhooksClient : IMameyWebhooksClient
{
    private readonly Faker _faker = new();

    public MockMameyWebhooksClient()
    {
        // Initialize mock webhooks data
    }
}
