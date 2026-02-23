using Mamey.BlazorWasm.Routing;

namespace MameyNode.Portals.Node;

public class NodeRouteService : IRouteService
{
    public List<Route> Routes { get; private set; } = new();

    public event Action<List<Route>>? RoutesChanged;

    public Task InitializeAsync(bool menu = false)
    {
        Routes = new List<Route>
        {
            new Route
            {
                Page = "/node",
                Title = "Node",
                Icon = "fas fa-server",
                AuthenticationRequired = false,  // Disabled for UI development
                RequiredRoles = new List<string>(),  // Disabled for UI development
                ChildRoutes = new List<Route>
                {
                    new Route { Page = "/node/deployment", Title = "Deployment", Icon = "fas fa-rocket" },
                    new Route { Page = "/node/orchestration", Title = "Container Orchestration", Icon = "fas fa-docker" },
                    new Route { Page = "/node/disaster-recovery", Title = "Disaster Recovery", Icon = "fas fa-redo" },
                    new Route { Page = "/node/security", Title = "Enhanced Security", Icon = "fas fa-shield-alt" },
                    new Route { Page = "/node/multi-region", Title = "Multi-Region", Icon = "fas fa-globe" },
                    new Route { Page = "/node/performance", Title = "Performance Validation", Icon = "fas fa-tachometer-alt" },
                    new Route { Page = "/node/audit", Title = "Security Audit", Icon = "fas fa-clipboard-check" },
                }
            }
        };

        RoutesChanged?.Invoke(Routes);
        return Task.CompletedTask;
    }
}
