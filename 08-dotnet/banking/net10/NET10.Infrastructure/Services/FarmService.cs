using NET10.Core.Interfaces;
using NET10.Core.Models;
using TransactionStatus = NET10.Core.Models.TransactionStatus;

namespace NET10.Infrastructure.Services;

public class FarmService : IFarmService
{
    private readonly List<Farm> _farms;
    private readonly List<FarmPosition> _positions;
    private readonly List<FarmTransaction> _transactions;

    public FarmService()
    {
        _positions = new List<FarmPosition>();
        _transactions = new List<FarmTransaction>();
        
        // Initialize demo farms
        _farms = new List<Farm>
        {
            new Farm
            {
                Id = "farm-igt-usdt-lp",
                Name = "IGT-USDT LP Farm",
                Description = "Stake IGT-USDT LP tokens to earn IGT-DEFI rewards",
                StakeTokenId = "pool-igt-usdt",
                StakeTokenSymbol = "IGT-USDT LP",
                RewardTokenId = "igt-defi",
                RewardTokenSymbol = "IGT-DEFI",
                RewardPerBlock = 10,
                TotalRewards = 10000000,
                TotalStaked = 500000,
                TVL = 500000,
                APR = 125.5m,
                APY = 245.8m,
                Multiplier = 2.0m,
                StakersCount = 1250,
                Status = FarmStatus.Active,
                IsActive = true,
                IsFeatured = true,
                StartTime = DateTime.UtcNow.AddDays(-30),
                EndTime = DateTime.UtcNow.AddDays(335)
            },
            new Farm
            {
                Id = "farm-igt-weth-lp",
                Name = "IGT-WETH LP Farm",
                Description = "Stake IGT-WETH LP tokens to earn IGT-DEFI rewards",
                StakeTokenId = "pool-igt-weth",
                StakeTokenSymbol = "IGT-WETH LP",
                RewardTokenId = "igt-defi",
                RewardTokenSymbol = "IGT-DEFI",
                RewardPerBlock = 15,
                TotalRewards = 15000000,
                TotalStaked = 1000000,
                TVL = 1000000,
                APR = 185.2m,
                APY = 512.5m,
                Multiplier = 3.0m,
                StakersCount = 2100,
                Status = FarmStatus.Active,
                IsActive = true,
                IsFeatured = true,
                StartTime = DateTime.UtcNow.AddDays(-15),
                EndTime = DateTime.UtcNow.AddDays(350)
            },
            new Farm
            {
                Id = "farm-igt-single",
                Name = "IGT Single Staking",
                Description = "Stake IGT to earn more IGT",
                StakeTokenId = "igt-main",
                StakeTokenSymbol = "IGT",
                RewardTokenId = "igt-main",
                RewardTokenSymbol = "IGT",
                RewardPerBlock = 5,
                TotalRewards = 5000000,
                TotalStaked = 2000000,
                TVL = 2000000,
                APR = 45.5m,
                APY = 56.8m,
                Multiplier = 1.0m,
                StakersCount = 5420,
                Status = FarmStatus.Active,
                IsActive = true,
                StartTime = DateTime.UtcNow.AddDays(-60),
                EndTime = DateTime.UtcNow.AddDays(305)
            },
            new Farm
            {
                Id = "farm-igt-pm-single",
                Name = "IGT-PM Governance Staking",
                Description = "Stake IGT-PM for governance rights and IGT-STAKE rewards",
                StakeTokenId = "igt-pm",
                StakeTokenSymbol = "IGT-PM",
                RewardTokenId = "igt-stake",
                RewardTokenSymbol = "IGT-STAKE",
                RewardPerBlock = 8,
                TotalRewards = 8000000,
                TotalStaked = 750000,
                TVL = 750000,
                APR = 78.5m,
                APY = 118.2m,
                Multiplier = 1.5m,
                LockDays = 30,
                StakersCount = 1850,
                Status = FarmStatus.Active,
                IsActive = true,
                StartTime = DateTime.UtcNow.AddDays(-45),
                EndTime = DateTime.UtcNow.AddDays(320)
            },
            new Farm
            {
                Id = "farm-igt-defi-lp",
                Name = "IGT-DEFI/IGT LP Farm (Boosted)",
                Description = "Boosted rewards for IGT-DEFI/IGT liquidity providers",
                StakeTokenId = "pool-igt-defi-igt",
                StakeTokenSymbol = "IGT-DEFI/IGT LP",
                RewardTokenId = "igt-defi",
                RewardTokenSymbol = "IGT-DEFI",
                RewardPerBlock = 25,
                TotalRewards = 25000000,
                TotalStaked = 350000,
                TVL = 350000,
                APR = 320.8m,
                APY = 2450.5m,
                Multiplier = 5.0m,
                StakersCount = 850,
                Status = FarmStatus.Active,
                IsActive = true,
                IsFeatured = true,
                StartTime = DateTime.UtcNow.AddDays(-7),
                EndTime = DateTime.UtcNow.AddDays(358)
            }
        };
    }

    public Task<List<Farm>> GetAllFarmsAsync()
    {
        return Task.FromResult(_farms.ToList());
    }

    public Task<List<Farm>> GetActiveFarmsAsync()
    {
        var active = _farms.Where(f => f.IsActive && f.Status == FarmStatus.Active).ToList();
        return Task.FromResult(active);
    }

    public Task<Farm?> GetFarmByIdAsync(string farmId)
    {
        return Task.FromResult(_farms.FirstOrDefault(f => f.Id == farmId));
    }

    public Task<Farm> CreateFarmAsync(Farm farm)
    {
        farm.Id = Guid.NewGuid().ToString();
        farm.CreatedAt = DateTime.UtcNow;
        _farms.Add(farm);
        return Task.FromResult(farm);
    }

    public Task<Farm> UpdateFarmAsync(Farm farm)
    {
        var existing = _farms.FirstOrDefault(f => f.Id == farm.Id);
        if (existing != null)
        {
            var index = _farms.IndexOf(existing);
            farm.UpdatedAt = DateTime.UtcNow;
            _farms[index] = farm;
        }
        return Task.FromResult(farm);
    }

    public Task<bool> EndFarmAsync(string farmId)
    {
        var farm = _farms.FirstOrDefault(f => f.Id == farmId);
        if (farm != null)
        {
            farm.Status = FarmStatus.Ended;
            farm.IsActive = false;
            farm.EndTime = DateTime.UtcNow;
            farm.UpdatedAt = DateTime.UtcNow;
            return Task.FromResult(true);
        }
        return Task.FromResult(false);
    }

    public async Task<FarmTransaction> StakeAsync(FarmActionRequest request)
    {
        var farm = await GetFarmByIdAsync(request.FarmId);
        if (farm == null)
            throw new ArgumentException("Farm not found");

        if (!farm.IsActive)
            throw new InvalidOperationException("Farm is not active");

        // Get or create position
        var position = _positions.FirstOrDefault(p => 
            p.UserId == request.UserId && p.FarmId == request.FarmId);
        
        if (position == null)
        {
            position = new FarmPosition
            {
                UserId = request.UserId,
                FarmId = request.FarmId,
                Farm = farm
            };
            _positions.Add(position);
        }

        // Calculate fee
        decimal fee = request.Amount * farm.DepositFee;
        decimal stakeAmount = request.Amount - fee;

        // Update position
        position.StakedAmount += stakeAmount;
        position.StakedAt = DateTime.UtcNow;
        if (farm.LockDays > 0)
        {
            position.UnlockAt = DateTime.UtcNow.AddDays(farm.LockDays);
            position.IsLocked = true;
        }
        position.UpdatedAt = DateTime.UtcNow;

        // Update farm
        farm.TotalStaked += stakeAmount;
        farm.TVL += stakeAmount;
        farm.StakersCount = _positions.Count(p => p.FarmId == farm.Id && p.StakedAmount > 0);
        farm.UpdatedAt = DateTime.UtcNow;

        var tx = new FarmTransaction
        {
            Id = Guid.NewGuid().ToString(),
            TxHash = $"0x{Guid.NewGuid():N}",
            UserId = request.UserId,
            FarmId = request.FarmId,
            Action = FarmAction.Stake,
            Amount = stakeAmount,
            Fee = fee,
            Status = TransactionStatus.Confirmed,
            CreatedAt = DateTime.UtcNow,
            ConfirmedAt = DateTime.UtcNow
        };

        _transactions.Add(tx);
        return tx;
    }

    public async Task<FarmTransaction> UnstakeAsync(FarmActionRequest request)
    {
        var farm = await GetFarmByIdAsync(request.FarmId);
        if (farm == null)
            throw new ArgumentException("Farm not found");

        var position = _positions.FirstOrDefault(p => 
            p.UserId == request.UserId && p.FarmId == request.FarmId);
        
        if (position == null || position.StakedAmount < request.Amount)
            throw new InvalidOperationException("Insufficient staked amount");

        if (position.IsLocked && position.UnlockAt > DateTime.UtcNow)
            throw new InvalidOperationException($"Tokens are locked until {position.UnlockAt}");

        // Calculate fee
        decimal fee = request.Amount * farm.WithdrawFee;
        decimal withdrawAmount = request.Amount - fee;

        // Update position
        position.StakedAmount -= request.Amount;
        position.IsLocked = position.StakedAmount > 0 && position.UnlockAt > DateTime.UtcNow;
        position.UpdatedAt = DateTime.UtcNow;

        // Update farm
        farm.TotalStaked -= request.Amount;
        farm.TVL -= request.Amount;
        farm.StakersCount = _positions.Count(p => p.FarmId == farm.Id && p.StakedAmount > 0);
        farm.UpdatedAt = DateTime.UtcNow;

        var tx = new FarmTransaction
        {
            Id = Guid.NewGuid().ToString(),
            TxHash = $"0x{Guid.NewGuid():N}",
            UserId = request.UserId,
            FarmId = request.FarmId,
            Action = FarmAction.Unstake,
            Amount = withdrawAmount,
            Fee = fee,
            Status = TransactionStatus.Confirmed,
            CreatedAt = DateTime.UtcNow,
            ConfirmedAt = DateTime.UtcNow
        };

        _transactions.Add(tx);
        return tx;
    }

    public async Task<FarmTransaction> HarvestAsync(FarmActionRequest request)
    {
        var position = _positions.FirstOrDefault(p => 
            p.UserId == request.UserId && p.FarmId == request.FarmId);
        
        if (position == null)
            throw new InvalidOperationException("No staking position found");

        decimal rewards = await CalculatePendingRewardsAsync(request.UserId, request.FarmId);
        
        position.HarvestedRewards += rewards;
        position.PendingRewards = 0;
        position.LastHarvestAt = DateTime.UtcNow;
        position.UpdatedAt = DateTime.UtcNow;

        var tx = new FarmTransaction
        {
            Id = Guid.NewGuid().ToString(),
            TxHash = $"0x{Guid.NewGuid():N}",
            UserId = request.UserId,
            FarmId = request.FarmId,
            Action = FarmAction.Harvest,
            Amount = rewards,
            Status = TransactionStatus.Confirmed,
            CreatedAt = DateTime.UtcNow,
            ConfirmedAt = DateTime.UtcNow
        };

        _transactions.Add(tx);
        return tx;
    }

    public async Task<FarmTransaction> CompoundAsync(FarmActionRequest request)
    {
        // Harvest and restake
        var harvestTx = await HarvestAsync(request);
        
        var stakeRequest = new FarmActionRequest
        {
            UserId = request.UserId,
            FarmId = request.FarmId,
            Amount = harvestTx.Amount
        };

        var stakeTx = await StakeAsync(stakeRequest);
        stakeTx.Action = FarmAction.Compound;
        return stakeTx;
    }

    public Task<List<FarmPosition>> GetUserPositionsAsync(string userId)
    {
        var positions = _positions.Where(p => p.UserId == userId).ToList();
        return Task.FromResult(positions);
    }

    public Task<FarmPosition?> GetUserPositionAsync(string userId, string farmId)
    {
        var position = _positions.FirstOrDefault(p => 
            p.UserId == userId && p.FarmId == farmId);
        return Task.FromResult(position);
    }

    public Task<decimal> CalculatePendingRewardsAsync(string userId, string farmId)
    {
        var position = _positions.FirstOrDefault(p => 
            p.UserId == userId && p.FarmId == farmId);
        
        if (position == null || position.StakedAmount == 0)
            return Task.FromResult(0m);

        var farm = _farms.FirstOrDefault(f => f.Id == farmId);
        if (farm == null)
            return Task.FromResult(0m);

        // Simplified reward calculation
        var timeStaked = (DateTime.UtcNow - position.StakedAt).TotalDays;
        decimal dailyRate = farm.APR / 365 / 100;
        decimal rewards = position.StakedAmount * dailyRate * (decimal)timeStaked * farm.Multiplier;
        
        return Task.FromResult(Math.Max(0, rewards));
    }

    public Task<List<FarmTransaction>> GetUserFarmHistoryAsync(string userId, int limit = 50)
    {
        var history = _transactions
            .Where(t => t.UserId == userId)
            .OrderByDescending(t => t.CreatedAt)
            .Take(limit)
            .ToList();
        return Task.FromResult(history);
    }

    public Task<List<Farm>> GetTopFarmsByAPRAsync(int limit = 10)
    {
        var farms = _farms
            .Where(f => f.IsActive)
            .OrderByDescending(f => f.APR)
            .Take(limit)
            .ToList();
        return Task.FromResult(farms);
    }

    public Task<List<Farm>> GetTopFarmsByTVLAsync(int limit = 10)
    {
        var farms = _farms
            .Where(f => f.IsActive)
            .OrderByDescending(f => f.TVL)
            .Take(limit)
            .ToList();
        return Task.FromResult(farms);
    }
}
