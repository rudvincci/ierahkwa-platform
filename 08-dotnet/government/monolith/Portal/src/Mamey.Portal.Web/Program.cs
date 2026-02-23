using Mamey;
using Mamey.Portal.Auth.Infrastructure;
using Mamey.Portal.Citizenship.Infrastructure;
using Mamey.Portal.Cms.Infrastructure;
using Mamey.Portal.Library.Infrastructure;
using Mamey.Portal.Tenant.Infrastructure;
using Mamey.Portal.Web;

var builder = WebApplication.CreateBuilder(args);

// Ensure static web assets from referenced Razor Class Libraries / NuGet packages are available
// even when running without a launch profile (e.g., ASPNETCORE_ENVIRONMENT != Development).
builder.WebHost.UseStaticWebAssets();

// Local developer overrides (secrets, machine-specific endpoints, etc).
// These files are gitignored by default.
builder.Configuration
    .AddJsonFile("appsettings.Local.json", optional: true, reloadOnChange: true)
    .AddJsonFile($"appsettings.{builder.Environment.EnvironmentName}.Local.json", optional: true, reloadOnChange: true);

var mameyBuilder = MameyBuilder.Create(builder.Services, builder.Configuration);

mameyBuilder
    .AddCitizenshipInfrastructure()
    .AddCmsInfrastructure()
    .AddTenantInfrastructure()
    .AddLibraryInfrastructure()
    .AddAuthInfrastructure();

var useOidc = builder.AddPortalWebServices();

var app = builder.Build();
app.ConfigurePortalApp(useOidc);

app.Run();
