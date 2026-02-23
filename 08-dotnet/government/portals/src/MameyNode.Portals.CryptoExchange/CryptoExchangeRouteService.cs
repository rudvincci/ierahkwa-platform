using Mamey.BlazorWasm.Routing;

namespace MameyNode.Portals.CryptoExchange;

// Note: CryptoExchange routes are now managed by FutureWampumXRouteService in Areas/FutureWampum/FutureWampumX/
// This service is kept for backward compatibility but routes are delegated to FutureWampumX
public class CryptoExchangeRouteService : IRouteService
{
    public List<Route> Routes { get; private set; } = new();

    public event Action<List<Route>>? RoutesChanged;

    public Task InitializeAsync(bool menu = false)
    {
        // Routes are now managed by FutureWampumXRouteService
        // This service returns empty to avoid duplicates
        Routes = new List<Route>();
        return Task.CompletedTask;
    }
}

