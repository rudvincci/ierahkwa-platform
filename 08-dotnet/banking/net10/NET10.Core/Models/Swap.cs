namespace NET10.Core.Models;

/// <summary>
/// Swap request for token exchange
/// </summary>
public class SwapRequest
{
    public string UserId { get; set; } = string.Empty;
    public string TokenInId { get; set; } = string.Empty;
    public string TokenOutId { get; set; } = string.Empty;
    public decimal AmountIn { get; set; }
    public decimal AmountOutMin { get; set; }
    public decimal SlippageTolerance { get; set; } = 0.5m; // 0.5%
    public string[] Path { get; set; } = Array.Empty<string>(); // Route through pools
    public DateTime Deadline { get; set; } = DateTime.UtcNow.AddMinutes(20);
    public string WalletAddress { get; set; } = string.Empty;
}

/// <summary>
/// Swap quote response
/// </summary>
public class SwapQuote
{
    public string TokenInId { get; set; } = string.Empty;
    public string TokenOutId { get; set; } = string.Empty;
    public string TokenInSymbol { get; set; } = string.Empty;
    public string TokenOutSymbol { get; set; } = string.Empty;
    public decimal AmountIn { get; set; }
    public decimal AmountOut { get; set; }
    public decimal Rate { get; set; }
    public decimal PriceImpact { get; set; }
    public decimal SwapFee { get; set; }
    public decimal AdminFee { get; set; }
    public decimal TotalFee { get; set; }
    public decimal MinimumReceived { get; set; }
    public string[] Route { get; set; } = Array.Empty<string>();
    public decimal UsdValueIn { get; set; }
    public decimal UsdValueOut { get; set; }
    public DateTime ValidUntil { get; set; } = DateTime.UtcNow.AddMinutes(1);
}

/// <summary>
/// Completed swap transaction
/// </summary>
public class SwapTransaction
{
    public string Id { get; set; } = Guid.NewGuid().ToString();
    public string TxHash { get; set; } = string.Empty;
    public string UserId { get; set; } = string.Empty;
    public string WalletAddress { get; set; } = string.Empty;
    
    // Tokens
    public string TokenInId { get; set; } = string.Empty;
    public string TokenOutId { get; set; } = string.Empty;
    public string TokenInSymbol { get; set; } = string.Empty;
    public string TokenOutSymbol { get; set; } = string.Empty;
    
    // Amounts
    public decimal AmountIn { get; set; }
    public decimal AmountOut { get; set; }
    public decimal Rate { get; set; }
    
    // Fees
    public decimal SwapFee { get; set; }
    public decimal AdminFee { get; set; }
    public decimal TotalFee { get; set; }
    
    // Value
    public decimal UsdValue { get; set; }
    public decimal PriceImpact { get; set; }
    
    // Route
    public string[] Route { get; set; } = Array.Empty<string>();
    public string PoolId { get; set; } = string.Empty;
    
    // Status
    public TransactionStatus Status { get; set; } = TransactionStatus.Pending;
    public string? ErrorMessage { get; set; }
    
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime? ConfirmedAt { get; set; }
}

/// <summary>
/// Price chart data point
/// </summary>
public class PricePoint
{
    public DateTime Timestamp { get; set; }
    public decimal Open { get; set; }
    public decimal High { get; set; }
    public decimal Low { get; set; }
    public decimal Close { get; set; }
    public decimal Volume { get; set; }
}
