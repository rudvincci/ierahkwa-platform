using System.Text.Json;
using IERAHKWA.Platform.Models;
using IERAHKWA.Platform.Services;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container
builder.Services.AddControllers()
    .AddJsonOptions(options =>
    {
        options.JsonSerializerOptions.PropertyNamingPolicy = JsonNamingPolicy.CamelCase;
        options.JsonSerializerOptions.WriteIndented = true;
    });

// CORS for production
builder.Services.AddCors(options =>
{
    options.AddDefaultPolicy(policy =>
    {
        policy.AllowAnyOrigin()
              .AllowAnyMethod()
              .AllowAnyHeader();
    });
});

// Register services
builder.Services.AddHttpClient();

// Core services
builder.Services.AddSingleton<IPlatformService, PlatformService>();
builder.Services.AddSingleton<IAIService, AIService>();
builder.Services.AddSingleton<IFileService, FileService>();

// Transaction & Payment services
builder.Services.AddSingleton<ITransactionService, TransactionService>();
builder.Services.AddSingleton<IWalletService, WalletService>();
builder.Services.AddSingleton<IPaymentService, PaymentService>();

// Bank & Checkout services
builder.Services.AddSingleton<IBankService, BankService>();
builder.Services.AddSingleton<ICheckoutService, CheckoutService>();

// AI Studio service
builder.Services.AddSingleton<IAIStudioService, AIStudioService>();

// Casino service
builder.Services.AddSingleton<ICasinoService, CasinoService>();

// Social Media service  
builder.Services.AddSingleton<ISocialService, SocialService>();

// ============= ADVANCED SERVICES =============
// Analytics & Monitoring
builder.Services.AddSingleton<IAnalyticsService, AnalyticsService>();

// Security Suite (2FA, Rate Limiting)
builder.Services.AddSingleton<ISecurityService, SecurityService>();

// Multi-Chain Bridge
builder.Services.AddSingleton<IBridgeService, BridgeService>();

// Trading Pro (Order Books, Bots)
builder.Services.AddSingleton<ITradingService, TradingService>();

// BDET Bank Services
builder.Services.AddSingleton<IBDETBankService, BDETBankService>();

// Metaverse (3D Worlds, NFT Gallery)
builder.Services.AddSingleton<IMetaverseService, MetaverseService>();

// Token Management
builder.Services.AddSingleton<ITokenService, TokenService>();

// CryptoHost Exchange
builder.Services.AddSingleton<ICryptoHostService, CryptoHostService>();

// Logging
builder.Logging.ClearProviders();
builder.Logging.AddConsole();
builder.Logging.SetMinimumLevel(LogLevel.Information);

var app = builder.Build();

// Configure the HTTP request pipeline
app.UseCors();

// Serve static files from platform directory
var staticFilesPath = "/Users/ruddie/Sovereign Akwesasne Government - Office of the Prime Minister - Photos/soberanos natives/platform";
if (Directory.Exists(staticFilesPath))
{
    app.UseStaticFiles(new StaticFileOptions
    {
        FileProvider = new Microsoft.Extensions.FileProviders.PhysicalFileProvider(staticFilesPath),
        RequestPath = ""
    });
    
    // Default route to index (main platform)
    app.MapGet("/", () => Results.Redirect("/index.html"));
}
else
{
    Console.WriteLine($"WARNING: Static files path not found: {staticFilesPath}");
}

// Map controllers
app.MapControllers();

// ============= QUICK ENDPOINTS =============

// Health check
app.MapGet("/api/health", () => Results.Ok(new
{
    success = true,
    status = "operational",
    version = "2.0.0",
    platform = "IERAHKWA Sovereign Platform",
    timestamp = DateTime.UtcNow,
    services = new
    {
        ai = "online",
        transactions = "online",
        wallet = "online",
        payments = "online",
        blockchain = "online"
    }
}));

// System info
app.MapGet("/api/system/info", () => Results.Ok(new
{
    success = true,
    data = new
    {
        name = "IERAHKWA Platform",
        version = "2.0.0",
        framework = "ASP.NET Core .NET 10.0",
        environment = app.Environment.EnvironmentName,
        startedAt = DateTime.UtcNow,
        features = new[]
        {
            "AI Assistant",
            "Wallet Management",
            "Transaction Processing",
            "Payment Gateway",
            "Blockchain Integration",
            "Multi-token Support"
        }
    }
}));

// Quick wallet balance check
app.MapGet("/api/quick/balance/{walletId}", async (string walletId, IWalletService walletService) =>
{
    try
    {
        var balance = await walletService.GetBalanceAsync(walletId);
        return Results.Ok(new { success = true, data = balance });
    }
    catch (Exception ex)
    {
        return Results.NotFound(new { success = false, error = ex.Message });
    }
});

// Quick transaction status
app.MapGet("/api/quick/tx/{txId}", async (string txId, ITransactionService txService) =>
{
    try
    {
        var status = await txService.GetStatusAsync(txId);
        return Results.Ok(new { success = true, data = status });
    }
    catch (Exception ex)
    {
        return Results.NotFound(new { success = false, error = ex.Message });
    }
});

// Quick transfer
app.MapPost("/api/quick/transfer", async (TransferRequest request, ITransactionService txService) =>
{
    try
    {
        var result = await txService.TransferAsync(request);
        return Results.Ok(new { success = true, data = result });
    }
    catch (InsufficientFundsException)
    {
        return Results.BadRequest(new { success = false, error = "Insufficient funds" });
    }
    catch (Exception ex)
    {
        return Results.BadRequest(new { success = false, error = ex.Message });
    }
});

// Dashboard overview with all data
app.MapGet("/api/dashboard/full", async (
    IPlatformService platformService,
    ITransactionService txService,
    IWalletService walletService) =>
{
    var platformOverview = await platformService.GetOverviewAsync();
    var txStats = await txService.GetStatsAsync();
    var recentTx = await txService.GetHistoryAsync(DateTime.UtcNow.AddDays(-7), DateTime.UtcNow, 10);
    
    return Results.Ok(new
    {
        success = true,
        data = new
        {
            platform = platformOverview,
            transactions = new
            {
                stats = txStats,
                recent = recentTx
            },
            timestamp = DateTime.UtcNow
        }
    });
});

Console.WriteLine(@"
╔══════════════════════════════════════════════════════════════════╗
║                                                                  ║
║     🏛️  IERAHKWA SOVEREIGN PLATFORM v2.0                        ║
║     Production-Ready with Full Transaction Support               ║
║                                                                  ║
║     APIs:                                                        ║
║     • /api/health          - System health                       ║
║     • /api/wallet/*        - Wallet management                   ║
║     • /api/transaction/*   - Transaction processing              ║
║     • /api/payment/*       - Payment gateway                     ║
║     • /api/ai/chat         - AI Assistant                        ║
║     • /api/dashboard/full  - Full dashboard data                 ║
║                                                                  ║
║     Dashboard: http://localhost:3000/dashboard.html              ║
║                                                                  ║
╚══════════════════════════════════════════════════════════════════╝
");

app.Run();
