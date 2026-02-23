using TradeX.Core.Interfaces;
using TradeX.Core.Models;
using TradeX.Infrastructure.Services;

var builder = WebApplication.CreateBuilder(args);

// ========================================
// Ierahkwa TradeX Exchange - .NET 10
// Sovereign Government Trading Platform
// ========================================

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Configure Ierahkwa Node
var nodeConfig = new IerahkwaNodeConfig
{
    NodeEndpoint = builder.Configuration["IerahkwaNode:Endpoint"] ?? "https://node.ierahkwa.gov",
    ApiKey = builder.Configuration["IerahkwaNode:ApiKey"] ?? "",
    NetworkId = "ierahkwa-mainnet",
    ChainId = 777777,
    Currency = "IGT"
};

builder.Services.AddSingleton(nodeConfig);

// Register HttpClient for Node Service
builder.Services.AddHttpClient<IIerahkwaNodeService, IerahkwaNodeService>();

// Crypto Wallet (CodeCanyon-style: BTC, ETH, ERC20, BEP20, Matic; non-custodial; admin fee; fiat on-ramp)
var cwSettings = builder.Configuration.GetSection("CryptoWallet").Get<CryptoWalletSettings>() ?? new CryptoWalletSettings();
var fiatOnRamp = builder.Configuration.GetSection("FiatOnRamp").Get<FiatOnRampConfig>() ?? new FiatOnRampConfig();
builder.Services.AddSingleton(cwSettings);
builder.Services.AddSingleton(fiatOnRamp);
builder.Services.AddSingleton<ICryptoWalletConfigService, CryptoWalletConfigService>();

// Register Services
builder.Services.AddSingleton<IWalletService, WalletService>();
builder.Services.AddSingleton<ITradingService, TradingService>();
builder.Services.AddSingleton<IStakingService, StakingService>();

// Add SignalR for real-time updates
builder.Services.AddSignalR();

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
    c.SwaggerEndpoint("/swagger/v1/swagger.json", "Ierahkwa TradeX API v1");
});

if (!app.Environment.IsDevelopment()) app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseCors("AllowAll");
app.UseAuthorization();
// Carpeta Transacciones registrada: Controllers/Transactions → /api/transactions (history, vip, status, deposit, withdraw, transfer)
app.MapControllers();

// Health check endpoint
app.MapGet("/health", () => new
{
    status = "healthy",
    service = "Ierahkwa TradeX Exchange",
    version = "1.0.0",
    node = "Ierahkwa Futurehead Mamey Node",
    timestamp = DateTime.UtcNow
});

// Root endpoint - serve index.html or redirect to swagger
app.MapGet("/", () => Results.Redirect("/index.html"));

Console.WriteLine(@"
╔═══════════════════════════════════════════════════════════════╗
║                                                               ║
║   🏛️  IERAHKWA TRADEX EXCHANGE                                ║
║   Sovereign Government Crypto Trading Platform                ║
║                                                               ║
║   Powered by: Ierahkwa Futurehead Mamey Node                  ║
║   Network: ierahkwa-mainnet (Chain ID: 777777)               ║
║                                                               ║
║   Features:                                                   ║
║   ✅ Spot Trading with Order Matching                        ║
║   ✅ Instant Swap                                            ║
║   ✅ P2P Trading with Escrow                                 ║
║   ✅ Staking Pools                                           ║
║   ✅ IGT Token Support (100+ Government Tokens)             ║
║   ✅ Multi-Chain (ERC20, BEP20, TRC20)                      ║
║                                                               ║
║   Sovereign Government of Ierahkwa Ne Kanienke               ║
║   © 2026 All Rights Reserved                                  ║
║                                                               ║
╚═══════════════════════════════════════════════════════════════╝
");

app.Run();
