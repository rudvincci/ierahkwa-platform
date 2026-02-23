using IDOFactory.Core.Interfaces;
using IDOFactory.Core.Models;

namespace IDOFactory.Infrastructure.Services;

public class IDOService : IIDOService
{
    private readonly List<IDOPool> _pools = new();
    private readonly List<IDOContribution> _contributions = new();

    public IDOService()
    {
        // Initialize with sample data
        InitializeSampleData();
    }

    private void InitializeSampleData()
    {
        _pools.Add(new IDOPool
        {
            Id = "pool-001",
            Name = "IGT Token Launch",
            Description = "Official IGT governance token launch on Ierahkwa Sovereign Blockchain",
            TokenSymbol = "IGT-GOV",
            TokenName = "Ierahkwa Governance Token",
            TokenAddress = "0x1234567890abcdef1234567890abcdef12345678",
            TokenDecimals = 18,
            LogoUrl = "/assets/tokens/igt-gov.png",
            TokenPrice = 0.1m,
            SoftCap = 50000,
            HardCap = 100000,
            MinContribution = 100,
            MaxContribution = 5000,
            TotalTokensForSale = 1000000,
            TokensSold = 450000,
            FundsRaised = 45000,
            PaymentToken = "USDT",
            RegistrationStart = DateTime.UtcNow.AddDays(-7),
            RegistrationEnd = DateTime.UtcNow.AddDays(-1),
            SaleStart = DateTime.UtcNow.AddHours(-12),
            SaleEnd = DateTime.UtcNow.AddDays(7),
            Status = IDOPoolStatus.Live,
            ParticipantsCount = 125,
            ChainId = 777777,
            Network = "ierahkwa-mainnet",
            Website = "https://ierahkwa.gov",
            Twitter = "https://twitter.com/ierahkwa",
            Telegram = "https://t.me/ierahkwa",
            PoolType = IDOPoolType.Public
        });

        _pools.Add(new IDOPool
        {
            Id = "pool-002",
            Name = "DEFI Protocol Token",
            Description = "Decentralized Finance protocol launching on Ierahkwa",
            TokenSymbol = "IDEFI",
            TokenName = "Ierahkwa DeFi",
            TokenAddress = "0xabcdef1234567890abcdef1234567890abcdef12",
            TokenDecimals = 18,
            LogoUrl = "/assets/tokens/idefi.png",
            TokenPrice = 0.05m,
            SoftCap = 25000,
            HardCap = 75000,
            MinContribution = 50,
            MaxContribution = 2500,
            TotalTokensForSale = 1500000,
            TokensSold = 0,
            FundsRaised = 0,
            PaymentToken = "USDT",
            RegistrationStart = DateTime.UtcNow.AddDays(1),
            RegistrationEnd = DateTime.UtcNow.AddDays(5),
            SaleStart = DateTime.UtcNow.AddDays(7),
            SaleEnd = DateTime.UtcNow.AddDays(14),
            Status = IDOPoolStatus.Upcoming,
            ParticipantsCount = 0,
            ChainId = 777777,
            Network = "ierahkwa-mainnet",
            Website = "https://defi.ierahkwa.gov",
            HasVesting = true,
            TgePercentage = 25,
            VestingMonths = 6,
            CliffMonths = 1,
            PoolType = IDOPoolType.Whitelist,
            WhitelistEnabled = true
        });

        _pools.Add(new IDOPool
        {
            Id = "pool-003",
            Name = "Gaming Token",
            Description = "Play-to-earn gaming ecosystem token",
            TokenSymbol = "IGAME",
            TokenName = "Ierahkwa Gaming",
            TokenAddress = "0x567890abcdef1234567890abcdef1234567890ab",
            TokenDecimals = 18,
            LogoUrl = "/assets/tokens/igame.png",
            TokenPrice = 0.02m,
            SoftCap = 20000,
            HardCap = 50000,
            MinContribution = 25,
            MaxContribution = 1000,
            TotalTokensForSale = 2500000,
            TokensSold = 2500000,
            FundsRaised = 50000,
            PaymentToken = "USDT",
            RegistrationStart = DateTime.UtcNow.AddDays(-30),
            RegistrationEnd = DateTime.UtcNow.AddDays(-25),
            SaleStart = DateTime.UtcNow.AddDays(-20),
            SaleEnd = DateTime.UtcNow.AddDays(-10),
            ClaimStart = DateTime.UtcNow.AddDays(-5),
            Status = IDOPoolStatus.Claiming,
            ParticipantsCount = 842,
            ChainId = 777777,
            Network = "ierahkwa-mainnet"
        });
    }

    // Pool Methods
    public Task<IEnumerable<IDOPool>> GetAllPoolsAsync() => 
        Task.FromResult<IEnumerable<IDOPool>>(_pools.ToList());

    public Task<IEnumerable<IDOPool>> GetPoolsByStatusAsync(IDOPoolStatus status) =>
        Task.FromResult<IEnumerable<IDOPool>>(_pools.Where(p => p.Status == status).ToList());

    public Task<IDOPool?> GetPoolByIdAsync(string poolId) =>
        Task.FromResult(_pools.FirstOrDefault(p => p.Id == poolId));

    public Task<IDOPool> CreatePoolAsync(IDOPool pool)
    {
        pool.Id = Guid.NewGuid().ToString();
        pool.CreatedAt = DateTime.UtcNow;
        pool.UpdatedAt = DateTime.UtcNow;
        pool.Status = IDOPoolStatus.Draft;
        _pools.Add(pool);
        return Task.FromResult(pool);
    }

    public Task<IDOPool> UpdatePoolAsync(IDOPool pool)
    {
        var existing = _pools.FirstOrDefault(p => p.Id == pool.Id);
        if (existing == null) throw new ArgumentException("Pool not found");
        
        var index = _pools.IndexOf(existing);
        pool.UpdatedAt = DateTime.UtcNow;
        _pools[index] = pool;
        return Task.FromResult(pool);
    }

    public Task<bool> DeletePoolAsync(string poolId)
    {
        var pool = _pools.FirstOrDefault(p => p.Id == poolId);
        if (pool == null) return Task.FromResult(false);
        if (pool.Status != IDOPoolStatus.Draft) throw new InvalidOperationException("Can only delete draft pools");
        _pools.Remove(pool);
        return Task.FromResult(true);
    }

    // Status Management
    public Task<IDOPool> StartRegistrationAsync(string poolId)
    {
        var pool = _pools.FirstOrDefault(p => p.Id == poolId) ?? throw new ArgumentException("Pool not found");
        pool.Status = IDOPoolStatus.Registration;
        pool.UpdatedAt = DateTime.UtcNow;
        return Task.FromResult(pool);
    }

    public Task<IDOPool> StartSaleAsync(string poolId)
    {
        var pool = _pools.FirstOrDefault(p => p.Id == poolId) ?? throw new ArgumentException("Pool not found");
        pool.Status = IDOPoolStatus.Live;
        pool.SaleStart = DateTime.UtcNow;
        pool.UpdatedAt = DateTime.UtcNow;
        return Task.FromResult(pool);
    }

    public Task<IDOPool> EndSaleAsync(string poolId)
    {
        var pool = _pools.FirstOrDefault(p => p.Id == poolId) ?? throw new ArgumentException("Pool not found");
        pool.Status = IDOPoolStatus.Ended;
        pool.SaleEnd = DateTime.UtcNow;
        pool.UpdatedAt = DateTime.UtcNow;
        return Task.FromResult(pool);
    }

    public Task<IDOPool> FinalizePoolAsync(string poolId)
    {
        var pool = _pools.FirstOrDefault(p => p.Id == poolId) ?? throw new ArgumentException("Pool not found");
        pool.Status = pool.FundsRaised >= pool.SoftCap ? IDOPoolStatus.Claiming : IDOPoolStatus.Finalized;
        pool.ClaimStart = DateTime.UtcNow;
        pool.UpdatedAt = DateTime.UtcNow;
        return Task.FromResult(pool);
    }

    public Task<IDOPool> CancelPoolAsync(string poolId)
    {
        var pool = _pools.FirstOrDefault(p => p.Id == poolId) ?? throw new ArgumentException("Pool not found");
        pool.Status = IDOPoolStatus.Cancelled;
        pool.UpdatedAt = DateTime.UtcNow;
        return Task.FromResult(pool);
    }

    // Contributions
    public Task<IDOContribution> ContributeAsync(string poolId, string userAddress, decimal amount)
    {
        var pool = _pools.FirstOrDefault(p => p.Id == poolId) ?? throw new ArgumentException("Pool not found");
        
        if (pool.Status != IDOPoolStatus.Live)
            throw new InvalidOperationException("Pool is not accepting contributions");
        
        if (amount < pool.MinContribution || amount > pool.MaxContribution)
            throw new ArgumentException($"Amount must be between {pool.MinContribution} and {pool.MaxContribution}");
        
        if (pool.FundsRaised + amount > pool.HardCap)
            throw new InvalidOperationException("Would exceed hard cap");
        
        var tokensAllocated = amount / pool.TokenPrice;
        
        var contribution = new IDOContribution
        {
            PoolId = poolId,
            UserAddress = userAddress,
            Amount = amount,
            PaymentToken = pool.PaymentToken,
            TokensAllocated = tokensAllocated,
            ChainId = pool.ChainId,
            Status = ContributionStatus.Confirmed,
            CreatedAt = DateTime.UtcNow,
            TransactionHash = $"0x{Guid.NewGuid():N}"
        };
        
        _contributions.Add(contribution);
        
        pool.FundsRaised += amount;
        pool.TokensSold += tokensAllocated;
        pool.ParticipantsCount = _contributions.Count(c => c.PoolId == poolId && c.Status == ContributionStatus.Confirmed);
        
        if (pool.FundsRaised >= pool.HardCap)
            pool.Status = IDOPoolStatus.Filled;
        
        return Task.FromResult(contribution);
    }

    public Task<IEnumerable<IDOContribution>> GetUserContributionsAsync(string userAddress) =>
        Task.FromResult<IEnumerable<IDOContribution>>(_contributions.Where(c => c.UserAddress == userAddress).ToList());

    public Task<IEnumerable<IDOContribution>> GetPoolContributionsAsync(string poolId) =>
        Task.FromResult<IEnumerable<IDOContribution>>(_contributions.Where(c => c.PoolId == poolId).ToList());

    public Task<IDOContribution> ClaimTokensAsync(string contributionId, string userAddress)
    {
        var contribution = _contributions.FirstOrDefault(c => c.Id == contributionId && c.UserAddress == userAddress)
            ?? throw new ArgumentException("Contribution not found");
        
        var pool = _pools.FirstOrDefault(p => p.Id == contribution.PoolId)
            ?? throw new ArgumentException("Pool not found");
        
        if (pool.Status != IDOPoolStatus.Claiming)
            throw new InvalidOperationException("Tokens are not yet claimable");
        
        contribution.TokensClaimed = contribution.TokensAllocated;
        contribution.Status = ContributionStatus.Claimed;
        contribution.ClaimedAt = DateTime.UtcNow;
        
        return Task.FromResult(contribution);
    }

    public Task<IDOContribution> RefundAsync(string contributionId, string userAddress)
    {
        var contribution = _contributions.FirstOrDefault(c => c.Id == contributionId && c.UserAddress == userAddress)
            ?? throw new ArgumentException("Contribution not found");
        
        var pool = _pools.FirstOrDefault(p => p.Id == contribution.PoolId)
            ?? throw new ArgumentException("Pool not found");
        
        if (pool.Status != IDOPoolStatus.Cancelled && pool.FundsRaised >= pool.SoftCap)
            throw new InvalidOperationException("Refunds not available");
        
        contribution.Status = ContributionStatus.Refunded;
        contribution.RefundedAt = DateTime.UtcNow;
        
        pool.FundsRaised -= contribution.Amount;
        pool.TokensSold -= contribution.TokensAllocated;
        
        return Task.FromResult(contribution);
    }

    // Whitelist
    public Task<bool> AddToWhitelistAsync(string poolId, IEnumerable<string> addresses)
    {
        var pool = _pools.FirstOrDefault(p => p.Id == poolId) ?? throw new ArgumentException("Pool not found");
        pool.Whitelist.AddRange(addresses.Where(a => !pool.Whitelist.Contains(a)));
        return Task.FromResult(true);
    }

    public Task<bool> RemoveFromWhitelistAsync(string poolId, IEnumerable<string> addresses)
    {
        var pool = _pools.FirstOrDefault(p => p.Id == poolId) ?? throw new ArgumentException("Pool not found");
        pool.Whitelist.RemoveAll(a => addresses.Contains(a));
        return Task.FromResult(true);
    }

    public Task<bool> IsWhitelistedAsync(string poolId, string address)
    {
        var pool = _pools.FirstOrDefault(p => p.Id == poolId) ?? throw new ArgumentException("Pool not found");
        return Task.FromResult(!pool.WhitelistEnabled || pool.Whitelist.Contains(address));
    }

    // Statistics
    public Task<IDOStatistics> GetPlatformStatisticsAsync()
    {
        return Task.FromResult(new IDOStatistics
        {
            TotalPools = _pools.Count,
            ActivePools = _pools.Count(p => p.Status == IDOPoolStatus.Live || p.Status == IDOPoolStatus.Registration),
            SuccessfulPools = _pools.Count(p => p.Status == IDOPoolStatus.Claiming || p.Status == IDOPoolStatus.Finalized),
            TotalFundsRaised = _pools.Sum(p => p.FundsRaised),
            TotalParticipants = _contributions.Select(c => c.UserAddress).Distinct().Count(),
            TotalProjects = _pools.Count
        });
    }

    public Task<IDOPoolStatistics> GetPoolStatisticsAsync(string poolId)
    {
        var pool = _pools.FirstOrDefault(p => p.Id == poolId) ?? throw new ArgumentException("Pool not found");
        var contributions = _contributions.Where(c => c.PoolId == poolId && c.Status == ContributionStatus.Confirmed).ToList();
        
        return Task.FromResult(new IDOPoolStatistics
        {
            PoolId = poolId,
            FundsRaised = pool.FundsRaised,
            PercentageFilled = pool.HardCap > 0 ? (pool.FundsRaised / pool.HardCap) * 100 : 0,
            Participants = contributions.Select(c => c.UserAddress).Distinct().Count(),
            AverageContribution = contributions.Count > 0 ? contributions.Average(c => c.Amount) : 0,
            TimeRemaining = pool.SaleEnd > DateTime.UtcNow ? pool.SaleEnd - DateTime.UtcNow : TimeSpan.Zero
        });
    }
}
