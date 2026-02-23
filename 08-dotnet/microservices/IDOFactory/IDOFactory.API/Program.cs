using IDOFactory.Core.Interfaces;
using IDOFactory.Infrastructure.Services;

var builder = WebApplication.CreateBuilder(args);

// ========================================
// Ierahkwa IDO Factory - .NET 10
// Sovereign Token Launchpad Platform
// ========================================

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new Microsoft.OpenApi.Models.OpenApiInfo
    {
        Title = "Ierahkwa IDO Factory API",
        Version = "v1",
        Description = "Token Launchpad & Token Locker API for Ierahkwa Sovereign Blockchain",
        Contact = new Microsoft.OpenApi.Models.OpenApiContact
        {
            Name = "Ierahkwa Government",
            Email = "launchpad@ierahkwa.gov",
            Url = new Uri("https://launchpad.ierahkwa.gov")
        }
    });
});

// Register Services
builder.Services.AddSingleton<IIDOService, IDOService>();
builder.Services.AddSingleton<ITokenLockerService, TokenLockerService>();
builder.Services.AddSingleton<IPlatformConfigService, PlatformConfigService>();

// CORS
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll", policy =>
    {
        policy.AllowAnyOrigin()
              .AllowAnyMethod()
              .AllowAnyHeader();
    });
});

var app = builder.Build();

// Configure Pipeline
app.UseSwagger();
app.UseSwaggerUI(c =>
{
    c.SwaggerEndpoint("/swagger/v1/swagger.json", "Ierahkwa IDO Factory API v1");
    c.RoutePrefix = "api";
});

if (!app.Environment.IsDevelopment()) app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseCors("AllowAll");
app.UseAuthorization();
app.MapControllers();

// Health check endpoint
app.MapGet("/health", () => new
{
    status = "healthy",
    service = "Ierahkwa IDO Factory",
    version = "1.0.0",
    node = "Ierahkwa Futurehead Mamey Node",
    chainId = 777777,
    timestamp = DateTime.UtcNow
});

// Root endpoint - serve index.html
app.MapGet("/", () => Results.Redirect("/index.html"));

Console.WriteLine(@"
╔═══════════════════════════════════════════════════════════════╗
║                                                               ║
║   🚀  IERAHKWA IDO FACTORY                                    ║
║   Sovereign Token Launchpad Platform                          ║
║                                                               ║
║   Powered by: Ierahkwa Futurehead Mamey Node                  ║
║   Network: ierahkwa-mainnet (Chain ID: 777777)               ║
║                                                               ║
║   Features:                                                   ║
║   ✅ Create IDO Pools (Public, Whitelist, Tiered, Lottery)  ║
║   ✅ Token Locker & Vesting                                  ║
║   ✅ All EVM Blockchains Supported                          ║
║   ✅ Admin Panel for Deployment & Management                 ║
║   ✅ Crowdsale & Fundraising                                 ║
║   ✅ KYC & Audit Integration                                 ║
║   ✅ Multi-Network Support                                   ║
║                                                               ║
║   API: http://localhost:5097/api                             ║
║   UI:  http://localhost:5097                                 ║
║                                                               ║
║   Sovereign Government of Ierahkwa Ne Kanienke               ║
║   © 2026 All Rights Reserved                                  ║
║                                                               ║
╚═══════════════════════════════════════════════════════════════╝
");

app.Run();
