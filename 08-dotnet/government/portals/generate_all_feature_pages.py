#!/usr/bin/env python3
"""
Generate all feature pages for all MameyNode portals based on the plan.
This script creates feature pages for all 24 portals according to the comprehensive plan.
"""

import os

# Portal feature pages configuration based on the plan
PORTAL_PAGES = {
    "Payments": [
        ("P2PPayments", "/payments/p2p", "fas fa-user-friends", "P2P Payments", "Send and receive peer-to-peer payments"),
        ("MerchantPayments", "/payments/merchant", "fas fa-store", "Merchant Payments", "Process merchant transactions"),
        ("PaymentGateway", "/payments/gateway", "fas fa-network-wired", "Payment Gateway", "Manage payment gateway configuration"),
        ("RecurringPayments", "/payments/recurring", "fas fa-redo", "Recurring Payments", "Set up and manage recurring payment schedules"),
        ("Subscriptions", "/payments/subscriptions", "fas fa-calendar-check", "Subscriptions", "Manage subscription plans and billing"),
        ("Invoicing", "/payments/invoicing", "fas fa-file-invoice", "Invoicing", "Create and manage invoices"),
        ("Remittances", "/payments/remittances", "fas fa-money-bill-wave", "Remittances", "Process remittance transactions"),
        ("BillPayments", "/payments/bill-payments", "fas fa-receipt", "Bill Payments", "Manage bill payment services"),
        ("Disbursements", "/payments/disbursements", "fas fa-hand-holding-usd", "Disbursements", "Handle disbursement operations"),
        ("MultisigPayments", "/payments/multisig", "fas fa-key", "Multi-Sig Payments", "Multi-signature payment management"),
        ("Loyalty", "/payments/loyalty", "fas fa-star", "Loyalty Programs", "Manage loyalty and rewards programs"),
    ],
    "Lending": [
        ("Loans", "/lending/loans", "fas fa-hand-holding-usd", "Loans", "Manage loan applications and approvals"),
        ("Collateral", "/lending/collateral", "fas fa-shield-alt", "Collateral", "Manage collateral assets"),
        ("InterestRates", "/lending/interest-rates", "fas fa-percentage", "Interest Rates", "Configure interest rate settings"),
        ("LendingPools", "/lending/pools", "fas fa-swimming-pool", "Lending Pools", "Manage lending pool operations"),
        ("P2PLending", "/lending/p2p", "fas fa-users", "P2P Lending", "Peer-to-peer lending marketplace"),
        ("AssetBasedLending", "/lending/asset-based", "fas fa-building", "Asset-Based Lending", "Asset-backed lending services"),
        ("Mortgages", "/lending/mortgages", "fas fa-home", "Mortgages", "Mortgage loan management"),
        ("StudentLoans", "/lending/student", "fas fa-graduation-cap", "Student Loans", "Student loan services"),
        ("Microloans", "/lending/microloans", "fas fa-seedling", "Microloans", "Microloan management"),
        ("CreditCards", "/lending/credit-cards", "fas fa-credit-card", "Credit Cards", "Credit card services"),
        ("MoneyMarket", "/lending/money-market", "fas fa-chart-line", "Money Market", "Money market operations"),
        ("CreditRisk", "/lending/credit-risk", "fas fa-exclamation-triangle", "Credit Risk", "Credit risk assessment"),
        ("Repayment", "/lending/repayment", "fas fa-calendar-alt", "Repayment", "Repayment schedule management"),
        ("Forgiveness", "/lending/forgiveness", "fas fa-heart", "Loan Forgiveness", "Loan forgiveness programs"),
    ],
    "Dex": [
        ("Swaps", "/dex/swaps", "fas fa-exchange-alt", "Swaps", "Token swap interface"),
        ("AMM", "/dex/amm", "fas fa-chart-area", "AMM", "Automated Market Maker"),
        ("LiquidityPools", "/dex/liquidity-pools", "fas fa-swimming-pool", "Liquidity Pools", "Liquidity pool management"),
        ("OrderBook", "/dex/order-book", "fas fa-book", "Order Book", "Order book visualization"),
        ("MatchingEngine", "/dex/matching-engine", "fas fa-cogs", "Matching Engine", "Matching engine status"),
        ("AdvancedOrders", "/dex/advanced-orders", "fas fa-sliders-h", "Advanced Orders", "Advanced order types"),
        ("Routing", "/dex/routing", "fas fa-route", "Routing", "Route optimization"),
        ("Oracle", "/dex/oracle", "fas fa-eye", "Oracle", "Oracle price feeds"),
    ],
    "CryptoExchange": [
        ("OrderManagement", "/crypto-exchange/order-management", "fas fa-tasks", "Order Management", "Order management interface"),
        ("TradingPairs", "/crypto-exchange/trading-pairs", "fas fa-link", "Trading Pairs", "Trading pair configuration"),
        ("WalletManagement", "/crypto-exchange/wallet-management", "fas fa-wallet", "Wallet Management", "Wallet management"),
        ("Custody", "/crypto-exchange/custody", "fas fa-vault", "Custody", "Custody services"),
        ("Staking", "/crypto-exchange/staking", "fas fa-coins", "Staking", "Staking operations"),
        ("StablecoinRouting", "/crypto-exchange/stablecoin-routing", "fas fa-exchange-alt", "Stablecoin Routing", "Stablecoin routing"),
        ("MultiCurrency", "/crypto-exchange/multi-currency", "fas fa-globe", "Multi-Currency", "Multi-currency support"),
        ("BankingIntegration", "/crypto-exchange/banking-integration", "fas fa-university", "Banking Integration", "Banking integration"),
        ("CryptoLending", "/crypto-exchange/crypto-lending", "fas fa-hand-holding-usd", "Crypto Lending", "Crypto lending services"),
        ("Derivatives", "/crypto-exchange/derivatives", "fas fa-chart-bar", "Derivatives", "Derivatives trading"),
    ],
    "SmartContracts": [
        ("ContractDeployment", "/smart-contracts/deployment", "fas fa-rocket", "Contract Deployment", "Deploy smart contracts"),
        ("ContractExecution", "/smart-contracts/execution", "fas fa-play", "Contract Execution", "Execute contract functions"),
        ("GasMetering", "/smart-contracts/gas-metering", "fas fa-gas-pump", "Gas Metering", "Gas usage monitoring"),
        ("Storage", "/smart-contracts/storage", "fas fa-database", "Storage", "Contract storage management"),
        ("Events", "/smart-contracts/events", "fas fa-bell", "Events", "Event logs and monitoring"),
        ("TokenStandards", "/smart-contracts/token-standards", "fas fa-coins", "Token Standards", "Token standard implementations"),
        ("ERC20", "/smart-contracts/erc20", "fas fa-coins", "ERC-20", "ERC-20 token management"),
        ("ERC721", "/smart-contracts/erc721", "fas fa-image", "ERC-721", "NFT management"),
        ("ERC1155", "/smart-contracts/erc1155", "fas fa-layer-group", "ERC-1155", "Multi-token management"),
        ("Proxy", "/smart-contracts/proxy", "fas fa-code-branch", "Proxy", "Proxy contract management"),
        ("Versioning", "/smart-contracts/versioning", "fas fa-code-branch", "Versioning", "Contract versioning"),
    ],
    "AccountAbstraction": [
        ("SmartWallets", "/account-abstraction/smart-wallets", "fas fa-wallet", "Smart Wallets", "Smart wallet management"),
        ("Factory", "/account-abstraction/factory", "fas fa-industry", "Factory", "Wallet factory"),
        ("Multisig", "/account-abstraction/multisig", "fas fa-key", "Multi-Sig", "Multi-signature wallets"),
        ("SocialRecovery", "/account-abstraction/social-recovery", "fas fa-user-friends", "Social Recovery", "Social recovery setup"),
        ("SessionKeys", "/account-abstraction/session-keys", "fas fa-key", "Session Keys", "Session key management"),
        ("Permissions", "/account-abstraction/permissions", "fas fa-user-shield", "Permissions", "Permission management"),
        ("Paymaster", "/account-abstraction/paymaster", "fas fa-money-check-alt", "Paymaster", "Paymaster services"),
        ("PaymasterPolicy", "/account-abstraction/paymaster-policy", "fas fa-policy", "Paymaster Policy", "Paymaster policy configuration"),
        ("AccountRecovery", "/account-abstraction/account-recovery", "fas fa-undo", "Account Recovery", "Account recovery"),
    ],
    "Bridge": [
        ("CrossChainBridge", "/bridge/cross-chain", "fas fa-exchange-alt", "Cross-Chain Bridge", "Cross-chain bridging"),
        ("EthereumBridge", "/bridge/ethereum", "fab fa-ethereum", "Ethereum Bridge", "Ethereum bridge"),
        ("BitcoinBridge", "/bridge/bitcoin", "fab fa-bitcoin", "Bitcoin Bridge", "Bitcoin bridge"),
        ("AccountMapping", "/bridge/account-mapping", "fas fa-map", "Account Mapping", "Account mapping"),
        ("TransactionBridge", "/bridge/transaction", "fas fa-exchange-alt", "Transaction Bridge", "Transaction bridging"),
        ("IdentityBridge", "/bridge/identity", "fas fa-id-card", "Identity Bridge", "Identity bridging"),
        ("Security", "/bridge/security", "fas fa-shield-alt", "Security", "Bridge security"),
    ],
    "ILP": [
        ("Packets", "/ilp/packets", "fas fa-box", "Packets", "Packet management"),
        ("Connector", "/ilp/connector", "fas fa-plug", "Connector", "Connector configuration"),
        ("Service", "/ilp/service", "fas fa-server", "Service", "ILP service management"),
        ("Routing", "/ilp/routing", "fas fa-route", "Routing", "ILP routing"),
        ("LedgerIntegration", "/ilp/ledger-integration", "fas fa-book", "Ledger Integration", "Ledger integration"),
        ("Handler", "/ilp/handler", "fas fa-cogs", "Handler", "Packet handler"),
        ("Settlement", "/ilp/settlement", "fas fa-handshake", "Settlement", "Settlement management"),
    ],
    "ODL": [
        ("LiquidityManagement", "/odl/liquidity-management", "fas fa-water", "Liquidity Management", "Liquidity management"),
        ("ExchangeRateOracle", "/odl/exchange-rate-oracle", "fas fa-chart-line", "Exchange Rate Oracle", "Exchange rate oracle"),
        ("PaymentExecution", "/odl/payment-execution", "fas fa-play", "Payment Execution", "Payment execution"),
        ("ProviderManagement", "/odl/provider-management", "fas fa-users-cog", "Provider Management", "Provider management"),
        ("Validation", "/odl/validation", "fas fa-check-circle", "Validation", "Validation services"),
        ("BridgeCurrency", "/odl/bridge-currency", "fas fa-coins", "Bridge Currency", "Bridge currency management"),
    ],
    "Pathfinding": [
        ("Pathfinder", "/pathfinding/pathfinder", "fas fa-search", "Pathfinder", "Path finding interface"),
        ("CurrencyGraph", "/pathfinding/currency-graph", "fas fa-project-diagram", "Currency Graph", "Currency graph visualization"),
        ("PathExecution", "/pathfinding/path-execution", "fas fa-play", "Path Execution", "Path execution"),
        ("DEXIntegration", "/pathfinding/dex-integration", "fas fa-link", "DEX Integration", "DEX integration"),
        ("ExchangeRateService", "/pathfinding/exchange-rate-service", "fas fa-chart-line", "Exchange Rate Service", "Exchange rate service"),
        ("LiquidityPoolIntegration", "/pathfinding/liquidity-pool-integration", "fas fa-swimming-pool", "Liquidity Pool Integration", "Liquidity pool integration"),
    ],
    "TravelRule": [
        ("IVMS101", "/travel-rule/ivms101", "fas fa-file-alt", "IVMS-101", "IVMS-101 compliance"),
        ("VASPDirectory", "/travel-rule/vasp-directory", "fas fa-address-book", "VASP Directory", "VASP directory"),
        ("MessageRouting", "/travel-rule/message-routing", "fas fa-route", "Message Routing", "Message routing"),
        ("Encryption", "/travel-rule/encryption", "fas fa-lock", "Encryption", "Encryption management"),
        ("TRP", "/travel-rule/trp", "fas fa-network-wired", "Travel Rule Protocol", "Travel Rule Protocol"),
        ("ComplianceIntegration", "/travel-rule/compliance-integration", "fas fa-shield-alt", "Compliance Integration", "Compliance integration"),
    ],
    "UPG": [
        ("ProtocolSupport", "/upg/protocol-support", "fas fa-network-wired", "Protocol Support", "Protocol support"),
        ("Adapters", "/upg/adapters", "fas fa-plug", "Adapters", "Adapter management"),
        ("Normalization", "/upg/normalization", "fas fa-align-center", "Normalization", "Message normalization"),
        ("MultiRailRouting", "/upg/multi-rail-routing", "fas fa-route", "Multi-Rail Routing", "Multi-rail routing"),
        ("HSM", "/upg/hsm", "fas fa-server", "HSM", "HSM integration"),
        ("FX", "/upg/fx", "fas fa-exchange-alt", "Foreign Exchange", "Foreign exchange"),
        ("POS", "/upg/pos", "fas fa-cash-register", "Point of Sale", "Point of sale"),
        ("Offline", "/upg/offline", "fas fa-wifi-slash", "Offline", "Offline payments"),
        ("Merchant", "/upg/merchant", "fas fa-store", "Merchant", "Merchant services"),
        ("RealTimePayments", "/upg/real-time-payments", "fas fa-bolt", "Real-Time Payments", "Real-time payments (FedNow, RTP, PIX, UPI)"),
    ],
    "Channels": [
        ("ChannelManagement", "/channels/channel-management", "fas fa-stream", "Channel Management", "Channel management"),
        ("Protocol", "/channels/protocol", "fas fa-network-wired", "Protocol", "Channel protocol"),
        ("Routing", "/channels/routing", "fas fa-route", "Routing", "Channel routing"),
        ("Funding", "/channels/funding", "fas fa-dollar-sign", "Funding", "Channel funding"),
        ("OffChainUpdates", "/channels/off-chain-updates", "fas fa-sync", "Off-Chain Updates", "Off-chain updates"),
        ("Closing", "/channels/closing", "fas fa-times-circle", "Closing", "Channel closing"),
    ],
    "Programmable": [
        ("Conditions", "/programmable/conditions", "fas fa-code", "Conditions", "Condition management"),
        ("Evaluator", "/programmable/evaluator", "fas fa-calculator", "Evaluator", "Condition evaluator"),
        ("Wallet", "/programmable/wallet", "fas fa-wallet", "Programmable Wallet", "Programmable wallet"),
        ("Enforcement", "/programmable/enforcement", "fas fa-gavel", "Enforcement", "Enforcement rules"),
        ("ExpiringBalances", "/programmable/expiring-balances", "fas fa-clock", "Expiring Balances", "Expiring balance management"),
    ],
    "Sharding": [
        ("ShardManagement", "/sharding/shard-management", "fas fa-layer-group", "Shard Management", "Shard management"),
        ("Assignment", "/sharding/assignment", "fas fa-tasks", "Assignment", "Shard assignment"),
        ("Routing", "/sharding/routing", "fas fa-route", "Cross-Shard Routing", "Cross-shard routing"),
        ("CrossShardCommunication", "/sharding/cross-shard-communication", "fas fa-comments", "Cross-Shard Communication", "Cross-shard communication"),
        ("BeaconChain", "/sharding/beacon-chain", "fas fa-satellite", "Beacon Chain", "Beacon chain"),
        ("StateManagement", "/sharding/state-management", "fas fa-database", "State Management", "State management"),
        ("ConsistentHashing", "/sharding/consistent-hashing", "fas fa-hashtag", "Consistent Hashing", "Consistent hashing"),
        ("TransactionCoordination", "/sharding/transaction-coordination", "fas fa-sync", "Transaction Coordination", "Transaction coordination"),
        ("Validation", "/sharding/validation", "fas fa-check-circle", "Validation", "Shard validation"),
        ("Consensus", "/sharding/consensus", "fas fa-handshake", "Consensus", "Shard consensus"),
    ],
    "Advanced": [
        ("Escrow", "/advanced/escrow", "fas fa-lock", "Escrow", "Escrow services"),
        ("Tokenization", "/advanced/tokenization", "fas fa-coins", "Tokenization", "Asset tokenization"),
        ("Insurance", "/advanced/insurance", "fas fa-shield-alt", "Insurance", "Insurance services"),
        ("Offline", "/advanced/offline", "fas fa-wifi-slash", "Offline", "Offline services"),
        ("Satellite", "/advanced/satellite", "fas fa-satellite", "Satellite", "Satellite node management"),
    ],
    "Compliance": [
        ("KYC", "/compliance/kyc", "fas fa-user-check", "KYC", "KYC management"),
        ("AML", "/compliance/aml", "fas fa-shield-alt", "AML/CFT", "AML/CFT compliance"),
        ("FraudDetection", "/compliance/fraud-detection", "fas fa-exclamation-triangle", "Fraud Detection", "Fraud detection"),
        ("SanctionsScreening", "/compliance/sanctions-screening", "fas fa-ban", "Sanctions Screening", "Sanctions screening"),
        ("TransactionMonitoring", "/compliance/transaction-monitoring", "fas fa-eye", "Transaction Monitoring", "Transaction monitoring"),
        ("RegulatoryReporting", "/compliance/regulatory-reporting", "fas fa-file-alt", "Regulatory Reporting", "Regulatory reporting"),
        ("DataPrivacy", "/compliance/data-privacy", "fas fa-user-shield", "Data Privacy", "Data privacy"),
        ("MarketSurveillance", "/compliance/market-surveillance", "fas fa-binoculars", "Market Surveillance", "Market surveillance"),
        ("WhitelistBlacklist", "/compliance/whitelist-blacklist", "fas fa-list", "Whitelist/Blacklist", "Whitelist/blacklist management"),
        ("Enforcement", "/compliance/enforcement", "fas fa-gavel", "Enforcement", "Enforcement actions"),
        ("Limits", "/compliance/limits", "fas fa-tachometer-alt", "Limits", "Limit management"),
        ("EnhancedAudit", "/compliance/enhanced-audit", "fas fa-clipboard-check", "Enhanced Audit", "Enhanced audit"),
        ("ZKPCompliance", "/compliance/zkp", "fas fa-key", "ZKP Compliance", "ZKP compliance"),
        ("CDD", "/compliance/cdd", "fas fa-user-tie", "Customer Due Diligence", "Customer due diligence"),
    ],
    "Metrics": [
        ("Collector", "/metrics/collector", "fas fa-database", "Metrics Collector", "Metrics collector"),
        ("Registry", "/metrics/registry", "fas fa-book", "Metrics Registry", "Metrics registry"),
        ("HTTPEndpoint", "/metrics/http-endpoint", "fas fa-server", "HTTP Endpoint", "HTTP endpoint"),
        ("Observability", "/metrics/observability", "fas fa-chart-line", "Enhanced Observability", "Enhanced observability"),
        ("HealthChecks", "/metrics/health-checks", "fas fa-heartbeat", "Health Checks", "Health checks"),
        ("Monitoring", "/metrics/monitoring", "fas fa-eye", "Enhanced Monitoring", "Enhanced monitoring"),
    ],
    "Webhooks": [
        ("WebhookManagement", "/webhooks/management", "fas fa-cogs", "Webhook Management", "Webhook management"),
        ("HTTPClient", "/webhooks/http-client", "fas fa-network-wired", "HTTP Client", "HTTP client configuration"),
        ("Queue", "/webhooks/queue", "fas fa-list", "Queue", "Webhook queue"),
        ("Signatures", "/webhooks/signatures", "fas fa-signature", "Signatures", "Signature management"),
        ("Persistence", "/webhooks/persistence", "fas fa-database", "Persistence", "Persistence configuration"),
        ("Health", "/webhooks/health", "fas fa-heartbeat", "Health", "Webhook health"),
        ("RateLimiting", "/webhooks/rate-limiting", "fas fa-tachometer-alt", "Rate Limiting", "Rate limiting"),
        ("API", "/webhooks/api", "fas fa-code", "Webhook API", "Webhook API"),
        ("Validation", "/webhooks/validation", "fas fa-check-circle", "Validation", "Validation rules"),
    ],
    "Callbacks": [
        ("TransactionCallbacks", "/callbacks/transaction", "fas fa-exchange-alt", "Transaction Callbacks", "Transaction callbacks"),
        ("SettlementCallbacks", "/callbacks/settlement", "fas fa-handshake", "Settlement Callbacks", "Settlement callbacks"),
        ("AccountCallbacks", "/callbacks/account", "fas fa-user-circle", "Account Callbacks", "Account callbacks"),
        ("Manager", "/callbacks/manager", "fas fa-cogs", "Callback Manager", "Callback manager"),
    ],
    "RBAC": [
        ("RoleManagement", "/rbac/role-management", "fas fa-user-tag", "Role Management", "Role management"),
        ("PermissionManagement", "/rbac/permission-management", "fas fa-key", "Permission Management", "Permission management"),
        ("Hierarchy", "/rbac/hierarchy", "fas fa-sitemap", "Role Hierarchy", "Role hierarchy"),
        ("Guard", "/rbac/guard", "fas fa-shield-alt", "Access Guard", "Access guard"),
    ],
    "TrustLines": [
        ("TrustLineManagement", "/trust-lines/management", "fas fa-link", "Trust Line Management", "Trust line management"),
        ("Storage", "/trust-lines/storage", "fas fa-database", "Storage", "Trust line storage"),
        ("Validation", "/trust-lines/validation", "fas fa-check-circle", "Validation", "Validation rules"),
        ("Indexing", "/trust-lines/indexing", "fas fa-search", "Indexing", "Indexing management"),
        ("Persistence", "/trust-lines/persistence", "fas fa-save", "Persistence", "Persistence configuration"),
    ],
    "LedgerIntegration": [
        ("TransactionLogging", "/ledger-integration/transaction-logging", "fas fa-file-alt", "Transaction Logging", "Transaction logging"),
        ("Compliance", "/ledger-integration/compliance", "fas fa-shield-alt", "Compliance", "Compliance integration"),
        ("CurrencyRegistry", "/ledger-integration/currency-registry", "fas fa-coins", "Currency Registry", "Currency registry"),
        ("CreditTracking", "/ledger-integration/credit-tracking", "fas fa-chart-line", "Credit Tracking", "Credit tracking"),
        ("Transparency", "/ledger-integration/transparency", "fas fa-eye", "Transparency", "Transparency reports"),
    ],
    "Node": [
        ("Deployment", "/node/deployment", "fas fa-rocket", "Deployment", "Deployment management"),
        ("ContainerOrchestration", "/node/container-orchestration", "fas fa-docker", "Container Orchestration", "Container orchestration"),
        ("DisasterRecovery", "/node/disaster-recovery", "fas fa-undo", "Disaster Recovery", "Disaster recovery"),
        ("EnhancedSecurity", "/node/enhanced-security", "fas fa-shield-alt", "Enhanced Security", "Enhanced security"),
        ("MultiRegion", "/node/multi-region", "fas fa-globe", "Multi-Region", "Multi-region management"),
        ("PerformanceValidation", "/node/performance-validation", "fas fa-tachometer-alt", "Performance Validation", "Performance validation"),
        ("SecurityAudit", "/node/security-audit", "fas fa-clipboard-check", "Security Audit", "Security audit"),
    ],
}

PAGE_TEMPLATE = """@page "{route}"
@using MameyNode.Portals.Shared.Components.Layout
@using MameyNode.Portals.Shared.Components.DataDisplay
@using MameyNode.Portals.Shared.Components.Forms
@using MudBlazor

<PageContainer>
    <SectionHeader Title="{title}" Icon="{icon}" Subtitle="{subtitle}">
        <ActionButton Icon="fas fa-plus" Text="New" OnClick="ShowNewDialog" />
    </SectionHeader>

    <MudGrid Spacing="3">
        <MudItem xs="12" md="4">
            <StatCard Title="Total" Value="$0.00" Icon="fas fa-dollar-sign" />
        </MudItem>
        <MudItem xs="12" md="4">
            <StatCard Title="Count" Value="0" Icon="fas fa-list" />
        </MudItem>
        <MudItem xs="12" md="4">
            <StatCard Title="Active" Value="0" Icon="fas fa-check-circle" />
        </MudItem>
    </MudGrid>

    <MudGrid Spacing="3" Class="mt-4">
        <MudItem xs="12">
            <CardContainer>
                <MudText Typo="Typo.h6" Class="mb-4">{title} Management</MudText>
                <EmptyState Icon="{icon}" Title="No {title_lower} yet" Message="Get started by creating your first {title_lower}." />
            </CardContainer>
        </MudItem>
    </MudGrid>
</PageContainer>

@code {{
    private void ShowNewDialog() {{ }}
}}
"""

def generate_portal_pages(portal_name, pages_config):
    """Generate all feature pages for a portal."""
    portal_dir = f"src/MameyNode.Portals.{portal_name}/Pages"
    os.makedirs(portal_dir, exist_ok=True)
    
    generated = []
    for page_name, route, icon, title, subtitle in pages_config:
        title_lower = title.lower()
        content = PAGE_TEMPLATE.format(
            route=route,
            icon=icon,
            title=title,
            subtitle=subtitle,
            title_lower=title_lower
        )
        
        file_path = os.path.join(portal_dir, f"{page_name}.razor")
        with open(file_path, 'w') as f:
            f.write(content)
        
        generated.append(file_path)
    
    return generated

def main():
    """Generate all feature pages for all portals."""
    total_pages = 0
    
    for portal_name, pages_config in PORTAL_PAGES.items():
        print(f"\nGenerating pages for {portal_name} portal...")
        generated = generate_portal_pages(portal_name, pages_config)
        total_pages += len(generated)
        print(f"  ✓ Generated {len(generated)} pages")
    
    print(f"\n{'='*60}")
    print(f"Total feature pages generated: {total_pages}")
    print(f"Portals processed: {len(PORTAL_PAGES)}")
    print(f"{'='*60}")

if __name__ == "__main__":
    main()

