namespace IDOFactory.Core.Models;

/// <summary>
/// Represents an IDO (Initial DEX Offering) Pool
/// </summary>
public class IDOPool
{
    public string Id { get; set; } = Guid.NewGuid().ToString();
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public string TokenAddress { get; set; } = string.Empty;
    public string TokenSymbol { get; set; } = string.Empty;
    public string TokenName { get; set; } = string.Empty;
    public int TokenDecimals { get; set; } = 18;
    public string LogoUrl { get; set; } = string.Empty;
    
    // Pool Configuration
    public decimal TokenPrice { get; set; }
    public decimal SoftCap { get; set; }
    public decimal HardCap { get; set; }
    public decimal MinContribution { get; set; }
    public decimal MaxContribution { get; set; }
    public decimal TotalTokensForSale { get; set; }
    public decimal TokensSold { get; set; }
    public decimal FundsRaised { get; set; }
    
    // Payment Token (e.g., USDT, ETH, BNB)
    public string PaymentToken { get; set; } = "USDT";
    public string PaymentTokenAddress { get; set; } = string.Empty;
    
    // Timing
    public DateTime RegistrationStart { get; set; }
    public DateTime RegistrationEnd { get; set; }
    public DateTime SaleStart { get; set; }
    public DateTime SaleEnd { get; set; }
    public DateTime? ClaimStart { get; set; }
    
    // Vesting
    public bool HasVesting { get; set; }
    public decimal TgePercentage { get; set; } = 100; // Token Generation Event %
    public int VestingMonths { get; set; }
    public int CliffMonths { get; set; }
    
    // Status
    public IDOPoolStatus Status { get; set; } = IDOPoolStatus.Draft;
    public int ParticipantsCount { get; set; }
    
    // Blockchain
    public int ChainId { get; set; } = 777777; // Ierahkwa Sovereign Blockchain
    public string Network { get; set; } = "ierahkwa-mainnet";
    public string PoolContractAddress { get; set; } = string.Empty;
    
    // Social Links
    public string Website { get; set; } = string.Empty;
    public string Twitter { get; set; } = string.Empty;
    public string Telegram { get; set; } = string.Empty;
    public string Discord { get; set; } = string.Empty;
    public string Medium { get; set; } = string.Empty;
    public string Whitepaper { get; set; } = string.Empty;
    
    // Pool Type
    public IDOPoolType PoolType { get; set; } = IDOPoolType.Public;
    public bool RequiresKYC { get; set; }
    public bool WhitelistEnabled { get; set; }
    
    // Admin
    public string CreatorAddress { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
    
    // Fees
    public decimal PlatformFeePercentage { get; set; } = 3.0m; // 3% platform fee
    
    public List<IDOContribution> Contributions { get; set; } = new();
    public List<string> Whitelist { get; set; } = new();
}

public enum IDOPoolStatus
{
    Draft,
    Upcoming,
    Registration,
    Live,
    Filled,
    Ended,
    Cancelled,
    Finalized,
    Claiming
}

public enum IDOPoolType
{
    Public,
    Private,
    Whitelist,
    Tiered,
    Lottery
}
