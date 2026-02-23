using DeFi.Core.Models;

namespace DeFi.Core.Interfaces;

public interface IStakingService
{
    Task<IEnumerable<StakingPool>> GetPoolsAsync();
    Task<StakingPool?> GetPoolAsync(string id);
    Task<StakingPosition> StakeAsync(string userId, string poolId, decimal amount);
    Task<StakingPosition> UnstakeAsync(string positionId);
    Task<IEnumerable<StakingPosition>> GetUserPositionsAsync(string userId);
    Task<decimal> ClaimRewardsAsync(string positionId);
}

public interface ILiquidityService
{
    Task<IEnumerable<LiquidityPool>> GetPoolsAsync();
    Task<LiquidityPool?> GetPoolAsync(string id);
    Task<decimal> AddLiquidityAsync(string userId, string poolId, decimal amount0, decimal amount1);
    Task<(decimal, decimal)> RemoveLiquidityAsync(string userId, string poolId, decimal lpTokens);
    Task<decimal> SwapAsync(string userId, string poolId, string tokenIn, decimal amountIn);
    Task<DeFiStats> GetStatsAsync();
}
