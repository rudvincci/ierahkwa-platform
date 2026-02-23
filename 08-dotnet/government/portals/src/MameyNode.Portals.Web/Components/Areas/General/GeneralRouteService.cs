using Mamey.BlazorWasm.Routing;
using Route = Mamey.BlazorWasm.Routing.Route;

namespace MameyNode.Portals.Web.Components.Areas.General;

public class GeneralRouteService : IRouteService
{
    public List<Route> Routes { get; private set; } = new();
    public event Action<List<Route>>? RoutesChanged;

    public Task InitializeAsync(bool menu = false)
    {
        Routes = new List<Route>
        {
            new Route
            {
                Page = "/explorer",
                Title = "General Portal",
                Icon = "fas fa-globe",
                AuthenticationRequired = false,
                RequiredRoles = new List<string>(),
                ChildRoutes = new List<Route>
                {
                    new Route { Page = "/explorer/blocks", Title = "Block Explorer", Icon = "fas fa-cubes" },
                    new Route { Page = "/explorer/transactions", Title = "Transaction Explorer", Icon = "fas fa-list" },
                    new Route { Page = "/explorer/accounts", Title = "Account Explorer", Icon = "fas fa-wallet" },
                    new Route { Page = "/explorer/network", Title = "Network Statistics", Icon = "fas fa-chart-line" },
                    new Route { Page = "/explorer/node", Title = "Node Information", Icon = "fas fa-server" },
                    new Route { Page = "/explorer/analytics", Title = "Chain Analytics", Icon = "fas fa-chart-bar" }
                }
            }
        };
        
        RoutesChanged?.Invoke(Routes);
        return Task.CompletedTask;
    }
}


