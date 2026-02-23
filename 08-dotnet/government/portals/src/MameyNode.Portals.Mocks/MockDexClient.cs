using Bogus;
using MameyNode.Portals.Mocks.Models;

namespace MameyNode.Portals.Mocks;

public interface IMameyDexClient
{
    Task<List<PoolInfo>> ListPoolsAsync(string? tokenFilter = null, AMMModel? modelFilter = null, int limit = 50, int offset = 0);
    Task<PoolInfo?> GetPoolAsync(string poolId);
    Task<SwapRoute?> GetBestRouteAsync(string tokenIn, string tokenOut, string amountIn, int maxHops = 3);
}

public class MockMameyDexClient : IMameyDexClient
{
    private readonly Faker _faker = new();
    private readonly List<PoolInfo> _pools = new();

    public MockMameyDexClient()
    {
        InitializeMockData();
    }

    private void InitializeMockData()
    {
        var tokens = new[] { "USDC", "USDT", "ETH", "BTC", "DAI", "WBTC", "LINK", "UNI" };
        
        var poolFaker = new Faker<PoolInfo>()
            .RuleFor(p => p.PoolId, f => $"POOL-{f.Random.AlphaNumeric(10).ToUpper()}")
            .RuleFor(p => p.TokenA, f => f.PickRandom(tokens))
            .RuleFor(p => p.TokenB, (f, p) => f.PickRandom(tokens.Where(t => t != p.TokenA)))
            .RuleFor(p => p.ReserveA, f => f.Finance.Amount(10000, 1000000, 2).ToString("F2"))
            .RuleFor(p => p.ReserveB, (f, p) => 
            {
                var reserveA = decimal.Parse(p.ReserveA);
                // Maintain reasonable ratio
                return f.Finance.Amount(reserveA * 0.8m, reserveA * 1.2m, 2).ToString("F2");
            })
            .RuleFor(p => p.TotalLpSupply, f => f.Finance.Amount(100000, 10000000, 0).ToString("F0"))
            .RuleFor(p => p.Model, f => f.PickRandom<AMMModel>())
            .RuleFor(p => p.FeeRate, f => f.PickRandom("0.001", "0.003", "0.005", "0.01"))
            .RuleFor(p => p.Status, f => f.PickRandom<PoolStatus>())
            .RuleFor(p => p.CreatedAt, f => f.Date.Past(12))
            .RuleFor(p => p.TotalVolume24h, f => f.Finance.Amount(10000, 500000, 2).ToString("F2"))
            .RuleFor(p => p.TotalFees24h, (f, p) => 
            {
                var volume = decimal.Parse(p.TotalVolume24h);
                var feeRate = decimal.Parse(p.FeeRate);
                return (volume * feeRate).ToString("F2");
            });

        _pools.AddRange(poolFaker.Generate(25));
    }

    public Task<List<PoolInfo>> ListPoolsAsync(string? tokenFilter = null, AMMModel? modelFilter = null, int limit = 50, int offset = 0)
    {
        var pools = _pools.AsQueryable();
        
        if (!string.IsNullOrEmpty(tokenFilter))
        {
            pools = pools.Where(p => p.TokenA == tokenFilter || p.TokenB == tokenFilter);
        }
        
        if (modelFilter.HasValue)
        {
            pools = pools.Where(p => p.Model == modelFilter.Value);
        }
        
        return Task.FromResult(pools
            .OrderByDescending(p => p.TotalVolume24h)
            .Skip(offset)
            .Take(limit)
            .ToList());
    }

    public Task<PoolInfo?> GetPoolAsync(string poolId)
    {
        return Task.FromResult(_pools.FirstOrDefault(p => p.PoolId == poolId));
    }

    public Task<SwapRoute?> GetBestRouteAsync(string tokenIn, string tokenOut, string amountIn, int maxHops = 3)
    {
        // Find a route through available pools
        var route = new SwapRoute
        {
            Tokens = new List<string> { tokenIn, tokenOut },
            ExpectedOutput = (decimal.Parse(amountIn) * 0.98m).ToString("F2"), // Mock 2% slippage
            PriceImpact = "0.5"
        };
        
        var intermediatePool = _pools.FirstOrDefault(p => 
            (p.TokenA == tokenIn && p.TokenB == tokenOut) ||
            (p.TokenB == tokenIn && p.TokenA == tokenOut));
        
        if (intermediatePool != null)
        {
            route.Pools = new List<string> { intermediatePool.PoolId };
        }
        
        return Task.FromResult<SwapRoute?>(route);
    }
}
