namespace IDOFactory.Core.Models;

/// <summary>
/// Represents a Token Lock/Vesting contract for liquidity or team tokens
/// </summary>
public class TokenLocker
{
    public string Id { get; set; } = Guid.NewGuid().ToString();
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    
    // Token Details
    public string TokenAddress { get; set; } = string.Empty;
    public string TokenSymbol { get; set; } = string.Empty;
    public string TokenName { get; set; } = string.Empty;
    public int TokenDecimals { get; set; } = 18;
    public string LogoUrl { get; set; } = string.Empty;
    
    // Lock Configuration
    public decimal Amount { get; set; }
    public decimal AmountUnlocked { get; set; }
    public decimal AmountRemaining => Amount - AmountUnlocked;
    
    public DateTime LockDate { get; set; }
    public DateTime UnlockDate { get; set; }
    public bool IsLinearVesting { get; set; }
    public int VestingPeriodDays { get; set; }
    
    // Lock Type
    public TokenLockType LockType { get; set; } = TokenLockType.Team;
    
    // Ownership
    public string OwnerAddress { get; set; } = string.Empty;
    public string BeneficiaryAddress { get; set; } = string.Empty;
    
    // Contract
    public string LockerContractAddress { get; set; } = string.Empty;
    public int ChainId { get; set; } = 777777;
    public string Network { get; set; } = "ierahkwa-mainnet";
    
    // Status
    public TokenLockStatus Status { get; set; } = TokenLockStatus.Active;
    
    // Timestamps
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
    public DateTime? WithdrawnAt { get; set; }
    
    // Link to IDO (optional)
    public string? RelatedPoolId { get; set; }
    
    // Fees
    public decimal LockFee { get; set; }
    public bool FeePaid { get; set; }
    
    // Transactions
    public List<TokenLockTransaction> Transactions { get; set; } = new();
}

public enum TokenLockType
{
    Team,
    Liquidity,
    Marketing,
    Development,
    Advisor,
    Presale,
    Private,
    Other
}

public enum TokenLockStatus
{
    Pending,
    Active,
    PartiallyUnlocked,
    FullyUnlocked,
    Cancelled
}

public class TokenLockTransaction
{
    public string Id { get; set; } = Guid.NewGuid().ToString();
    public string LockerId { get; set; } = string.Empty;
    public TokenLockTransactionType Type { get; set; }
    public decimal Amount { get; set; }
    public string TransactionHash { get; set; } = string.Empty;
    public DateTime Timestamp { get; set; } = DateTime.UtcNow;
}

public enum TokenLockTransactionType
{
    Lock,
    Unlock,
    EmergencyWithdraw,
    TransferOwnership
}
