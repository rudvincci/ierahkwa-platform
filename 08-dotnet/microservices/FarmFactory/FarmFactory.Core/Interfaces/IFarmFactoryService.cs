using FarmFactory.Core.Models;

namespace FarmFactory.Core.Interfaces;

/// <summary>
/// FarmFactory Service Interface - IERAHKWA
/// Assets staking & yield farming on Ethereum, BSC, Polygon, Aurora, xDai, IERAHKWA.
/// Deposit/Withdraw ERC20/BEP20; rewards by (amount × time) share.
/// </summary>
public interface IFarmFactoryService
{
    Task<IEnumerable<FarmPool>> GetPoolsAsync(string? network = null, bool activeOnly = true);
    Task<FarmPool?> GetPoolAsync(Guid poolId);
    Task<FarmPool> CreatePoolAsync(CreatePoolRequest request);

    Task<FarmDeposit> DepositAsync(Guid poolId, string userWallet, decimal amount);
    Task<FarmDeposit> WithdrawAsync(Guid depositId, string userWallet);
    Task<decimal> ClaimAsync(Guid? depositId, string userWallet, Guid? poolId = null);

    Task<IEnumerable<FarmDeposit>> GetDepositsAsync(string? userWallet = null, Guid? poolId = null);
    Task<FarmDeposit?> GetDepositAsync(Guid depositId);
    Task<decimal> GetPendingRewardAsync(Guid depositId);
    Task<decimal> GetPendingRewardForWalletInPoolAsync(string userWallet, Guid poolId);
}

public record CreatePoolRequest(
    string Name,
    string Description,
    string Network,
    string StakingTokenAddress,
    string StakingTokenSymbol,
    int StakingTokenDecimals,
    string RewardTokenAddress,
    string RewardTokenSymbol,
    int RewardTokenDecimals,
    decimal TotalRewardAmount,
    DateTime StartTime,
    DateTime EndTime
);
