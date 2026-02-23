// ============================================================================
// IERAHKWA FUTUREHEAD - FOREX INVESTMENT SYSTEM
// Domain Entity: ForexTransaction
// Version: 1.0.0 - .NET 10 LTS
// © 2026 Ierahkwa Ne Kanienke Sovereign Government - All Rights Reserved
// ============================================================================

using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Ierahkwa.ForexInvestment.Domain.Entities;

/// <summary>
/// Represents all financial transactions in the Forex system
/// </summary>
public class ForexTransaction
{
    [Key]
    public Guid Id { get; set; } = Guid.NewGuid();
    
    [Required]
    public Guid UserId { get; set; }
    
    [Required]
    public Guid AccountId { get; set; }
    
    [Required]
    [StringLength(50)]
    public string TransactionNumber { get; set; } = string.Empty;
    
    [Required]
    public TransactionType Type { get; set; }
    
    [Required]
    [Column(TypeName = "decimal(18,8)")]
    public decimal Amount { get; set; }
    
    [Required]
    public string Currency { get; set; } = "USD";
    
    [Column(TypeName = "decimal(18,8)")]
    public decimal AmountInUSD { get; set; }
    
    [Column(TypeName = "decimal(18,8)")]
    public decimal Fee { get; set; } = 0;
    
    [Column(TypeName = "decimal(5,2)")]
    public decimal FeePercentage { get; set; } = 0;
    
    [Column(TypeName = "decimal(18,8)")]
    public decimal NetAmount { get; set; }
    
    [Column(TypeName = "decimal(18,8)")]
    public decimal BalanceBefore { get; set; }
    
    [Column(TypeName = "decimal(18,8)")]
    public decimal BalanceAfter { get; set; }
    
    public TransactionStatus Status { get; set; } = TransactionStatus.Pending;
    
    // For deposits
    public DepositMethod? DepositMethod { get; set; }
    
    public string? DepositAddress { get; set; }
    
    public string? BlockchainNetwork { get; set; }
    
    public string? BlockchainTxHash { get; set; }
    
    public int? BlockchainConfirmations { get; set; }
    
    // For withdrawals
    public string? WithdrawalAddress { get; set; }
    
    public string? BankAccountNumber { get; set; }
    
    public string? BankName { get; set; }
    
    public string? SwiftCode { get; set; }
    
    // For internal transfers
    public Guid? FromAccountId { get; set; }
    
    public Guid? ToAccountId { get; set; }
    
    // Related entities
    public Guid? InvestmentId { get; set; }
    
    public Guid? TradeId { get; set; }
    
    [StringLength(500)]
    public string? Description { get; set; }
    
    [StringLength(500)]
    public string? AdminNote { get; set; }
    
    // Audit
    public string? IpAddress { get; set; }
    
    public string? UserAgent { get; set; }
    
    public string? DeviceFingerprint { get; set; }
    
    // Risk Assessment
    [Column(TypeName = "decimal(5,2)")]
    public decimal RiskScore { get; set; } = 0;
    
    public bool FraudFlagged { get; set; } = false;
    
    public string? FraudReason { get; set; }
    
    public bool RequiresReview { get; set; } = false;
    
    public Guid? ReviewedBy { get; set; }
    
    public DateTime? ReviewedAt { get; set; }
    
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
    
    public DateTime? CompletedAt { get; set; }
    
    public DateTime? CancelledAt { get; set; }
    
    public string? MetaData { get; set; }
    
    // Navigation properties
    public virtual ForexAccount? Account { get; set; }
    public virtual Investment? Investment { get; set; }
}

public enum TransactionType
{
    Deposit = 0,
    Withdrawal = 1,
    InternalTransfer = 2,
    InvestmentDeposit = 3,
    InvestmentReturn = 4,
    Profit = 5,
    Loss = 6,
    Fee = 7,
    Bonus = 8,
    Refund = 9,
    SignalSubscription = 10,
    Commission = 11,
    Referral = 12
}

public enum TransactionStatus
{
    Pending = 0,
    Processing = 1,
    Completed = 2,
    Failed = 3,
    Cancelled = 4,
    Rejected = 5,
    Refunded = 6,
    OnHold = 7
}

public enum DepositMethod
{
    CreditCard = 0,
    BankTransfer = 1,
    Crypto = 2,
    EWallet = 3,
    InternalTransfer = 4
}
