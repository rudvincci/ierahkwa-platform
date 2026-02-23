namespace FarmFactory.Core.Models;

/// <summary>
/// Farm Deposit - IERAHKWA FarmFactory
/// User stake (deposit) of staking tokens in a pool. Rewards calculated by share:
/// (amount × time_staked) / total(amount × time_staked) × rewards_distributed.
/// </summary>
public class FarmDeposit
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public Guid PoolId { get; set; }
    /// <summary>User wallet address (blockchain).</summary>
    public string UserWallet { get; set; } = string.Empty;
    public decimal Amount { get; set; }
    public DateTime StakedAt { get; set; } = DateTime.UtcNow;
    public DateTime? UnstakedAt { get; set; }
    /// <summary>Reward tokens already claimed.</summary>
    public decimal ClaimedReward { get; set; }

    public bool IsActive => UnstakedAt == null;
}
