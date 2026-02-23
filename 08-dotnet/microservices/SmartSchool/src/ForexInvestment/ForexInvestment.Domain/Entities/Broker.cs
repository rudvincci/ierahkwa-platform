// ============================================================================
// IERAHKWA FUTUREHEAD - FOREX INVESTMENT SYSTEM
// Domain Entity: Broker
// Version: 1.0.0 - .NET 10 LTS
// © 2026 Ierahkwa Ne Kanienke Sovereign Government - All Rights Reserved
// ============================================================================

using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Ierahkwa.ForexInvestment.Domain.Entities;

/// <summary>
/// Represents a supported Forex broker with MT4/MT5 integration
/// </summary>
public class Broker
{
    [Key]
    public Guid Id { get; set; } = Guid.NewGuid();
    
    [Required]
    [StringLength(100)]
    public string Name { get; set; } = string.Empty;
    
    [Required]
    [StringLength(50)]
    public string Code { get; set; } = string.Empty;
    
    [StringLength(500)]
    public string Description { get; set; } = string.Empty;
    
    public string? LogoUrl { get; set; }
    
    public string? WebsiteUrl { get; set; }
    
    public bool IsActive { get; set; } = true;
    
    public bool IsFeatured { get; set; } = false;
    
    // Platform Configuration
    public bool SupportsMT4 { get; set; } = true;
    
    public bool SupportsMT5 { get; set; } = true;
    
    public string? MT4Server { get; set; }
    
    public string? MT5Server { get; set; }
    
    public string? ApiEndpoint { get; set; }
    
    public string? ApiKey { get; set; }
    
    public string? ApiSecret { get; set; }
    
    // Account Types
    public string? AccountTypes { get; set; } // JSON array
    
    public string? AvailableLeverages { get; set; } // JSON array
    
    public string? AvailableCurrencies { get; set; } // JSON array
    
    // Trading Conditions
    [Column(TypeName = "decimal(5,2)")]
    public decimal MinSpread { get; set; } = 0;
    
    [Column(TypeName = "decimal(18,8)")]
    public decimal MinDeposit { get; set; } = 100;
    
    [Column(TypeName = "decimal(10,5)")]
    public decimal MinLotSize { get; set; } = 0.01m;
    
    [Column(TypeName = "decimal(10,5)")]
    public decimal MaxLotSize { get; set; } = 100;
    
    [Column(TypeName = "decimal(5,2)")]
    public decimal Commission { get; set; } = 0;
    
    // Regulation
    public string? Regulation { get; set; }
    
    public string? LicenseNumber { get; set; }
    
    public string? Country { get; set; }
    
    // Fees
    [Column(TypeName = "decimal(5,2)")]
    public decimal DepositFeePercent { get; set; } = 0;
    
    [Column(TypeName = "decimal(18,8)")]
    public decimal DepositFeeFixed { get; set; } = 0;
    
    [Column(TypeName = "decimal(5,2)")]
    public decimal WithdrawalFeePercent { get; set; } = 0;
    
    [Column(TypeName = "decimal(18,8)")]
    public decimal WithdrawalFeeFixed { get; set; } = 0;
    
    public int SortOrder { get; set; } = 0;
    
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
    
    public string? MetaData { get; set; }
}

/// <summary>
/// Withdrawal limits configuration
/// </summary>
public class WithdrawalLimit
{
    [Key]
    public Guid Id { get; set; } = Guid.NewGuid();
    
    public Guid? UserId { get; set; } // Null for global limits
    
    public AccountType? AccountType { get; set; }
    
    [Column(TypeName = "decimal(18,8)")]
    public decimal DailyLimit { get; set; } = 5000;
    
    [Column(TypeName = "decimal(18,8)")]
    public decimal MonthlyLimit { get; set; } = 50000;
    
    [Column(TypeName = "decimal(18,8)")]
    public decimal SingleTransactionLimit { get; set; } = 10000;
    
    [Column(TypeName = "decimal(18,8)")]
    public decimal DailyUsed { get; set; } = 0;
    
    [Column(TypeName = "decimal(18,8)")]
    public decimal MonthlyUsed { get; set; } = 0;
    
    public DateTime DailyResetAt { get; set; } = DateTime.UtcNow.Date.AddDays(1);
    
    public DateTime MonthlyResetAt { get; set; } = new DateTime(DateTime.UtcNow.Year, DateTime.UtcNow.Month, 1).AddMonths(1);
    
    public bool IsActive { get; set; } = true;
    
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
}
