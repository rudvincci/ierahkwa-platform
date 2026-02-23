using Bogus;

namespace MameyNode.Portals.Mocks;

public interface IMameyProgrammableClient
{
    // Programmable money operations will be added when SDK is available
}

public class MockMameyProgrammableClient : IMameyProgrammableClient
{
    private readonly Faker _faker = new();

    public MockMameyProgrammableClient()
    {
        // Initialize mock programmable money data
    }
}
