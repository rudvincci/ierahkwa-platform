using System;
using System.Collections.Generic;

namespace TradeX.Core.Models;

/// <summary>
/// Cryptocurrency/Token Asset - Ierahkwa TradeX
/// Supports IGT Tokens, ERC20, BEP20, TRC20, Native Coins
/// </summary>
public class Asset
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public string Symbol { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public string? ContractAddress { get; set; }
    public string Network { get; set; } = "IERAHKWA"; // IERAHKWA, ETH, BSC, TRON, POLYGON, etc.
    public AssetType Type { get; set; } = AssetType.Token;
    
    // Pricing
    public decimal CurrentPrice { get; set; }
    public decimal PriceChange24h { get; set; }
    public decimal Volume24h { get; set; }
    public decimal MarketCap { get; set; }
    
    // Supply
    public decimal TotalSupply { get; set; }
    public decimal CirculatingSupply { get; set; }
    public int Decimals { get; set; } = 18;
    
    // Status
    public bool IsActive { get; set; } = true;
    public bool DepositEnabled { get; set; } = true;
    public bool WithdrawalEnabled { get; set; } = true;
    public bool TradingEnabled { get; set; } = true;
    
    // Fees
    public decimal WithdrawalFee { get; set; }
    public decimal MinWithdrawal { get; set; }
    public decimal MaxWithdrawal { get; set; }
    
    // Ierahkwa Integration
    public bool IsIGTToken { get; set; }
    public string? IerahkwaNodeEndpoint { get; set; }
    
    // Metadata
    public string? IconUrl { get; set; }
    public string? Website { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    
    public virtual ICollection<TradingPair> BasePairs { get; set; } = new List<TradingPair>();
    public virtual ICollection<TradingPair> QuotePairs { get; set; } = new List<TradingPair>();
}

public enum AssetType
{
    NativeCoin,  // ETH, BNB, TRX, etc.
    Token,       // ERC20, BEP20, TRC20
    IGTToken,    // Ierahkwa Government Tokens
    Fiat,        // USD, EUR, etc.
    Commodity    // Gold, Silver, etc.
}
