# Mamey Templates

This directory contains .NET templates for generating Mamey Framework projects and components.

## Available Templates

### 1. BlazorWasm Micro-Frontend Template (`mamey-blazorwasm`)

**Purpose**: Generate BlazorWasm Razor Class Library projects for micro-frontend implementation.

**Location**: `Mamey/Templates/mamey-blazorwasm/`

**Installation**:
```bash
dotnet new install ./Mamey/Templates/mamey-blazorwasm
```

**Usage**:
```bash
dotnet new mamey-blazorwasm \
  --Domain "Government" \
  --Service "Citizens" \
  --Entity "Citizen" \
  --Entities "Citizens" \
  --service "citizens" \
  --Icon "People" \
  --RequiredRoles "User,Citizen" \
  --ApiBaseUrl "https://api.example.com"
```

**Parameters**:
- `--Domain`: Domain name (e.g., BIIS, SICB, Government, FBDETB)
- `--Service`: Service name (e.g., TreatyLiquidityPools, Citizens)
- `--Entity`: Primary entity name, singular (e.g., TreatyLiquidityPool, Citizen)
- `--Entities`: Plural entity name (e.g., TreatyLiquidityPools, Citizens)
- `--service`: Service route (lowercase, kebab-case) (e.g., treaty-liquidity-pools, citizens)
- `--Icon`: Material icon name (e.g., AccountBalance, People, Dashboard)
- `--RequiredRoles`: Comma-separated list of required roles (e.g., Admin,Treasury, User)
- `--ApiBaseUrl`: Base URL for API client (from configuration)

**What Gets Generated**:
- Complete BlazorWasm project structure
- RouteService implementation (IRouteService)
- API Client interface (using Refit)
- Service layer with CRUD operations
- Component pages (Index, Create, Edit, Details)
- Shared components (List, Card)
- Extensions for dependency injection
- _Imports.razor with all required imports

**Documentation**: See `mamey-blazorwasm/README.md` for complete documentation.

---

### 2. RCL (Razor Class Library) Template (`mamey-rcl`)

**Purpose**: Generate RCL projects that aggregate multiple BlazorWasm micro-frontends.

**Location**: `Mamey/Templates/mamey-rcl/`

**Installation**:
```bash
dotnet new install ./Mamey/Templates/mamey-rcl
```

**Usage**:
```bash
dotnet new mamey-rcl --Domain "Government"
```

**Parameters**:
- `--Domain`: Domain name (e.g., BIIS, SICB, Government, FBDETB)

**What Gets Generated**:
- Complete RCL project structure
- App.razor with route discovery
- MainLayout and NavLayout components
- Extensions for registering BlazorWasm services
- Home and Error pages
- _Imports.razor with all required imports

**Documentation**: See `mamey-rcl/README.md` for complete documentation.

---

## Template Architecture

### Micro-Frontend Architecture

The templates follow a three-layer micro-frontend architecture:

```
┌─────────────────────────────────────┐
│   BlazorServer Applications         │
│   (CitizenPortal/GovernmentPortal) │
└──────────────┬──────────────────────┘
               │ References
┌───────────────▼──────────────────────┐
│   RCL (This Template)                │
│   Mamey.Domain.Blazor                │
└──────────────┬──────────────────────┘
               │ References
┌──────────────▼──────────────────────┐
│   BlazorWasm Micro-Frontends        │
│   - Service1.BlazorWasm             │
│   - Service2.BlazorWasm             │
│   - Service3.BlazorWasm             │
└─────────────────────────────────────┘
```

### Integration with Mamey.TemplateEngine

These templates complement `Mamey.TemplateEngine` for microservice generation:

1. **Mamey.TemplateEngine** generates the backend microservice (Api, Application, Domain, Infrastructure, Contracts)
2. **mamey-blazorwasm** generates the BlazorWasm micro-frontend for the service
3. **mamey-rcl** generates the RCL that aggregates multiple BlazorWasm projects

**Workflow**:
```bash
# Step 1: Generate microservice backend
dotnet new mamey-microservice --name Government.Citizens --entity Citizen --port 5001

# Step 2: Generate BlazorWasm micro-frontend
dotnet new mamey-blazorwasm \
  --Domain "Government" \
  --Service "Citizens" \
  --Entity "Citizen" \
  --Entities "Citizens" \
  --service "citizens" \
  --Icon "People" \
  --RequiredRoles "User,Citizen"

# Step 3: Generate RCL (if creating domain aggregation)
dotnet new mamey-rcl --Domain "Government"
```

## Template Development

### Creating New Templates

To create a new template:

1. Create a directory in `Mamey/Templates/` with the template name
2. Create `.template.config/template.json` with template metadata
3. Create template files with placeholders (e.g., `{Domain}`, `{Service}`)
4. Add `README.md` with documentation
5. Test the template: `dotnet new install ./Mamey/Templates/{template-name}`

### Template Structure

```
Mamey/Templates/{template-name}/
├── .template.config/
│   └── template.json          # Template metadata and parameters
├── {Template Files}/          # Template source files
└── README.md                  # Template documentation
```

### Template Parameters

Templates use the following parameter conventions:

- `{Domain}`: Domain name (e.g., BIIS, SICB, Government)
- `{Service}`: Service name (e.g., TreatyLiquidityPools, Citizens)
- `{Entity}`: Entity name, singular (e.g., TreatyLiquidityPool, Citizen)
- `{Entities}`: Entity name, plural (e.g., TreatyLiquidityPools, Citizens)
- `{service}`: Service route (lowercase, kebab-case)
- `{Icon}`: Material icon name
- `{RequiredRoles}`: Comma-separated roles
- `{ApiBaseUrl}`: API base URL

## References

- **Master Integration Blazor Plan**: `.cursor/plans/Master_Integration_Blazor_Plan.plan.md`
- **Microservice Creation Rules**: `.cursor/rules/microservice-creation.md`
- **Mamey.TemplateEngine**: For backend microservice generation
- **Mamey Framework Documentation**: `Mamey/docs/`

## Contributing

When adding new templates:

1. Follow the existing template structure
2. Include comprehensive README.md documentation
3. Test the template installation and generation
4. Update this README.md with the new template
5. Follow Mamey Framework patterns and conventions

---

**Mamey Framework - Building better microservices with .NET**











