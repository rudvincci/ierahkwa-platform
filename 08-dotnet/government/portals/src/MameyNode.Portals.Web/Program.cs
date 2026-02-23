using MameyNode.Portals.Web.Components;
using MameyNode.Portals.Web.Infrastructure;
using MameyNode.Portals.Infrastructure;
using Mamey.Auth.Multi;
using Mamey;
using MameyNode.Portals.Mocks;
using MameyNode.Portals.Mocks.Interfaces;
using Mamey.Blockchain.Node;
using Mamey.Blockchain.Banking;
using Mamey.Blockchain.Government;
using MameyNode.Portals.Contracts;
using Mamey.FWID.Identities.BlazorWasm;
using Mamey.FWID.DIDs.BlazorWasm;
using Mamey.FWID.Credentials.BlazorWasm;
using Mamey.FWID.ZKPs.BlazorWasm;
using Mamey.FWID.AccessControls.BlazorWasm;
using Mamey.FutureWampum.Blazor;
using MudBlazor.Services;
using Microsoft.AspNetCore.Components.Authorization;
using Mamey.BlazorWasm.Routing;

var builder = WebApplication.CreateBuilder(args);

// Add Mamey services
builder.Services.AddMamey();
    // .AddMultiAuth()  // Commented out to avoid DI errors with missing ISecurityProvider during development without full auth
    // Removed .AddIdentitiesBlazorWasm() - Authentication handled via FWID API Gateway, not direct BlazorWasm registration

// Add HttpClient for FWID API Gateway
var fwidGatewayUrl = builder.Configuration.GetValue<string>("FWID:ApiGatewayUrl") ?? "http://localhost:5001"; // Default or from config
builder.Services.AddHttpClient("FWIDGateway", client =>
{
    client.BaseAddress = new Uri(fwidGatewayUrl);
    client.DefaultRequestHeaders.Add("Accept", "application/json");
});

// Add FutureWampumID BlazorWasm services
builder.Services.AddFutureWampumBlazor(builder.Configuration);
builder.Services.AddIdentitiesBlazorWasm(builder.Configuration);
builder.Services.AddDIDsBlazorWasm(builder.Configuration);
builder.Services.AddCredentialsBlazorWasm(builder.Configuration);
builder.Services.AddZKPsBlazorWasm(builder.Configuration);
builder.Services.AddAccessControlsBlazorWasm(builder.Configuration);

// Register Application Route Services (based on TDD applications)
builder.Services.AddScoped<IRouteService, MameyNode.Portals.Web.Components.Areas.BIIS.BIISRouteService>();
builder.Services.AddScoped<IRouteService, MameyNode.Portals.Web.Components.Areas.SICB.SICBRouteService>();
builder.Services.AddScoped<IRouteService, MameyNode.Portals.Web.Components.Areas.FBDETB.FBDETBRouteService>();
builder.Services.AddScoped<IRouteService, MameyNode.Portals.Web.Components.Areas.Government.GovernmentRouteService>();
builder.Services.AddScoped<IRouteService, MameyNode.Portals.Web.Components.Areas.FutureWampum.FutureWampumGov.FutureWampumGovRouteService>();
builder.Services.AddScoped<IRouteService, MameyNode.Portals.Web.Components.Areas.FutureWampum.FutureWampumId.FutureWampumIdRouteService>();
builder.Services.AddScoped<IRouteService, MameyNode.Portals.Web.Components.Areas.FutureWampum.FutureWampumPay.FutureWampumPayRouteService>();
builder.Services.AddScoped<IRouteService, MameyNode.Portals.Web.Components.Areas.FutureWampum.FutureWampumMerchant.FutureWampumMerchantRouteService>();
builder.Services.AddScoped<IRouteService, MameyNode.Portals.Web.Components.Areas.FutureWampum.FutureWampumLedger.FutureWampumLedgerRouteService>();
builder.Services.AddScoped<IRouteService, MameyNode.Portals.Web.Components.Areas.FutureWampum.FutureWampumX.FutureWampumXRouteService>();
builder.Services.AddScoped<IRouteService, MameyNode.Portals.Web.Components.Areas.Infrastructure.InfrastructureRouteService>();
builder.Services.AddScoped<IRouteService, MameyNode.Portals.Web.Components.Areas.General.GeneralRouteService>();

// Note: All legacy RCL RouteServices removed - all routes now in Web/Components/Areas
// This prevents duplicate route errors

// Configure Portal Settings
builder.Services.Configure<Dictionary<string, PortalSettings>>(builder.Configuration.GetSection("portals"));

// Add services to the container.
builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents();

// Add MudBlazor services
builder.Services.AddMudServices();

// Blockchain Clients Configuration
var nodeUrl = builder.Configuration.GetValue<string>("MameyNode:Url") ?? "http://localhost:5000";
var useMocks = builder.Configuration.GetValue<bool>("UseMocks", true);

if (useMocks)
{
    builder.Services.AddSingleton<IMameyNodeClient, MockMameyNodeClient>();
    builder.Services.AddSingleton<IMameyBankingClient, MockMameyBankingClient>();
    builder.Services.AddSingleton<IMameyGovernmentClient, MockMameyGovernmentClient>();
    
    // Register all mock portal clients
    builder.Services.AddSingleton<IMameyPaymentsClient, MockMameyPaymentsClient>();
    builder.Services.AddSingleton<IMameyLendingClient, MockMameyLendingClient>();
    builder.Services.AddSingleton<IMameyDexClient, MockMameyDexClient>();
    builder.Services.AddSingleton<IMameyCryptoExchangeClient, MockMameyCryptoExchangeClient>();
    builder.Services.AddSingleton<IMameySmartContractsClient, MockMameySmartContractsClient>();
    builder.Services.AddSingleton<IMameyAccountAbstractionClient, MockMameyAccountAbstractionClient>();
    builder.Services.AddSingleton<IMameyBridgeClient, MockMameyBridgeClient>();
    builder.Services.AddSingleton<IMameyILPClient, MockMameyILPClient>();
    builder.Services.AddSingleton<IMameyPathfindingClient, MockMameyPathfindingClient>();
    builder.Services.AddSingleton<IMameyTravelRuleClient, MockMameyTravelRuleClient>();
    
    // Register new application-specific mock clients
    builder.Services.AddSingleton<IBIISClient, MockBIISClient>();
    builder.Services.AddSingleton<ISICBClient, MockSICBClient>();
    builder.Services.AddSingleton<IUPGClient, MockUPGClient>();
    builder.Services.AddSingleton<IODLClient, MockODLClient>();
    builder.Services.AddSingleton<IFBDETBClient, MockFBDETBClient>();
    builder.Services.AddSingleton<IGovernmentClient, MockGovernmentClient>();
    builder.Services.AddSingleton<IFutureWampumGovClient, MockFutureWampumGovClient>();
    builder.Services.AddSingleton<IFutureWampumIdClient, MockFutureWampumIdClient>();
    builder.Services.AddSingleton<IFutureWampumLedgerClient, MockFutureWampumLedgerClient>();
    builder.Services.AddSingleton<IFutureWampumMerchantClient, MockFutureWampumMerchantClient>();
    builder.Services.AddSingleton<IFutureWampumXClient, MockFutureWampumXClient>();
    builder.Services.AddSingleton<IInfrastructureClient, MockInfrastructureClient>();
    builder.Services.AddSingleton<IFutureWampumPayClient, MockFutureWampumPayClient>();
    builder.Services.AddSingleton<IMameyChannelsClient, MockMameyChannelsClient>();
    builder.Services.AddSingleton<IMameyProgrammableClient, MockMameyProgrammableClient>();
    builder.Services.AddSingleton<IMameyShardingClient, MockMameyShardingClient>();
    builder.Services.AddSingleton<IMameyAdvancedClient, MockMameyAdvancedClient>();
    builder.Services.AddSingleton<IMameyComplianceClient, MockMameyComplianceClient>();
    builder.Services.AddSingleton<IMameyMetricsClient, MockMameyMetricsClient>();
    builder.Services.AddSingleton<IMameyWebhooksClient, MockMameyWebhooksClient>();
    builder.Services.AddSingleton<IMameyCallbacksClient, MockMameyCallbacksClient>();
    builder.Services.AddSingleton<IMameyRBACClient, MockMameyRBACClient>();
    builder.Services.AddSingleton<IMameyTrustLinesClient, MockMameyTrustLinesClient>();
    builder.Services.AddSingleton<IMameyLedgerIntegrationClient, MockMameyLedgerIntegrationClient>();
    builder.Services.AddSingleton<IMameyNodeManagementClient, MockMameyNodeManagementClient>();
    
    // Override AuthenticationStateProvider for development
    builder.Services.AddScoped<AuthenticationStateProvider, MockAuthenticationStateProvider>();
}
else
{
    builder.Services.AddMameyNodeClient(o => o.NodeUrl = nodeUrl);
    builder.Services.AddMameyBankingClient(o => o.NodeUrl = nodeUrl);
    builder.Services.AddMameyGovernmentClient(o => o.NodeUrl = nodeUrl);
}

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error", createScopeForErrors: true);
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();

// Use Portal Infrastructure (includes error handling and correlation ID)
app.UsePortalInfrastructure();

// Use Mamey Multi-Auth
// app.UseMamey();  // Disabled for UI development - will re-enable later
// app.UseMultiAuth();  // Commented out to avoid DI errors with missing ISecurityProvider during development without full auth

// Use Portal Auth Middleware
app.UseMiddleware<PortalAuthMiddleware>();

app.UseAntiforgery();

app.MapStaticAssets();
app.MapRazorComponents<App>()
    .AddInteractiveServerRenderMode()
    .AddAdditionalAssemblies(
        // Portal RCLs - Existing
        typeof(MameyNode.Portals.Banking.BankingRouteService).Assembly,
        typeof(MameyNode.Portals.Citizen.CitizenRouteService).Assembly,
        typeof(MameyNode.Portals.Government.GovernmentRouteService).Assembly,
        typeof(MameyNode.Portals.General.GeneralRouteService).Assembly,
        // Portal RCLs - Financial Services
        typeof(MameyNode.Portals.Payments.PaymentsRouteService).Assembly,
        typeof(MameyNode.Portals.Lending.LendingRouteService).Assembly,
        typeof(MameyNode.Portals.Dex.DexRouteService).Assembly,
        typeof(MameyNode.Portals.CryptoExchange.CryptoExchangeRouteService).Assembly,
        // Portal RCLs - Smart Contracts
        typeof(MameyNode.Portals.SmartContracts.SmartContractsRouteService).Assembly,
        typeof(MameyNode.Portals.AccountAbstraction.AccountAbstractionRouteService).Assembly,
        // Portal RCLs - Integration & Protocol
        typeof(MameyNode.Portals.Bridge.BridgeRouteService).Assembly,
        typeof(MameyNode.Portals.ILP.ILPRouteService).Assembly,
        typeof(MameyNode.Portals.ODL.ODLRouteService).Assembly,
        typeof(MameyNode.Portals.Pathfinding.PathfindingRouteService).Assembly,
        typeof(MameyNode.Portals.TravelRule.TravelRuleRouteService).Assembly,
        typeof(MameyNode.Portals.UPG.UPGRouteService).Assembly,
        // Portal RCLs - Advanced Features
        typeof(MameyNode.Portals.Channels.ChannelsRouteService).Assembly,
        typeof(MameyNode.Portals.Programmable.ProgrammableRouteService).Assembly,
        typeof(MameyNode.Portals.Sharding.ShardingRouteService).Assembly,
        typeof(MameyNode.Portals.Advanced.AdvancedRouteService).Assembly,
        // Portal RCLs - Infrastructure & Support
        typeof(MameyNode.Portals.Compliance.ComplianceRouteService).Assembly,
        typeof(MameyNode.Portals.Metrics.MetricsRouteService).Assembly,
        typeof(MameyNode.Portals.Webhooks.WebhooksRouteService).Assembly,
        typeof(MameyNode.Portals.Callbacks.CallbacksRouteService).Assembly,
        typeof(MameyNode.Portals.RBAC.RBACRouteService).Assembly,
        typeof(MameyNode.Portals.TrustLines.TrustLinesRouteService).Assembly,
        typeof(MameyNode.Portals.LedgerIntegration.LedgerIntegrationRouteService).Assembly,
        typeof(MameyNode.Portals.Node.NodeRouteService).Assembly,
        // FutureWampumID BlazorWasm RCLs
        typeof(Mamey.FutureWampum.Blazor.Configuration.FutureWampumSettings).Assembly,
        typeof(Mamey.FWID.Identities.BlazorWasm.Services.IdentityService).Assembly,
        typeof(Mamey.FWID.DIDs.BlazorWasm.Services.DIDService).Assembly,
        typeof(Mamey.FWID.Credentials.BlazorWasm.Services.CredentialService).Assembly,
        typeof(Mamey.FWID.ZKPs.BlazorWasm.Services.ZKPService).Assembly,
        typeof(Mamey.FWID.AccessControls.BlazorWasm.Services.AccessControlService).Assembly
    );

app.Run();
