// ============================================================================
// IERAHKWA FUTUREHEAD - FOREX INVESTMENT SYSTEM
// Domain Entity: Trade
// Version: 1.0.0 - .NET 10 LTS
// © 2026 Ierahkwa Ne Kanienke Sovereign Government - All Rights Reserved
// ============================================================================

using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Ierahkwa.ForexInvestment.Domain.Entities;

/// <summary>
/// Represents an executed trade in the Forex system
/// </summary>
public class Trade
{
    [Key]
    public Guid Id { get; set; } = Guid.NewGuid();
    
    [Required]
    public Guid AccountId { get; set; }
    
    public Guid? SignalId { get; set; } // If from copy trading
    
    [Required]
    public long TicketNumber { get; set; } // MT4/MT5 ticket
    
    [Required]
    [StringLength(20)]
    public string Symbol { get; set; } = string.Empty;
    
    [Required]
    public TradeType Type { get; set; }
    
    [Required]
    [Column(TypeName = "decimal(10,5)")]
    public decimal LotSize { get; set; }
    
    [Required]
    [Column(TypeName = "decimal(18,8)")]
    public decimal OpenPrice { get; set; }
    
    [Column(TypeName = "decimal(18,8)")]
    public decimal? ClosePrice { get; set; }
    
    [Column(TypeName = "decimal(18,8)")]
    public decimal? StopLoss { get; set; }
    
    [Column(TypeName = "decimal(18,8)")]
    public decimal? TakeProfit { get; set; }
    
    [Column(TypeName = "decimal(18,8)")]
    public decimal Swap { get; set; } = 0;
    
    [Column(TypeName = "decimal(18,8)")]
    public decimal Commission { get; set; } = 0;
    
    [Column(TypeName = "decimal(18,8)")]
    public decimal Profit { get; set; } = 0;
    
    [Column(TypeName = "decimal(18,8)")]
    public decimal ProfitPips { get; set; } = 0;
    
    public TradeStatus Status { get; set; } = TradeStatus.Open;
    
    public TradeCloseReason? CloseReason { get; set; }
    
    [StringLength(200)]
    public string? Comment { get; set; }
    
    public int MagicNumber { get; set; } = 0; // EA identifier
    
    public DateTime OpenTime { get; set; } = DateTime.UtcNow;
    
    public DateTime? CloseTime { get; set; }
    
    public TimeSpan? Duration => CloseTime.HasValue ? CloseTime.Value - OpenTime : null;
    
    public string? MetaData { get; set; }
    
    // Navigation properties
    public virtual ForexAccount? Account { get; set; }
    public virtual TradingSignal? Signal { get; set; }
}

public enum TradeType
{
    Buy = 0,
    Sell = 1,
    BuyLimit = 2,
    SellLimit = 3,
    BuyStop = 4,
    SellStop = 5,
    BuyStopLimit = 6,
    SellStopLimit = 7
}

public enum TradeStatus
{
    Open = 0,
    Closed = 1,
    Pending = 2,
    Cancelled = 3,
    Expired = 4
}

public enum TradeCloseReason
{
    Manual = 0,
    TakeProfit = 1,
    StopLoss = 2,
    StopOut = 3,
    Signal = 4,
    Expiration = 5
}
