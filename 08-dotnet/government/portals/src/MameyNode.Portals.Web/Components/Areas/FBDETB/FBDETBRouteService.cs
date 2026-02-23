using Mamey.BlazorWasm.Routing;
using Route = Mamey.BlazorWasm.Routing.Route;

namespace MameyNode.Portals.Web.Components.Areas.FBDETB;

public class FBDETBRouteService : IRouteService
{
    public List<Route> Routes { get; private set; } = new();
    public event Action<List<Route>>? RoutesChanged;

    public Task InitializeAsync(bool menu = false)
    {
        Routes = new List<Route>
        {
            new Route
            {
                Page = "/banking",
                Title = "Future BDET Bank",
                Icon = "fas fa-university",
                AuthenticationRequired = false,
                RequiredRoles = new List<string>(),
                ChildRoutes = new List<Route>
                {
                    // Domain A: Identity, Access & Trust
                    new Route { Page = "/banking/identity", Title = "Identity & Access", Icon = "fas fa-id-card" },
                    new Route { Page = "/banking/biometric", Title = "Biometric Auth", Icon = "fas fa-fingerprint" },
                    new Route { Page = "/banking/kyc-aml", Title = "KYC/AML", Icon = "fas fa-user-check" },
                    // Domain B: Account & Wallet Management
                    new Route { Page = "/banking/accounts", Title = "Accounts", Icon = "fas fa-wallet" },
                    new Route { Page = "/banking/treasury-accounts", Title = "Treasury Accounts", Icon = "fas fa-vault" },
                    new Route { Page = "/banking/savings", Title = "Savings & Deposits", Icon = "fas fa-piggy-bank" },
                    new Route { Page = "/banking/trust-accounts", Title = "Trust Accounts", Icon = "fas fa-handshake" },
                    new Route { Page = "/banking/clan-pools", Title = "Clan Pool Accounts", Icon = "fas fa-users" },
                    // Domain C: Card Services
                    new Route { Page = "/banking/cards", Title = "Card Services", Icon = "fas fa-credit-card" },
                    new Route { Page = "/banking/terminals", Title = "Terminal Access", Icon = "fas fa-mobile-alt" },
                    // Domain D: Payments & Settlements
                    new Route { Page = "/banking/p2p", Title = "P2P Payments", Icon = "fas fa-user-friends" },
                    new Route { Page = "/banking/merchant-payments", Title = "Merchant Payments", Icon = "fas fa-store" },
                    new Route { Page = "/banking/disbursements", Title = "Disbursements", Icon = "fas fa-hand-holding-usd" },
                    new Route { Page = "/banking/recurring", Title = "Recurring Payments", Icon = "fas fa-redo" },
                    new Route { Page = "/banking/pos-settlement", Title = "POS Settlement", Icon = "fas fa-cash-register" },
                    new Route { Page = "/banking/interbank", Title = "Interbank Transfers", Icon = "fas fa-exchange-alt" },
                    // Domain E: Lending & Credit
                    new Route { Page = "/banking/loans", Title = "Loans", Icon = "fas fa-file-invoice-dollar" },
                    new Route { Page = "/banking/microloans", Title = "Microloans", Icon = "fas fa-coins" },
                    new Route { Page = "/banking/credit-risk", Title = "Credit Risk", Icon = "fas fa-exclamation-triangle" },
                    new Route { Page = "/banking/collateral", Title = "Collateral", Icon = "fas fa-shield-alt" },
                    // Domain F: Exchange & Treasury
                    new Route { Page = "/banking/exchange", Title = "Exchange", Icon = "fas fa-chart-line" },
                    new Route { Page = "/banking/treasury", Title = "Treasury", Icon = "fas fa-coins" },
                    // Domain G: Compliance & Security
                    new Route { Page = "/banking/compliance", Title = "Compliance", Icon = "fas fa-gavel" },
                    new Route { Page = "/banking/security", Title = "Security", Icon = "fas fa-shield-alt" },
                    // Advanced - Insurance for FBDETB
                    new Route { Page = "/advanced/insurance", Title = "Insurance", Icon = "fas fa-shield-alt" }
                }
            }
        };

        RoutesChanged?.Invoke(Routes);
        return Task.CompletedTask;
    }
}

