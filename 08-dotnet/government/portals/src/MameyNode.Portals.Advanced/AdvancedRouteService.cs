using Mamey.BlazorWasm.Routing;

namespace MameyNode.Portals.Advanced;

public class AdvancedRouteService : IRouteService
{
    public List<Route> Routes { get; private set; } = new();

    public event Action<List<Route>>? RoutesChanged;

    public Task InitializeAsync(bool menu = false)
    {
        // Advanced routes are distributed:
        // - /advanced/escrow -> FutureWampumPay (smart-contracts/escrow)
        // - /advanced/tokenization -> FutureWampumX
        // - /advanced/insurance -> FBDETB
        // - /advanced/offline -> PortableBankingNode (upg/offline)
        // - /advanced/satellite -> Node (infrastructure)
        // This service returns empty to avoid duplicates
        Routes = new List<Route>();
        return Task.CompletedTask;
    }
}
