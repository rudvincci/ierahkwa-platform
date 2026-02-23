using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TradeX.Core.Interfaces;
using TradeX.Core.Models;

namespace TradeX.Infrastructure.Services;

/// <summary>
/// Staking Service - Ierahkwa TradeX
/// Earn passive income by locking IGT tokens
/// </summary>
public class StakingService : IStakingService
{
    private static readonly List<StakingPool> _pools = InitializePools();
    private static readonly List<Stake> _stakes = new();
    
    public Task<IEnumerable<StakingPool>> GetActivePoolsAsync()
    {
        return Task.FromResult<IEnumerable<StakingPool>>(_pools.Where(p => p.IsActive));
    }
    
    public Task<StakingPool?> GetPoolAsync(Guid poolId)
    {
        return Task.FromResult(_pools.FirstOrDefault(p => p.Id == poolId));
    }
    
    public Task<Stake> StakeAsync(Guid userId, Guid poolId, decimal amount)
    {
        var pool = _pools.FirstOrDefault(p => p.Id == poolId)
            ?? throw new Exception("Staking pool not found");
        
        if (!pool.IsActive)
            throw new Exception("Staking pool is not active");
        
        if (amount < pool.MinStakeAmount)
            throw new Exception($"Minimum stake amount is {pool.MinStakeAmount}");
        
        if (amount > pool.MaxStakeAmount)
            throw new Exception($"Maximum stake amount is {pool.MaxStakeAmount}");
        
        if (amount > pool.AvailableCapacity)
            throw new Exception("Pool capacity exceeded");
        
        // Calculate estimated reward
        var estimatedReward = amount * (pool.APY / 100m) * (pool.LockPeriodDays / 365m);
        
        var stake = new Stake
        {
            UserId = userId,
            StakingPoolId = poolId,
            Amount = amount,
            EstimatedReward = estimatedReward,
            UnlockDate = DateTime.UtcNow.AddDays(pool.LockPeriodDays),
            Status = StakeStatus.Active
        };
        
        lock (_stakes)
        {
            _stakes.Add(stake);
            pool.CurrentStaked += amount;
        }
        
        return Task.FromResult(stake);
    }
    
    public Task<Stake> UnstakeAsync(Guid stakeId, Guid userId, bool early = false)
    {
        lock (_stakes)
        {
            var stake = _stakes.FirstOrDefault(s => s.Id == stakeId && s.UserId == userId)
                ?? throw new Exception("Stake not found");
            
            if (stake.Status != StakeStatus.Active)
                throw new Exception("Stake is not active");
            
            var pool = _pools.First(p => p.Id == stake.StakingPoolId);
            
            if (early && DateTime.UtcNow < stake.UnlockDate)
            {
                if (!pool.EarlyWithdrawalAllowed)
                    throw new Exception("Early withdrawal is not allowed for this pool");
                
                // Apply penalty
                stake.EarnedReward *= (1 - pool.EarlyWithdrawalPenalty);
                stake.Status = StakeStatus.EarlyWithdrawn;
            }
            else
            {
                stake.Status = StakeStatus.Completed;
            }
            
            stake.UnstakedAt = DateTime.UtcNow;
            pool.CurrentStaked -= stake.Amount;
            
            return Task.FromResult(stake);
        }
    }
    
    public Task<decimal> ClaimRewardsAsync(Guid stakeId, Guid userId)
    {
        lock (_stakes)
        {
            var stake = _stakes.FirstOrDefault(s => s.Id == stakeId && s.UserId == userId)
                ?? throw new Exception("Stake not found");
            
            var claimable = stake.EarnedReward - stake.ClaimedReward;
            if (claimable <= 0)
                throw new Exception("No rewards to claim");
            
            stake.ClaimedReward += claimable;
            stake.LastRewardAt = DateTime.UtcNow;
            
            return Task.FromResult(claimable);
        }
    }
    
    public Task<IEnumerable<Stake>> GetUserStakesAsync(Guid userId)
    {
        return Task.FromResult<IEnumerable<Stake>>(_stakes.Where(s => s.UserId == userId));
    }
    
    public Task CalculateRewardsAsync()
    {
        lock (_stakes)
        {
            foreach (var stake in _stakes.Where(s => s.Status == StakeStatus.Active))
            {
                var pool = _pools.First(p => p.Id == stake.StakingPoolId);
                var daysPassed = (DateTime.UtcNow - stake.StakedAt).TotalDays;
                var dailyRate = pool.APY / 100m / 365m;
                stake.EarnedReward = stake.Amount * dailyRate * (decimal)daysPassed;
            }
        }
        return Task.CompletedTask;
    }
    
    private static List<StakingPool> InitializePools()
    {
        return new List<StakingPool>
        {
            new()
            {
                Name = "IGT Flexible Staking",
                Description = "Stake IGT tokens with flexible withdrawal",
                APY = 8m,
                LockPeriodDays = 30,
                MinStakeAmount = 100,
                MaxStakeAmount = 1000000,
                TotalPoolSize = 100000000,
                EarlyWithdrawalAllowed = true,
                EarlyWithdrawalPenalty = 0.05m,
                IsActive = true,
                StartDate = DateTime.UtcNow
            },
            new()
            {
                Name = "IGT 90-Day Lock",
                Description = "Higher APY with 90-day lock period",
                APY = 15m,
                LockPeriodDays = 90,
                MinStakeAmount = 500,
                MaxStakeAmount = 5000000,
                TotalPoolSize = 50000000,
                EarlyWithdrawalAllowed = true,
                EarlyWithdrawalPenalty = 0.15m,
                IsActive = true,
                StartDate = DateTime.UtcNow
            },
            new()
            {
                Name = "IGT Annual Premium",
                Description = "Maximum rewards with 365-day lock",
                APY = 25m,
                LockPeriodDays = 365,
                MinStakeAmount = 1000,
                MaxStakeAmount = 10000000,
                TotalPoolSize = 25000000,
                EarlyWithdrawalAllowed = false,
                IsActive = true,
                StartDate = DateTime.UtcNow
            },
            new()
            {
                Name = "BDET Bank Savings",
                Description = "Stake with BDET Central Bank backing",
                APY = 12m,
                LockPeriodDays = 180,
                MinStakeAmount = 250,
                MaxStakeAmount = 2500000,
                TotalPoolSize = 75000000,
                EarlyWithdrawalAllowed = true,
                EarlyWithdrawalPenalty = 0.1m,
                IsActive = true,
                StartDate = DateTime.UtcNow
            }
        };
    }
}
