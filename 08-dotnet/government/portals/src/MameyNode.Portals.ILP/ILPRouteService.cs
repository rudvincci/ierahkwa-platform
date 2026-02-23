using Mamey.BlazorWasm.Routing;

namespace MameyNode.Portals.ILP;

public class ILPRouteService : IRouteService
{
    public List<Route> Routes { get; private set; } = new();

    public event Action<List<Route>>? RoutesChanged;

    public Task InitializeAsync(bool menu = false)
    {
        Routes = new List<Route>
        {
            new Route
            {
                Page = "/ilp",
                Title = "ILP",
                Icon = "fas fa-network-wired",
                AuthenticationRequired = false,  // Disabled for UI development
                RequiredRoles = new List<string>(),  // Disabled for UI development
                ChildRoutes = new List<Route>
                {
                    new Route { Page = "/ilp/packets", Title = "Packets", Icon = "fas fa-box" },
                    new Route { Page = "/ilp/connector", Title = "Connector", Icon = "fas fa-plug" },
                    new Route { Page = "/ilp/service", Title = "ILP Service", Icon = "fas fa-server" },
                    new Route { Page = "/ilp/routing", Title = "ILP Routing", Icon = "fas fa-route" },
                    new Route { Page = "/ilp/ledger-integration", Title = "Ledger Integration", Icon = "fas fa-book" },
                    new Route { Page = "/ilp/handler", Title = "Packet Handler", Icon = "fas fa-cogs" },
                    new Route { Page = "/ilp/settlement", Title = "Settlement", Icon = "fas fa-handshake" },
                }
            }
        };

        RoutesChanged?.Invoke(Routes);
        return Task.CompletedTask;
    }
}
