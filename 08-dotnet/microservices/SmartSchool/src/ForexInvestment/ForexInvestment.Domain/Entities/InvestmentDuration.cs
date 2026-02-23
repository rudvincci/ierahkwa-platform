// ============================================================================
// IERAHKWA FUTUREHEAD - FOREX INVESTMENT SYSTEM
// Domain Entity: InvestmentDuration
// Version: 1.0.0 - .NET 10 LTS
// © 2026 Ierahkwa Ne Kanienke Sovereign Government - All Rights Reserved
// ============================================================================

using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Ierahkwa.ForexInvestment.Domain.Entities;

/// <summary>
/// Represents investment duration options (hourly, daily, weekly, monthly)
/// </summary>
public class InvestmentDuration
{
    [Key]
    public Guid Id { get; set; } = Guid.NewGuid();
    
    [Required]
    [StringLength(100)]
    public string Name { get; set; } = string.Empty;
    
    [StringLength(200)]
    public string Description { get; set; } = string.Empty;
    
    [Required]
    public DurationType Type { get; set; }
    
    [Required]
    public int Value { get; set; } // Number of hours/days/weeks/months
    
    public int TotalHours => CalculateTotalHours();
    
    [Column(TypeName = "decimal(5,2)")]
    public decimal ROIBonus { get; set; } = 0; // Additional ROI for longer durations
    
    public bool IsActive { get; set; } = true;
    
    public int SortOrder { get; set; } = 0;
    
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    
    // Navigation properties
    public virtual ICollection<PlanDurationMapping> PlanMappings { get; set; } = new List<PlanDurationMapping>();
    
    private int CalculateTotalHours()
    {
        return Type switch
        {
            DurationType.Hourly => Value,
            DurationType.Daily => Value * 24,
            DurationType.Weekly => Value * 24 * 7,
            DurationType.Monthly => Value * 24 * 30,
            DurationType.Yearly => Value * 24 * 365,
            _ => Value
        };
    }
}

public enum DurationType
{
    Hourly = 0,
    Daily = 1,
    Weekly = 2,
    Monthly = 3,
    Yearly = 4
}

/// <summary>
/// Maps plans to their available durations
/// </summary>
public class PlanDurationMapping
{
    [Key]
    public Guid Id { get; set; } = Guid.NewGuid();
    
    [Required]
    public Guid PlanId { get; set; }
    
    [Required]
    public Guid DurationId { get; set; }
    
    [Column(TypeName = "decimal(5,2)")]
    public decimal SpecificROI { get; set; } = 0; // Override ROI for this combination
    
    public bool IsActive { get; set; } = true;
    
    // Navigation properties
    public virtual InvestmentPlan? Plan { get; set; }
    public virtual InvestmentDuration? Duration { get; set; }
}
