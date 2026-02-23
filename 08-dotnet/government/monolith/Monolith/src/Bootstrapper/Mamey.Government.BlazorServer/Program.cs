using System.Reflection;
using Mamey;
using Mamey.Government.BlazorServer;
using Mamey.Government.BlazorServer.Auth;
using Mamey.Government.BlazorServer.Seeding;
using Mamey.Government.UI;
using Mamey.MicroMonolith.Infrastructure;
using Mamey.MicroMonolith.Infrastructure.Logging;
using Mamey.MicroMonolith.Infrastructure.Modules;
using Mamey.Persistence.MongoDB;
using Mamey.Persistence.Redis;
using Mamey.Secrets.Vault;
using MongoDB.Bson;
using MongoDB.Bson.Serialization;
using MongoDB.Bson.Serialization.Serializers;
using MudBlazor.Services;

var builder = WebApplication.CreateBuilder(args);
builder.AddModuleSettings();
// ── Load and register your modules ──────────────────────────────────────────────
var configuration = builder.Configuration;
var moduleNamespace = "Mamey.Government.Modules.";

var assemblies    = ModuleLoader.LoadAssemblies(configuration, moduleNamespace)
    .Where(a => a.GetName().Name!.StartsWith(moduleNamespace)).ToList();
var modules       = ModuleLoader.LoadModules(assemblies);

// Load Core assemblies by getting them from Api assembly references
// Core assemblies contain handlers and need to be included in the scan BEFORE AddModularInfrastructure
// var coreAssemblyNames = assemblies
//     .SelectMany(a => a.GetReferencedAssemblies())
//     .Where(refAsm => refAsm.Name != null && 
//                      refAsm.Name.StartsWith(moduleNamespace) && 
//                      refAsm.Name.EndsWith(".Core"))
//     .DistinctBy(a => a.FullName)
//     .ToList();
//
// // Get already loaded assemblies first
// var loadedAssemblies = AppDomain.CurrentDomain.GetAssemblies().ToDictionary(a => a.GetName().FullName);
//
// foreach (var asmName in coreAssemblyNames)
// {
//     try
//     {
//         Assembly? coreAsm = null;
//         
//         // Try to get already loaded assembly first
//         if (loadedAssemblies.TryGetValue(asmName.FullName, out var loaded))
//         {
//             coreAsm = loaded;
//         }
//         else
//         {
//             // Try to load it
//             coreAsm = Assembly.Load(asmName);
//         }
//         
//         if (coreAsm != null && !assemblies.Any(a => a.FullName == coreAsm.FullName))
//             assemblies.Add(coreAsm);
//     }
//     catch
//     {
//         // Assembly might not be found - ignore
//     }
// }

// Now add infrastructure (this will scan assemblies for handlers)
foreach (var module in modules)
    module.Register(builder.Services);

builder.Services
    .AddMamey(configuration: builder.Configuration,
        hostEnvironment: builder.Environment)
    .AddModularInfrastructure(assemblies, modules);


// ── UI + Blazor Server setup ──────────────────────────────────────────────────
builder.Services.AddMudServices();

builder.Services
    .AddRazorComponents()                   // minimal hosting model for Blazor Server
    .AddInteractiveServerComponents();      // enable interactive SignalR circuit

builder.Services.AddCascadingAuthenticationState();  // for <CascadingAuthenticationState>
builder.Services.AddDatabaseDeveloperPageExceptionFilter();
builder.Services.AddControllers();                   // API controllers
builder.Services.AddScoped<ICache, RedisCache>();
builder.Services.AddHealthChecks();                  // Health checks for Docker

// ── Government UI Services (HttpClient-based) ────────────────────────────────
// Use a fixed base URL since Blazor Server runs over SignalR without HTTP context
var baseUrl = builder.Configuration["ApiBaseUrl"] 
    ?? (builder.Environment.IsDevelopment() ? "https://localhost:7295" : "https://localhost:7295");
builder.Services.AddGovernmentUIServices(baseUrl);

// ── Authentication (Authentik OIDC) ───────────────────────────────────────────
var useOidc = builder.Services.AddGovernmentAuthentication(builder.Configuration, builder.Environment);

// ── Mamey infra + your custom extensions ───────────────────────────────────────
// Configure Guid representation for MongoDB
// This must be done before creating any MongoClient instances
// Register Guid serializer with Standard representation to avoid serialization errors
// Use try-catch to handle cases where serializer is already registered (e.g., in tests)
try
{
    BsonSerializer.RegisterSerializer(new GuidSerializer(GuidRepresentation.Standard));
}
catch (BsonSerializationException)
{
    // Serializer already registered, ignore
}

// ── Data Seeding (for development) ────────────────────────────────────────────
// builder.Services.AddDataSeeding();

builder.Host
    // .ConfigureModules()  // wires up module‑specific host settings
    .UseLogging()        // Mamey logging extension
    .UseVault();         // Mamey Vault integration

// ── Build the app ──────────────────────────────────────────────────────────────
var app = builder.Build();

// ── Error pages & HSTS ────────────────────────────────────────────────────────
if (app.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage();  // Show detailed errors in development
    app.UseMigrationsEndPoint();
}
else
{
    app.UseExceptionHandler("/Error", createScopeForErrors: true);
    app.UseHsts();
}

// ── Common middleware ─────────────────────────────────────────────────────────
app.UseHttpsRedirection();
app.UseStaticFiles();  // Standard static files middleware

await app.UseModularInfrastructure();

// ── Routing middleware (required before UseAntiforgery) ────────────────────────
app.UseRouting();

// ── Authentication & Authorization ─────────────────────────────────────────────
app.UseAuthentication();
app.UseAuthorization();

// ── Antiforgery middleware (must be AFTER routing and auth, BEFORE endpoints) ──
app.UseAntiforgery();

// ── Endpoint registration ─────────────────────────────────────────────────────
// Map Blazor (uses endpoint routing)
app.MapRazorComponents<App>()
   .AddInteractiveServerRenderMode();

app.MapHealthChecks("/health");  // Health check endpoint for Docker
app.MapControllers();            // API controllers

// ── Module configuration ──────────────────────────────────────────────────────
app.ConfigureApplication(assemblies, modules);
app.UseMamey();

// Government Monolith configured

// ── Data Seeding (development) ────────────────────────────────────────────────
// await app.UseDevelopmentSeedingAsync(app.Environment, configuration);

// ── Cleanup and run ────────────────────────────────────────────────────────────
assemblies.Clear();
modules.Clear();

await app.RunAsync();
