using Mamey.BlazorWasm.Routing;
using Route = Mamey.BlazorWasm.Routing.Route;

namespace MameyNode.Portals.Web.Components.Areas.FutureWampum.FutureWampumMerchant;

public class FutureWampumMerchantRouteService : IRouteService
{
    public List<Route> Routes { get; private set; } = new();
    public event Action<List<Route>>? RoutesChanged;

    public Task InitializeAsync(bool menu = false)
    {
        Routes = new List<Route>
        {
            new Route
            {
                Page = "/merchant",
                Title = "FutureWampumMerchant",
                Icon = "fas fa-store",
                AuthenticationRequired = false,
                RequiredRoles = new List<string>(),
                ChildRoutes = new List<Route>
                {
                    new Route { Page = "/merchant/onboarding", Title = "Merchant Onboarding", Icon = "fas fa-user-plus" },
                    new Route { Page = "/merchant/payments", Title = "Payment Processing", Icon = "fas fa-credit-card" },
                    new Route { Page = "/merchant/settlement", Title = "Settlement", Icon = "fas fa-handshake" },
                    new Route { Page = "/merchant/invoices", Title = "Invoicing", Icon = "fas fa-file-invoice" },
                    new Route { Page = "/merchant/qr-codes", Title = "QR Codes", Icon = "fas fa-qrcode" },
                    new Route { Page = "/merchant/analytics", Title = "Analytics", Icon = "fas fa-chart-line" },
                    new Route { Page = "/merchant/compliance", Title = "Compliance", Icon = "fas fa-gavel" }
                }
            }
        };

        RoutesChanged?.Invoke(Routes);
        return Task.CompletedTask;
    }
}

