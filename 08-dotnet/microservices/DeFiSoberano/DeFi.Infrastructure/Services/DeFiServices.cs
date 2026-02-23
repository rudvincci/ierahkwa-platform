using DeFi.Core.Interfaces;
using DeFi.Core.Models;

namespace DeFi.Infrastructure.Services;

public class StakingService : IStakingService
{
    private static readonly List<StakingPool> _pools = new()
    {
        new() { Id = "POOL-IGT", Name = "IGT Staking", TokenSymbol = "IGT", APY = 12.5m, TotalStaked = 5000000, MinStake = 100, LockPeriodDays = 30 },
        new() { Id = "POOL-ETH", Name = "ETH Staking", TokenSymbol = "WETH", APY = 5.2m, TotalStaked = 1200, MinStake = 0.1m, LockPeriodDays = 90 },
        new() { Id = "POOL-USDT", Name = "USDT Yield", TokenSymbol = "USDT", APY = 8.0m, TotalStaked = 2500000, MinStake = 100, LockPeriodDays = 7 }
    };
    private static readonly List<StakingPosition> _positions = new();

    public Task<IEnumerable<StakingPool>> GetPoolsAsync() => Task.FromResult(_pools.AsEnumerable());
    public Task<StakingPool?> GetPoolAsync(string id) => Task.FromResult(_pools.FirstOrDefault(p => p.Id == id));

    public Task<StakingPosition> StakeAsync(string userId, string poolId, decimal amount)
    {
        var pool = _pools.FirstOrDefault(p => p.Id == poolId);
        var position = new StakingPosition
        {
            UserId = userId,
            PoolId = poolId,
            Amount = amount,
            UnlocksAt = DateTime.UtcNow.AddDays(pool?.LockPeriodDays ?? 30)
        };
        _positions.Add(position);
        if (pool != null) pool.TotalStaked += amount;
        return Task.FromResult(position);
    }

    public Task<StakingPosition> UnstakeAsync(string positionId)
    {
        var pos = _positions.FirstOrDefault(p => p.Id == positionId);
        if (pos != null) pos.Status = PositionStatus.Withdrawn;
        return Task.FromResult(pos ?? new StakingPosition());
    }

    public Task<IEnumerable<StakingPosition>> GetUserPositionsAsync(string userId) =>
        Task.FromResult(_positions.Where(p => p.UserId == userId));

    public Task<decimal> ClaimRewardsAsync(string positionId)
    {
        var pos = _positions.FirstOrDefault(p => p.Id == positionId);
        var rewards = pos?.Rewards ?? 0;
        if (pos != null) pos.Rewards = 0;
        return Task.FromResult(rewards);
    }
}

public class LiquidityService : ILiquidityService
{
    private static readonly List<LiquidityPool> _pools = new()
    {
        new() { Id = "LP-IGT-USDT", Token0 = "IGT", Token1 = "USDT", Reserve0 = 1000000, Reserve1 = 500000, TotalLiquidity = 750000, APR = 45.5m, Volume24h = 125000 },
        new() { Id = "LP-ETH-USDT", Token0 = "WETH", Token1 = "USDT", Reserve0 = 500, Reserve1 = 1500000, TotalLiquidity = 1500000, APR = 25.0m, Volume24h = 850000 }
    };

    public Task<IEnumerable<LiquidityPool>> GetPoolsAsync() => Task.FromResult(_pools.AsEnumerable());
    public Task<LiquidityPool?> GetPoolAsync(string id) => Task.FromResult(_pools.FirstOrDefault(p => p.Id == id));

    public Task<decimal> AddLiquidityAsync(string userId, string poolId, decimal amount0, decimal amount1)
    {
        var pool = _pools.FirstOrDefault(p => p.Id == poolId);
        if (pool != null)
        {
            pool.Reserve0 += amount0;
            pool.Reserve1 += amount1;
            pool.TotalLiquidity += (amount0 + amount1) / 2;
        }
        return Task.FromResult((amount0 + amount1) / 2);
    }

    public Task<(decimal, decimal)> RemoveLiquidityAsync(string userId, string poolId, decimal lpTokens) =>
        Task.FromResult((lpTokens, lpTokens));

    public Task<decimal> SwapAsync(string userId, string poolId, string tokenIn, decimal amountIn)
    {
        var pool = _pools.FirstOrDefault(p => p.Id == poolId);
        var amountOut = amountIn * (1 - (pool?.Fee ?? 0.003m));
        return Task.FromResult(amountOut);
    }

    public Task<DeFiStats> GetStatsAsync() => Task.FromResult(new DeFiStats
    {
        TotalValueLocked = 15000000,
        ActivePools = _pools.Count,
        TotalRewardsDistributed = 500000,
        UniqueStakers = 1234
    });
}
