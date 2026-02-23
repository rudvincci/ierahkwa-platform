using Mamey.BlazorWasm.Routing;

namespace MameyNode.Portals.Callbacks;

public class CallbacksRouteService : IRouteService
{
    public List<Route> Routes { get; private set; } = new();

    public event Action<List<Route>>? RoutesChanged;

    public Task InitializeAsync(bool menu = false)
    {
        Routes = new List<Route>
        {
            new Route
            {
                Page = "/callbacks",
                Title = "Callbacks",
                Icon = "fas fa-phone",
                AuthenticationRequired = false,  // Disabled for UI development
                RequiredRoles = new List<string>(),  // Disabled for UI development
                ChildRoutes = new List<Route>
                {
                    new Route { Page = "/callbacks/transaction", Title = "Transaction Callbacks", Icon = "fas fa-exchange-alt" },
                    new Route { Page = "/callbacks/settlement", Title = "Settlement Callbacks", Icon = "fas fa-handshake" },
                    new Route { Page = "/callbacks/account", Title = "Account Callbacks", Icon = "fas fa-user-circle" },
                    new Route { Page = "/callbacks/manager", Title = "Callback Manager", Icon = "fas fa-cogs" },
                }
            }
        };

        RoutesChanged?.Invoke(Routes);
        return Task.CompletedTask;
    }
}
