using Bogus;

namespace MameyNode.Portals.Mocks;

public interface IMameyRBACClient
{
    // RBAC operations will be added when SDK is available
}

public class MockMameyRBACClient : IMameyRBACClient
{
    private readonly Faker _faker = new();

    public MockMameyRBACClient()
    {
        // Initialize mock RBAC data
    }
}
