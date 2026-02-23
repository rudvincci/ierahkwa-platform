namespace DeFi.Core.Models;

public class StakingPool
{
    public string Id { get; set; } = Guid.NewGuid().ToString();
    public string Name { get; set; } = string.Empty;
    public string TokenSymbol { get; set; } = string.Empty;
    public decimal APY { get; set; }
    public decimal TotalStaked { get; set; }
    public decimal MinStake { get; set; }
    public int LockPeriodDays { get; set; }
    public bool IsActive { get; set; } = true;
}

public class StakingPosition
{
    public string Id { get; set; } = Guid.NewGuid().ToString();
    public string UserId { get; set; } = string.Empty;
    public string PoolId { get; set; } = string.Empty;
    public decimal Amount { get; set; }
    public decimal Rewards { get; set; }
    public DateTime StakedAt { get; set; } = DateTime.UtcNow;
    public DateTime? UnlocksAt { get; set; }
    public PositionStatus Status { get; set; } = PositionStatus.Active;
}

public enum PositionStatus { Active, Pending, Unlocked, Withdrawn }

public class LiquidityPool
{
    public string Id { get; set; } = Guid.NewGuid().ToString();
    public string Token0 { get; set; } = string.Empty;
    public string Token1 { get; set; } = string.Empty;
    public decimal Reserve0 { get; set; }
    public decimal Reserve1 { get; set; }
    public decimal TotalLiquidity { get; set; }
    public decimal APR { get; set; }
    public decimal Volume24h { get; set; }
    public decimal Fee { get; set; } = 0.003m;
}

public class DeFiStats
{
    public decimal TotalValueLocked { get; set; }
    public int ActivePools { get; set; }
    public decimal TotalRewardsDistributed { get; set; }
    public int UniqueStakers { get; set; }
}
