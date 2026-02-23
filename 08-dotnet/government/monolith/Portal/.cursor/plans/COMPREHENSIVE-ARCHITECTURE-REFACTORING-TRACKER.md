# Comprehensive Architecture Refactoring Tracker

**Project**: Mamey Government Portal
**Last Updated**: 2026-01-08
**Status**: 🔴 CRITICAL - Architecture Refactoring Required
**Estimated Total Time**: 7-10 days

This document provides exhaustive step-by-step tracking for the architecture refactoring effort. Every task includes specific file paths, code examples, verification commands, and success criteria.

---

## Table of Contents

1. [Phase 1: Infrastructure Extensions (6 tasks, ~1 day)](#phase-1)
2. [Phase 2: Domain Layer Implementation (29 tasks, ~2 days)](#phase-2)
3. [Phase 3: Application Layer Separation (15 tasks, ~1 day)](#phase-3)
4. [Phase 4: Repository Pattern (20 tasks, ~1 day)](#phase-4)
5. [Phase 5: EF Core Migrations (5 tasks, ~1 day)](#phase-5)
6. [Phase 6: Mamey.Word Integration (3 tasks, ~2 days)](#phase-6)
7. [Phase 7: Configuration (1 task, ~30 mins)](#phase-7)
8. [Verification (2 tasks, ~2 hours)](#verification)

---

<a name="phase-1"></a>
## Phase 1: Infrastructure Extensions (6/6 completed) ⏱️ Est: 1 day

**Goal**: Extract all service registrations from Program.cs into module-specific Extensions.cs files.

**Progress**: ✅ ARCH-1.1 (Citizenship) | ✅ ARCH-1.2 (CMS) | ✅ ARCH-1.3 (Tenant) | ✅ ARCH-1.4 (Library) | ✅ ARCH-1.5 (Auth) | ✅ ARCH-1.6 (Program.cs)

**Reference**: `/Volumes/Barracuda/mamey-io/code-final/Mamey.Government/Mamey.Government.Citizens/src/Mamey.Government.Citizens.Infrastructure/Extensions.cs`

---

### ARCH-1.1: Create Citizenship Infrastructure Extensions ⏱️ 2-3 hours

**Status**: ✅ COMPLETED
**Time Spent**: ~45 minutes
**Completed**: 2026-01-08

**Task**: Create Extensions.cs for Citizenship module
**File Path**: `/Volumes/Barracuda/mamey-io/code-final/Mamey.Government/Portal/src/Mamey.Portal.Citizenship.Infrastructure/Extensions.cs`
**Lines to Move from Program.cs**: 60-158 (DbContext, services, storage)

**Implementation Pattern**:
```csharp
using Microsoft.Extensions.DependencyInjection;
using Microsoft.EntityFrameworkCore;
using Mamey;
using Mamey.Portal.Citizenship.Infrastructure.Persistence;
using Mamey.Portal.Citizenship.Infrastructure.Services;
using Mamey.Portal.Citizenship.Application.Services;

namespace Mamey.Portal.Citizenship.Infrastructure;

public static class Extensions
{
    public static IMameyBuilder AddCitizenshipInfrastructure(
        this IMameyBuilder builder,
        string connectionString)
    {
        // 1. Ensure logging is registered
        if (!builder.Services.Any(s => s.ServiceType == typeof(ILoggerFactory)))
        {
            builder.Services.AddLogging();
        }

        // 2. Register DbContext
        builder.Services.AddDbContext<CitizenshipDbContext>(options =>
        {
            options.UseNpgsql(connectionString);
            options.EnableSensitiveDataLogging(builder.Configuration.GetValue<bool>("Logging:EnableSensitiveDataLogging"));
        });

        // 3. Register Application Services (MUST be BEFORE infrastructure)
        builder.Services.AddApplicationServices();

        // 4. Register Storage Services (MinIO)
        RegisterStorageServices(builder);

        // 5. Register Infrastructure Services
        RegisterInfrastructureServices(builder);

        // 6. Register Background Services
        builder.Services.AddHostedService<ApplicationWorkflowBackgroundService>();

        return builder;
    }

    private static void AddApplicationServices(this IServiceCollection services)
    {
        // These will be in Application layer after Phase 3
        // For now, keep registrations here
        services.AddScoped<ICitizenshipApplicationService, CitizenshipApplicationService>();
        services.AddScoped<IApplicationFormPdfGenerator, ApplicationFormPdfGenerator>();
        services.AddScoped<IApplicationWorkflowService, ApplicationWorkflowService>();
        services.AddScoped<IApplicationStatusService, ApplicationStatusService>();
        services.AddScoped<IDocumentIssuanceService, DocumentIssuanceService>();
        services.AddScoped<IPublicDocumentValidationService, PublicDocumentValidationService>();
        services.AddScoped<IIntakeReviewService, IntakeReviewService>();
        services.AddScoped<IPaymentPlanService, PaymentPlanService>();
    }

    private static void RegisterStorageServices(IMameyBuilder builder)
    {
        var minioEndpoint = builder.Configuration["MinIO:Endpoint"] ?? "localhost:9000";
        var minioAccessKey = builder.Configuration["MinIO:AccessKey"] ?? "minioadmin";
        var minioSecretKey = builder.Configuration["MinIO:SecretKey"] ?? "minioadmin";

        builder.Services.AddMinio(configureClient => configureClient
            .WithEndpoint(minioEndpoint)
            .WithCredentials(minioAccessKey, minioSecretKey)
            .WithSSL(false)
            .Build());

        builder.Services.AddScoped<IDocumentStorageService, MinIODocumentStorageService>();
    }

    private static void RegisterInfrastructureServices(IMameyBuilder builder)
    {
        // Barcode services
        builder.Services.AddScoped<IAamvaBarcodeService, AamvaBarcodeService>();
        builder.Services.AddScoped<IBarcodeScanningService, BarcodeScanningService>();

        // Standards compliance
        builder.Services.AddScoped<IStandardsComplianceValidator, StandardsComplianceValidator>();
    }
}
```

**Services to Register**:
- [x] CitizenshipDbContext
- [x] ICitizenshipApplicationService → CitizenshipApplicationService
- [x] IApplicationFormPdfGenerator → ApplicationFormPdfGenerator
- [x] IApplicationWorkflowService → ApplicationWorkflowService
- [x] IApplicationStatusService → ApplicationStatusService
- [x] IDocumentIssuanceService → DocumentIssuanceService
- [x] IPublicDocumentValidationService → PublicDocumentValidationService
- [x] IIntakeReviewService → IntakeReviewService
- [x] IPaymentPlanService → PaymentPlanService
- [x] IAamvaBarcodeService → AamvaBarcodeService
- [x] IBarcodeScanningService → BarcodeScanningService
- [x] ApplicationWorkflowBackgroundService (hosted service)

**Verification Commands**:
```bash
# Build Infrastructure project
dotnet build src/Mamey.Portal.Citizenship.Infrastructure

# Should compile without errors
# Expected output: Build succeeded. 0 Error(s)
```

**Success Criteria**:
- [x] Extensions.cs file created
- [x] All services registered in Extensions.cs
- [x] Project compiles without errors (0 errors, 2 warnings)
- [ ] No references to Program.cs service registrations (will be done in ARCH-1.6)

---

### ARCH-1.2: Create CMS Infrastructure Extensions ⏱️ 30 mins

**Status**: ✅ COMPLETED
**Time Spent**: ~30 minutes
**Completed**: 2026-01-08

**Task**: Create Extensions.cs for CMS module
**File Path**: `/Volumes/Barracuda/mamey-io/code-final/Mamey.Government/Portal/src/Mamey.Portal.Cms.Infrastructure/Extensions.cs`
**Lines to Move from Program.cs**: 159-180

**Implementation Pattern**:
```csharp
namespace Mamey.Portal.Cms.Infrastructure;

public static class Extensions
{
    public static IMameyBuilder AddCmsInfrastructure(
        this IMameyBuilder builder)
    {
        var connectionString = builder.Configuration.GetConnectionString("PortalDb")
            ?? "Host=localhost;Database=mamey_portal_dev;Username=postgres;Password=postgres";

        // DbContext
        builder.Services.AddDbContext<CmsDbContext>(options =>
        {
            options.UseNpgsql(connectionString);
            var enableSensitiveLogging = builder.Configuration.GetValue<bool>("Logging:EnableSensitiveDataLogging");
            if (enableSensitiveLogging)
            {
                options.EnableSensitiveDataLogging();
            }
        });

        // Application Services
        services.AddScoped<ICmsContentService, CmsContentService>();
        services.AddScoped<ICmsPageService, CmsPageService>();

        return builder;
    }
}
```

**Services to Register**:
- [x] CmsDbContext
- [x] ICmsContentService → CmsContentService (Note: not ICmsNewsService)
- [x] ICmsPageService → CmsPageService

**Verification Commands**:
```bash
dotnet build src/Mamey.Portal.Cms.Infrastructure
```

**Build Result**:
```
Build succeeded.
    0 Warning(s)
    0 Error(s)
Time Elapsed 00:00:05.76
```

**Success Criteria**:
- [x] Extensions.cs file created
- [x] All services registered
- [x] Project compiles without errors (0 errors, 0 warnings)

---

### ARCH-1.3: Create Tenant Infrastructure Extensions ⏱️ 1 hour

**Status**: ✅ COMPLETED

**Task**: Create Extensions.cs for Tenant module
**File Path**: `/Volumes/Barracuda/mamey-io/code-final/Mamey.Government/Portal/src/Mamey.Portal.Tenant.Infrastructure/Extensions.cs`
**Lines to Move from Program.cs**: 181-250

**Implementation Pattern**:
```csharp
namespace Mamey.Portal.Tenant.Infrastructure;

public static class Extensions
{
    public static IMameyBuilder AddTenantInfrastructure(
        this IMameyBuilder builder,
        string connectionString)
    {
        // DbContext
        builder.Services.AddDbContext<TenantDbContext>(options =>
            options.UseNpgsql(connectionString));

        // Application Services
        builder.Services.AddScoped<ITenantService, TenantService>();
        builder.Services.AddScoped<ITenantSettingsService, TenantSettingsService>();
        builder.Services.AddScoped<IDocumentTemplateService, DocumentTemplateService>();
        builder.Services.AddScoped<IDocumentNamingConventionService, DocumentNamingConventionService>();
        builder.Services.AddScoped<IUserTenantMappingService, UserTenantMappingService>();
        builder.Services.AddScoped<IUserInviteService, UserInviteService>();

        // Middleware
        builder.Services.AddScoped<TenantMiddleware>();
        builder.Services.AddScoped<CurrentTenantAccessor>();

        return builder;
    }
}
```

**Services to Register**:
- [x] TenantDbContext
- [x] ITenantService → TenantService
- [x] ITenantSettingsService → TenantSettingsService
- [x] IDocumentTemplateService → DocumentTemplateService
- [x] IDocumentNamingConventionService → DocumentNamingConventionService
- [x] IUserTenantMappingService → UserTenantMappingService
- [x] IUserInviteService → UserInviteService
- [x] TenantMiddleware
- [x] CurrentTenantAccessor

**Verification Commands**:
```bash
dotnet build src/Mamey.Portal.Tenant.Infrastructure
```

**Success Criteria**:
- [ ] Extensions.cs file created
- [ ] All services registered
- [ ] Project compiles without errors

---

### ARCH-1.4: Create Library Infrastructure Extensions ⏱️ 20 mins

**Status**: ✅ COMPLETED

**Task**: Create Extensions.cs for Library module
**File Path**: `/Volumes/Barracuda/mamey-io/code-final/Mamey.Government/Portal/src/Mamey.Portal.Library.Infrastructure/Extensions.cs`
**Lines to Move from Program.cs**: 251-260

**Implementation Pattern**:
```csharp
namespace Mamey.Portal.Library.Infrastructure;

public static class Extensions
{
    public static IMameyBuilder AddLibraryInfrastructure(
        this IMameyBuilder builder,
        string connectionString)
    {
        // DbContext
        builder.Services.AddDbContext<LibraryDbContext>(options =>
            options.UseNpgsql(connectionString));

        // Application Services
        builder.Services.AddScoped<ILibraryService, LibraryService>();

        return builder;
    }
}
```

**Services to Register**:
- [x] LibraryDbContext
- [x] ILibraryService → LibraryService

**Verification Commands**:
```bash
dotnet build src/Mamey.Portal.Library.Infrastructure
```

**Success Criteria**:
- [ ] Extensions.cs file created
- [ ] Services registered
- [ ] Project compiles without errors

---

### ARCH-1.5: Create Auth Infrastructure Extensions ⏱️ 30 mins

**Status**: ✅ COMPLETED

**Task**: Create Extensions.cs for Auth module
**File Path**: `/Volumes/Barracuda/mamey-io/code-final/Mamey.Government/Portal/src/Mamey.Portal.Auth.Infrastructure/Extensions.cs`
**Lines to Move from Program.cs**: 80-150 (OIDC configuration)

**Implementation Pattern**:
```csharp
namespace Mamey.Portal.Auth.Infrastructure;

public static class Extensions
{
    public static IMameyBuilder AddAuthInfrastructure(
        this IMameyBuilder builder)
    {
        var useOidc = builder.Configuration.GetValue<bool>("Authentication:UseOIDC");

        if (useOidc)
        {
            AddAuthentikAuthentication(builder);
        }
        else
        {
            AddMockAuthentication(builder);
        }

        AddAuthorizationPolicies(builder);

        return builder;
    }

    private static void AddAuthentikAuthentication(IMameyBuilder builder)
    {
        builder.Services
            .AddAuthentication(options =>
            {
                options.DefaultScheme = CookieAuthenticationDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = OpenIdConnectDefaults.AuthenticationScheme;
            })
            .AddCookie(CookieAuthenticationDefaults.AuthenticationScheme)
            .AddOpenIdConnect(OpenIdConnectDefaults.AuthenticationScheme, options =>
            {
                options.Authority = builder.Configuration["Authentication:Authentik:Authority"];
                options.ClientId = builder.Configuration["Authentication:Authentik:ClientId"];
                options.ClientSecret = builder.Configuration["Authentication:Authentik:ClientSecret"];
                options.ResponseType = "code";
                options.SaveTokens = true;
                options.GetClaimsFromUserInfoEndpoint = true;
                // ... more configuration
            });
    }

    private static void AddMockAuthentication(IMameyBuilder builder)
    {
        builder.Services.AddAuthentication("MockAuth")
            .AddScheme<AuthenticationSchemeOptions, MockAuthenticationHandler>("MockAuth", null);
        builder.Services.AddScoped<AuthenticationStateProvider, MockAuthStateProvider>();
    }

    private static void AddAuthorizationPolicies(IMameyBuilder builder)
    {
        builder.Services.AddAuthorizationBuilder()
            .AddPolicy("Citizen", policy => policy.RequireRole("User", "Citizen"))
            .AddPolicy("Government", policy => policy.RequireRole("Government"))
            .AddPolicy("Admin", policy => policy.RequireRole("Admin"))
            .AddPolicy("Treasury", policy => policy.RequireRole("Treasury"));
    }
}
```

**Services to Register**:
- [x] Authentication (Authentik OIDC or Mock)
- [x] Authorization policies
- [x] MockAuthStateProvider (if !useOidc)

**Verification Commands**:
```bash
dotnet build src/Mamey.Portal.Auth.Infrastructure
```

**Success Criteria**:
- [ ] Extensions.cs file created
- [ ] Authentication configured
- [ ] Authorization policies registered
- [ ] Project compiles without errors

---

### ARCH-1.6: Refactor Program.cs to use IMameyBuilder ⏱️ 1 hour

**Status**: ✅ COMPLETED

**Task**: Simplify Program.cs to < 150 lines
**File Path**: `/Volumes/Barracuda/mamey-io/code-final/Mamey.Government/Portal/src/Mamey.Portal.Web/Program.cs`
**Current Lines**: ~1600 lines
**Target Lines**: < 150 lines

**Before**: 1600+ lines with all service registrations

**After Pattern**:
```csharp
using Mamey;
using Mamey.Portal.Citizenship.Infrastructure;
using Mamey.Portal.Cms.Infrastructure;
using Mamey.Portal.Tenant.Infrastructure;
using Mamey.Portal.Library.Infrastructure;
using Mamey.Portal.Auth.Infrastructure;
using MudBlazor.Services;

var builder = WebApplication.CreateBuilder(args);

// Create Mamey builder
var mameyBuilder = MameyBuilder.Create(builder.Services, builder.Configuration);

// Get connection string
var connectionString = builder.Configuration.GetConnectionString("PortalDb")
    ?? "Host=localhost;Database=mamey_portal_dev;Username=postgres;Password=postgres";

// Register domain modules
mameyBuilder
    .AddCitizenshipInfrastructure(connectionString)
    .AddCmsInfrastructure(connectionString)
    .AddTenantInfrastructure(connectionString)
    .AddLibraryInfrastructure(connectionString)
    .AddAuthInfrastructure();

// Register Web-specific services
builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents();
builder.Services.AddMudServices();
builder.Services.AddSignalR();

// Register ViewModels
builder.Services.AddScoped<BecomeCitizenViewModel>();
builder.Services.AddScoped<ValidateCitizenViewModel>();
// ... other ViewModels

var app = builder.Build();

// Apply migrations in development
if (app.Environment.IsDevelopment())
{
    await ApplyMigrationsAsync(app.Services);
}

// Middleware pipeline
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
    app.UseHsts();
}

app.UseStaticFiles();
app.UseRouting();
app.UseAuthentication();
app.UseAuthorization();
app.UseMiddleware<TenantMiddleware>();

app.MapRazorComponents<App>()
    .AddInteractiveServerRenderMode();
app.MapHub<ApplicationHub>("/applicationHub");

// Health endpoint
app.MapGet("/healthz", () => "ok");

app.Run();

static async Task ApplyMigrationsAsync(IServiceProvider services)
{
    using var scope = services.CreateScope();

    var citizenshipDb = scope.ServiceProvider
        .GetRequiredService<CitizenshipDbContext>();
    await citizenshipDb.Database.MigrateAsync();

    var cmsDb = scope.ServiceProvider
        .GetRequiredService<CmsDbContext>();
    await cmsDb.Database.MigrateAsync();

    var tenantDb = scope.ServiceProvider
        .GetRequiredService<TenantDbContext>();
    await tenantDb.Database.MigrateAsync();

    var libraryDb = scope.ServiceProvider
        .GetRequiredService<LibraryDbContext>();
    await libraryDb.Database.MigrateAsync();
}
```

**Verification Commands**:
```bash
# Build Web project
dotnet build src/Mamey.Portal.Web

# Should compile without errors
# Expected: Build succeeded. 0 Error(s)

# Run portal
dotnet run --project src/Mamey.Portal.Web --no-build

# Should start without errors
# Expected: Now listening on: http://localhost:5180

# Test health endpoint
curl http://localhost:5180/healthz

# Expected: "ok"
```

**Success Criteria**:
- [ ] Program.cs < 150 lines
- [ ] All domain modules use Extensions.cs
- [ ] Portal starts without errors
- [ ] All endpoints return 200 OK
- [ ] No service registration in Program.cs (moved to Extensions)

**Lines Removed from Program.cs**: ~1450 lines

---

## Success Metrics for Phase 1

**Time Spent**: ___ hours (Target: 8 hours)
**Files Created**: 5 Extensions.cs files
**Files Modified**: 1 (Program.cs)
**Lines Reduced**: ~1450 lines from Program.cs
**Build Status**: ✅ All projects compile
**Portal Status**: ✅ Starts without errors

---

**Continue to Phase 2**: [Domain Layer Implementation](#phase-2)

---

*This is a comprehensive tracker. Update checkboxes as you complete tasks. Track time spent vs. estimated time. Document any blockers or deviations from the plan.*

---

<a name="phase-2"></a>
## Phase 2: Domain Layer Implementation (Started)

**Progress**: ✅ Citizenship | ✅ CMS | ✅ Tenant | ✅ Library | ✅ Auth

**Completed Items**:
- [x] Citizenship domain entities, value objects, repositories, events, exceptions
- [x] CMS domain entities, value objects, repositories, events
- [x] Tenant domain entities, value objects, repositories, events
- [x] Library domain entities, value objects, repositories, events
- [x] Auth domain entities, value objects, repositories

**Notes**:
- Initial scaffolding added for all modules; mapping and behavior refinements will follow in Phase 3+.


---

<a name="phase-3"></a>
## Phase 3: Application Layer Separation (Completed)

**Progress**: ✅ Citizenship | ✅ CMS | ✅ Tenant | ✅ Library

**Completed Items**:
- [x] Move `ApplicationFormPdfGenerator` to Application layer
- [x] Move `BarcodeScanningService` to Application layer
- [x] Move `ApplicationStatusService` to Application layer (store abstraction added)
- [x] Move `ApplicationWorkflowService` to Application layer (store abstraction added)
- [x] Move `CitizenshipApplicationService` to Application layer (submission store abstraction added)
- [x] Move `PaymentPlanService` to Application layer (payment plan store abstraction added)
- [x] Move `CitizenshipStatusService` to Application layer (status store abstraction added)
- [x] Move `StatusProgressionService` to Application layer (progression store abstraction added)
- [x] Move `PublicDocumentValidationService` to Application layer (document validation store abstraction added)
- [x] Move `CitizenshipCitizenPortalService` to Application layer (store abstraction added)
- [x] Move `CitizenshipBackofficeService` to Application layer (store abstraction added)
- [x] Move `CmsContentService` to Application layer (store abstraction added)
- [x] Move `CmsPageService` to Application layer (store abstraction added)
- [x] Move `CmsHtmlSanitizer` to Application layer
- [x] Move `TenantOnboardingService` to Application layer (store abstraction added)
- [x] Move `TenantUserMappingService` to Application layer (store abstraction added)
- [x] Move `LibraryService` to Application layer (store abstraction added)

---

<a name="phase-4"></a>
## Phase 4: Repository Pattern (Completed)

**Progress**: ✅ Citizenship | ✅ CMS | ✅ Tenant | ✅ Library

**Completed Items**:
- [x] Implement Citizenship repositories (Applications, uploads, issued docs, intake reviews, payment plans, statuses, progression)
- [x] Add Citizenship row ↔ domain mapping extensions
- [x] Implement CMS repositories (news + pages)
- [x] Implement Tenant repositories (tenants, settings, naming, templates, mappings, invites)
- [x] Implement Library repositories (items)
- [x] Register repository implementations in module Extensions

---

<a name="phase-5"></a>
## Phase 5: EF Core Migrations (Completed)

**Progress**: ✅ Citizenship | ✅ CMS | ✅ Tenant | ✅ Library

**Completed Items**:
- [x] Create EF Core migrations for Citizenship
- [x] Create EF Core migrations for CMS
- [x] Create EF Core migrations for Tenant
- [x] Create EF Core migrations for Library
- [x] Replace dev inline SQL with `Database.Migrate()` bootstrap

---

<a name="phase-6"></a>
## Phase 6: Mamey.Word Integration (Completed)

**Progress**: ✅ Citizenship DOCX generation

**Completed Items**:
- [x] Register Mamey.Word in Citizenship infrastructure
- [x] Add DOCX templates as embedded resources
- [x] Switch certificate/passport/id-card/vehicle-tag/travel-id issuance to DOCX with HTML fallback

---

<a name="phase-7"></a>
## Phase 7: Configuration (Completed)

**Progress**: ✅ InternalsVisibleTo declarations

**Completed Items**:
- [x] Add InternalsVisibleTo declarations for all module infrastructure projects

---

<a name="verification"></a>
## Verification (Completed)

**Completed Items**:
- [x] Run full test suite (`dotnet test Mamey.Government.Portal.sln`) — passed with existing warnings
- [x] Verify portal startup + health endpoint (`dotnet run --project src/Mamey.Portal.Web --no-build --urls http://localhost:5180` and `GET /healthz` returns `ok`)
