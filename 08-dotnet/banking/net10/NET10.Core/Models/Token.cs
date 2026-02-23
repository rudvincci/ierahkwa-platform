namespace NET10.Core.Models;

/// <summary>
/// Represents a token in the DeFi ecosystem
/// </summary>
public class Token
{
    public string Id { get; set; } = Guid.NewGuid().ToString();
    public string Symbol { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public string Address { get; set; } = string.Empty;
    public int Decimals { get; set; } = 18;
    public string LogoUrl { get; set; } = string.Empty;
    public string ChainId { get; set; } = "777777"; // Ierahkwa Sovereign Blockchain
    public decimal TotalSupply { get; set; }
    public decimal Price { get; set; }
    public decimal PriceChange24h { get; set; }
    public decimal Volume24h { get; set; }
    public decimal MarketCap { get; set; }
    public bool IsNative { get; set; }
    public bool IsVerified { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}

/// <summary>
/// Token pair for trading
/// </summary>
public class TokenPair
{
    public string Id { get; set; } = Guid.NewGuid().ToString();
    public string PairAddress { get; set; } = string.Empty;
    public Token Token0 { get; set; } = new();
    public Token Token1 { get; set; } = new();
    public decimal Reserve0 { get; set; }
    public decimal Reserve1 { get; set; }
    public decimal TotalLiquidity { get; set; }
    public decimal Volume24h { get; set; }
    public decimal Fee { get; set; } = 0.003m; // 0.3% default fee
    public decimal AdminFee { get; set; } = 0.001m; // 0.1% admin fee
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}

/// <summary>
/// User token balance
/// </summary>
public class TokenBalance
{
    public string UserId { get; set; } = string.Empty;
    public string TokenId { get; set; } = string.Empty;
    public Token? Token { get; set; }
    public decimal Balance { get; set; }
    public decimal LockedBalance { get; set; }
    public decimal UsdValue { get; set; }
}
