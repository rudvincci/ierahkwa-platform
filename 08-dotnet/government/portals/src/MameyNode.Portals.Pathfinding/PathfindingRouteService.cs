using Mamey.BlazorWasm.Routing;

namespace MameyNode.Portals.Pathfinding;

public class PathfindingRouteService : IRouteService
{
    public List<Route> Routes { get; private set; } = new();

    public event Action<List<Route>>? RoutesChanged;

    public Task InitializeAsync(bool menu = false)
    {
        // Routes are now managed by FutureWampumPayRouteService
        // This service returns empty to avoid duplicates
        Routes = new List<Route>();
        return Task.CompletedTask;
    }
}
