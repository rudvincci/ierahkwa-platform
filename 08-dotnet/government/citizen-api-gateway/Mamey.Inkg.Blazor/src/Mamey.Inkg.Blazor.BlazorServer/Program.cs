using System.Text;
using Mamey.Government.Blazor;
using Mamey.Government.Blazor.Components;
using Mamey;
using Mamey.Inkg.Blazor.BlazorServer;
using Mamey.Auth.Identity;
using Mamey.Auth.Identity.Configuration;
using Mamey.Persistence.Redis;
using Mamey.Serialization;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Identity;
using Microsoft.IdentityModel.Tokens;
using MudBlazor.Services;
using tusdotnet;
using tusdotnet.Models;
using tusdotnet.Models.Configuration;
using tusdotnet.Stores;

var builder = WebApplication.CreateBuilder(args);

// Add MudBlazor services
builder.Services.AddMudServices();


// Add services to the container.
builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents();


builder.Services.AddCascadingAuthenticationState();

builder.Services.AddDatabaseDeveloperPageExceptionFilter();

builder.Services.AddControllers();
// for calls from UI to API

builder.Services
    .AddTransient<ISerializer, SystemTextJsonSerializer>();
using (var sp = builder.Services.BuildServiceProvider())
{
    var scope = sp.CreateScope();
    var nav = scope.ServiceProvider.GetRequiredService<NavigationManager>();
    builder.Services.AddHttpClient("BlazorHost", client =>
    {
        // No BaseAddress means it defaults to relative paths from the Blazor host
        client.BaseAddress = new Uri(nav.Uri);

        // Default headers for JSON requests
        client.DefaultRequestHeaders.Accept.Clear();
        client.DefaultRequestHeaders.Accept.Add(
            new System.Net.Http.Headers.MediaTypeWithQualityHeaderValue("application/json"));
    });
}
// builder.Services.AddAntiforgery(o =>
// {
//     o.FormFieldName = "__RequestVerificationToken";
//     o.HeaderName    = "RequestVerificationToken"; // useful for fetch/XHR
//     o.Cookie.Name   = "RequestVerificationToken";
// });
var authOptions = builder.Services.GetOptions<AuthOptions>("auth");
builder.Services.AddAuthentication(o =>
    {
        o.DefaultScheme            = "smart";
        o.DefaultAuthenticateScheme = "smart";
        o.DefaultChallengeScheme    = "smart";
    })
    .AddPolicyScheme("smart", "Cookies for pages, Bearer for APIs", o =>
    {
        o.ForwardDefaultSelector = ctx =>
        {
            if (!string.IsNullOrEmpty(ctx.Request.Headers.Authorization))
                return JwtBearerDefaults.AuthenticationScheme;                   // Authorization: Bearer
            if (ctx.Request.Path.StartsWithSegments("/api", StringComparison.OrdinalIgnoreCase))
                return JwtBearerDefaults.AuthenticationScheme;                   // /api => Bearer
            return IdentityConstants.ApplicationScheme;                           // pages/components => Cookie
        };
    })
    .AddCookie(IdentityConstants.ApplicationScheme, o =>
    {
        o.Cookie.Name = "MameyIdentity";
#if DEBUG
        o.Cookie.SameSite     = SameSiteMode.None;
        o.Cookie.SecurePolicy = CookieSecurePolicy.SameAsRequest;
        o.Cookie.Domain       = null; // never set Domain on localhost
#else
        o.Cookie.SameSite     = SameSiteMode.Lax; // or None if truly cross-site
        o.Cookie.SecurePolicy = CookieSecurePolicy.Always;
        // o.Cookie.Domain    = ".yourdomain.tld";
#endif
        o.LoginPath        = "/Account/Login";
        o.LogoutPath       = "/Account/Logout";
        o.AccessDeniedPath = "/Account/AccessDenied";
        o.SlidingExpiration = true;
        o.ExpireTimeSpan    = TimeSpan.FromHours(8);
    })
    .AddJwtBearer(opt =>
    {
        opt.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidIssuer = "auth.mamey.io",
            ValidateAudience = true,
            ValidAudience = "localhost",
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(authOptions.IssuerSigningKey)),
            ValidateLifetime = true,
            ClockSkew = TimeSpan.FromSeconds(30)
        };
    });

builder.Services.AddAuthorizationCore(options =>
{
    options.FallbackPolicy = null;
            // — Permission‑based policies —  
            // e.g. decorate an API endpoint that reads identity data:
            // [Authorize(Policy = "RequireIdentityRead")]
            options.AddPolicy("RequireIdentityRead",
                p => p.RequireClaim(ClaimCategory.Permission, ClaimValues.Permission.IdentityRead));
            // e.g. protect a write operation on identity entities:
            // [Authorize(Policy = "RequireIdentityWrite")]
            options.AddPolicy("RequireIdentityWrite",
                p => p.RequireClaim(ClaimCategory.Permission, ClaimValues.Permission.IdentityWrite));
            // e.g. full administrative tasks:
            // [Authorize(Policy = "RequireIdentityAdmin")]
            options.AddPolicy("RequireIdentityAdmin",
                p => p.RequireClaim(ClaimCategory.Permission, ClaimValues.Permission.IdentityAdmin));
            // e.g. system‑wide controls:
            // [Authorize(Policy = "RequireSystemAll")]
            options.AddPolicy("RequireSystemAll",
                p => p.RequireClaim(ClaimCategory.Permission, ClaimValues.Permission.SystemAll));
            // e.g. background/daemon operations:
            // [Authorize(Policy = "RequireDaemonExecute")]
            options.AddPolicy("RequireDaemonExecute",
                p => p.RequireClaim(ClaimCategory.Permission, ClaimValues.Permission.DaemonExecute));

            // — Role‑based policies —  
            // e.g. only system admins can access this page:
            // [Authorize(Policy = "AdminOnly")]
            options.AddPolicy("AdminOnly", p => p.RequireRole(ClaimValues.Role.Admin));
            // e.g. tenant‑scoped admin actions:
            // [Authorize(Policy = "TenantAdminOnly")]
            options.AddPolicy("TenantAdminOnly", p => p.RequireRole(ClaimValues.Role.TenantAdmin));
            // e.g. support staff dashboard:
            // [Authorize(Policy = "SupportOnly")]
            options.AddPolicy("SupportOnly", p => p.RequireRole(ClaimValues.Role.Support));
            // e.g. regular user pages:
            // [Authorize(Policy = "UserOnly")]
            options.AddPolicy("UserOnly", p => p.RequireRole(ClaimValues.Role.User));
            // e.g. daemon‑only endpoints:
            // [Authorize(Policy = "DaemonOnly")]
            options.AddPolicy("DaemonOnly", p => p.RequireRole(ClaimValues.Role.Daemon));

            // — Feature‑flag policies —  
            // e.g. show beta dashboard if enabled:
            // [Authorize(Policy = "FeatureBetaX")]
            options.AddPolicy("FeatureBetaX",
                p => p.RequireClaim(ClaimCategory.Feature, ClaimValues.Feature.BetaFeatureX));
            // e.g. grant access to new reports preview:
            // [Authorize(Policy = "FeatureReportsPreview")]
            options.AddPolicy("FeatureReportsPreview",
                p => p.RequireClaim(ClaimCategory.Feature, ClaimValues.Feature.ReportsPreview));

            // — Scope policies —  
            // e.g. for OIDC openid scope:
            // [Authorize(Policy = "ScopeOpenId")]
            options.AddPolicy("ScopeOpenId", p => p.RequireClaim(ClaimCategory.Scope, ClaimValues.Scope.OpenId));
            // e.g. for email scope:
            // [Authorize(Policy = "ScopeEmail")]
            options.AddPolicy("ScopeEmail", p => p.RequireClaim(ClaimCategory.Scope, ClaimValues.Scope.Email));

            // — Department policies —  
            // e.g. restrict to HR department:
            // [Authorize(Policy = "DeptHR")]
            options.AddPolicy("DeptHR", p => p.RequireClaim(ClaimCategory.Department, ClaimValues.Department.HR));
            // e.g. finance reports:
            // [Authorize(Policy = "DeptFinance")]
            options.AddPolicy("DeptFinance",
                p => p.RequireClaim(ClaimCategory.Department, ClaimValues.Department.Finance));

            // — System policies —  
            // e.g. maintenance mode pages:
            // [Authorize(Policy = "SystemMaintenance")]
            options.AddPolicy("SystemMaintenance",
                p => p.RequireClaim(ClaimCategory.System, ClaimValues.System.MaintenanceMode));

            // — Preference policies —  
            // e.g. dark‑theme preview:
            // [Authorize(Policy = "PrefThemeDark")]
            options.AddPolicy("PrefThemeDark",
                p => p.RequireClaim(ClaimCategory.Preference, ClaimValues.Preference.ThemeDark));

            // — Group policies —  
            // e.g. only members of ProjectX:
            // [Authorize(Policy = "GroupProjectX")]
            options.AddPolicy("GroupProjectX",
                p => p.RequireClaim(ClaimCategory.Group, ClaimValues.Group.ProjectXTeam));

            // — Resource policies —  
            // e.g. customer creation endpoint:
            // [Authorize(Policy = "ResourceCustomerCreate")]
            options.AddPolicy("ResourceCustomerCreate",
                p => p.RequireClaim(ClaimCategory.Resource, ClaimValues.Resource.CustomerCreate));

            // — Environment policies —  
            // e.g. restrict to production environment:
            // [Authorize(Policy = "EnvProduction")]
            options.AddPolicy("EnvProduction",
                p => p.RequireClaim(ClaimCategory.Environment, ClaimValues.Environment.Production));

            // — Authentication method policies —  
            // e.g. require two‑factor authentication:
            // [Authorize(Policy = "Auth2FA")]
            options.AddPolicy("Auth2FA",
                p => p.RequireClaim(ClaimCategory.Authentication, ClaimValues.Authentication.TwoFactor));

            // — Identity provider policies —  
            // e.g. only Azure AD users:
            // [Authorize(Policy = "IdP_AzureAD")]
            options.AddPolicy("IdP_AzureAD",
                p => p.RequireClaim(ClaimCategory.IdentityProvider, ClaimValues.IdentityProvider.AzureAD));

            // — Locale policies —  
            // e.g. restrict to US English:
            // [Authorize(Policy = "LocaleEnUS")]
            options.AddPolicy("LocaleEnUS", p => p.RequireClaim(ClaimCategory.Locale, ClaimValues.Locale.EnUS));

            // — Time policies —  
            // e.g. ensure issued‑at claim is present:
            // [Authorize(Policy = "TimeIssuedAt")]
            options.AddPolicy("TimeIssuedAt", p => p.RequireClaim(ClaimCategory.Time, ClaimValues.Time.IssuedAt));

         
        });       // Server-side
builder.Services
    .AddMamey(configuration:builder.Configuration, hostEnvironment:builder.Environment)
    .AddRedis()
    .AddFutureBdetBank();
builder.Services.AddSingleton<DebugProbeMiddleware>();
builder.Services.AddServerSideBlazor()
    .AddCircuitOptions(o => o.DetailedErrors = true);

// Optional: extra SignalR details for hub failures
builder.Services.AddSignalR(o => o.EnableDetailedErrors = true);
var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseMigrationsEndPoint();
}
else
{
    app.UseExceptionHandler("/Error", createScopeForErrors: true);
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();

app.MapControllers();
app.UseAuthorization();
app.UseAuthorization();
app.UseAntiforgery();
app.UseStaticFiles();
app.MapStaticAssets();

app.Use(async (ctx, next) =>
{
    var before = ctx.GetEndpoint();
    Console.WriteLine($"[TRACE:BEFORE] {ctx.Request.Method} {ctx.Request.Path} -> endpoint: {before?.DisplayName ?? "(none)"}");
    await next();
    var after = ctx.GetEndpoint();
    var routePattern = (after as Microsoft.AspNetCore.Routing.RouteEndpoint)?.RoutePattern?.RawText ?? "(n/a)";
    Console.WriteLine($"[TRACE:AFTER ] {ctx.Request.Method} {ctx.Request.Path} -> endpoint: {after?.DisplayName ?? "(none)"}, pattern: {routePattern}, status: {ctx.Response?.StatusCode}");
});

app.UseMiddleware<DebugProbeMiddleware>();
// TUS endpoint at /files
app.UseTus(httpContext => new DefaultTusConfiguration
{
    Store = new TusDiskStore(Path.Combine(app.Environment.ContentRootPath, "App_Data", "TusFiles")),
    UrlPath = "/files",
    MaxAllowedUploadSizeInBytesLong = 2L * 1024 * 1024 * 1024,
    Events = new Events
    {
        OnFileCompleteAsync = async ctx =>
        {
            var file = await ctx.GetFileAsync();
            var metadata = await file.GetMetadataAsync(ctx.CancellationToken);

            // Option A: length from the stored content (works with TusDiskStore)
            await using var content = await file.GetContentAsync(ctx.CancellationToken);
            var lengthBytes = content.CanSeek ? content.Length : -1; // -1 if stream doesn't report length

            // TODO: your pipeline (db record, virus scan, move file, etc.)
        }
    }
});
await app.MapFutureBdetBankAsync();
app.MapRazorComponents<App>()
    .AddInteractiveServerRenderMode();



// app.MapBlazorHub();


await app.RunAsync();
