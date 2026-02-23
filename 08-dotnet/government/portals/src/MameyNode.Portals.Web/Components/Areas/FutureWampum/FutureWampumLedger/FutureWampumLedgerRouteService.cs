using Mamey.BlazorWasm.Routing;
using Route = Mamey.BlazorWasm.Routing.Route;

namespace MameyNode.Portals.Web.Components.Areas.FutureWampum.FutureWampumLedger;

public class FutureWampumLedgerRouteService : IRouteService
{
    public List<Route> Routes { get; private set; } = new();
    public event Action<List<Route>>? RoutesChanged;

    public Task InitializeAsync(bool menu = false)
    {
        Routes = new List<Route>
        {
            new Route
            {
                Page = "/ledger",
                Title = "FutureWampumLedger",
                Icon = "fas fa-book",
                AuthenticationRequired = false,
                RequiredRoles = new List<string>(),
                ChildRoutes = new List<Route>
                {
                    new Route { Page = "/ledger/transactions", Title = "Transactions", Icon = "fas fa-list" },
                    new Route { Page = "/ledger/blocks", Title = "Blocks", Icon = "fas fa-cubes" },
                    new Route { Page = "/ledger/transparency", Title = "Transparency", Icon = "fas fa-eye" },
                    new Route { Page = "/ledger/audit", Title = "Audit Trail", Icon = "fas fa-shield-alt" },
                    new Route { Page = "/ledger/sync", Title = "Synchronization", Icon = "fas fa-sync" },
                    // Ledger Integration - Part of FutureWampumLedger
                    new Route { Page = "/ledger-integration", Title = "Ledger Integration", Icon = "fas fa-book" },
                    new Route { Page = "/ledger-integration/transaction-logging", Title = "Transaction Logging", Icon = "fas fa-file-alt" },
                    new Route { Page = "/ledger-integration/compliance", Title = "Compliance", Icon = "fas fa-shield-alt" },
                    new Route { Page = "/ledger-integration/currency-registry", Title = "Currency Registry", Icon = "fas fa-coins" },
                    new Route { Page = "/ledger-integration/credit-tracking", Title = "Credit Tracking", Icon = "fas fa-chart-line" },
                    new Route { Page = "/ledger-integration/transparency", Title = "Transparency", Icon = "fas fa-eye" }
                }
            }
        };

        RoutesChanged?.Invoke(Routes);
        return Task.CompletedTask;
    }
}

