using Mamey.BlazorWasm.Routing;

namespace MameyNode.Portals.Metrics;

public class MetricsRouteService : IRouteService
{
    public List<Route> Routes { get; private set; } = new();

    public event Action<List<Route>>? RoutesChanged;

    public Task InitializeAsync(bool menu = false)
    {
        Routes = new List<Route>
        {
            new Route
            {
                Page = "/metrics",
                Title = "Metrics",
                Icon = "fas fa-chart-bar",
                AuthenticationRequired = false,  // Disabled for UI development
                RequiredRoles = new List<string>(),  // Disabled for UI development
                ChildRoutes = new List<Route>
                {
                    new Route { Page = "/metrics/collector", Title = "Metrics Collector", Icon = "fas fa-database" },
                    new Route { Page = "/metrics/registry", Title = "Metrics Registry", Icon = "fas fa-book" },
                    new Route { Page = "/metrics/http-endpoint", Title = "HTTP Endpoint", Icon = "fas fa-server" },
                    new Route { Page = "/metrics/observability", Title = "Enhanced Observability", Icon = "fas fa-eye" },
                    new Route { Page = "/metrics/health-checks", Title = "Health Checks", Icon = "fas fa-heartbeat" },
                    new Route { Page = "/metrics/monitoring", Title = "Enhanced Monitoring", Icon = "fas fa-chart-line" },
                }
            }
        };

        RoutesChanged?.Invoke(Routes);
        return Task.CompletedTask;
    }
}
