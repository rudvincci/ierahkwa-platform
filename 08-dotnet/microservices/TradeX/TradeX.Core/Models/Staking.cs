using System;
using System.Collections.Generic;

namespace TradeX.Core.Models;

/// <summary>
/// Staking Pool - Ierahkwa TradeX
/// Earn passive income by locking assets
/// </summary>
public class StakingPool
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public Guid AssetId { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    
    // APY Configuration
    public decimal APY { get; set; } // Annual Percentage Yield
    public int LockPeriodDays { get; set; } // Lock duration
    
    // Limits
    public decimal MinStakeAmount { get; set; }
    public decimal MaxStakeAmount { get; set; }
    public decimal TotalPoolSize { get; set; }
    public decimal CurrentStaked { get; set; }
    public decimal AvailableCapacity => TotalPoolSize - CurrentStaked;
    
    // Status
    public bool IsActive { get; set; } = true;
    public bool EarlyWithdrawalAllowed { get; set; }
    public decimal EarlyWithdrawalPenalty { get; set; } = 0.1m; // 10%
    
    public DateTime StartDate { get; set; }
    public DateTime? EndDate { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    
    public virtual Asset Asset { get; set; } = null!;
    public virtual ICollection<Stake> Stakes { get; set; } = new List<Stake>();
}

/// <summary>
/// User Stake Record
/// </summary>
public class Stake
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public Guid UserId { get; set; }
    public Guid StakingPoolId { get; set; }
    
    public decimal Amount { get; set; }
    public decimal EstimatedReward { get; set; }
    public decimal EarnedReward { get; set; }
    public decimal ClaimedReward { get; set; }
    
    public StakeStatus Status { get; set; } = StakeStatus.Active;
    
    public DateTime StakedAt { get; set; } = DateTime.UtcNow;
    public DateTime UnlockDate { get; set; }
    public DateTime? UnstakedAt { get; set; }
    public DateTime? LastRewardAt { get; set; }
    
    public virtual User User { get; set; } = null!;
    public virtual StakingPool StakingPool { get; set; } = null!;
}

public enum StakeStatus
{
    Active,
    Completed,
    EarlyWithdrawn,
    Cancelled
}
