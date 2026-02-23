using Microsoft.AspNetCore.Mvc;
using NET10.Core.Interfaces;

namespace NET10.API.Controllers;

/// <summary>
/// Dashboard Controller - Real-time statistics and metrics
/// </summary>
[ApiController]
[Route("api/[controller]")]
[Produces("application/json")]
public class DashboardController : ControllerBase
{
    private readonly ITokenService _tokenService;
    private readonly IPoolService _poolService;
    private readonly ISwapService _swapService;
    private readonly IFarmService _farmService;
    
    public DashboardController(
        ITokenService tokenService,
        IPoolService poolService,
        ISwapService swapService,
        IFarmService farmService)
    {
        _tokenService = tokenService;
        _poolService = poolService;
        _swapService = swapService;
        _farmService = farmService;
    }
    
    /// <summary>
    /// Get complete dashboard data
    /// </summary>
    [HttpGet]
    public async Task<ActionResult<DashboardData>> GetDashboard()
    {
        var tokens = await _tokenService.GetAllTokensAsync();
        var pools = await _poolService.GetAllPoolsAsync();
        var farms = await _farmService.GetAllFarmsAsync();
        
        var dashboard = new DashboardData
        {
            GeneratedAt = DateTime.UtcNow,
            
            Overview = new OverviewStats
            {
                TotalValueLocked = pools.Sum(p => p.TotalLiquidity),
                TotalTokens = tokens.Count,
                TotalPools = pools.Count,
                TotalFarms = farms.Count,
                ActiveUsers = 1247,
                TransactionsToday = 3456,
                Volume24h = 1_250_000m
            },
            
            TopTokens = tokens
                .OrderByDescending(t => t.MarketCap)
                .Take(10)
                .Select(t => new TokenSummary
                {
                    Symbol = t.Symbol,
                    Name = t.Name,
                    Price = t.Price,
                    Change24h = t.PriceChange24h,
                    Volume24h = t.Volume24h,
                    MarketCap = t.MarketCap
                })
                .ToList(),
            
            TopPools = pools
                .OrderByDescending(p => p.TotalLiquidity)
                .Take(5)
                .Select(p => new PoolSummary
                {
                    Name = p.Name,
                    Token0 = p.Token0?.Symbol ?? p.Token0Id,
                    Token1 = p.Token1?.Symbol ?? p.Token1Id,
                    TVL = p.TotalLiquidity,
                    APR = p.APR,
                    Volume24h = p.Volume24h
                })
                .ToList(),
            
            TopFarms = farms
                .OrderByDescending(f => f.APR)
                .Take(5)
                .Select(f => new FarmSummary
                {
                    Name = f.Name,
                    StakeToken = f.StakeToken?.Symbol ?? "",
                    RewardToken = f.RewardToken?.Symbol ?? "",
                    APR = f.APR,
                    TotalStaked = f.TotalStaked
                })
                .ToList(),
            
            RecentActivity = GenerateRecentActivity(),
            
            ChainStats = new ChainStats
            {
                ChainId = 777777,
                ChainName = "Ierahkwa Mainnet",
                BlockNumber = 12_345_678,
                GasPrice = 0.001m,
                TPS = 1500
            }
        };
        
        return Ok(dashboard);
    }
    
    /// <summary>
    /// Get TVL history for charts
    /// </summary>
    [HttpGet("tvl-history")]
    public ActionResult<List<TVLDataPoint>> GetTVLHistory([FromQuery] int days = 30)
    {
        var history = new List<TVLDataPoint>();
        var baseValue = 50_000_000m;
        var random = new Random(42); // Seed for consistency
        
        for (int i = days; i >= 0; i--)
        {
            var date = DateTime.UtcNow.Date.AddDays(-i);
            var variance = (decimal)(random.NextDouble() * 0.1 - 0.05); // ±5%
            baseValue *= (1 + variance);
            
            history.Add(new TVLDataPoint
            {
                Date = date,
                TVL = baseValue,
                Change = variance * 100
            });
        }
        
        return Ok(history);
    }
    
    /// <summary>
    /// Get volume history for charts
    /// </summary>
    [HttpGet("volume-history")]
    public ActionResult<List<VolumeDataPoint>> GetVolumeHistory([FromQuery] int days = 30)
    {
        var history = new List<VolumeDataPoint>();
        var random = new Random(123);
        
        for (int i = days; i >= 0; i--)
        {
            var date = DateTime.UtcNow.Date.AddDays(-i);
            var volume = 500_000m + (decimal)(random.NextDouble() * 2_000_000);
            var swapCount = random.Next(500, 5000);
            
            history.Add(new VolumeDataPoint
            {
                Date = date,
                Volume = volume,
                SwapCount = swapCount,
                UniqueUsers = random.Next(100, 1000)
            });
        }
        
        return Ok(history);
    }
    
    /// <summary>
    /// Get platform statistics
    /// </summary>
    [HttpGet("stats")]
    public ActionResult<PlatformStats> GetStats()
    {
        return Ok(new PlatformStats
        {
            TotalUsers = 15_847,
            ActiveWallets = 8_234,
            TotalTransactions = 1_234_567,
            TotalVolume = 987_654_321m,
            TotalFees = 2_345_678m,
            AverageAPR = 45.5m,
            TopAPR = 250m,
            TokensListed = 103,
            PoolsActive = 25,
            FarmsActive = 12
        });
    }
    
    private List<ActivityItem> GenerateRecentActivity()
    {
        var activities = new List<ActivityItem>
        {
            new() { Type = "swap", Description = "Swapped 1,000 USDT for 0.5 ETH", User = "0x1234...5678", Timestamp = DateTime.UtcNow.AddMinutes(-2), Amount = 1000 },
            new() { Type = "liquidity", Description = "Added liquidity to IGT-PM/USDT", User = "0xabcd...ef01", Timestamp = DateTime.UtcNow.AddMinutes(-5), Amount = 5000 },
            new() { Type = "farm", Description = "Staked 10,000 LP tokens in Farm #1", User = "0x9876...5432", Timestamp = DateTime.UtcNow.AddMinutes(-8), Amount = 10000 },
            new() { Type = "harvest", Description = "Harvested 500 IGT-BDET rewards", User = "0xfedc...ba98", Timestamp = DateTime.UtcNow.AddMinutes(-12), Amount = 500 },
            new() { Type = "swap", Description = "Swapped 5 ETH for 2,500 IGT-PM", User = "0x2468...1357", Timestamp = DateTime.UtcNow.AddMinutes(-15), Amount = 2500 },
            new() { Type = "withdraw", Description = "Withdrew liquidity from AVAX/USDT", User = "0x1357...2468", Timestamp = DateTime.UtcNow.AddMinutes(-20), Amount = 3000 },
            new() { Type = "bridge", Description = "Bridged 10,000 USDT to Polygon", User = "0xaaaa...bbbb", Timestamp = DateTime.UtcNow.AddMinutes(-25), Amount = 10000 },
            new() { Type = "swap", Description = "Swapped 100 MATIC for IGT-MFA", User = "0xcccc...dddd", Timestamp = DateTime.UtcNow.AddMinutes(-30), Amount = 100 }
        };
        
        return activities;
    }
}

// ═══════════════════════════════════════════════════════════════
// DASHBOARD MODELS
// ═══════════════════════════════════════════════════════════════

public class DashboardData
{
    public DateTime GeneratedAt { get; set; }
    public OverviewStats Overview { get; set; } = new();
    public List<TokenSummary> TopTokens { get; set; } = new();
    public List<PoolSummary> TopPools { get; set; } = new();
    public List<FarmSummary> TopFarms { get; set; } = new();
    public List<ActivityItem> RecentActivity { get; set; } = new();
    public ChainStats ChainStats { get; set; } = new();
}

public class OverviewStats
{
    public decimal TotalValueLocked { get; set; }
    public int TotalTokens { get; set; }
    public int TotalPools { get; set; }
    public int TotalFarms { get; set; }
    public int ActiveUsers { get; set; }
    public int TransactionsToday { get; set; }
    public decimal Volume24h { get; set; }
}

public class TokenSummary
{
    public string Symbol { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public decimal Price { get; set; }
    public decimal Change24h { get; set; }
    public decimal Volume24h { get; set; }
    public decimal MarketCap { get; set; }
}

public class PoolSummary
{
    public string Name { get; set; } = string.Empty;
    public string Token0 { get; set; } = string.Empty;
    public string Token1 { get; set; } = string.Empty;
    public decimal TVL { get; set; }
    public decimal APR { get; set; }
    public decimal Volume24h { get; set; }
}

public class FarmSummary
{
    public string Name { get; set; } = string.Empty;
    public string StakeToken { get; set; } = string.Empty;
    public string RewardToken { get; set; } = string.Empty;
    public decimal APR { get; set; }
    public decimal TotalStaked { get; set; }
}

public class ActivityItem
{
    public string Type { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public string User { get; set; } = string.Empty;
    public DateTime Timestamp { get; set; }
    public decimal Amount { get; set; }
}

public class ChainStats
{
    public int ChainId { get; set; }
    public string ChainName { get; set; } = string.Empty;
    public long BlockNumber { get; set; }
    public decimal GasPrice { get; set; }
    public int TPS { get; set; }
}

public class TVLDataPoint
{
    public DateTime Date { get; set; }
    public decimal TVL { get; set; }
    public decimal Change { get; set; }
}

public class VolumeDataPoint
{
    public DateTime Date { get; set; }
    public decimal Volume { get; set; }
    public int SwapCount { get; set; }
    public int UniqueUsers { get; set; }
}

public class PlatformStats
{
    public int TotalUsers { get; set; }
    public int ActiveWallets { get; set; }
    public long TotalTransactions { get; set; }
    public decimal TotalVolume { get; set; }
    public decimal TotalFees { get; set; }
    public decimal AverageAPR { get; set; }
    public decimal TopAPR { get; set; }
    public int TokensListed { get; set; }
    public int PoolsActive { get; set; }
    public int FarmsActive { get; set; }
}
