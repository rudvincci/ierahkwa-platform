using NET10.Core.Interfaces;
using NET10.Core.Models;
using TransactionStatus = NET10.Core.Models.TransactionStatus;

namespace NET10.Infrastructure.Services;

public class PoolService : IPoolService
{
    private readonly ITokenService _tokenService;
    private readonly List<LiquidityPool> _pools;
    private readonly List<LiquidityPosition> _positions;
    private readonly List<LiquidityTransaction> _transactions;

    public PoolService(ITokenService tokenService)
    {
        _tokenService = tokenService;
        _positions = new List<LiquidityPosition>();
        _transactions = new List<LiquidityTransaction>();
        
        // Initialize demo pools
        _pools = new List<LiquidityPool>
        {
            new LiquidityPool
            {
                Id = "pool-igt-usdt",
                Name = "IGT/USDT",
                Token0Id = "igt-main",
                Token1Id = "usdt",
                Reserve0 = 1000000,
                Reserve1 = 1000000,
                TotalLiquidity = 2000000,
                LPTokenSupply = 1000000,
                TVL = 2000000,
                Volume24h = 500000,
                APR = 45.5m,
                IsActive = true,
                IsVerified = true
            },
            new LiquidityPool
            {
                Id = "pool-igt-pm-usdt",
                Name = "IGT-PM/USDT",
                Token0Id = "igt-pm",
                Token1Id = "usdt",
                Reserve0 = 500000,
                Reserve1 = 500000,
                TotalLiquidity = 1000000,
                LPTokenSupply = 500000,
                TVL = 1000000,
                Volume24h = 250000,
                APR = 38.2m,
                IsActive = true,
                IsVerified = true
            },
            new LiquidityPool
            {
                Id = "pool-igt-weth",
                Name = "IGT/WETH",
                Token0Id = "igt-main",
                Token1Id = "weth",
                Reserve0 = 3500000,
                Reserve1 = 1000,
                TotalLiquidity = 7000000,
                LPTokenSupply = 1750000,
                TVL = 7000000,
                Volume24h = 1500000,
                APR = 62.8m,
                IsActive = true,
                IsVerified = true
            },
            new LiquidityPool
            {
                Id = "pool-weth-usdt",
                Name = "WETH/USDT",
                Token0Id = "weth",
                Token1Id = "usdt",
                Reserve0 = 2857,
                Reserve1 = 10000000,
                TotalLiquidity = 20000000,
                LPTokenSupply = 5000000,
                TVL = 20000000,
                Volume24h = 5000000,
                APR = 28.5m,
                IsActive = true,
                IsVerified = true
            },
            new LiquidityPool
            {
                Id = "pool-igt-defi-igt",
                Name = "IGT-DEFI/IGT",
                Token0Id = "igt-defi",
                Token1Id = "igt-main",
                Reserve0 = 2000000,
                Reserve1 = 1000000,
                TotalLiquidity = 1500000,
                LPTokenSupply = 750000,
                TVL = 1500000,
                Volume24h = 350000,
                APR = 125.5m,
                IsActive = true,
                IsVerified = true
            },
            new LiquidityPool
            {
                Id = "pool-igt-net-usdt",
                Name = "IGT-NET/USDT",
                Token0Id = "igt-net",
                Token1Id = "usdt",
                Reserve0 = 10000000,
                Reserve1 = 1000000,
                TotalLiquidity = 2000000,
                LPTokenSupply = 1000000,
                TVL = 2000000,
                Volume24h = 450000,
                APR = 85.3m,
                IsActive = true,
                IsVerified = true
            }
        };
    }

    public Task<List<LiquidityPool>> GetAllPoolsAsync()
    {
        return Task.FromResult(_pools.ToList());
    }

    public Task<LiquidityPool?> GetPoolByIdAsync(string poolId)
    {
        return Task.FromResult(_pools.FirstOrDefault(p => p.Id == poolId));
    }

    public Task<LiquidityPool?> GetPoolByPairAsync(string token0Id, string token1Id)
    {
        var pool = _pools.FirstOrDefault(p =>
            (p.Token0Id == token0Id && p.Token1Id == token1Id) ||
            (p.Token0Id == token1Id && p.Token1Id == token0Id));
        return Task.FromResult(pool);
    }

    public Task<LiquidityPool> CreatePoolAsync(string token0Id, string token1Id, decimal fee)
    {
        var pool = new LiquidityPool
        {
            Id = Guid.NewGuid().ToString(),
            Token0Id = token0Id,
            Token1Id = token1Id,
            Name = $"{token0Id}/{token1Id}",
            SwapFee = fee,
            IsActive = true,
            CreatedAt = DateTime.UtcNow
        };
        _pools.Add(pool);
        return Task.FromResult(pool);
    }

    public Task<LiquidityPool> UpdatePoolAsync(LiquidityPool pool)
    {
        var existing = _pools.FirstOrDefault(p => p.Id == pool.Id);
        if (existing != null)
        {
            var index = _pools.IndexOf(existing);
            pool.UpdatedAt = DateTime.UtcNow;
            _pools[index] = pool;
        }
        return Task.FromResult(pool);
    }

    public async Task<LiquidityTransaction> AddLiquidityAsync(LiquidityRequest request)
    {
        var pool = await GetPoolByIdAsync(request.PoolId);
        if (pool == null)
            throw new ArgumentException("Pool not found");

        // Calculate LP tokens to mint
        decimal lpTokens = request.Token0Amount; // Simplified calculation
        
        // Update pool reserves
        pool.Reserve0 += request.Token0Amount;
        pool.Reserve1 += request.Token1Amount;
        pool.TotalLiquidity += request.Token0Amount + request.Token1Amount;
        pool.LPTokenSupply += lpTokens;
        pool.UpdatedAt = DateTime.UtcNow;

        // Create/update user position
        var position = _positions.FirstOrDefault(p => 
            p.UserId == request.UserId && p.PoolId == request.PoolId);
        
        if (position == null)
        {
            position = new LiquidityPosition
            {
                UserId = request.UserId,
                PoolId = request.PoolId,
                Pool = pool
            };
            _positions.Add(position);
        }

        position.LPTokenBalance += lpTokens;
        position.Token0Amount += request.Token0Amount;
        position.Token1Amount += request.Token1Amount;
        position.SharePercentage = (position.LPTokenBalance / pool.LPTokenSupply) * 100;

        var tx = new LiquidityTransaction
        {
            Id = Guid.NewGuid().ToString(),
            TxHash = $"0x{Guid.NewGuid():N}",
            UserId = request.UserId,
            PoolId = request.PoolId,
            Action = LiquidityAction.Add,
            Token0Amount = request.Token0Amount,
            Token1Amount = request.Token1Amount,
            LPTokenAmount = lpTokens,
            Status = TransactionStatus.Confirmed,
            CreatedAt = DateTime.UtcNow,
            ConfirmedAt = DateTime.UtcNow
        };

        _transactions.Add(tx);
        return tx;
    }

    public async Task<LiquidityTransaction> RemoveLiquidityAsync(LiquidityRequest request)
    {
        var pool = await GetPoolByIdAsync(request.PoolId);
        if (pool == null)
            throw new ArgumentException("Pool not found");

        var position = _positions.FirstOrDefault(p => 
            p.UserId == request.UserId && p.PoolId == request.PoolId);
        
        if (position == null || position.LPTokenBalance < request.LPTokenAmount)
            throw new InvalidOperationException("Insufficient LP tokens");

        // Calculate tokens to return
        decimal sharePercent = request.LPTokenAmount / pool.LPTokenSupply;
        decimal token0Amount = pool.Reserve0 * sharePercent;
        decimal token1Amount = pool.Reserve1 * sharePercent;

        // Update pool
        pool.Reserve0 -= token0Amount;
        pool.Reserve1 -= token1Amount;
        pool.TotalLiquidity -= token0Amount + token1Amount;
        pool.LPTokenSupply -= request.LPTokenAmount;
        pool.UpdatedAt = DateTime.UtcNow;

        // Update position
        position.LPTokenBalance -= request.LPTokenAmount;
        position.Token0Amount -= token0Amount;
        position.Token1Amount -= token1Amount;
        position.SharePercentage = pool.LPTokenSupply > 0 
            ? (position.LPTokenBalance / pool.LPTokenSupply) * 100 
            : 0;

        var tx = new LiquidityTransaction
        {
            Id = Guid.NewGuid().ToString(),
            TxHash = $"0x{Guid.NewGuid():N}",
            UserId = request.UserId,
            PoolId = request.PoolId,
            Action = LiquidityAction.Remove,
            Token0Amount = token0Amount,
            Token1Amount = token1Amount,
            LPTokenAmount = request.LPTokenAmount,
            Status = TransactionStatus.Confirmed,
            CreatedAt = DateTime.UtcNow,
            ConfirmedAt = DateTime.UtcNow
        };

        _transactions.Add(tx);
        return tx;
    }

    public async Task<(decimal token0Amount, decimal token1Amount)> CalculateAddLiquidityAsync(
        string poolId, decimal token0Amount)
    {
        var pool = await GetPoolByIdAsync(poolId);
        if (pool == null || pool.Reserve0 == 0)
            return (token0Amount, token0Amount);

        decimal ratio = pool.Reserve1 / pool.Reserve0;
        decimal token1Amount = token0Amount * ratio;
        return (token0Amount, token1Amount);
    }

    public async Task<(decimal token0Amount, decimal token1Amount)> CalculateRemoveLiquidityAsync(
        string poolId, decimal lpTokenAmount)
    {
        var pool = await GetPoolByIdAsync(poolId);
        if (pool == null || pool.LPTokenSupply == 0)
            return (0, 0);

        decimal sharePercent = lpTokenAmount / pool.LPTokenSupply;
        return (pool.Reserve0 * sharePercent, pool.Reserve1 * sharePercent);
    }

    public Task<List<LiquidityPosition>> GetUserPositionsAsync(string userId)
    {
        var positions = _positions.Where(p => p.UserId == userId).ToList();
        return Task.FromResult(positions);
    }

    public Task<LiquidityPosition?> GetUserPositionAsync(string userId, string poolId)
    {
        var position = _positions.FirstOrDefault(p => 
            p.UserId == userId && p.PoolId == poolId);
        return Task.FromResult(position);
    }

    public Task<List<LiquidityTransaction>> GetUserLiquidityHistoryAsync(string userId, int limit = 50)
    {
        var history = _transactions
            .Where(t => t.UserId == userId)
            .OrderByDescending(t => t.CreatedAt)
            .Take(limit)
            .ToList();
        return Task.FromResult(history);
    }

    public Task<List<LiquidityPool>> GetTopPoolsByTVLAsync(int limit = 10)
    {
        var pools = _pools
            .OrderByDescending(p => p.TVL)
            .Take(limit)
            .ToList();
        return Task.FromResult(pools);
    }

    public Task<List<LiquidityPool>> GetTopPoolsByVolumeAsync(int limit = 10)
    {
        var pools = _pools
            .OrderByDescending(p => p.Volume24h)
            .Take(limit)
            .ToList();
        return Task.FromResult(pools);
    }
}
