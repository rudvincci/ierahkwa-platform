using Bogus;

namespace MameyNode.Portals.Mocks;

public interface IMameyCryptoExchangeClient
{
    // Crypto exchange operations will be added when SDK is available
}

public class MockMameyCryptoExchangeClient : IMameyCryptoExchangeClient
{
    private readonly Faker _faker = new();

    public MockMameyCryptoExchangeClient()
    {
        // Initialize mock crypto exchange data
    }
}
