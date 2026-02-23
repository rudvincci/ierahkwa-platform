using Mamey.BlazorWasm.Routing;
using Route = Mamey.BlazorWasm.Routing.Route;

namespace MameyNode.Portals.Web.Components.Areas.BIIS;

public class BIISRouteService : IRouteService
{
    public List<Route> Routes { get; private set; } = new();
    public event Action<List<Route>>? RoutesChanged;

    public Task InitializeAsync(bool menu = false)
    {
        Routes = new List<Route>
        {
            new Route
            {
                Page = "/biis",
                Title = "BIIS",
                Icon = "fas fa-globe-americas",
                AuthenticationRequired = false,
                RequiredRoles = new List<string>(),
                ChildRoutes = new List<Route>
                {
                    new Route { Page = "/biis/liquidity-pool", Title = "Treaty Liquidity Pool", Icon = "fas fa-swimming-pool" },
                    new Route { Page = "/biis/currency-exchange", Title = "Currency Exchange", Icon = "fas fa-exchange-alt" },
                    new Route { Page = "/biis/cross-border", Title = "Cross-Border Settlement", Icon = "fas fa-plane" },
                    new Route { Page = "/biis/interbank", Title = "Interbank Channels", Icon = "fas fa-network-wired" },
                    new Route { Page = "/biis/transparency", Title = "Blockchain Transparency", Icon = "fas fa-eye" },
                    new Route { Page = "/biis/collateralization", Title = "Asset Collateralization", Icon = "fas fa-shield-alt" },
                    new Route { Page = "/biis/identity-compliance", Title = "Identity & Treaty Compliance", Icon = "fas fa-id-card" },
                    new Route { Page = "/biis/zkp-privacy", Title = "ZKP Privacy", Icon = "fas fa-lock" },
                    new Route { Page = "/biis/treaty-enforcement", Title = "Treaty Enforcement", Icon = "fas fa-gavel" },
                    new Route { Page = "/biis/liquidity-risk", Title = "Liquidity Risk Monitoring", Icon = "fas fa-chart-line" }
                }
            }
        };

        RoutesChanged?.Invoke(Routes);
        return Task.CompletedTask;
    }
}

