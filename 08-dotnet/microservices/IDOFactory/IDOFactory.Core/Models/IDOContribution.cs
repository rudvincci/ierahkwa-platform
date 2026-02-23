namespace IDOFactory.Core.Models;

/// <summary>
/// Represents a user's contribution to an IDO pool
/// </summary>
public class IDOContribution
{
    public string Id { get; set; } = Guid.NewGuid().ToString();
    public string PoolId { get; set; } = string.Empty;
    public string UserAddress { get; set; } = string.Empty;
    
    public decimal Amount { get; set; }
    public string PaymentToken { get; set; } = "USDT";
    public decimal TokensAllocated { get; set; }
    public decimal TokensClaimed { get; set; }
    
    public string TransactionHash { get; set; } = string.Empty;
    public int ChainId { get; set; } = 777777;
    
    public ContributionStatus Status { get; set; } = ContributionStatus.Pending;
    
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime? ClaimedAt { get; set; }
    public DateTime? RefundedAt { get; set; }
    
    // Vesting tracking
    public decimal VestedAmount { get; set; }
    public DateTime? LastVestingClaim { get; set; }
}

public enum ContributionStatus
{
    Pending,
    Confirmed,
    Claimed,
    PartialClaimed,
    Refunded,
    Cancelled
}
