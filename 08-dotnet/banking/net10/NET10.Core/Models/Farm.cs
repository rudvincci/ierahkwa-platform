namespace NET10.Core.Models;

/// <summary>
/// Yield Farming pool
/// </summary>
public class Farm
{
    public string Id { get; set; } = Guid.NewGuid().ToString();
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    
    // Stake token (usually LP token)
    public string StakeTokenId { get; set; } = string.Empty;
    public Token? StakeToken { get; set; }
    public string StakeTokenSymbol { get; set; } = string.Empty;
    
    // Reward token
    public string RewardTokenId { get; set; } = string.Empty;
    public Token? RewardToken { get; set; }
    public string RewardTokenSymbol { get; set; } = string.Empty;
    
    // Rewards
    public decimal RewardPerBlock { get; set; }
    public decimal RewardPerSecond { get; set; }
    public decimal TotalRewards { get; set; }
    public decimal DistributedRewards { get; set; }
    
    // Pool stats
    public decimal TotalStaked { get; set; }
    public decimal TVL { get; set; }
    public decimal APR { get; set; }
    public decimal APY { get; set; }
    public int StakersCount { get; set; }
    
    // Multiplier for boosted rewards
    public decimal Multiplier { get; set; } = 1.0m;
    public decimal DepositFee { get; set; } = 0; // Optional deposit fee
    public decimal WithdrawFee { get; set; } = 0; // Optional withdraw fee
    
    // Duration
    public DateTime StartTime { get; set; }
    public DateTime EndTime { get; set; }
    public int LockDays { get; set; } = 0; // Optional lock period
    
    // Status
    public FarmStatus Status { get; set; } = FarmStatus.Active;
    public bool IsActive { get; set; } = true;
    public bool IsFeatured { get; set; }
    
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
}

public enum FarmStatus
{
    Upcoming,
    Active,
    Ended,
    Paused
}

/// <summary>
/// User's farming position
/// </summary>
public class FarmPosition
{
    public string Id { get; set; } = Guid.NewGuid().ToString();
    public string UserId { get; set; } = string.Empty;
    public string FarmId { get; set; } = string.Empty;
    public Farm? Farm { get; set; }
    
    // Staked amount
    public decimal StakedAmount { get; set; }
    public decimal StakedUsdValue { get; set; }
    
    // Rewards
    public decimal PendingRewards { get; set; }
    public decimal HarvestedRewards { get; set; }
    public decimal PendingRewardsUsd { get; set; }
    
    // Lock
    public DateTime StakedAt { get; set; } = DateTime.UtcNow;
    public DateTime? UnlockAt { get; set; }
    public bool IsLocked { get; set; }
    
    public DateTime LastHarvestAt { get; set; }
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
}

/// <summary>
/// Farm action request (stake/unstake/harvest)
/// </summary>
public class FarmActionRequest
{
    public string UserId { get; set; } = string.Empty;
    public string FarmId { get; set; } = string.Empty;
    public FarmAction Action { get; set; }
    public decimal Amount { get; set; }
}

public enum FarmAction
{
    Stake,
    Unstake,
    Harvest,
    Compound
}

/// <summary>
/// Farm transaction record
/// </summary>
public class FarmTransaction
{
    public string Id { get; set; } = Guid.NewGuid().ToString();
    public string TxHash { get; set; } = string.Empty;
    public string UserId { get; set; } = string.Empty;
    public string FarmId { get; set; } = string.Empty;
    public FarmAction Action { get; set; }
    public decimal Amount { get; set; }
    public decimal UsdValue { get; set; }
    public decimal Fee { get; set; }
    public TransactionStatus Status { get; set; } = TransactionStatus.Pending;
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime? ConfirmedAt { get; set; }
}
