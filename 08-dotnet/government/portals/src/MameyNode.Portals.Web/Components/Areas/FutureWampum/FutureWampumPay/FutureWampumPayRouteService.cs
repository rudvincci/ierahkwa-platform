using Mamey.BlazorWasm.Routing;
using Route = Mamey.BlazorWasm.Routing.Route;

namespace MameyNode.Portals.Web.Components.Areas.FutureWampum.FutureWampumPay;

public class FutureWampumPayRouteService : IRouteService
{
    public List<Route> Routes { get; private set; } = new();
    public event Action<List<Route>>? RoutesChanged;

    public Task InitializeAsync(bool menu = false)
    {
        Routes = new List<Route>
        {
            new Route
            {
                Page = "/payments",
                Title = "FutureWampumPay",
                Icon = "fas fa-credit-card",
                AuthenticationRequired = false,
                RequiredRoles = new List<string>(),
                ChildRoutes = new List<Route>
                {
                    new Route { Page = "/payments/wallet", Title = "Wallet", Icon = "fas fa-wallet" },
                    new Route { Page = "/payments/p2p", Title = "P2P Payments", Icon = "fas fa-user-friends" },
                    new Route { Page = "/payments/merchant", Title = "Merchant Payments", Icon = "fas fa-store" },
                    new Route { Page = "/payments/disbursements", Title = "Disbursements", Icon = "fas fa-hand-holding-usd" },
                    new Route { Page = "/payments/invoicing", Title = "Invoicing", Icon = "fas fa-file-invoice" },
                    new Route { Page = "/payments/qr-nfc", Title = "QR/NFC Payments", Icon = "fas fa-qrcode" },
                    new Route { Page = "/payments/offline", Title = "Offline Payments", Icon = "fas fa-wifi-slash" },
                    new Route { Page = "/payments/currency-conversion", Title = "Currency Conversion", Icon = "fas fa-exchange-alt" },
                    new Route { Page = "/payments/ledger-sync", Title = "Ledger Sync", Icon = "fas fa-sync" },
                    new Route { Page = "/payments/compliance", Title = "Compliance Audit", Icon = "fas fa-gavel" },
                    new Route { Page = "/payments/zkp", Title = "ZKP Payments", Icon = "fas fa-lock" },
                    new Route { Page = "/payments/multisig", Title = "Multisig", Icon = "fas fa-key" },
                    new Route { Page = "/payments/delegation", Title = "Delegated Access", Icon = "fas fa-user-shield" },
                    new Route { Page = "/payments/geo-policy", Title = "Geo-Fenced Spending", Icon = "fas fa-map-marker-alt" },
                    new Route { Page = "/payments/consent", Title = "Consent Policies", Icon = "fas fa-check-circle" },
                    new Route { Page = "/payments/credentials", Title = "Credential Access", Icon = "fas fa-id-card" },
                    new Route { Page = "/payments/escrow", Title = "Smart Contracts & Escrow", Icon = "fas fa-handshake" },
                    new Route { Page = "/payments/group-wallets", Title = "Group Wallets", Icon = "fas fa-users" },
                    new Route { Page = "/payments/automation", Title = "Automated Deductions", Icon = "fas fa-cog" },
                    new Route { Page = "/payments/disputes", Title = "Dispute Resolution", Icon = "fas fa-balance-scale" },
                    new Route { Page = "/payments/vault", Title = "Vault Integration", Icon = "fas fa-vault" },
                    // Smart Contracts & Escrow - Part of FutureWampumPay
                    new Route { Page = "/smart-contracts", Title = "Smart Contracts", Icon = "fas fa-file-contract" },
                    new Route { Page = "/smart-contracts/deployment", Title = "Contract Deployment", Icon = "fas fa-upload" },
                    new Route { Page = "/smart-contracts/execution", Title = "Contract Execution", Icon = "fas fa-play" },
                    new Route { Page = "/smart-contracts/escrow", Title = "Escrow", Icon = "fas fa-lock" },
                    new Route { Page = "/smart-contracts/token-standards", Title = "Token Standards", Icon = "fas fa-coins" },
                    // Account Abstraction - Part of FutureWampumPay
                    new Route { Page = "/account-abstraction", Title = "Account Abstraction", Icon = "fas fa-user-shield" },
                    new Route { Page = "/account-abstraction/smart-wallets", Title = "Smart Wallets", Icon = "fas fa-wallet" },
                    new Route { Page = "/account-abstraction/factory", Title = "Factory", Icon = "fas fa-industry" },
                    new Route { Page = "/account-abstraction/multisig", Title = "Multi-Sig", Icon = "fas fa-key" },
                    new Route { Page = "/account-abstraction/social-recovery", Title = "Social Recovery", Icon = "fas fa-users" },
                    new Route { Page = "/account-abstraction/session-keys", Title = "Session Keys", Icon = "fas fa-key" },
                    new Route { Page = "/account-abstraction/permissions", Title = "Permissions", Icon = "fas fa-user-lock" },
                    new Route { Page = "/account-abstraction/paymaster", Title = "Paymaster", Icon = "fas fa-money-check" },
                    new Route { Page = "/account-abstraction/account-recovery", Title = "Account Recovery", Icon = "fas fa-redo" },
                    // Programmable Payments - Part of FutureWampumPay
                    new Route { Page = "/programmable", Title = "Programmable Payments", Icon = "fas fa-code" },
                    new Route { Page = "/programmable/conditions", Title = "Conditions", Icon = "fas fa-list-check" },
                    new Route { Page = "/programmable/evaluator", Title = "Evaluator", Icon = "fas fa-calculator" },
                    new Route { Page = "/programmable/wallet", Title = "Programmable Wallet", Icon = "fas fa-wallet" },
                    new Route { Page = "/programmable/enforcement", Title = "Enforcement", Icon = "fas fa-gavel" },
                    new Route { Page = "/programmable/expiring-balances", Title = "Expiring Balances", Icon = "fas fa-clock" },
                    // Payment Channels - Part of FutureWampumPay
                    new Route { Page = "/channels", Title = "Payment Channels", Icon = "fas fa-stream" },
                    new Route { Page = "/channels/channel-management", Title = "Channel Management", Icon = "fas fa-cogs" },
                    new Route { Page = "/channels/protocol", Title = "Channel Protocol", Icon = "fas fa-network-wired" },
                    new Route { Page = "/channels/routing", Title = "Channel Routing", Icon = "fas fa-route" },
                    new Route { Page = "/channels/funding", Title = "Channel Funding", Icon = "fas fa-dollar-sign" },
                    new Route { Page = "/channels/off-chain-updates", Title = "Off-Chain Updates", Icon = "fas fa-sync" },
                    new Route { Page = "/channels/closing", Title = "Channel Closing", Icon = "fas fa-times-circle" },
                    // Pathfinding - Part of FutureWampumPay payment routing
                    new Route { Page = "/pathfinding", Title = "Pathfinding", Icon = "fas fa-route" },
                    new Route { Page = "/pathfinding/pathfinder", Title = "Pathfinder", Icon = "fas fa-search" },
                    new Route { Page = "/pathfinding/currency-graph", Title = "Currency Graph", Icon = "fas fa-project-diagram" },
                    new Route { Page = "/pathfinding/path-execution", Title = "Path Execution", Icon = "fas fa-play" },
                    new Route { Page = "/pathfinding/dex-integration", Title = "DEX Integration", Icon = "fas fa-exchange-alt" },
                    new Route { Page = "/pathfinding/exchange-rate", Title = "Exchange Rate Service", Icon = "fas fa-chart-line" },
                    new Route { Page = "/pathfinding/liquidity-pool", Title = "Liquidity Pool Integration", Icon = "fas fa-swimming-pool" }
                }
            }
        };

        RoutesChanged?.Invoke(Routes);
        return Task.CompletedTask;
    }
}

