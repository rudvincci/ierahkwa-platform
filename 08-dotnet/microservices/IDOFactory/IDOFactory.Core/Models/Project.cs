namespace IDOFactory.Core.Models;

/// <summary>
/// Represents a project launching on the IDO platform
/// </summary>
public class Project
{
    public string Id { get; set; } = Guid.NewGuid().ToString();
    
    // Basic Info
    public string Name { get; set; } = string.Empty;
    public string Symbol { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public string LongDescription { get; set; } = string.Empty;
    public string LogoUrl { get; set; } = string.Empty;
    public string BannerUrl { get; set; } = string.Empty;
    
    // Token Details
    public string TokenAddress { get; set; } = string.Empty;
    public int TokenDecimals { get; set; } = 18;
    public decimal TotalSupply { get; set; }
    
    // Tokenomics
    public List<TokenDistribution> Tokenomics { get; set; } = new();
    
    // Social & Links
    public string Website { get; set; } = string.Empty;
    public string Whitepaper { get; set; } = string.Empty;
    public string Twitter { get; set; } = string.Empty;
    public string Telegram { get; set; } = string.Empty;
    public string Discord { get; set; } = string.Empty;
    public string Medium { get; set; } = string.Empty;
    public string Github { get; set; } = string.Empty;
    
    // Team
    public List<TeamMember> Team { get; set; } = new();
    
    // Roadmap
    public List<RoadmapItem> Roadmap { get; set; } = new();
    
    // Partners & Investors
    public List<Partner> Partners { get; set; } = new();
    
    // Audit
    public bool IsAudited { get; set; }
    public string AuditReport { get; set; } = string.Empty;
    public string AuditedBy { get; set; } = string.Empty;
    
    // KYC
    public bool IsKYCVerified { get; set; }
    public string KYCProvider { get; set; } = string.Empty;
    
    // Verification
    public ProjectVerificationStatus VerificationStatus { get; set; } = ProjectVerificationStatus.Pending;
    
    // Owner
    public string OwnerAddress { get; set; } = string.Empty;
    public string OwnerEmail { get; set; } = string.Empty;
    
    // Related IDOs
    public List<string> PoolIds { get; set; } = new();
    
    // Blockchain
    public int ChainId { get; set; } = 777777;
    public string Network { get; set; } = "ierahkwa-mainnet";
    
    // Timestamps
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
    public DateTime? ApprovedAt { get; set; }
}

public class TokenDistribution
{
    public string Category { get; set; } = string.Empty;
    public decimal Percentage { get; set; }
    public decimal Amount { get; set; }
    public string VestingSchedule { get; set; } = string.Empty;
    public string LockPeriod { get; set; } = string.Empty;
}

public class TeamMember
{
    public string Name { get; set; } = string.Empty;
    public string Role { get; set; } = string.Empty;
    public string Bio { get; set; } = string.Empty;
    public string PhotoUrl { get; set; } = string.Empty;
    public string LinkedIn { get; set; } = string.Empty;
    public string Twitter { get; set; } = string.Empty;
}

public class RoadmapItem
{
    public string Phase { get; set; } = string.Empty;
    public string Title { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public string Quarter { get; set; } = string.Empty;
    public bool IsCompleted { get; set; }
}

public class Partner
{
    public string Name { get; set; } = string.Empty;
    public string LogoUrl { get; set; } = string.Empty;
    public string Website { get; set; } = string.Empty;
    public string Type { get; set; } = string.Empty; // Investor, Partner, Advisor
}

public enum ProjectVerificationStatus
{
    Pending,
    UnderReview,
    Approved,
    Rejected,
    Suspended
}
