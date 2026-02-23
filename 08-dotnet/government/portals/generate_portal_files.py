#!/usr/bin/env python3
"""Script to generate essential portal files for all MameyNode portals."""

import os
import re

PORTALS = {
    "Lending": {
        "route": "/lending",
        "title": "Lending",
        "icon": "fas fa-hand-holding-usd",
        "children": [
            ("/lending/loans", "Loans", "fas fa-file-invoice-dollar"),
            ("/lending/collateral", "Collateral", "fas fa-shield-alt"),
            ("/lending/interest-rates", "Interest Rates", "fas fa-percentage"),
            ("/lending/pools", "Lending Pools", "fas fa-swimming-pool"),
            ("/lending/p2p", "P2P Lending", "fas fa-users"),
            ("/lending/asset-based", "Asset-Based Lending", "fas fa-building"),
            ("/lending/mortgages", "Mortgages", "fas fa-home"),
            ("/lending/student-loans", "Student Loans", "fas fa-graduation-cap"),
            ("/lending/microloans", "Microloans", "fas fa-coins"),
            ("/lending/credit-cards", "Credit Cards", "fas fa-credit-card"),
            ("/lending/money-market", "Money Market", "fas fa-chart-line"),
            ("/lending/credit-risk", "Credit Risk", "fas fa-exclamation-triangle"),
            ("/lending/repayment", "Repayment", "fas fa-calendar-check"),
            ("/lending/forgiveness", "Forgiveness", "fas fa-heart"),
        ]
    },
    "Dex": {
        "route": "/dex",
        "title": "DEX",
        "icon": "fas fa-exchange-alt",
        "children": [
            ("/dex/swaps", "Swaps", "fas fa-sync-alt"),
            ("/dex/amm", "AMM", "fas fa-chart-pie"),
            ("/dex/liquidity-pools", "Liquidity Pools", "fas fa-swimming-pool"),
            ("/dex/order-book", "Order Book", "fas fa-book"),
            ("/dex/matching-engine", "Matching Engine", "fas fa-cogs"),
            ("/dex/advanced-orders", "Advanced Orders", "fas fa-sliders-h"),
            ("/dex/routing", "Routing", "fas fa-route"),
            ("/dex/oracle", "Oracle", "fas fa-database"),
        ]
    },
    "CryptoExchange": {
        "route": "/crypto-exchange",
        "title": "Crypto Exchange",
        "icon": "fas fa-coins",
        "children": [
            ("/crypto-exchange/orders", "Order Management", "fas fa-list"),
            ("/crypto-exchange/trading-pairs", "Trading Pairs", "fas fa-link"),
            ("/crypto-exchange/wallets", "Wallet Management", "fas fa-wallet"),
            ("/crypto-exchange/custody", "Custody", "fas fa-vault"),
            ("/crypto-exchange/staking", "Staking", "fas fa-gem"),
            ("/crypto-exchange/stablecoin-routing", "Stablecoin Routing", "fas fa-route"),
            ("/crypto-exchange/multi-currency", "Multi-Currency", "fas fa-globe"),
            ("/crypto-exchange/banking-integration", "Banking Integration", "fas fa-university"),
            ("/crypto-exchange/crypto-lending", "Crypto Lending", "fas fa-hand-holding-usd"),
            ("/crypto-exchange/derivatives", "Derivatives", "fas fa-chart-bar"),
        ]
    },
}

def generate_route_service(portal_name, config):
    """Generate RouteService.cs file."""
    portal_class = portal_name.replace(" ", "")
    route_var = config["route"].replace("/", "").replace("-", "")
    
    children_code = "\n".join([
        f'                    new Route {{ Page = "{child[0]}", Title = "{child[1]}", Icon = "{child[2]}" }},'
        for child in config["children"]
    ])
    
    return f"""using Mamey.BlazorWasm.Routing;

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

def generate_home_page(portal_name, config):
    """Generate Home.razor page."""
    portal_class = portal_name.replace(" ", "")
    route = config["route"]
    
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
    
    return f"""@page "{route}"

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

def generate_imports():
    """Generate _Imports.razor content."""
    return """@using Microsoft.AspNetCore.Components.Web
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

if __name__ == "__main__":
    base_path = "/Volumes/Barracuda/mamey-io/code-final/MameyNode.Portals/src"
    
    for portal_name, config in PORTALS.items():
        portal_dir = f"{base_path}/MameyNode.Portals.{portal_name}"
        portal_class = portal_name.replace(" ", "")
        
        # Create Pages directory
        pages_dir = f"{portal_dir}/Pages"
        os.makedirs(pages_dir, exist_ok=True)
        
        # Generate RouteService
        route_service = generate_route_service(portal_name, config)
        with open(f"{portal_dir}/{portal_class}RouteService.cs", "w") as f:
            f.write(route_service)
        
        # Generate Home page
        home_page = generate_home_page(portal_name, config)
        with open(f"{pages_dir}/{portal_class}Home.razor", "w") as f:
            f.write(home_page)
        
        # Generate _Imports.razor
        imports = generate_imports()
        with open(f"{portal_dir}/_Imports.razor", "w") as f:
            f.write(imports)
        
        print(f"Generated files for {portal_name}")

