using Mamey.BlazorWasm.Routing;

namespace MameyNode.Portals.General;

// Note: Explorer routes are general MameyNode routes (block explorer)
// Portable Node routes are part of SICB application (mapped to /portable in SICBRouteService)
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
                Title = "Block Explorer",
                Icon = "fas fa-search",
                AuthenticationRequired = false,
                RequiredRoles = new List<string>(),
                ChildRoutes = new List<Route>
                {
                    new Route { Page = "/explorer/blocks", Title = "Blocks", Icon = "fas fa-cubes" },
                    new Route { Page = "/explorer/transactions", Title = "Transactions", Icon = "fas fa-list" },
                    new Route { Page = "/explorer/accounts", Title = "Accounts", Icon = "fas fa-wallet" }
                }
            }
        };
        
        RoutesChanged?.Invoke(Routes);
        return Task.CompletedTask;
    }
}





