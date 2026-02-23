using Mamey.BlazorWasm.Routing;

namespace MameyNode.Portals.Webhooks;

public class WebhooksRouteService : IRouteService
{
    public List<Route> Routes { get; private set; } = new();

    public event Action<List<Route>>? RoutesChanged;

    public Task InitializeAsync(bool menu = false)
    {
        Routes = new List<Route>
        {
            new Route
            {
                Page = "/webhooks",
                Title = "Webhooks",
                Icon = "fas fa-webhook",
                AuthenticationRequired = false,  // Disabled for UI development
                RequiredRoles = new List<string>(),  // Disabled for UI development
                ChildRoutes = new List<Route>
                {
                    new Route { Page = "/webhooks/management", Title = "Webhook Management", Icon = "fas fa-cogs" },
                    new Route { Page = "/webhooks/http-client", Title = "HTTP Client", Icon = "fas fa-server" },
                    new Route { Page = "/webhooks/queue", Title = "Webhook Queue", Icon = "fas fa-list" },
                    new Route { Page = "/webhooks/signatures", Title = "Signatures", Icon = "fas fa-signature" },
                    new Route { Page = "/webhooks/persistence", Title = "Persistence", Icon = "fas fa-database" },
                    new Route { Page = "/webhooks/health", Title = "Webhook Health", Icon = "fas fa-heartbeat" },
                    new Route { Page = "/webhooks/rate-limiting", Title = "Rate Limiting", Icon = "fas fa-tachometer-alt" },
                    new Route { Page = "/webhooks/api", Title = "Webhook API", Icon = "fas fa-code" },
                    new Route { Page = "/webhooks/validation", Title = "Validation", Icon = "fas fa-check-circle" },
                }
            }
        };

        RoutesChanged?.Invoke(Routes);
        return Task.CompletedTask;
    }
}
