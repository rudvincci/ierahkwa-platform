using Mamey.BlazorWasm.Routing;
using Route = Mamey.BlazorWasm.Routing.Route;

namespace MameyNode.Portals.Web.Components.Areas.FutureWampum.FutureWampumId;

public class FutureWampumIdRouteService : IRouteService
{
    public List<Route> Routes { get; private set; } = new();
    public event Action<List<Route>>? RoutesChanged;

    public Task InitializeAsync(bool menu = false)
    {
        Routes = new List<Route>
        {
            new Route
            {
                Page = "/futurewampum/identity",
                Title = "FutureWampumID",
                Icon = "fas fa-id-card",
                AuthenticationRequired = false,
                RequiredRoles = new List<string>(),
                ChildRoutes = new List<Route>
                {
                    new Route { Page = "/futurewampum/identity/verification", Title = "Identity Verification", Icon = "fas fa-check-circle" },
                    new Route { Page = "/futurewampum/identity/credentials", Title = "Digital Credentials", Icon = "fas fa-certificate" },
                    new Route { Page = "/futurewampum/identity/wallets", Title = "Identity Wallets", Icon = "fas fa-wallet" },
                    new Route { Page = "/futurewampum/identity/attestations", Title = "Attestations", Icon = "fas fa-stamp" },
                    new Route { Page = "/futurewampum/identity/recovery", Title = "Recovery", Icon = "fas fa-key" }
                }
            }
        };

        RoutesChanged?.Invoke(Routes);
        return Task.CompletedTask;
    }
}
