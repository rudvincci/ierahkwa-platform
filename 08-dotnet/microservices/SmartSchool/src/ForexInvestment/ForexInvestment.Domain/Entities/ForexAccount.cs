// ============================================================================
// IERAHKWA FUTUREHEAD - FOREX INVESTMENT SYSTEM
// Domain Entity: ForexAccount
// Version: 1.0.0 - .NET 10 LTS
// © 2026 Ierahkwa Ne Kanienke Sovereign Government - All Rights Reserved
// ============================================================================

using System.ComponentModel.DataAnnotations;

namespace Ierahkwa.ForexInvestment.Domain.Entities;

/// <summary>
/// Represents a Forex trading account with MT4/MT5 broker integration
/// </summary>
public class ForexAccount
{
    [Key]
    public Guid Id { get; set; } = Guid.NewGuid();
    
    [Required]
    public Guid UserId { get; set; }
    
    [Required]
    [StringLength(50)]
    public string AccountNumber { get; set; } = string.Empty;
    
    [Required]
    public AccountType Type { get; set; } = AccountType.Demo;
    
    [Required]
    public string BrokerId { get; set; } = string.Empty;
    
    [StringLength(100)]
    public string BrokerName { get; set; } = string.Empty;
    
    [Required]
    public string Platform { get; set; } = "MT5"; // MT4 or MT5
    
    [Required]
    public string ServerAddress { get; set; } = string.Empty;
    
    public string Leverage { get; set; } = "1:100";
    
    [Required]
    public string Currency { get; set; } = "USD";
    
    public decimal Balance { get; set; } = 0;
    
    public decimal Equity { get; set; } = 0;
    
    public decimal Margin { get; set; } = 0;
    
    public decimal FreeMargin { get; set; } = 0;
    
    public decimal MarginLevel { get; set; } = 0;
    
    public decimal Profit { get; set; } = 0;
    
    public decimal TotalDeposited { get; set; } = 0;
    
    public decimal TotalWithdrawn { get; set; } = 0;
    
    public AccountStatus Status { get; set; } = AccountStatus.Pending;
    
    public bool IsSignalSubscribed { get; set; } = false;
    
    public Guid? SignalProviderId { get; set; }
    
    public decimal RiskPerTrade { get; set; } = 1.0m; // Percentage
    
    public int MaxOpenTrades { get; set; } = 10;
    
    public bool AutoTrading { get; set; } = false;
    
    public bool CopyTrading { get; set; } = false;
    
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
    
    public DateTime? LastSyncAt { get; set; }
    
    public string? MetaData { get; set; }
    
    // Navigation properties
    public virtual ICollection<Investment> Investments { get; set; } = new List<Investment>();
    public virtual ICollection<ForexTransaction> Transactions { get; set; } = new List<ForexTransaction>();
    public virtual ICollection<Trade> Trades { get; set; } = new List<Trade>();
}

public enum AccountType
{
    Demo = 0,
    Live = 1,
    Managed = 2,
    MAM = 3,  // Multi-Account Manager
    PAMM = 4  // Percentage Allocation Money Management
}

public enum AccountStatus
{
    Pending = 0,
    Active = 1,
    Suspended = 2,
    Closed = 3,
    Rejected = 4
}
