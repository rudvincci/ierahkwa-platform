namespace FarmFactory.Core.Models;

/// <summary>
/// Farm Pool - IERAHKWA FarmFactory
/// Staking & yield farming: stake ERC20/BEP20 tokens, earn reward tokens.
/// Supports ETH, BSC, Polygon, Aurora, xDai, IERAHKWA.
/// </summary>
public class FarmPool
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;

    /// <summary>ETH | BSC | POLYGON | AURORA | XDAI | IERAHKWA</summary>
    public string Network { get; set; } = "ETH";

    // Staking token (what users deposit)
    public string StakingTokenAddress { get; set; } = string.Empty;
    public string StakingTokenSymbol { get; set; } = string.Empty;
    public int StakingTokenDecimals { get; set; } = 18;

    // Reward token (what users earn)
    public string RewardTokenAddress { get; set; } = string.Empty;
    public string RewardTokenSymbol { get; set; } = string.Empty;
    public int RewardTokenDecimals { get; set; } = 18;

    /// <summary>Total REWARD tokens to distribute over the farming period.</summary>
    public decimal TotalRewardAmount { get; set; }

    public DateTime StartTime { get; set; }
    public DateTime EndTime { get; set; }
    public bool IsActive { get; set; } = true;
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    /// <summary>Farming period in seconds.</summary>
    public double PeriodSeconds => (EndTime - StartTime).TotalSeconds;

    /// <summary>Reward per second (TotalReward / PeriodSeconds).</summary>
    public decimal RewardPerSecond => PeriodSeconds > 0 ? TotalRewardAmount / (decimal)PeriodSeconds : 0;
}
