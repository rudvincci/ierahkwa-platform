using FarmFactory.Core.Interfaces;
using FarmFactory.Core.Models;

namespace FarmFactory.Infrastructure.Services;

/// <summary>
/// FarmFactory Service - IERAHKWA
/// Staking & yield farming: rewards = (amount × time_staked) / total_weight × rewards_distributed.
/// Supports ETH, BSC, Polygon, Aurora, xDai, IERAHKWA. ERC20/BEP20.
/// </summary>
public class FarmFactoryService : IFarmFactoryService
{
    private static readonly List<FarmPool> Pools = new();
    private static readonly List<FarmDeposit> Deposits = new();
    private static readonly object Lock = new();

    static FarmFactoryService()
    {
        SeedPools();
    }

    public Task<IEnumerable<FarmPool>> GetPoolsAsync(string? network = null, bool activeOnly = true)
    {
        lock (Lock)
        {
            var q = Pools.AsEnumerable();
            if (!string.IsNullOrEmpty(network))
                q = q.Where(p => string.Equals(p.Network, network, StringComparison.OrdinalIgnoreCase));
            if (activeOnly)
                q = q.Where(p => p.IsActive);
            return Task.FromResult(q.ToList().AsEnumerable());
        }
    }

    public Task<FarmPool?> GetPoolAsync(Guid poolId)
    {
        lock (Lock)
            return Task.FromResult(Pools.FirstOrDefault(p => p.Id == poolId));
    }

    public Task<FarmPool> CreatePoolAsync(CreatePoolRequest req)
    {
        var pool = new FarmPool
        {
            Name = req.Name,
            Description = req.Description,
            Network = req.Network.ToUpperInvariant(),
            StakingTokenAddress = req.StakingTokenAddress,
            StakingTokenSymbol = req.StakingTokenSymbol,
            StakingTokenDecimals = req.StakingTokenDecimals,
            RewardTokenAddress = req.RewardTokenAddress,
            RewardTokenSymbol = req.RewardTokenSymbol,
            RewardTokenDecimals = req.RewardTokenDecimals,
            TotalRewardAmount = req.TotalRewardAmount,
            StartTime = req.StartTime,
            EndTime = req.EndTime,
            IsActive = true
        };
        lock (Lock)
            Pools.Add(pool);
        return Task.FromResult(pool);
    }

    public Task<FarmDeposit> DepositAsync(Guid poolId, string userWallet, decimal amount)
    {
        if (amount <= 0) throw new ArgumentException("Amount must be positive.", nameof(amount));
        lock (Lock)
        {
            var pool = Pools.FirstOrDefault(p => p.Id == poolId) ?? throw new InvalidOperationException("Pool not found.");
            if (!pool.IsActive) throw new InvalidOperationException("Pool is not active.");
            var now = DateTime.UtcNow;
            if (now > pool.EndTime) throw new InvalidOperationException("Farming period has ended.");
            if (now < pool.StartTime) throw new InvalidOperationException("Farming has not started yet.");

            var d = new FarmDeposit
            {
                PoolId = poolId,
                UserWallet = userWallet.Trim(),
                Amount = amount,
                StakedAt = now
            };
            Deposits.Add(d);
            return Task.FromResult(d);
        }
    }

    public Task<FarmDeposit> WithdrawAsync(Guid depositId, string userWallet)
    {
        lock (Lock)
        {
            var d = Deposits.FirstOrDefault(x => x.Id == depositId && string.Equals(x.UserWallet, userWallet.Trim(), StringComparison.OrdinalIgnoreCase))
                ?? throw new InvalidOperationException("Deposit not found.");
            if (d.UnstakedAt.HasValue) throw new InvalidOperationException("Already withdrawn.");
            d.UnstakedAt = DateTime.UtcNow;
            return Task.FromResult(d);
        }
    }

    public Task<decimal> ClaimAsync(Guid? depositId, string userWallet, Guid? poolId = null)
    {
        var wallet = userWallet.Trim();
        lock (Lock)
        {
            var list = Deposits.AsEnumerable();
            if (depositId.HasValue)
                list = list.Where(x => x.Id == depositId.Value);
            if (poolId.HasValue)
                list = list.Where(x => x.PoolId == poolId.Value);
            list = list.Where(x => string.Equals(x.UserWallet, wallet, StringComparison.OrdinalIgnoreCase));

            decimal total = 0;
            foreach (var d in list)
            {
                var pending = ComputePendingReward(d);
                if (pending > 0)
                {
                    d.ClaimedReward += pending;
                    total += pending;
                }
            }
            return Task.FromResult(total);
        }
    }

    public Task<IEnumerable<FarmDeposit>> GetDepositsAsync(string? userWallet = null, Guid? poolId = null)
    {
        lock (Lock)
        {
            var q = Deposits.AsEnumerable();
            if (!string.IsNullOrEmpty(userWallet))
                q = q.Where(x => string.Equals(x.UserWallet, userWallet.Trim(), StringComparison.OrdinalIgnoreCase));
            if (poolId.HasValue)
                q = q.Where(x => x.PoolId == poolId.Value);
            return Task.FromResult(q.ToList().AsEnumerable());
        }
    }

    public Task<FarmDeposit?> GetDepositAsync(Guid depositId)
    {
        lock (Lock)
            return Task.FromResult(Deposits.FirstOrDefault(x => x.Id == depositId));
    }

    public Task<decimal> GetPendingRewardAsync(Guid depositId)
    {
        lock (Lock)
        {
            var d = Deposits.FirstOrDefault(x => x.Id == depositId);
            return Task.FromResult(d == null ? 0 : ComputePendingReward(d));
        }
    }

    public Task<decimal> GetPendingRewardForWalletInPoolAsync(string userWallet, Guid poolId)
    {
        lock (Lock)
        {
            var list = Deposits.Where(x => x.PoolId == poolId && string.Equals(x.UserWallet, userWallet.Trim(), StringComparison.OrdinalIgnoreCase));
            var sum = list.Sum(ComputePendingReward);
            return Task.FromResult(sum);
        }
    }

    /// <summary>
    /// Reward = (amount × seconds_staked) / total_weight × rewards_distributed_so_far − claimed.
    /// seconds = from max(StakedAt, Start) to min(UnstakedAt ?? End, End, Now).
    /// </summary>
    private static decimal ComputePendingReward(FarmDeposit d)
    {
        var pool = Pools.FirstOrDefault(p => p.Id == d.PoolId);
        if (pool == null || pool.PeriodSeconds <= 0) return 0;

        var now = DateTime.UtcNow;
        var start = pool.StartTime;
        var end = pool.EndTime;
        var from = d.StakedAt > start ? d.StakedAt : start;
        var to = d.UnstakedAt ?? (now < end ? now : end);
        if (to <= from) return 0;

        var seconds = (to - from).TotalSeconds;
        var weight = d.Amount * (decimal)seconds;

        var allInPool = Deposits.Where(x => x.PoolId == d.PoolId).ToList();
        decimal totalWeight = 0;
        foreach (var x in allInPool)
        {
            var f = x.StakedAt > start ? x.StakedAt : start;
            var t = x.UnstakedAt ?? (now < end ? now : end);
            if (t > f)
                totalWeight += x.Amount * (decimal)(t - f).TotalSeconds;
        }

        if (totalWeight <= 0) return 0;
        var cutOff = end < now ? end : now;
        var elapsed = (cutOff - start).TotalSeconds;
        if (elapsed <= 0) return 0;
        var rewardsDistributed = pool.TotalRewardAmount * (decimal)(elapsed / pool.PeriodSeconds);
        var totalEarned = (weight / totalWeight) * rewardsDistributed;
        var pending = totalEarned - d.ClaimedReward;
        return pending > 0 ? pending : 0;
    }

    private static void SeedPools()
    {
        var now = DateTime.UtcNow;
        var yearEnd = now.AddYears(1);

        Pools.AddRange(new[]
        {
            new FarmPool
            {
                Name = "Stake USDT, Farm USDT",
                Description = "Simple interest: stake and earn USDT. Ethereum mainnet.",
                Network = "ETH",
                StakingTokenAddress = "0xdAC17F958D2ee523a2206206994597C13D831ec7",
                StakingTokenSymbol = "USDT",
                StakingTokenDecimals = 6,
                RewardTokenAddress = "0xdAC17F958D2ee523a2206206994597C13D831ec7",
                RewardTokenSymbol = "USDT",
                RewardTokenDecimals = 6,
                TotalRewardAmount = 36500,
                StartTime = now,
                EndTime = yearEnd,
                IsActive = true
            },
            new FarmPool
            {
                Name = "Stake WBNB, Farm IGT",
                Description = "Stake BNB (WBNB) on BSC, earn IGT rewards.",
                Network = "BSC",
                StakingTokenAddress = "0xbb4CdB9CBd36B01bD1cBaEBF2De08d9173bc095c",
                StakingTokenSymbol = "WBNB",
                StakingTokenDecimals = 18,
                RewardTokenAddress = "0xIGT_BSC_PLACEHOLDER",
                RewardTokenSymbol = "IGT",
                RewardTokenDecimals = 9,
                TotalRewardAmount = 100_000,
                StartTime = now,
                EndTime = yearEnd,
                IsActive = true
            },
            new FarmPool
            {
                Name = "Stake IGT-STAKE, Farm IGT-SOVEREIGN",
                Description = "IERAHKWA: stake IGT-STAKE, farm IGT-SOVEREIGN. Ierahkwa Sovereign Blockchain.",
                Network = "IERAHKWA",
                StakingTokenAddress = "0xIGT_STAKE_IERAHKWA",
                StakingTokenSymbol = "IGT-STAKE",
                StakingTokenDecimals = 9,
                RewardTokenAddress = "0xIGT_SOVEREIGN_IERAHKWA",
                RewardTokenSymbol = "IGT-SOVEREIGN",
                RewardTokenDecimals = 9,
                TotalRewardAmount = 500_000,
                StartTime = now,
                EndTime = yearEnd,
                IsActive = true
            },
            new FarmPool
            {
                Name = "Stake USDC, Farm USDC (Polygon)",
                Description = "Stake USDC on Polygon, earn USDC. Interest-style yield.",
                Network = "POLYGON",
                StakingTokenAddress = "0x2791Bca1f2de4661ED88A30C99A7a9449Aa84174",
                StakingTokenSymbol = "USDC",
                StakingTokenDecimals = 6,
                RewardTokenAddress = "0x2791Bca1f2de4661ED88A30C99A7a9449Aa84174",
                RewardTokenSymbol = "USDC",
                RewardTokenDecimals = 6,
                TotalRewardAmount = 18250,
                StartTime = now,
                EndTime = yearEnd,
                IsActive = true
            },
            new FarmPool
            {
                Name = "Stake ETH, Farm REWARD (Aurora)",
                Description = "Stake ETH on Aurora, earn REWARD tokens.",
                Network = "AURORA",
                StakingTokenAddress = "0xC9BdeEd33CD01541e1eeD10f90519d2C06Fe3feB",
                StakingTokenSymbol = "WETH",
                StakingTokenDecimals = 18,
                RewardTokenAddress = "0xREWARD_AURORA",
                RewardTokenSymbol = "REWARD",
                RewardTokenDecimals = 18,
                TotalRewardAmount = 50_000,
                StartTime = now,
                EndTime = yearEnd,
                IsActive = true
            }
        });
    }
}
