using Mamey.BlazorWasm.Routing;

namespace MameyNode.Portals.Banking;

// Note: Banking routes are now managed by FBDETBRouteService in Areas/FBDETB/
// This service is kept for backward compatibility but routes are delegated to FBDETB
public class BankingRouteService : IRouteService
{
    public List<Route> Routes { get; private set; } = new();

    public event Action<List<Route>>? RoutesChanged;

    public Task InitializeAsync(bool menu = false)
    {
        // Routes are now managed by FBDETBRouteService
        // This service returns empty to avoid duplicates
        Routes = new List<Route>();
        return Task.CompletedTask;
    }
}





