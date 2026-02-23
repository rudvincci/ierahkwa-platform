using Mamey.BlazorWasm.Routing;

namespace MameyNode.Portals.Citizen;

// Note: Citizen routes are part of Government Services application
// Routes mapped to /citizen/* for citizen portal access
// This service is kept for backward compatibility
public class CitizenRouteService : IRouteService
{
    public List<Route> Routes { get; private set; } = new();

    public event Action<List<Route>>? RoutesChanged;

    public Task InitializeAsync(bool menu = true)
    {
        Routes = new List<Route>
        {
            new Route
            {
                Page = "/citizen",
                Title = "Citizen Portal",
                Icon = "fas fa-user-circle",
                AuthenticationRequired = false,
                RequiredRoles = new List<string>(),
                ChildRoutes = new List<Route>
                {
                    new Route { Page = "/citizen/wallet", Title = "Wallet", Icon = "fas fa-wallet" },
                    new Route { Page = "/citizen/transactions", Title = "Transactions", Icon = "fas fa-list" }
                }
            }
        };
        
        RoutesChanged?.Invoke(Routes);
        return Task.CompletedTask;
    }
}





