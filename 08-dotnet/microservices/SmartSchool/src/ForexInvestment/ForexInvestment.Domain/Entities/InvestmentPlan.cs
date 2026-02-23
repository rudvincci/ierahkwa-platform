// ============================================================================
// IERAHKWA FUTUREHEAD - FOREX INVESTMENT SYSTEM
// Domain Entity: InvestmentPlan
// Version: 1.0.0 - .NET 10 LTS
// © 2026 Ierahkwa Ne Kanienke Sovereign Government - All Rights Reserved
// ============================================================================

using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Ierahkwa.ForexInvestment.Domain.Entities;

/// <summary>
/// Represents a Forex investment plan with configurable ROI and limits
/// </summary>
public class InvestmentPlan
{
    [Key]
    public Guid Id { get; set; } = Guid.NewGuid();
    
    [Required]
    [StringLength(100)]
    public string Name { get; set; } = string.Empty;
    
    [StringLength(500)]
    public string Description { get; set; } = string.Empty;
    
    [Required]
    [StringLength(50)]
    public string Code { get; set; } = string.Empty;
    
    public PlanType Type { get; set; } = PlanType.Standard;
    
    public PlanCategory Category { get; set; } = PlanCategory.Forex;
    
    [Column(TypeName = "decimal(18,8)")]
    public decimal MinAmount { get; set; } = 100;
    
    [Column(TypeName = "decimal(18,8)")]
    public decimal MaxAmount { get; set; } = 100000;
    
    [Column(TypeName = "decimal(5,2)")]
    public decimal MinROI { get; set; } = 5;
    
    [Column(TypeName = "decimal(5,2)")]
    public decimal MaxROI { get; set; } = 25;
    
    [Column(TypeName = "decimal(5,2)")]
    public decimal GuaranteedROI { get; set; } = 0; // If > 0, this is the guaranteed return
    
    public string Currency { get; set; } = "USD";
    
    public bool IsTrending { get; set; } = false;
    
    public bool IsFeatured { get; set; } = false;
    
    public bool IsActive { get; set; } = true;
    
    public int SortOrder { get; set; } = 0;
    
    public string? ImageUrl { get; set; }
    
    public string? IconClass { get; set; }
    
    public string? BrandColor { get; set; }
    
    // Risk Configuration
    public RiskLevel RiskLevel { get; set; } = RiskLevel.Medium;
    
    [Column(TypeName = "decimal(5,2)")]
    public decimal MaxDrawdown { get; set; } = 20; // Maximum drawdown percentage
    
    // Compounding
    public bool AllowCompounding { get; set; } = true;
    
    [Column(TypeName = "decimal(5,2)")]
    public decimal CompoundingBonus { get; set; } = 2; // Extra % for compounding
    
    // Fees
    [Column(TypeName = "decimal(5,2)")]
    public decimal ManagementFee { get; set; } = 0; // Annual management fee
    
    [Column(TypeName = "decimal(5,2)")]
    public decimal PerformanceFee { get; set; } = 0; // % of profits
    
    // Limits
    public int MaxActiveInvestments { get; set; } = 0; // 0 = unlimited
    
    public int MaxInvestorsTotal { get; set; } = 0; // 0 = unlimited
    
    public int CurrentInvestorsCount { get; set; } = 0;
    
    [Column(TypeName = "decimal(18,8)")]
    public decimal TotalInvestedAmount { get; set; } = 0;
    
    [Column(TypeName = "decimal(18,8)")]
    public decimal TotalCapacity { get; set; } = 0; // 0 = unlimited
    
    // Trading Strategy
    public string? TradingStrategy { get; set; }
    
    public string? TradingPairs { get; set; } // JSON array of pairs
    
    public string? AllowedInstruments { get; set; }
    
    // Terms
    public string? TermsAndConditions { get; set; }
    
    public string? TermsVersion { get; set; } = "1.0";
    
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
    
    public string? MetaData { get; set; }
    
    // Navigation properties
    public virtual ICollection<PlanDurationMapping> DurationMappings { get; set; } = new List<PlanDurationMapping>();
    public virtual ICollection<Investment> Investments { get; set; } = new List<Investment>();
}

public enum PlanType
{
    Standard = 0,
    Premium = 1,
    VIP = 2,
    Institutional = 3,
    Custom = 4
}

public enum PlanCategory
{
    Forex = 0,
    Stocks = 1,
    Indices = 2,
    Commodities = 3,
    Crypto = 4,
    Mixed = 5
}

public enum RiskLevel
{
    VeryLow = 0,
    Low = 1,
    Medium = 2,
    High = 3,
    VeryHigh = 4
}
