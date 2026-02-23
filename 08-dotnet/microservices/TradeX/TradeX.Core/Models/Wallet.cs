using System;
using System.Collections.Generic;

namespace TradeX.Core.Models;

/// <summary>
/// User Wallet - Ierahkwa TradeX
/// Supports multi-chain wallets with Ierahkwa Node integration
/// </summary>
public class Wallet
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public Guid UserId { get; set; }
    public Guid AssetId { get; set; }
    
    // Balances
    public decimal AvailableBalance { get; set; }
    public decimal LockedBalance { get; set; } // In orders or staking
    public decimal TotalBalance => AvailableBalance + LockedBalance;
    
    // Addresses
    public string? DepositAddress { get; set; }
    public string? PrivateKeyEncrypted { get; set; }
    
    // Status
    public bool IsActive { get; set; } = true;
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime? LastTransactionAt { get; set; }
    
    // Navigation
    public virtual User User { get; set; } = null!;
    public virtual Asset Asset { get; set; } = null!;
    public virtual ICollection<Transaction> Transactions { get; set; } = new List<Transaction>();
}

/// <summary>
/// Transaction Record
/// </summary>
public class Transaction
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public Guid WalletId { get; set; }
    public Guid UserId { get; set; }
    
    public TransactionType Type { get; set; }
    public decimal Amount { get; set; }
    public decimal Fee { get; set; }
    public decimal NetAmount => Amount - Fee;
    
    public string? TxHash { get; set; }
    public string? FromAddress { get; set; }
    public string? ToAddress { get; set; }
    
    public TransactionStatus Status { get; set; } = TransactionStatus.Pending;
    public string? Note { get; set; }
    
    // VIP - Prioridad y descuento en comisión
    public bool IsVipPriority { get; set; }
    public decimal VipFeeDiscountPercent { get; set; }
    
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime? ConfirmedAt { get; set; }
    
    public virtual Wallet Wallet { get; set; } = null!;
}

public enum TransactionType
{
    Deposit,
    Withdrawal,
    Trade,
    Swap,
    Transfer,
    StakingLock,
    StakingReward,
    ReferralBonus,
    GiftCard,
    Fee
}

public enum TransactionStatus
{
    Pending,
    Processing,
    Confirmed,
    Failed,
    Cancelled
}
