using Mamey.BlazorWasm.Routing;

namespace MameyNode.Portals.Government;

public class GovernmentRouteService : IRouteService
{
    public List<Route> Routes { get; private set; } = new();

    public event Action<List<Route>>? RoutesChanged;

    public Task InitializeAsync(bool menu = false)
    {
        Routes = new List<Route>
        {
            new Route
            {
                Page = "/admin",
                Title = "Government",
                Icon = "fas fa-building",
                AuthenticationRequired = false,  // Disabled for UI development
                RequiredRoles = new List<string>(),  // Disabled for UI development
                ChildRoutes = new List<Route>
                {
                    new Route { Page = "/admin/nodes", Title = "Node Management", Icon = "fas fa-server" },
                    new Route { Page = "/admin/compliance", Title = "Compliance", Icon = "fas fa-gavel" },
                    new Route { Page = "/admin/identity", Title = "Identity Management", Icon = "fas fa-id-card" }
                }
            }
        };
        
        return Task.CompletedTask;
    }
}
