// ============================================================================
// IERAHKWA FUTUREHEAD - FOREX INVESTMENT SYSTEM
// Domain Entity: SignalProvider & TradingSignal
// Version: 1.0.0 - .NET 10 LTS
// © 2026 Ierahkwa Ne Kanienke Sovereign Government - All Rights Reserved
// ============================================================================

using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Ierahkwa.ForexInvestment.Domain.Entities;

/// <summary>
/// Represents a trading signal provider for copy trading
/// </summary>
public class SignalProvider
{
    [Key]
    public Guid Id { get; set; } = Guid.NewGuid();
    
    [Required]
    public Guid UserId { get; set; } // Provider's user account
    
    [Required]
    [StringLength(100)]
    public string Name { get; set; } = string.Empty;
    
    [StringLength(500)]
    public string Description { get; set; } = string.Empty;
    
    public string? AvatarUrl { get; set; }
    
    public bool IsVerified { get; set; } = false;
    
    public bool IsActive { get; set; } = true;
    
    public bool AcceptingSubscribers { get; set; } = true;
    
    // Performance Metrics
    [Column(TypeName = "decimal(10,2)")]
    public decimal TotalProfit { get; set; } = 0;
    
    [Column(TypeName = "decimal(5,2)")]
    public decimal WinRate { get; set; } = 0;
    
    public int TotalTrades { get; set; } = 0;
    
    public int WinningTrades { get; set; } = 0;
    
    public int LosingTrades { get; set; } = 0;
    
    [Column(TypeName = "decimal(5,2)")]
    public decimal AverageROI { get; set; } = 0;
    
    [Column(TypeName = "decimal(5,2)")]
    public decimal MaxDrawdown { get; set; } = 0;
    
    [Column(TypeName = "decimal(10,2)")]
    public decimal ProfitFactor { get; set; } = 0;
    
    // Subscription
    [Column(TypeName = "decimal(18,8)")]
    public decimal MonthlyFee { get; set; } = 0;
    
    [Column(TypeName = "decimal(5,2)")]
    public decimal PerformanceFee { get; set; } = 0; // % of subscriber profits
    
    public int MaxSubscribers { get; set; } = 0; // 0 = unlimited
    
    public int CurrentSubscribers { get; set; } = 0;
    
    // Trading Style
    public TradingStyle Style { get; set; } = TradingStyle.Swing;
    
    public RiskLevel RiskLevel { get; set; } = RiskLevel.Medium;
    
    public string? TradingPairs { get; set; } // JSON array
    
    public string? TradingHours { get; set; }
    
    // Ratings
    [Column(TypeName = "decimal(3,2)")]
    public decimal Rating { get; set; } = 0;
    
    public int ReviewCount { get; set; } = 0;
    
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
    
    public string? MetaData { get; set; }
    
    // Navigation properties
    public virtual ICollection<TradingSignal> Signals { get; set; } = new List<TradingSignal>();
    public virtual ICollection<SignalSubscription> Subscriptions { get; set; } = new List<SignalSubscription>();
}

public enum TradingStyle
{
    Scalping = 0,
    DayTrading = 1,
    Swing = 2,
    Position = 3,
    Algorithmic = 4
}

/// <summary>
/// Represents a trading signal for copy trading
/// </summary>
public class TradingSignal
{
    [Key]
    public Guid Id { get; set; } = Guid.NewGuid();
    
    [Required]
    public Guid ProviderId { get; set; }
    
    [Required]
    [StringLength(20)]
    public string Symbol { get; set; } = string.Empty; // e.g., EURUSD
    
    [Required]
    public SignalType Type { get; set; }
    
    [Required]
    public SignalAction Action { get; set; }
    
    [Column(TypeName = "decimal(18,8)")]
    public decimal EntryPrice { get; set; }
    
    [Column(TypeName = "decimal(18,8)")]
    public decimal? StopLoss { get; set; }
    
    [Column(TypeName = "decimal(18,8)")]
    public decimal? TakeProfit1 { get; set; }
    
    [Column(TypeName = "decimal(18,8)")]
    public decimal? TakeProfit2 { get; set; }
    
    [Column(TypeName = "decimal(18,8)")]
    public decimal? TakeProfit3 { get; set; }
    
    [Column(TypeName = "decimal(5,2)")]
    public decimal RiskRewardRatio { get; set; }
    
    [Column(TypeName = "decimal(5,2)")]
    public decimal LotSize { get; set; } = 0.01m;
    
    public SignalStatus Status { get; set; } = SignalStatus.Active;
    
    public SignalResult? Result { get; set; }
    
    [Column(TypeName = "decimal(18,8)")]
    public decimal? ExitPrice { get; set; }
    
    [Column(TypeName = "decimal(18,8)")]
    public decimal? ProfitPips { get; set; }
    
    [StringLength(500)]
    public string? Analysis { get; set; }
    
    public string? ChartImageUrl { get; set; }
    
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    
    public DateTime? ExpiresAt { get; set; }
    
    public DateTime? ExecutedAt { get; set; }
    
    public DateTime? ClosedAt { get; set; }
    
    public string? MetaData { get; set; }
    
    // Navigation properties
    public virtual SignalProvider? Provider { get; set; }
}

public enum SignalType
{
    Manual = 0,
    Automated = 1,
    AI = 2
}

public enum SignalAction
{
    Buy = 0,
    Sell = 1,
    BuyLimit = 2,
    SellLimit = 3,
    BuyStop = 4,
    SellStop = 5
}

public enum SignalStatus
{
    Active = 0,
    Executed = 1,
    Closed = 2,
    Cancelled = 3,
    Expired = 4
}

public enum SignalResult
{
    Pending = 0,
    TP1Hit = 1,
    TP2Hit = 2,
    TP3Hit = 3,
    StopLossHit = 4,
    BreakEven = 5,
    ManualClose = 6
}

/// <summary>
/// Represents a subscription to a signal provider
/// </summary>
public class SignalSubscription
{
    [Key]
    public Guid Id { get; set; } = Guid.NewGuid();
    
    [Required]
    public Guid AccountId { get; set; }
    
    [Required]
    public Guid ProviderId { get; set; }
    
    public bool IsActive { get; set; } = true;
    
    public bool AutoCopy { get; set; } = true;
    
    [Column(TypeName = "decimal(5,2)")]
    public decimal LotMultiplier { get; set; } = 1.0m;
    
    [Column(TypeName = "decimal(5,2)")]
    public decimal MaxRiskPerTrade { get; set; } = 2.0m;
    
    public int MaxConcurrentTrades { get; set; } = 5;
    
    public DateTime SubscribedAt { get; set; } = DateTime.UtcNow;
    
    public DateTime? ExpiresAt { get; set; }
    
    public DateTime? CancelledAt { get; set; }
    
    // Navigation properties
    public virtual ForexAccount? Account { get; set; }
    public virtual SignalProvider? Provider { get; set; }
}
