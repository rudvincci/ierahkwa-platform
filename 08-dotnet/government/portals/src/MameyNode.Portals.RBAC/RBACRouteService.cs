using Mamey.BlazorWasm.Routing;

namespace MameyNode.Portals.RBAC;

public class RBACRouteService : IRouteService
{
    public List<Route> Routes { get; private set; } = new();

    public event Action<List<Route>>? RoutesChanged;

    public Task InitializeAsync(bool menu = false)
    {
        // Routes are now managed by GovernmentRouteService
        // This service returns empty to avoid duplicates
        Routes = new List<Route>();
        return Task.CompletedTask;
    }
}
