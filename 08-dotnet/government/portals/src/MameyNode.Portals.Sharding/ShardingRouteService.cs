using Mamey.BlazorWasm.Routing;

namespace MameyNode.Portals.Sharding;

public class ShardingRouteService : IRouteService
{
    public List<Route> Routes { get; private set; } = new();

    public event Action<List<Route>>? RoutesChanged;

    public Task InitializeAsync(bool menu = false)
    {
        Routes = new List<Route>
        {
            new Route
            {
                Page = "/sharding",
                Title = "Sharding",
                Icon = "fas fa-layer-group",
                AuthenticationRequired = false,  // Disabled for UI development
                RequiredRoles = new List<string>(),  // Disabled for UI development
                ChildRoutes = new List<Route>
                {
                    new Route { Page = "/sharding/management", Title = "Shard Management", Icon = "fas fa-cogs" },
                    new Route { Page = "/sharding/assignment", Title = "Shard Assignment", Icon = "fas fa-tasks" },
                    new Route { Page = "/sharding/routing", Title = "Cross-Shard Routing", Icon = "fas fa-route" },
                    new Route { Page = "/sharding/cross-shard", Title = "Cross-Shard Communication", Icon = "fas fa-comments" },
                    new Route { Page = "/sharding/beacon-chain", Title = "Beacon Chain", Icon = "fas fa-link" },
                    new Route { Page = "/sharding/state", Title = "State Management", Icon = "fas fa-database" },
                    new Route { Page = "/sharding/hashing", Title = "Consistent Hashing", Icon = "fas fa-hashtag" },
                    new Route { Page = "/sharding/coordination", Title = "Transaction Coordination", Icon = "fas fa-sync" },
                    new Route { Page = "/sharding/validation", Title = "Shard Validation", Icon = "fas fa-check-circle" },
                    new Route { Page = "/sharding/consensus", Title = "Shard Consensus", Icon = "fas fa-handshake" },
                }
            }
        };

        RoutesChanged?.Invoke(Routes);
        return Task.CompletedTask;
    }
}
