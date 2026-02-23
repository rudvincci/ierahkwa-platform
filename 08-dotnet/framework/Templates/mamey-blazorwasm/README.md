# Mamey BlazorWasm Micro-Frontend Template

This template creates a BlazorWasm Razor Class Library (RCL) project for micro-frontend implementation following the Mamey Framework architecture.

## Installation

```bash
dotnet new install Mamey.Templates
```

Or install from local directory:

```bash
dotnet new install ./Mamey/Templates/mamey-blazorwasm
```

## Usage

```bash
dotnet new mamey-blazorwasm \
  --Domain "BIIS" \
  --Service "TreatyLiquidityPools" \
  --Entity "TreatyLiquidityPool" \
  --Entities "TreatyLiquidityPools" \
  --service "treaty-liquidity-pools" \
  --Icon "AccountBalance" \
  --RequiredRoles "Admin,Treasury" \
  --ApiBaseUrl "https://api.example.com"
```

## Parameters

| Parameter | Description | Example |
|-----------|-------------|---------|
| `Domain` | The domain name | `BIIS`, `SICB`, `Government`, `FBDETB` |
| `Service` | The service name | `TreatyLiquidityPools`, `Citizens` |
| `Entity` | The primary entity name | `TreatyLiquidityPool`, `Citizen` |
| `Entities` | The plural entity name | `TreatyLiquidityPools`, `Citizens` |
| `service` | The service route (lowercase, kebab-case) | `treaty-liquidity-pools`, `citizens` |
| `Icon` | Material icon name | `AccountBalance`, `People`, `Dashboard` |
| `RequiredRoles` | Comma-separated list of required roles | `Admin,Treasury`, `User` |
| `ApiBaseUrl` | Base URL for API client | `https://api.example.com` |

## Generated Structure

```
Mamey.{Domain}.{Service}.BlazorWasm/
├── Components/
│   ├── Pages/
│   │   ├── EntityIndex.razor
│   │   ├── EntityCreate.razor
│   │   ├── EntityEdit.razor
│   │   └── EntityDetails.razor
│   └── Shared/
│       ├── EntityList.razor
│       └── EntityCard.razor
├── Routing/
│   └── ServiceRouteService.cs
├── ApiClients/
│   └── IServiceApiClient.cs
├── Services/
│   └── ServiceService.cs
├── Extensions.cs
├── _Imports.razor
└── Mamey.{Domain}.{Service}.BlazorWasm.csproj
```

## Features

- **Contracts-Only Integration**: Only references the Contracts project, never Application or Infrastructure
- **RouteService Pattern**: Implements `IRouteService` for dynamic navigation
- **Type-Safe API Clients**: Uses Refit for type-safe HTTP clients
- **MameyPro Components**: Uses MameyPro components from `Mamey.BlazorWasm`
- **Role-Based Access**: Configurable role requirements for routes
- **Standard CRUD Pages**: Index, Create, Edit, Details pages
- **Shared Components**: Reusable list and card components

## Post-Generation Steps

1. Update the API client interface to match your actual Contracts
2. Update the service implementation to match your API
3. Customize the component templates for your specific entity
4. Add view-specific folders (Public, Citizen, Admin, Treasury) as needed
5. Configure the API base URL in appsettings.json
6. Register the service in your RCL or BlazorServer application:
   ```csharp
   builder.AddServiceBlazorWasm();
   ```

## Integration with RCLs

This BlazorWasm project should be referenced by an RCL (e.g., `Mamey.BIIS.Blazor`) which aggregates multiple BlazorWasm micro-frontends. The RCL is then referenced by the main BlazorServer applications.

## See Also

- [Master Integration Blazor Plan](.cursor/plans/Master_Integration_Blazor_Plan.plan.md)
- [Mamey Framework Documentation](../../docs/)











