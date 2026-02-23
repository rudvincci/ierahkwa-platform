using Mamey.BlazorWasm.Routing;
using Route = Mamey.BlazorWasm.Routing.Route;

namespace MameyNode.Portals.Web.Components.Areas.Infrastructure;

public class InfrastructureRouteService : IRouteService
{
    public List<Route> Routes { get; private set; } = new();
    public event Action<List<Route>>? RoutesChanged;

    public Task InitializeAsync(bool menu = false)
    {
        Routes = new List<Route>
        {
            new Route
            {
                Page = "/mameynode",
                Title = "MameyNode",
                Icon = "fas fa-server",
                AuthenticationRequired = false,
                RequiredRoles = new List<string>(),
                ChildRoutes = new List<Route>
                {
                    // Block Explorer
                    new Route { Page = "/explorer", Title = "Block Explorer", Icon = "fas fa-search" },
                    // ILP (Interledger Protocol)
                    new Route { Page = "/ilp", Title = "ILP", Icon = "fas fa-network-wired" },
                    // Metrics
                    new Route { Page = "/metrics", Title = "Metrics", Icon = "fas fa-chart-bar" },
                    // Node Management
                    new Route { Page = "/node", Title = "Node", Icon = "fas fa-server" },
                    // Sharding
                    new Route { Page = "/sharding", Title = "Sharding", Icon = "fas fa-layer-group" },
                    // Webhooks
                    new Route { Page = "/webhooks", Title = "Webhooks", Icon = "fas fa-webhook" },
                    // Callbacks
                    new Route { Page = "/callbacks", Title = "Callbacks", Icon = "fas fa-phone" },
                    // Advanced - Satellite
                    new Route { Page = "/advanced/satellite", Title = "Satellite", Icon = "fas fa-satellite" }
                }
            }
        };

        RoutesChanged?.Invoke(Routes);
        return Task.CompletedTask;
    }
}
