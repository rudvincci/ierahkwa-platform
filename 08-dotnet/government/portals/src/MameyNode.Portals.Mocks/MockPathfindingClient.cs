using Bogus;

namespace MameyNode.Portals.Mocks;

public interface IMameyPathfindingClient
{
    // Pathfinding operations will be added when SDK is available
}

public class MockMameyPathfindingClient : IMameyPathfindingClient
{
    private readonly Faker _faker = new();

    public MockMameyPathfindingClient()
    {
        // Initialize mock pathfinding data
    }
}
