

using FutureheadGroup.Blazor;
using FutureheadGroup.Blazor.Components;
using Mamey;
using Mamey.Casino.BlazorServer;
using Mamey.MicroMonolith.Infrastructure;
using Mamey.MicroMonolith.Infrastructure.Logging;
using Mamey.MicroMonolith.Infrastructure.Modules;
using Mamey.Persistence.Redis;
using Mamey.Secrets.Vault;
using MudBlazor.Services;

var builder = WebApplication.CreateBuilder(args);
builder.AddModuleSettings();
// ── Load and register your modules ──────────────────────────────────────────────
var configuration = builder.Configuration;
var moduleNamespace = "Mamey.ApplicationName.Modules.";

var assemblies    = ModuleLoader.LoadAssemblies(configuration, moduleNamespace)
    .Where(a => a.GetName().Name!.StartsWith(moduleNamespace)).ToList();
var modules       = ModuleLoader.LoadModules(assemblies);

builder.Services.AddModularInfrastructure(assemblies, modules);
foreach (var module in modules)
    module.Register(builder.Services);

// ── UI + Blazor Server setup ──────────────────────────────────────────────────
builder.Services.AddMudServices();

builder.Services
    .AddRazorComponents()                   // minimal hosting model for Blazor Server
    .AddInteractiveServerComponents();      // enable interactive SignalR circuit

builder.Services.AddCascadingAuthenticationState();  // for <CascadingAuthenticationState>
builder.Services.AddDatabaseDeveloperPageExceptionFilter();
builder.Services.AddControllers();                   // API controllers
builder.Services.AddScoped<ICache, RedisCache>();
// ── Mamey infra + your custom extensions ───────────────────────────────────────
builder.Services
    .AddMamey(configuration: builder.Configuration,
              hostEnvironment: builder.Environment)
    .AddRedis()
    // .AddCasinoInfrastructure()
    .AddFutureheadGroup();

builder.Host
    // .ConfigureModules()  // wires up module‑specific host settings
    .UseLogging()        // Mamey logging extension
    .UseVault();         // Mamey Vault integration

// ── Build the app ──────────────────────────────────────────────────────────────
var app = builder.Build();

// ── Error pages & HSTS ────────────────────────────────────────────────────────
if (app.Environment.IsDevelopment())
{
    app.UseMigrationsEndPoint();
}
else
{
    app.UseExceptionHandler("/Error", createScopeForErrors: true);
    app.UseHsts();
}

// ── Common middleware ─────────────────────────────────────────────────────────
app.UseHttpsRedirection();
app.MapStaticAssets();         // serve your static assets from wherever MapStaticAssets points

await app.UseModularInfrastructure();
app.ConfigureApplication(assemblies, modules);

// ── Endpoint registration ─────────────────────────────────────────────────────
app.MapControllers();

app.MapRazorComponents<App>()  // your root component
   .AddInteractiveServerRenderMode();

// await app.UseCasinoInfrastructureAsync();
await app.MapFutureheadGroupAsync();  // your Identity /Account Razor components

// ── Cleanup and run ────────────────────────────────────────────────────────────
assemblies.Clear();
modules.Clear();

await app.RunAsync();
