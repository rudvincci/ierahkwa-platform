using Mamey.BlazorWasm.Routing;

namespace MameyNode.Portals.Lending;

// Note: Lending routes are now part of FBDETB (Future BDET Bank) application
// Routes mapped to /banking/lending/* in FBDETBRouteService
// This service is kept for backward compatibility but routes are delegated to FBDETB
public class LendingRouteService : IRouteService
{
    public List<Route> Routes { get; private set; } = new();

    public event Action<List<Route>>? RoutesChanged;

    public Task InitializeAsync(bool menu = false)
    {
        // Routes are now managed by FBDETBRouteService under /banking/lending/*
        // This service returns empty to avoid duplicates
        Routes = new List<Route>();
        return Task.CompletedTask;
    }
}

