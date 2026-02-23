using System;

namespace TradeX.Core.Models;

/// <summary>
/// Trading Pair - Ierahkwa TradeX
/// </summary>
public class TradingPair
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public Guid BaseAssetId { get; set; }
    public Guid QuoteAssetId { get; set; }
    
    public string Symbol { get; set; } = string.Empty; // e.g., "IGT/USDT"
    public decimal LastPrice { get; set; }
    public decimal High24h { get; set; }
    public decimal Low24h { get; set; }
    public decimal Volume24h { get; set; }
    public decimal PriceChange24h { get; set; }
    
    // Trading Settings
    public decimal MinOrderAmount { get; set; } = 1m;
    public decimal MaxOrderAmount { get; set; } = 1000000m;
    public decimal MakerFee { get; set; } = 0.001m; // 0.1%
    public decimal TakerFee { get; set; } = 0.001m; // 0.1%
    public int PricePrecision { get; set; } = 8;
    public int AmountPrecision { get; set; } = 8;
    
    public bool IsActive { get; set; } = true;
    public bool BotEnabled { get; set; } = true;
    
    public virtual Asset BaseAsset { get; set; } = null!;
    public virtual Asset QuoteAsset { get; set; } = null!;
}

/// <summary>
/// Order - Buy/Sell Orders
/// </summary>
public class Order
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public string OrderNumber { get; set; } = $"ORD-{DateTime.UtcNow:yyyyMMddHHmmss}-{Guid.NewGuid().ToString()[..8]}";
    public Guid UserId { get; set; }
    public Guid TradingPairId { get; set; }
    
    public OrderSide Side { get; set; } // Buy or Sell
    public OrderType Type { get; set; } // Market, Limit, StopLimit
    
    public decimal Price { get; set; }
    public decimal Amount { get; set; }
    public decimal FilledAmount { get; set; }
    public decimal RemainingAmount => Amount - FilledAmount;
    
    public decimal? StopPrice { get; set; } // For stop-limit orders
    public decimal Total => Price * Amount;
    public decimal Fee { get; set; }
    
    public OrderStatus Status { get; set; } = OrderStatus.Open;
    public bool IsBot { get; set; }
    
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime? FilledAt { get; set; }
    public DateTime? CancelledAt { get; set; }
    
    public virtual User User { get; set; } = null!;
    public virtual TradingPair TradingPair { get; set; } = null!;
}

/// <summary>
/// Trade - Executed Trade Record
/// </summary>
public class Trade
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public string TradeNumber { get; set; } = $"TRD-{DateTime.UtcNow:yyyyMMddHHmmss}-{Guid.NewGuid().ToString()[..8]}";
    
    public Guid BuyOrderId { get; set; }
    public Guid SellOrderId { get; set; }
    public Guid TradingPairId { get; set; }
    public Guid BuyerId { get; set; }
    public Guid SellerId { get; set; }
    
    public decimal Price { get; set; }
    public decimal Amount { get; set; }
    public decimal Total => Price * Amount;
    
    public decimal BuyerFee { get; set; }
    public decimal SellerFee { get; set; }
    
    public bool IsBotTrade { get; set; }
    public DateTime ExecutedAt { get; set; } = DateTime.UtcNow;
    
    public virtual TradingPair TradingPair { get; set; } = null!;
}

/// <summary>
/// Swap Request - Instant Token Exchange
/// </summary>
public class SwapRequest
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public Guid UserId { get; set; }
    public Guid FromAssetId { get; set; }
    public Guid ToAssetId { get; set; }
    
    public decimal FromAmount { get; set; }
    public decimal ToAmount { get; set; }
    public decimal ExchangeRate { get; set; }
    public decimal Fee { get; set; }
    
    public SwapStatus Status { get; set; } = SwapStatus.Pending;
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime? CompletedAt { get; set; }
    
    public virtual User User { get; set; } = null!;
    public virtual Asset FromAsset { get; set; } = null!;
    public virtual Asset ToAsset { get; set; } = null!;
}

public enum OrderSide { Buy, Sell }
public enum OrderType { Market, Limit, StopLimit }
public enum OrderStatus { Open, PartiallyFilled, Filled, Cancelled, Expired }
public enum SwapStatus { Pending, Processing, Completed, Failed }
