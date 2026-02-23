# Mamey RCL (Razor Class Library) Template

This template creates an RCL (Razor Class Library) project that aggregates multiple BlazorWasm micro-frontend projects into a single composable UI library.

## Installation

```bash
dotnet new install Mamey.Templates
```

Or install from local directory:

```bash
dotnet new install ./Mamey/Templates/mamey-rcl
```

## Usage

```bash
dotnet new mamey-rcl \
  --Domain "Government" \
  --BlazorWasmProjects "Citizens.BlazorWasm,Diplomats.BlazorWasm,CitizenshipApplications.BlazorWasm"
```

## Parameters

| Parameter | Description | Example |
|-----------|-------------|---------|
| `Domain` | The domain name | `BIIS`, `SICB`, `Government`, `FBDETB` |
| `BlazorWasmProjects` | Comma-separated list of BlazorWasm project names (optional, add manually) | `Citizens.BlazorWasm,Diplomats.BlazorWasm` |

## Generated Structure

```
Mamey.{Domain}.Blazor/
├── Components/
│   ├── App.razor
│   ├── Layout/
│   │   ├── MainLayout.razor
│   │   └── NavLayout.razor
│   └── Pages/
│       ├── Home.razor
│       └── Error.razor
├── Extensions.cs
├── _Imports.razor
└── Mamey.{Domain}.Blazor.csproj
```

## Features

- **Micro-Frontend Aggregation**: References multiple BlazorWasm projects
- **Route Discovery**: Automatically discovers routes from all BlazorWasm projects via RouteLoader
- **Shared Layouts**: Provides common layouts and navigation
- **Dependency Injection**: Extensions for registering all BlazorWasm services
- **MameyPro Components**: Uses MameyPro components from `Mamey.BlazorWasm`

## Post-Generation Steps

1. **Add BlazorWasm Project References**:
   Edit the `.csproj` file to add project references:
   ```xml
   <ItemGroup>
     <ProjectReference Include="..\..\Mamey.Domain.Service1.BlazorWasm\Mamey.Domain.Service1.BlazorWasm.csproj" />
     <ProjectReference Include="..\..\Mamey.Domain.Service2.BlazorWasm\Mamey.Domain.Service2.BlazorWasm.csproj" />
   </ItemGroup>
   ```

2. **Register BlazorWasm Services**:
   Edit `Extensions.cs` to register all BlazorWasm services:
   ```csharp
   public static IMameyBuilder AddDomainBlazor(this IMameyBuilder builder)
   {
       if (!builder.TryRegister(RegistryName))
       {
           return builder;
       }

       builder
           .AddService1BlazorWasm()
           .AddService2BlazorWasm()
           .AddService3BlazorWasm();

       return builder;
   }
   ```

3. **Configure Route Discovery**:
   The `App.razor` file automatically discovers routes from all referenced BlazorWasm projects via `RouteLoader.GetRoutesAsync()`.

4. **Customize Layouts**:
   Customize `MainLayout.razor` and `NavLayout.razor` for your domain-specific branding and navigation.

5. **Add Domain-Specific Components**:
   Add any domain-specific components, pages, or areas as needed.

## Integration with BlazorServer Applications

This RCL should be referenced by the main BlazorServer applications:

- `Mamey.Government.CitizenPortal.BlazorServer`
- `Mamey.Government.GovernmentPortal.BlazorServer`

Example registration in BlazorServer `Program.cs`:
```csharp
builder.AddGovernmentBlazor();
// ...
await app.UseGovernmentBlazorAsync();
```

## Architecture

```
┌─────────────────────────────────────┐
│   BlazorServer Application         │
│   (CitizenPortal/GovernmentPortal) │
└──────────────┬──────────────────────┘
               │ References
┌───────────────▼──────────────────────┐
│   RCL (This Template)               │
│   Mamey.Domain.Blazor               │
└──────────────┬──────────────────────┘
               │ References
┌──────────────▼──────────────────────┐
│   BlazorWasm Micro-Frontends        │
│   - Service1.BlazorWasm             │
│   - Service2.BlazorWasm             │
│   - Service3.BlazorWasm             │
└─────────────────────────────────────┘
```

## See Also

- [Master Integration Blazor Plan](.cursor/plans/Master_Integration_Blazor_Plan.plan.md)
- [BlazorWasm Template](../mamey-blazorwasm/README.md)
- [Mamey Framework Documentation](../../docs/)











