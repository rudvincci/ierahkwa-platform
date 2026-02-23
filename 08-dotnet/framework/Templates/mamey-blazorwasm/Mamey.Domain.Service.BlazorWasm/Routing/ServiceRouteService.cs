using Mamey.BlazorWasm.Routing;
using Microsoft.AspNetCore.Components.Routing;

namespace Mamey.Domain.Service.BlazorWasm.Routing;

public class ServiceRouteService : IRouteService
{
    public event Action<List<Route>>? RoutesChanged;
    private List<Route> _routes = new();

    public List<Route> Routes => _routes;

    public async Task InitializeAsync(bool menu = false)
    {
        // Parse required roles from comma-separated string
        var requiredRoles = new List<string>();
        if (!string.IsNullOrWhiteSpace("RequiredRoles"))
        {
            if ("RequiredRoles".Contains(','))
            {
                requiredRoles = "RequiredRoles".Split(',').Select(r => r.Trim()).ToList();
            }
            else
            {
                requiredRoles.Add("RequiredRoles".Trim());
            }
        }

        _routes = new List<Route>
        {
            new Route
            {
                Title = "Service",
                Icon = Icons.Material.Filled.Icon,
                Page = "/service",
                Match = NavLinkMatch.Prefix,
                AuthenticationRequired = requiredRoles.Any(),
                RequiredRoles = requiredRoles,
                ChildRoutes = new List<Route>
                {
                    new Route
                    {
                        Title = "Dashboard",
                        Page = "/service",
                        Icon = Icons.Material.Filled.Dashboard,
                        Match = NavLinkMatch.All,
                        RequiredRoles = requiredRoles
                    },
                    new Route
                    {
                        Title = "Create",
                        Page = "/service/create",
                        Icon = Icons.Material.Filled.Add,
                        Match = NavLinkMatch.All,
                        RequiredRoles = requiredRoles
                    }
                }
            }
        };
        
        RoutesChanged?.Invoke(_routes);
        await Task.CompletedTask;
    }
}

