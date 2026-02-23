using FarmFactory.Core.Interfaces;
using FarmFactory.Infrastructure.Services;

var builder = WebApplication.CreateBuilder(args);

// ========================================
// IERAHKWA FarmFactory - .NET 10
// Assets staking & yield farming
// Ethereum, BSC, Polygon, Aurora, xDai, IERAHKWA
// ========================================

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new() { Title = "IERAHKWA FarmFactory API", Version = "v1.0", Description = "Staking & yield farming. Deposit/Withdraw ERC20/BEP20. Rewards by (amount × time) share." });
});

builder.Services.AddSingleton<IFarmFactoryService, FarmFactoryService>();

builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll", policy =>
    {
        policy.AllowAnyOrigin().AllowAnyMethod().AllowAnyHeader();
    });
});

var app = builder.Build();

app.UseSwagger();
app.UseSwaggerUI(c =>
{
    c.SwaggerEndpoint("/swagger/v1/swagger.json", "IERAHKWA FarmFactory API v1.0");
    c.DocumentTitle = "IERAHKWA FarmFactory API";
});

if (!app.Environment.IsDevelopment()) app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseCors("AllowAll");
app.UseAuthorization();
app.MapControllers();

app.MapGet("/health", () => new
{
    status = "healthy",
    service = "IERAHKWA FarmFactory",
    version = "1.0",
    platform = "IERAHKWA Futurehead Platform",
    networks = new[] { "ETH", "BSC", "POLYGON", "AURORA", "XDAI", "IERAHKWA" },
    features = new[] { "Deposit/Withdraw", "Staking tokens", "Reward tokens", "Share-based rewards (amount × time)", "Multi-chain" },
    timestamp = DateTime.UtcNow
});

app.MapGet("/", () => Results.Redirect("/index.html"));

Console.WriteLine(@"
╔═══════════════════════════════════════════════════════════════╗
║                                                               ║
║   🌾  IERAHKWA FARMFACTORY                                    ║
║   Assets Staking & Yield Farming                              ║
║                                                               ║
║   Networks: ETH · BSC · Polygon · Aurora · xDai · IERAHKWA   ║
║   Tokens: ERC20 / BEP20                                       ║
║                                                               ║
║   • Stake STAKE tokens → Farm REWARD tokens                  ║
║   • Share = (amount × time) / total(amount × time)           ║
║   • Deposit / Withdraw / Claim anytime                       ║
║                                                               ║
║   🏛️  Sovereign Government of Ierahkwa Ne Kanienke           ║
║   © 2026 All Rights Reserved                                  ║
║                                                               ║
╚═══════════════════════════════════════════════════════════════╝
");

app.Run();
