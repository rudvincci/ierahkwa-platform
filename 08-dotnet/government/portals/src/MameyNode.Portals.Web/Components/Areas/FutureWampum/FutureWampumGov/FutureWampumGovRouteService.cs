using Mamey.BlazorWasm.Routing;
using Route = Mamey.BlazorWasm.Routing.Route;

namespace MameyNode.Portals.Web.Components.Areas.FutureWampum.FutureWampumGov;

public class FutureWampumGovRouteService : IRouteService
{
    public List<Route> Routes { get; private set; } = new();
    public event Action<List<Route>>? RoutesChanged;

    public Task InitializeAsync(bool menu = false)
    {
        Routes = new List<Route>
        {
            new Route
            {
                Page = "/futurewampum/gov",
                Title = "FutureWampumGov",
                Icon = "fas fa-building",
                AuthenticationRequired = false,
                RequiredRoles = new List<string>(),
                ChildRoutes = new List<Route>
                {
                    new Route { Page = "/futurewampum/gov/disbursements", Title = "Disbursements", Icon = "fas fa-hand-holding-usd" },
                    new Route { Page = "/futurewampum/gov/ubi", Title = "UBI Programs", Icon = "fas fa-users" },
                    new Route { Page = "/futurewampum/gov/budget", Title = "Budget Allocation", Icon = "fas fa-chart-pie" },
                    new Route { Page = "/futurewampum/gov/programs", Title = "Program Disbursement", Icon = "fas fa-tasks" },
                    new Route { Page = "/futurewampum/gov/transparency", Title = "Transparency Dashboard", Icon = "fas fa-eye" }
                }
            }
        };

        RoutesChanged?.Invoke(Routes);
        return Task.CompletedTask;
    }
}

