using Mamey.BlazorWasm.Routing;

namespace MameyNode.Portals.Payments;

// Note: Payment routes are now managed by FutureWampumPay application
// Routes mapped to /payments/* in FutureWampumPay route service
// This service is kept for backward compatibility but routes are delegated to FutureWampumPay
public class PaymentsRouteService : IRouteService
{
    public List<Route> Routes { get; private set; } = new();

    public event Action<List<Route>>? RoutesChanged;

    public Task InitializeAsync(bool menu = false)
    {
        // Routes are now managed by FutureWampumPay route service
        // This service returns empty to avoid duplicates
        Routes = new List<Route>();
        return Task.CompletedTask;
    }
}

