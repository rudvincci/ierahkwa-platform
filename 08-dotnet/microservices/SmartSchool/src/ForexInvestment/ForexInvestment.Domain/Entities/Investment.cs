// ============================================================================
// IERAHKWA FUTUREHEAD - FOREX INVESTMENT SYSTEM
// Domain Entity: Investment
// Version: 1.0.0 - .NET 10 LTS
// © 2026 Ierahkwa Ne Kanienke Sovereign Government - All Rights Reserved
// ============================================================================

using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Ierahkwa.ForexInvestment.Domain.Entities;

/// <summary>
/// Represents an investment in a Forex trading plan
/// </summary>
public class Investment
{
    [Key]
    public Guid Id { get; set; } = Guid.NewGuid();
    
    [Required]
    public Guid UserId { get; set; }
    
    [Required]
    public Guid AccountId { get; set; }
    
    [Required]
    public Guid PlanId { get; set; }
    
    [Required]
    public Guid DurationId { get; set; }
    
    [Required]
    [StringLength(50)]
    public string InvestmentNumber { get; set; } = string.Empty;
    
    [Required]
    [Column(TypeName = "decimal(18,8)")]
    public decimal Amount { get; set; }
    
    [Required]
    public string Currency { get; set; } = "USD";
    
    [Column(TypeName = "decimal(18,8)")]
    public decimal AmountInUSD { get; set; }
    
    [Column(TypeName = "decimal(5,2)")]
    public decimal ROIPercentage { get; set; }
    
    [Column(TypeName = "decimal(18,8)")]
    public decimal ExpectedProfit { get; set; }
    
    [Column(TypeName = "decimal(18,8)")]
    public decimal ActualProfit { get; set; } = 0;
    
    [Column(TypeName = "decimal(18,8)")]
    public decimal TotalReturn { get; set; } = 0;
    
    public InvestmentStatus Status { get; set; } = InvestmentStatus.Pending;
    
    public InvestmentResult? Result { get; set; }
    
    public DateTime StartDate { get; set; }
    
    public DateTime EndDate { get; set; }
    
    public DateTime? CompletedAt { get; set; }
    
    public DateTime? CancelledAt { get; set; }
    
    public string? CancellationReason { get; set; }
    
    public bool TermsAccepted { get; set; } = false;
    
    public DateTime? TermsAcceptedAt { get; set; }
    
    public string? TermsVersion { get; set; }
    
    public bool AutoReinvest { get; set; } = false;
    
    public int ReinvestCount { get; set; } = 0;
    
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
    
    public string? MetaData { get; set; }
    
    // Risk Management
    [Column(TypeName = "decimal(5,2)")]
    public decimal RiskScore { get; set; } = 0;
    
    public bool FraudFlagged { get; set; } = false;
    
    public string? FraudReason { get; set; }
    
    // Navigation properties
    public virtual ForexAccount? Account { get; set; }
    public virtual InvestmentPlan? Plan { get; set; }
    public virtual InvestmentDuration? Duration { get; set; }
}

public enum InvestmentStatus
{
    Pending = 0,
    Active = 1,
    Completed = 2,
    Cancelled = 3,
    Rejected = 4,
    Processing = 5,
    OnHold = 6
}

public enum InvestmentResult
{
    Pending = 0,
    Win = 1,
    Loss = 2,
    Draw = 3,
    PartialWin = 4
}
