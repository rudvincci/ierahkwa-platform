using System;
using System.Collections.Generic;

namespace TradeX.Core.Models;

/// <summary>
/// P2P Trade Advertisement - Ierahkwa TradeX
/// Peer-to-peer trading with escrow protection
/// </summary>
public class P2PAd
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public Guid UserId { get; set; }
    public Guid AssetId { get; set; }
    
    public P2PAdType Type { get; set; } // Buy or Sell
    public decimal Price { get; set; }
    public string FiatCurrency { get; set; } = "USD";
    
    public decimal MinAmount { get; set; }
    public decimal MaxAmount { get; set; }
    public decimal AvailableAmount { get; set; }
    
    // Payment Methods
    public List<string> PaymentMethods { get; set; } = new();
    public int PaymentTimeLimit { get; set; } = 15; // minutes
    
    // Terms
    public string? Terms { get; set; }
    public string? AutoReply { get; set; }
    
    public bool IsActive { get; set; } = true;
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    
    public virtual User User { get; set; } = null!;
    public virtual Asset Asset { get; set; } = null!;
    public virtual ICollection<P2POrder> Orders { get; set; } = new List<P2POrder>();
}

/// <summary>
/// P2P Order with Escrow
/// </summary>
public class P2POrder
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public string OrderNumber { get; set; } = $"P2P-{DateTime.UtcNow:yyyyMMddHHmmss}-{Guid.NewGuid().ToString()[..8]}";
    
    public Guid AdId { get; set; }
    public Guid BuyerId { get; set; }
    public Guid SellerId { get; set; }
    
    public decimal CryptoAmount { get; set; }
    public decimal FiatAmount { get; set; }
    public decimal Price { get; set; }
    
    public string PaymentMethod { get; set; } = string.Empty;
    public P2POrderStatus Status { get; set; } = P2POrderStatus.Created;
    
    // Escrow
    public bool EscrowLocked { get; set; }
    public DateTime? EscrowLockedAt { get; set; }
    
    // Timestamps
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime? PaidAt { get; set; }
    public DateTime? ReleasedAt { get; set; }
    public DateTime? CancelledAt { get; set; }
    public DateTime? DisputedAt { get; set; }
    
    // Chat
    public virtual ICollection<P2PMessage> Messages { get; set; } = new List<P2PMessage>();
    
    public virtual P2PAd Ad { get; set; } = null!;
    public virtual User Buyer { get; set; } = null!;
    public virtual User Seller { get; set; } = null!;
}

/// <summary>
/// P2P Chat Message
/// </summary>
public class P2PMessage
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public Guid OrderId { get; set; }
    public Guid SenderId { get; set; }
    
    public string Message { get; set; } = string.Empty;
    public string? AttachmentUrl { get; set; }
    public bool IsRead { get; set; }
    
    public DateTime SentAt { get; set; } = DateTime.UtcNow;
    
    public virtual P2POrder Order { get; set; } = null!;
    public virtual User Sender { get; set; } = null!;
}

public enum P2PAdType { Buy, Sell }
public enum P2POrderStatus { Created, EscrowLocked, Paid, Released, Completed, Cancelled, Disputed }
