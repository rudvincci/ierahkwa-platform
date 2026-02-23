using Mamey.BlazorWasm.Routing;
using Route = Mamey.BlazorWasm.Routing.Route;

namespace MameyNode.Portals.Web.Components.Areas.FutureWampum.FutureWampumX;

public class FutureWampumXRouteService : IRouteService
{
    public List<Route> Routes { get; private set; } = new();
    public event Action<List<Route>>? RoutesChanged;

    public Task InitializeAsync(bool menu = false)
    {
        Routes = new List<Route>
        {
            new Route
            {
                Page = "/dex",
                Title = "FutureWampumX",
                Icon = "fas fa-exchange-alt",
                AuthenticationRequired = false,
                RequiredRoles = new List<string>(),
                ChildRoutes = new List<Route>
                {
                    new Route { Page = "/dex/swaps", Title = "Currency Swaps", Icon = "fas fa-sync-alt" },
                    new Route { Page = "/dex/wallets", Title = "Multi-Currency Wallets", Icon = "fas fa-wallet" },
                    new Route { Page = "/dex/compliance", Title = "Compliance & KYC/AML", Icon = "fas fa-user-check" },
                    new Route { Page = "/dex/treaty-zones", Title = "Treaty-Zone Authorization", Icon = "fas fa-globe" },
                    new Route { Page = "/dex/interop", Title = "Stablecoin & Fiat Interop", Icon = "fas fa-link" },
                    new Route { Page = "/dex/identity", Title = "Identity & Risk Tiering", Icon = "fas fa-id-card" },
                    new Route { Page = "/dex/liquidity", Title = "Liquidity Routing", Icon = "fas fa-route" },
                    new Route { Page = "/dex/settlement", Title = "Cross-Border Settlement", Icon = "fas fa-plane" },
                    new Route { Page = "/dex/oracle", Title = "Exchange Rate Oracle", Icon = "fas fa-database" },
                    new Route { Page = "/dex/orders", Title = "Order Lifecycle", Icon = "fas fa-list" },
                    new Route { Page = "/dex/audit", Title = "Audit Trail", Icon = "fas fa-book" },
                    new Route { Page = "/dex/zone-access", Title = "Zone Access Control", Icon = "fas fa-map-marker-alt" },
                    new Route { Page = "/dex/admin", Title = "Admin Dashboard", Icon = "fas fa-tachometer-alt" },
                    new Route { Page = "/dex/treasury", Title = "Treasury Balancing", Icon = "fas fa-coins" },
                    new Route { Page = "/dex/analytics", Title = "Analytics & Reports", Icon = "fas fa-chart-line" },
                    new Route { Page = "/dex/fraud", Title = "Fraud Detection", Icon = "fas fa-shield-alt" },
                    // Also include crypto-exchange routes
                    new Route { Page = "/crypto-exchange", Title = "Crypto Exchange", Icon = "fas fa-coins" },
                    new Route { Page = "/crypto-exchange/order-management", Title = "Order Management", Icon = "fas fa-list" },
                    new Route { Page = "/crypto-exchange/trading-pairs", Title = "Trading Pairs", Icon = "fas fa-link" },
                    new Route { Page = "/crypto-exchange/wallet-management", Title = "Wallet Management", Icon = "fas fa-wallet" },
                    new Route { Page = "/crypto-exchange/custody", Title = "Custody", Icon = "fas fa-vault" },
                    new Route { Page = "/crypto-exchange/staking", Title = "Staking", Icon = "fas fa-gem" },
                    new Route { Page = "/crypto-exchange/stablecoin-routing", Title = "Stablecoin Routing", Icon = "fas fa-route" },
                    new Route { Page = "/crypto-exchange/multi-currency", Title = "Multi-Currency", Icon = "fas fa-globe" },
                    new Route { Page = "/crypto-exchange/banking-integration", Title = "Banking Integration", Icon = "fas fa-university" },
                    new Route { Page = "/crypto-exchange/crypto-lending", Title = "Crypto Lending", Icon = "fas fa-hand-holding-usd" },
                    new Route { Page = "/crypto-exchange/derivatives", Title = "Derivatives", Icon = "fas fa-chart-bar" },
                    // Bridge - Cross-chain for FutureWampumX
                    new Route { Page = "/bridge", Title = "Cross-Chain Bridge", Icon = "fas fa-bridge" },
                    new Route { Page = "/bridge/cross-chain", Title = "Cross-Chain Bridge", Icon = "fas fa-exchange-alt" },
                    new Route { Page = "/bridge/ethereum", Title = "Ethereum Bridge", Icon = "fas fa-ethereum" },
                    new Route { Page = "/bridge/bitcoin", Title = "Bitcoin Bridge", Icon = "fas fa-btc" },
                    new Route { Page = "/bridge/account-mapping", Title = "Account Mapping", Icon = "fas fa-map" },
                    new Route { Page = "/bridge/transaction", Title = "Transaction Bridge", Icon = "fas fa-exchange-alt" },
                    new Route { Page = "/bridge/identity", Title = "Identity Bridge", Icon = "fas fa-id-card" },
                    new Route { Page = "/bridge/security", Title = "Bridge Security", Icon = "fas fa-shield-alt" },
                    // Travel Rule - Compliance for FutureWampumX
                    new Route { Page = "/travel-rule", Title = "Travel Rule", Icon = "fas fa-passport" },
                    new Route { Page = "/travel-rule/ivms101", Title = "IVMS-101", Icon = "fas fa-file-alt" },
                    new Route { Page = "/travel-rule/vasp-directory", Title = "VASP Directory", Icon = "fas fa-address-book" },
                    new Route { Page = "/travel-rule/message-routing", Title = "Message Routing", Icon = "fas fa-route" },
                    new Route { Page = "/travel-rule/encryption", Title = "Encryption", Icon = "fas fa-lock" },
                    new Route { Page = "/travel-rule/trp", Title = "Travel Rule Protocol", Icon = "fas fa-network-wired" },
                    new Route { Page = "/travel-rule/compliance", Title = "Compliance Integration", Icon = "fas fa-shield-alt" },
                    // Trust Lines - Part of FutureWampumX
                    new Route { Page = "/trust-lines", Title = "Trust Lines", Icon = "fas fa-link" },
                    new Route { Page = "/trust-lines/management", Title = "Trust Line Management", Icon = "fas fa-cogs" },
                    new Route { Page = "/trust-lines/storage", Title = "Trust Line Storage", Icon = "fas fa-database" },
                    new Route { Page = "/trust-lines/validation", Title = "Validation", Icon = "fas fa-check-circle" },
                    new Route { Page = "/trust-lines/indexing", Title = "Indexing", Icon = "fas fa-search" },
                    new Route { Page = "/trust-lines/persistence", Title = "Persistence", Icon = "fas fa-save" },
                    // Advanced - Tokenization for FutureWampumX
                    new Route { Page = "/advanced/tokenization", Title = "Tokenization", Icon = "fas fa-coins" }
                }
            }
        };

        RoutesChanged?.Invoke(Routes);
        return Task.CompletedTask;
    }
}

