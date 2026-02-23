# MameyNode Portals Architecture

## Overview

The MameyNode Portals system provides a comprehensive, modular UI framework for all MameyNode functional areas. The architecture follows Material Design 3.0 principles, emphasizing reusability, consistency, and modern aesthetics.

## Architecture Principles

### Modular Design
- Each portal is a self-contained Razor Class Library (RCL)
- Portals can be independently developed, tested, and deployed
- Shared components library ensures consistency across portals

### Material Design 3.0 Compliance
- All components follow Google's Material Design 3.0 guidelines
- Consistent spacing, elevation, typography, and color system
- Responsive design with mobile-first approach

### Component Reusability
- 30+ shared components in `MameyNode.Portals.Shared`
- Portal-specific components when needed
- Priority: MameyPro > MudBlazor > Shared Components > Portal-Specific

## Portal Structure

Each portal follows this standard structure:

```
MameyNode.Portals.{Portal}/
├── _Imports.razor              # Namespace imports
├── {Portal}RouteService.cs     # Route registration
├── MameyNode.Portals.{Portal}.csproj
├── Pages/
│   ├── {Portal}Home.razor      # Main dashboard
│   ├── {Feature1}.razor        # Feature pages
│   └── {Feature2}.razor
└── Components/ (optional)
    └── {Component}.razor       # Portal-specific components
```

## Portal List

### Phase 1: Financial Services Portals (4 portals)
1. **Payments** (`MameyNode.Portals.Payments`)
   - 14 modules: P2P, Merchant, Payment Gateway, Recurring, Subscriptions, Invoicing, Remittances, Bill Payments, Disbursements, Multi-Sig, Loyalty
   - 12 pages + Home

2. **Lending** (`MameyNode.Portals.Lending`)
   - 17 modules: Loans, Collateral, Interest Rates, Lending Pools, P2P Lending, Asset-Based Lending, Mortgages, Student Loans, Microloans, Credit Cards, Money Market, Credit Risk, Repayment, Forgiveness
   - 15 pages + Home

3. **DEX** (`MameyNode.Portals.Dex`)
   - 11 modules: Swaps, AMM, Liquidity Pools, Order Books, Matching Engine, Advanced Orders, Routing, Oracle
   - 9 pages + Home

4. **Crypto Exchange** (`MameyNode.Portals.CryptoExchange`)
   - 13 modules: Exchange Engine, Order Management, Trading Pairs, Wallet Management, Custody, Staking, Stablecoin Routing, Multi-Currency, Banking Integration, Crypto Lending, Derivatives
   - 11 pages + Home

### Phase 2: Smart Contracts Portals (2 portals)
5. **Smart Contracts** (`MameyNode.Portals.SmartContracts`)
   - 23 modules: Runtime, Gas Metering, Storage, Events, Call Stack, Host Functions, API, Validation, Recovery, Metrics, Multi-Call, Ownable, RBAC, Token Standards (ERC-20/721/1155), Proxy, Versioning
   - 12 pages + Home

6. **Account Abstraction** (`MameyNode.Portals.AccountAbstraction`)
   - 12 modules: Smart Contract Wallets, Factory, Multi-Sig, Social Recovery, Session Keys, Permissions, Paymaster, Paymaster Policy, Account Recovery
   - 10 pages + Home

### Phase 3: Integration & Protocol Portals (6 portals)
7. **Bridge** (`MameyNode.Portals.Bridge`)
   - 9 modules: Account Mapping, Transaction Bridge, Identity Bridge, Cross-Chain Bridge, Ethereum Bridge, Bitcoin Bridge, Security
   - 8 pages + Home

8. **ILP** (`MameyNode.Portals.ILP`)
   - 8 modules: Packets, Connector, Service, Routing, Ledger Integration, Handler, Settlement
   - 8 pages + Home

9. **ODL** (`MameyNode.Portals.ODL`)
   - 11 modules: Bridge Currency Management, Exchange Rate Oracle, Execution, Payment Execution, Liquidity Management, Provider Management, Validation, API
   - 7 pages + Home

10. **Pathfinding** (`MameyNode.Portals.Pathfinding`)
    - 9 modules: Pathfinder, Currency Graph, Path Execution, DEX Integration, Exchange Rate Service, Liquidity Pool Integration, Helpers
    - 7 pages + Home

11. **Travel Rule** (`MameyNode.Portals.TravelRule`)
    - 11 modules: IVMS-101, VASP Directory, Message Routing, Encryption, TRP, Compliance Integration
    - 7 pages + Home

12. **UPG** (`MameyNode.Portals.UPG`)
    - 12 modules: Protocol Support, Adapters, Normalization, Multi-Rail Routing, HSM, FX, POS, Offline, Merchant, Real-Time Payments (FedNow/RTP/PIX/UPI)
    - 11 pages + Home

### Phase 4: Advanced Features Portals (4 portals)
13. **Channels** (`MameyNode.Portals.Channels`)
    - 8 modules: Channel Management, Protocol, Routing, Funding, Off-Chain Updates, Closing
    - 7 pages + Home

14. **Programmable** (`MameyNode.Portals.Programmable`)
    - 7 modules: Conditions, Evaluator, Wallet, Enforcement, Expiring Balances
    - 6 pages + Home

15. **Sharding** (`MameyNode.Portals.Sharding`)
    - 13 modules: Shard Management, Assignment, Routing, Cross-Shard Communication, Beacon Chain, State Management, Consistent Hashing, Transaction Coordination, Validation, Consensus
    - 11 pages + Home

16. **Advanced** (`MameyNode.Portals.Advanced`)
    - 7 modules: Escrow, Tokenization, Insurance, Offline, Satellite
    - 6 pages + Home

### Phase 5: Infrastructure & Support Portals (8 portals)
17. **Compliance** (`MameyNode.Portals.Compliance`)
    - 19 modules: KYC, AML/CFT, Fraud Detection, Sanctions Screening, Transaction Monitoring, Regulatory Reporting, Data Privacy, Market Surveillance, Whitelist/Blacklist, Enforcement, Limits, Enhanced Audit, ZKP Compliance, CDD
    - 15 pages + Home

18. **Metrics** (`MameyNode.Portals.Metrics`)
    - 7 modules: Metrics Collector, Registry, HTTP Endpoint, Enhanced Observability, Health Checks, Enhanced Monitoring
    - 7 pages + Home

19. **Webhooks** (`MameyNode.Portals.Webhooks`)
    - 13 modules: Client, HTTP Client, Queue, Signatures, Persistence, Health, Rate Limiting, API, Validation, Helpers
    - 10 pages + Home

20. **Callbacks** (`MameyNode.Portals.Callbacks`)
    - 6 modules: Transaction Callbacks, Settlement Callbacks, Account Callbacks, Manager
    - 5 pages + Home

21. **RBAC** (`MameyNode.Portals.RBAC`)
    - 7 modules: Role Management, Permission Management, Hierarchy, Guard
    - 5 pages + Home

22. **Trust Lines** (`MameyNode.Portals.TrustLines`)
    - 8 modules: Trust Line Data Structures, Manager, Storage, Validation, Indexing, Persistence
    - 6 pages + Home

23. **Ledger Integration** (`MameyNode.Portals.LedgerIntegration`)
    - 7 modules: Transaction Logging, Compliance, Currency Registry, Credit Tracking, Transparency
    - 6 pages + Home

24. **Node** (`MameyNode.Portals.Node`)
    - 9 modules: Main, Deployment, Container Orchestration, Disaster Recovery, Enhanced Security, Multi-Region, Performance Validation, Security Audit
    - 8 pages + Home

## Shared Components Library

The `MameyNode.Portals.Shared` project provides 30+ reusable components organized into categories:

### Layout Components
- `AppBar.razor` - Material App Bar with navigation and actions
- `NavigationDrawer.razor` - Material Navigation Drawer with portal menu
- `PageContainer.razor` - Consistent page container with proper spacing
- `SectionHeader.razor` - Section headers with icons and actions
- `CardContainer.razor` - Material Card wrapper with elevation and padding

### Data Display Components
- `TransactionCard.razor` - Modern transaction card with Material elevation
- `AccountCard.razor` - Account display card with avatar and status
- `StatusBadge.razor` - Material Chip-based status badge with color coding
- `AmountDisplay.razor` - Currency amount display with formatting
- `DateTimeDisplay.razor` - Date/time display with relative time support
- `DataTable.razor` - Material DataTable with sorting, filtering, pagination
- `StatCard.razor` - Statistics card with icon, value, and trend indicator
- `ChartCard.razor` - Chart container with Material elevation

### Form Components
- `FormCard.razor` - Form container with Material styling
- `FormSection.razor` - Form section divider with optional header
- `ActionButton.razor` - Material FAB or Button with consistent styling
- `SearchBar.razor` - Material search bar with icon and clear action
- `FilterPanel.razor` - Expandable filter panel with Material accordion

### Feedback Components
- `LoadingSpinner.razor` - Material CircularProgress with overlay
- `ErrorDisplay.razor` - Material Alert for error messages
- `SuccessSnackbar.razor` - Material Snackbar for success notifications
- `ConfirmationDialog.razor` - Material Dialog for confirmations
- `InfoDialog.razor` - Material Dialog for information display
- `ProgressIndicator.razor` - Linear progress for multi-step processes

### Navigation Components
- `BreadcrumbNav.razor` - Material Breadcrumb navigation
- `TabNavigation.razor` - Material Tabs for section navigation
- `Pagination.razor` - Material Pagination component
- `QuickActionMenu.razor` - Floating action menu with Material FAB

### Specialized Components
- `EmptyState.razor` - Empty state display with icon and message
- `SkeletonLoader.razor` - Material Skeleton for loading states
- `Timeline.razor` - Material Timeline for transaction history
- `AvatarStack.razor` - Stacked avatars for multi-user displays
- `TagList.razor` - Material Chip list for tags/categories
- `MetricCard.razor` - Metric display with icon, value, and change indicator

## Routing System

Each portal implements `IRouteService` to register its routes:

```csharp
public class {Portal}RouteService : IRouteService
{
    public List<Route> Routes { get; private set; } = new();
    
    public Task InitializeAsync(bool menu = false)
    {
        Routes = new List<Route>
        {
            new Route
            {
                Page = "/{portal}",
                Title = "{Portal Name}",
                Icon = "fas fa-icon",
                AuthenticationRequired = true,
                RequiredRoles = new List<string> { "Role1", "Role2" },
                ChildRoutes = new List<Route> { /* ... */ }
            }
        };
        return Task.CompletedTask;
    }
}
```

Routes are automatically discovered and registered in `Program.cs` via `AddAdditionalAssemblies`.

## Mock Services

The `MameyNode.Portals.Mocks` project provides mock implementations for all portal services using the Bogus library for realistic test data. Mock services are registered when `UseMocks=true` in configuration.

## Testing Strategy

Each portal has a corresponding test project:
- `tests/MameyNode.Portals.{Portal}.Tests/`
- Test framework: xUnit
- Component testing: bUnit
- Mocking: Moq
- Assertions: FluentAssertions

Test coverage includes:
- RouteService tests
- Page component tests
- Component tests
- Integration tests

Target: >80% code coverage for all portals.

## Material Design 3.0 Implementation

### Color System
- Primary, Secondary, Surface colors
- Semantic colors: Success, Warning, Error, Info
- WCAG 2.1 AA contrast compliance

### Typography
- Material type scale (H1-H6)
- Roboto font family
- Responsive typography scaling

### Spacing
- Material spacing scale: 4dp, 8dp, 16dp, 24dp, 32dp, 48dp, 64dp
- Container padding: 16dp mobile, 24dp tablet, 32dp desktop

### Elevation
- 0dp (flat), 1dp (hover), 2dp (cards), 4dp (dialogs), 8dp (app bar)

### Responsive Breakpoints
- xs: 0px
- sm: 600px
- md: 960px
- lg: 1280px
- xl: 1920px

## Component Priority

When building UI, follow this priority order:
1. **MameyPro Components** - Use when available (`Mamey.BlazorWasm.Components.MameyPro`)
2. **MudBlazor Components** - Material Design components
3. **Shared Components** - Reusable components from `MameyNode.Portals.Shared`
4. **Portal-Specific Components** - Only when no suitable shared component exists

## Accessibility

All portals must comply with WCAG 2.1 AA:
- Keyboard navigation for all interactive elements
- Clear focus indicators
- Proper ARIA labels and roles
- Color contrast compliance
- Minimum 48x48dp touch targets

## Performance

- Lazy loading for heavy components
- Virtualization for large data sets
- Optimized rendering with Blazor Server
- Efficient state management

## Integration

Portals are integrated into the main web application via:
1. Project references in solution file
2. Assembly registration in `Program.cs` `AddAdditionalAssemblies`
3. Route discovery via `IRouteService` implementations
4. Mock service registration when `UseMocks=true`

## Future Enhancements

- Portal-specific themes
- Advanced analytics integration
- Real-time updates via SignalR
- Progressive Web App (PWA) support
- Internationalization (i18n) support


