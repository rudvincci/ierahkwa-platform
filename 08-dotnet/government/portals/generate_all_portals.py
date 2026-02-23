#!/usr/bin/env python3
"""Generate all remaining portal RouteServices and Home pages."""

import os

# All remaining portal configurations
ALL_PORTALS = {
    "Bridge": {
        "route": "/bridge",
        "title": "Bridge",
        "icon": "fas fa-bridge",
        "children": [
            ("/bridge/cross-chain", "Cross-Chain Bridge", "fas fa-exchange-alt"),
            ("/bridge/ethereum", "Ethereum Bridge", "fas fa-ethereum"),
            ("/bridge/bitcoin", "Bitcoin Bridge", "fas fa-btc"),
            ("/bridge/account-mapping", "Account Mapping", "fas fa-map"),
            ("/bridge/transaction", "Transaction Bridge", "fas fa-exchange-alt"),
            ("/bridge/identity", "Identity Bridge", "fas fa-id-card"),
            ("/bridge/security", "Bridge Security", "fas fa-shield-alt"),
        ]
    },
    "ILP": {
        "route": "/ilp",
        "title": "ILP",
        "icon": "fas fa-network-wired",
        "children": [
            ("/ilp/packets", "Packets", "fas fa-box"),
            ("/ilp/connector", "Connector", "fas fa-plug"),
            ("/ilp/service", "ILP Service", "fas fa-server"),
            ("/ilp/routing", "ILP Routing", "fas fa-route"),
            ("/ilp/ledger-integration", "Ledger Integration", "fas fa-book"),
            ("/ilp/handler", "Packet Handler", "fas fa-cogs"),
            ("/ilp/settlement", "Settlement", "fas fa-handshake"),
        ]
    },
    "ODL": {
        "route": "/odl",
        "title": "ODL",
        "icon": "fas fa-exchange-alt",
        "children": [
            ("/odl/liquidity", "Liquidity Management", "fas fa-swimming-pool"),
            ("/odl/exchange-rate", "Exchange Rate Oracle", "fas fa-chart-line"),
            ("/odl/payment-execution", "Payment Execution", "fas fa-play"),
            ("/odl/provider-management", "Provider Management", "fas fa-users-cog"),
            ("/odl/validation", "Validation", "fas fa-check-circle"),
            ("/odl/bridge-currency", "Bridge Currency", "fas fa-coins"),
        ]
    },
    "Pathfinding": {
        "route": "/pathfinding",
        "title": "Pathfinding",
        "icon": "fas fa-route",
        "children": [
            ("/pathfinding/pathfinder", "Pathfinder", "fas fa-search"),
            ("/pathfinding/currency-graph", "Currency Graph", "fas fa-project-diagram"),
            ("/pathfinding/path-execution", "Path Execution", "fas fa-play"),
            ("/pathfinding/dex-integration", "DEX Integration", "fas fa-exchange-alt"),
            ("/pathfinding/exchange-rate", "Exchange Rate Service", "fas fa-chart-line"),
            ("/pathfinding/liquidity-pool", "Liquidity Pool Integration", "fas fa-swimming-pool"),
        ]
    },
    "TravelRule": {
        "route": "/travel-rule",
        "title": "Travel Rule",
        "icon": "fas fa-passport",
        "children": [
            ("/travel-rule/ivms101", "IVMS-101", "fas fa-file-alt"),
            ("/travel-rule/vasp-directory", "VASP Directory", "fas fa-address-book"),
            ("/travel-rule/message-routing", "Message Routing", "fas fa-route"),
            ("/travel-rule/encryption", "Encryption", "fas fa-lock"),
            ("/travel-rule/trp", "Travel Rule Protocol", "fas fa-network-wired"),
            ("/travel-rule/compliance", "Compliance Integration", "fas fa-shield-alt"),
        ]
    },
    "UPG": {
        "route": "/upg",
        "title": "UPG",
        "icon": "fas fa-globe",
        "children": [
            ("/upg/protocol-support", "Protocol Support", "fas fa-network-wired"),
            ("/upg/adapters", "Adapters", "fas fa-plug"),
            ("/upg/normalization", "Normalization", "fas fa-align-center"),
            ("/upg/multi-rail", "Multi-Rail Routing", "fas fa-route"),
            ("/upg/hsm", "HSM", "fas fa-key"),
            ("/upg/fx", "Foreign Exchange", "fas fa-exchange-alt"),
            ("/upg/pos", "Point of Sale", "fas fa-cash-register"),
            ("/upg/offline", "Offline Payments", "fas fa-wifi-slash"),
            ("/upg/merchant", "Merchant Services", "fas fa-store"),
            ("/upg/real-time", "Real-Time Payments", "fas fa-bolt"),
        ]
    },
    "Channels": {
        "route": "/channels",
        "title": "Channels",
        "icon": "fas fa-stream",
        "children": [
            ("/channels/management", "Channel Management", "fas fa-cogs"),
            ("/channels/protocol", "Channel Protocol", "fas fa-network-wired"),
            ("/channels/routing", "Channel Routing", "fas fa-route"),
            ("/channels/funding", "Channel Funding", "fas fa-dollar-sign"),
            ("/channels/off-chain", "Off-Chain Updates", "fas fa-sync"),
            ("/channels/closing", "Channel Closing", "fas fa-times-circle"),
        ]
    },
    "Programmable": {
        "route": "/programmable",
        "title": "Programmable",
        "icon": "fas fa-code",
        "children": [
            ("/programmable/conditions", "Conditions", "fas fa-list-check"),
            ("/programmable/evaluator", "Evaluator", "fas fa-calculator"),
            ("/programmable/wallet", "Programmable Wallet", "fas fa-wallet"),
            ("/programmable/enforcement", "Enforcement", "fas fa-gavel"),
            ("/programmable/expiring-balances", "Expiring Balances", "fas fa-clock"),
        ]
    },
    "Sharding": {
        "route": "/sharding",
        "title": "Sharding",
        "icon": "fas fa-layer-group",
        "children": [
            ("/sharding/management", "Shard Management", "fas fa-cogs"),
            ("/sharding/assignment", "Shard Assignment", "fas fa-tasks"),
            ("/sharding/routing", "Cross-Shard Routing", "fas fa-route"),
            ("/sharding/cross-shard", "Cross-Shard Communication", "fas fa-comments"),
            ("/sharding/beacon-chain", "Beacon Chain", "fas fa-link"),
            ("/sharding/state", "State Management", "fas fa-database"),
            ("/sharding/hashing", "Consistent Hashing", "fas fa-hashtag"),
            ("/sharding/coordination", "Transaction Coordination", "fas fa-sync"),
            ("/sharding/validation", "Shard Validation", "fas fa-check-circle"),
            ("/sharding/consensus", "Shard Consensus", "fas fa-handshake"),
        ]
    },
    "Advanced": {
        "route": "/advanced",
        "title": "Advanced",
        "icon": "fas fa-rocket",
        "children": [
            ("/advanced/escrow", "Escrow", "fas fa-lock"),
            ("/advanced/tokenization", "Tokenization", "fas fa-coins"),
            ("/advanced/insurance", "Insurance", "fas fa-shield-alt"),
            ("/advanced/offline", "Offline Services", "fas fa-wifi-slash"),
            ("/advanced/satellite", "Satellite Node", "fas fa-satellite"),
        ]
    },
    "Compliance": {
        "route": "/compliance",
        "title": "Compliance",
        "icon": "fas fa-shield-alt",
        "children": [
            ("/compliance/kyc", "KYC", "fas fa-user-check"),
            ("/compliance/aml", "AML/CFT", "fas fa-exclamation-triangle"),
            ("/compliance/fraud", "Fraud Detection", "fas fa-user-secret"),
            ("/compliance/sanctions", "Sanctions Screening", "fas fa-ban"),
            ("/compliance/monitoring", "Transaction Monitoring", "fas fa-eye"),
            ("/compliance/reporting", "Regulatory Reporting", "fas fa-file-alt"),
            ("/compliance/privacy", "Data Privacy", "fas fa-lock"),
            ("/compliance/surveillance", "Market Surveillance", "fas fa-binoculars"),
            ("/compliance/whitelist", "Whitelist/Blacklist", "fas fa-list"),
            ("/compliance/enforcement", "Enforcement", "fas fa-gavel"),
            ("/compliance/limits", "Limits", "fas fa-sliders-h"),
            ("/compliance/audit", "Enhanced Audit", "fas fa-clipboard-check"),
            ("/compliance/zkp", "ZKP Compliance", "fas fa-key"),
            ("/compliance/cdd", "Customer Due Diligence", "fas fa-user-shield"),
        ]
    },
    "Metrics": {
        "route": "/metrics",
        "title": "Metrics",
        "icon": "fas fa-chart-bar",
        "children": [
            ("/metrics/collector", "Metrics Collector", "fas fa-database"),
            ("/metrics/registry", "Metrics Registry", "fas fa-book"),
            ("/metrics/http-endpoint", "HTTP Endpoint", "fas fa-server"),
            ("/metrics/observability", "Enhanced Observability", "fas fa-eye"),
            ("/metrics/health-checks", "Health Checks", "fas fa-heartbeat"),
            ("/metrics/monitoring", "Enhanced Monitoring", "fas fa-chart-line"),
        ]
    },
    "Webhooks": {
        "route": "/webhooks",
        "title": "Webhooks",
        "icon": "fas fa-webhook",
        "children": [
            ("/webhooks/management", "Webhook Management", "fas fa-cogs"),
            ("/webhooks/http-client", "HTTP Client", "fas fa-server"),
            ("/webhooks/queue", "Webhook Queue", "fas fa-list"),
            ("/webhooks/signatures", "Signatures", "fas fa-signature"),
            ("/webhooks/persistence", "Persistence", "fas fa-database"),
            ("/webhooks/health", "Webhook Health", "fas fa-heartbeat"),
            ("/webhooks/rate-limiting", "Rate Limiting", "fas fa-tachometer-alt"),
            ("/webhooks/api", "Webhook API", "fas fa-code"),
            ("/webhooks/validation", "Validation", "fas fa-check-circle"),
        ]
    },
    "Callbacks": {
        "route": "/callbacks",
        "title": "Callbacks",
        "icon": "fas fa-phone",
        "children": [
            ("/callbacks/transaction", "Transaction Callbacks", "fas fa-exchange-alt"),
            ("/callbacks/settlement", "Settlement Callbacks", "fas fa-handshake"),
            ("/callbacks/account", "Account Callbacks", "fas fa-user-circle"),
            ("/callbacks/manager", "Callback Manager", "fas fa-cogs"),
        ]
    },
    "RBAC": {
        "route": "/rbac",
        "title": "RBAC",
        "icon": "fas fa-user-lock",
        "children": [
            ("/rbac/roles", "Role Management", "fas fa-users-cog"),
            ("/rbac/permissions", "Permission Management", "fas fa-key"),
            ("/rbac/hierarchy", "Role Hierarchy", "fas fa-sitemap"),
            ("/rbac/guard", "Access Guard", "fas fa-shield-alt"),
        ]
    },
    "TrustLines": {
        "route": "/trust-lines",
        "title": "Trust Lines",
        "icon": "fas fa-link",
        "children": [
            ("/trust-lines/management", "Trust Line Management", "fas fa-cogs"),
            ("/trust-lines/storage", "Trust Line Storage", "fas fa-database"),
            ("/trust-lines/validation", "Validation", "fas fa-check-circle"),
            ("/trust-lines/indexing", "Indexing", "fas fa-search"),
            ("/trust-lines/persistence", "Persistence", "fas fa-save"),
        ]
    },
    "LedgerIntegration": {
        "route": "/ledger-integration",
        "title": "Ledger Integration",
        "icon": "fas fa-book",
        "children": [
            ("/ledger-integration/transaction-logging", "Transaction Logging", "fas fa-file-alt"),
            ("/ledger-integration/compliance", "Compliance", "fas fa-shield-alt"),
            ("/ledger-integration/currency-registry", "Currency Registry", "fas fa-coins"),
            ("/ledger-integration/credit-tracking", "Credit Tracking", "fas fa-chart-line"),
            ("/ledger-integration/transparency", "Transparency", "fas fa-eye"),
        ]
    },
    "Node": {
        "route": "/node",
        "title": "Node",
        "icon": "fas fa-server",
        "children": [
            ("/node/deployment", "Deployment", "fas fa-rocket"),
            ("/node/orchestration", "Container Orchestration", "fas fa-docker"),
            ("/node/disaster-recovery", "Disaster Recovery", "fas fa-redo"),
            ("/node/security", "Enhanced Security", "fas fa-shield-alt"),
            ("/node/multi-region", "Multi-Region", "fas fa-globe"),
            ("/node/performance", "Performance Validation", "fas fa-tachometer-alt"),
            ("/node/audit", "Security Audit", "fas fa-clipboard-check"),
        ]
    },
}

def generate_files():
    base_path = "/Volumes/Barracuda/mamey-io/code-final/MameyNode.Portals/src"
    imports_template = """@using Microsoft.AspNetCore.Components.Web
@using Microsoft.AspNetCore.Components.Routing
@using Microsoft.AspNetCore.Components.Forms
@using Microsoft.AspNetCore.Components.Authorization
@using MudBlazor
@using Mamey.BlazorWasm.Components
@using Mamey.BlazorWasm.Components.MameyPro
@using Mamey.BlazorWasm.Routing
@using MameyNode.Portals.Shared.Components.Layout
@using MameyNode.Portals.Shared.Components.DataDisplay
@using MameyNode.Portals.Shared.Components.Forms
@using MameyNode.Portals.Shared.Components.Feedback
@using MameyNode.Portals.Shared.Components.Navigation
@using MameyNode.Portals.Shared.Components.Specialized
"""
    
    for portal_name, config in ALL_PORTALS.items():
        portal_dir = f"{base_path}/MameyNode.Portals.{portal_name}"
        portal_class = portal_name.replace(" ", "")
        
        if not os.path.exists(portal_dir):
            print(f"Skipping {portal_name} - directory not found")
            continue
        
        pages_dir = f"{portal_dir}/Pages"
        os.makedirs(pages_dir, exist_ok=True)
        
        # Generate RouteService
        children_code = "\n".join([
            f'                    new Route {{ Page = "{child[0]}", Title = "{child[1]}", Icon = "{child[2]}" }},'
            for child in config["children"]
        ])
        
        route_service = f"""using Mamey.BlazorWasm.Routing;

namespace MameyNode.Portals.{portal_class};

public class {portal_class}RouteService : IRouteService
{{
    public List<Route> Routes {{ get; private set; }} = new();

    public event Action<List<Route>>? RoutesChanged;

    public Task InitializeAsync(bool menu = false)
    {{
        Routes = new List<Route>
        {{
            new Route
            {{
                Page = "{config["route"]}",
                Title = "{config["title"]}",
                Icon = "{config["icon"]}",
                AuthenticationRequired = true,
                RequiredRoles = new List<string> {{ "User", "{portal_class}" }},
                ChildRoutes = new List<Route>
                {{
{children_code}
                }}
            }}
        }};

        RoutesChanged?.Invoke(Routes);
        return Task.CompletedTask;
    }}
}}
"""
        
        route_service_path = f"{portal_dir}/{portal_class}RouteService.cs"
        if not os.path.exists(route_service_path):
            with open(route_service_path, "w") as f:
                f.write(route_service)
        
        # Generate _Imports.razor
        imports_path = f"{portal_dir}/_Imports.razor"
        if not os.path.exists(imports_path):
            with open(imports_path, "w") as f:
                f.write(imports_template)
        
        # Generate Home page
        stat_cards = "\n".join([
            f"""        <MudItem xs="12" sm="6" md="4">
            <StatCard 
                Title="{child[1]}"
                Value="0"
                Trend="+0%"
                Icon="{child[2]}" />
        </MudItem>"""
            for child in config["children"][:3]
        ])
        
        nav_cards = "\n".join([
            f"""        <MudItem xs="12" sm="6" md="4">
            <CardContainer Title="{child[1]}" Elevation="2">
                <MudText Typo="Typo.body2">{child[1]} management and operations</MudText>
                <MudButton Href="{child[0]}" Variant="Variant.Filled" Color="Color.Primary" Class="mt-2">
                    Go to {child[1]}
                </MudButton>
            </CardContainer>
        </MudItem>"""
            for child in config["children"]
        ])
        
        home_page = f"""@page "{config["route"]}"

<PageContainer Title="{config["title"]} Portal">
    <SectionHeader Title="{config["title"]} Dashboard" Icon="{config["icon"]}" />
    
    <MudGrid Spacing="3" Class="mt-4">
{stat_cards}
    </MudGrid>

    <MudGrid Spacing="3" Class="mt-4">
{nav_cards}
    </MudGrid>
</PageContainer>
"""
        
        home_page_path = f"{pages_dir}/{portal_class}Home.razor"
        if not os.path.exists(home_page_path):
            with open(home_page_path, "w") as f:
                f.write(home_page)
        
        print(f"Generated files for {portal_name}")

if __name__ == "__main__":
    generate_files()

