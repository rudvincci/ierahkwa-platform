using Bogus;

namespace MameyNode.Portals.Mocks;

public interface IMameyTravelRuleClient
{
    // Travel rule operations will be added when SDK is available
}

public class MockMameyTravelRuleClient : IMameyTravelRuleClient
{
    private readonly Faker _faker = new();

    public MockMameyTravelRuleClient()
    {
        // Initialize mock travel rule data
    }
}
