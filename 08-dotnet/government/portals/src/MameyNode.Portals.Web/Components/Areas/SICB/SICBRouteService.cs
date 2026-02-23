using Mamey.BlazorWasm.Routing;
using Route = Mamey.BlazorWasm.Routing.Route;

namespace MameyNode.Portals.Web.Components.Areas.SICB;

public class SICBRouteService : IRouteService
{
    public List<Route> Routes { get; private set; } = new();
    public event Action<List<Route>>? RoutesChanged;

    public Task InitializeAsync(bool menu = false)
    {
        Routes = new List<Route>
        {
            new Route
            {
                Page = "/sicb",
                Title = "SICB",
                Icon = "fas fa-landmark",
                AuthenticationRequired = false,
                RequiredRoles = new List<string>(),
                ChildRoutes = new List<Route>
                {
                    new Route { Page = "/sicb/monetary-instruments", Title = "Monetary Instruments", Icon = "fas fa-coins" },
                    new Route { Page = "/sicb/ledger-reserves", Title = "Ledger & Reserves", Icon = "fas fa-book" },
                    new Route { Page = "/sicb/lending", Title = "Credit & Lending", Icon = "fas fa-hand-holding-usd" },
                    new Route { Page = "/sicb/monetary-policy", Title = "Monetary Policy", Icon = "fas fa-sliders-h" },
                    new Route { Page = "/sicb/fiscal-operations", Title = "Fiscal Operations", Icon = "fas fa-building" },
                    new Route { Page = "/sicb/treasury", Title = "Treasury Programs", Icon = "fas fa-chart-pie" },
                    new Route { Page = "/sicb/foreign-exchange", Title = "Foreign Exchange", Icon = "fas fa-globe" },
                    new Route { Page = "/sicb/compliance", Title = "Compliance & Enforcement", Icon = "fas fa-gavel" },
                    new Route { Page = "/sicb/citizen-tools", Title = "Citizen Tools", Icon = "fas fa-users" },
                    new Route { Page = "/sicb/system-integrity", Title = "System Integrity", Icon = "fas fa-shield-alt" },
                    new Route { Page = "/sicb/treasury-instruments", Title = "Treasury Instruments", Icon = "fas fa-briefcase" },
                    new Route { Page = "/portable", Title = "Portable Banking Node", Icon = "fas fa-laptop-code" },
                    // UPG (Universal Protocol Gateway) - Part of PortableBankingNode
                    new Route { Page = "/upg", Title = "Universal Protocol Gateway", Icon = "fas fa-globe" },
                    new Route { Page = "/upg/protocol-support", Title = "Protocol Support", Icon = "fas fa-network-wired" },
                    new Route { Page = "/upg/adapters", Title = "Adapters", Icon = "fas fa-plug" },
                    new Route { Page = "/upg/normalization", Title = "Normalization", Icon = "fas fa-align-center" },
                    new Route { Page = "/upg/multi-rail", Title = "Multi-Rail Routing", Icon = "fas fa-route" },
                    new Route { Page = "/upg/hsm", Title = "HSM", Icon = "fas fa-key" },
                    new Route { Page = "/upg/fx", Title = "Foreign Exchange", Icon = "fas fa-exchange-alt" },
                    new Route { Page = "/upg/pos", Title = "Point of Sale", Icon = "fas fa-cash-register" },
                    new Route { Page = "/upg/offline", Title = "Offline Payments", Icon = "fas fa-wifi-slash" },
                    new Route { Page = "/upg/merchant", Title = "Merchant Services", Icon = "fas fa-store" },
                    new Route { Page = "/upg/real-time", Title = "Real-Time Payments", Icon = "fas fa-bolt" },
                    // ODL (On-Demand Liquidity) - Part of SICB liquidity management
                    new Route { Page = "/odl", Title = "On-Demand Liquidity", Icon = "fas fa-exchange-alt" },
                    new Route { Page = "/odl/liquidity", Title = "Liquidity Management", Icon = "fas fa-swimming-pool" },
                    new Route { Page = "/odl/exchange-rate", Title = "Exchange Rate Oracle", Icon = "fas fa-chart-line" },
                    new Route { Page = "/odl/payment-execution", Title = "Payment Execution", Icon = "fas fa-play" },
                    new Route { Page = "/odl/provider-management", Title = "Provider Management", Icon = "fas fa-users-cog" },
                    new Route { Page = "/odl/validation", Title = "Validation", Icon = "fas fa-check-circle" },
                    new Route { Page = "/odl/bridge-currency", Title = "Bridge Currency", Icon = "fas fa-coins" }
                }
            }
        };

        RoutesChanged?.Invoke(Routes);
        return Task.CompletedTask;
    }
}

