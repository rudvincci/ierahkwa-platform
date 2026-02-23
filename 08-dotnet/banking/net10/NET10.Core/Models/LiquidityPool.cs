namespace NET10.Core.Models;

/// <summary>
/// Liquidity Pool for AMM (Automated Market Maker)
/// </summary>
public class LiquidityPool
{
    public string Id { get; set; } = Guid.NewGuid().ToString();
    public string PoolAddress { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    
    // Token pair
    public string Token0Id { get; set; } = string.Empty;
    public string Token1Id { get; set; } = string.Empty;
    public Token? Token0 { get; set; }
    public Token? Token1 { get; set; }
    
    // Reserves
    public decimal Reserve0 { get; set; }
    public decimal Reserve1 { get; set; }
    
    // Liquidity
    public decimal TotalLiquidity { get; set; }
    public decimal LPTokenSupply { get; set; }
    
    // Fees
    public decimal SwapFee { get; set; } = 0.003m; // 0.3%
    public decimal AdminFee { get; set; } = 0.001m; // 0.1% for admin
    public decimal LPFee { get; set; } = 0.002m; // 0.2% for LPs
    
    // Stats
    public decimal Volume24h { get; set; }
    public decimal Volume7d { get; set; }
    public decimal Fees24h { get; set; }
    public decimal APR { get; set; }
    public decimal TVL { get; set; } // Total Value Locked
    
    // Status
    public bool IsActive { get; set; } = true;
    public bool IsVerified { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
}

/// <summary>
/// User's liquidity position in a pool
/// </summary>
public class LiquidityPosition
{
    public string Id { get; set; } = Guid.NewGuid().ToString();
    public string UserId { get; set; } = string.Empty;
    public string PoolId { get; set; } = string.Empty;
    public LiquidityPool? Pool { get; set; }
    
    // LP tokens
    public decimal LPTokenBalance { get; set; }
    public decimal SharePercentage { get; set; }
    
    // Underlying assets
    public decimal Token0Amount { get; set; }
    public decimal Token1Amount { get; set; }
    
    // Value
    public decimal UsdValue { get; set; }
    public decimal EarnedFees { get; set; }
    
    public DateTime AddedAt { get; set; } = DateTime.UtcNow;
}

/// <summary>
/// Add/Remove liquidity request
/// </summary>
public class LiquidityRequest
{
    public string UserId { get; set; } = string.Empty;
    public string PoolId { get; set; } = string.Empty;
    public LiquidityAction Action { get; set; }
    public decimal Token0Amount { get; set; }
    public decimal Token1Amount { get; set; }
    public decimal LPTokenAmount { get; set; }
    public decimal SlippageTolerance { get; set; } = 0.5m; // 0.5%
    public DateTime Deadline { get; set; } = DateTime.UtcNow.AddMinutes(20);
}

public enum LiquidityAction
{
    Add,
    Remove
}

/// <summary>
/// Liquidity transaction record
/// </summary>
public class LiquidityTransaction
{
    public string Id { get; set; } = Guid.NewGuid().ToString();
    public string TxHash { get; set; } = string.Empty;
    public string UserId { get; set; } = string.Empty;
    public string PoolId { get; set; } = string.Empty;
    public LiquidityAction Action { get; set; }
    public decimal Token0Amount { get; set; }
    public decimal Token1Amount { get; set; }
    public decimal LPTokenAmount { get; set; }
    public decimal UsdValue { get; set; }
    public TransactionStatus Status { get; set; } = TransactionStatus.Pending;
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime? ConfirmedAt { get; set; }
}

public enum TransactionStatus
{
    Pending,
    Confirmed,
    Failed,
    Cancelled
}
