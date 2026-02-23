namespace Mamey.SICB.Treasury.Models;

/// <summary>
/// Treasury operation for SICB (Sovereign Indigenous Central Bank)
/// </summary>
public class TreasuryOperation
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public string OperationId { get; set; } = string.Empty;
    public TreasuryOperationType Type { get; set; }
    public TreasuryOperationStatus Status { get; set; } = TreasuryOperationStatus.Pending;
    
    // Amount
    public decimal Amount { get; set; }
    public string Currency { get; set; } = "WAMPUM";
    public decimal? ExchangeRate { get; set; }
    public decimal? AmountInUSD { get; set; }
    
    // Parties
    public string? FromAccount { get; set; }
    public string? ToAccount { get; set; }
    public string? BeneficiaryId { get; set; }
    public string? BeneficiaryName { get; set; }
    
    // Authorization
    public string InitiatedBy { get; set; } = string.Empty;
    public string? ApprovedBy { get; set; }
    public DateTime? ApprovedAt { get; set; }
    public int RequiredApprovals { get; set; } = 1;
    public int CurrentApprovals { get; set; }
    public List<TreasuryApproval> Approvals { get; set; } = new();
    
    // Treaty compliance
    public string? TreatyReference { get; set; }
    public bool RequiresTreatyCompliance { get; set; }
    public string? ComplianceProofId { get; set; } // ZKP reference
    public bool IsTreatyCompliant { get; set; }
    
    // Governance
    public bool RequiresGovernanceVote { get; set; }
    public string? GovernanceProposalId { get; set; }
    public bool GovernanceApproved { get; set; }
    
    // Blockchain
    public string? TransactionHash { get; set; }
    public ulong? BlockNumber { get; set; }
    
    // Metadata
    public string? Description { get; set; }
    public string? Category { get; set; }
    public string? Reference { get; set; }
    public Dictionary<string, string>? Metadata { get; set; }
    
    // Timestamps
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime? ExecutedAt { get; set; }
    public DateTime? CompletedAt { get; set; }
}

public class TreasuryApproval
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public Guid OperationId { get; set; }
    public string ApproverId { get; set; } = string.Empty;
    public string ApproverName { get; set; } = string.Empty;
    public string ApproverRole { get; set; } = string.Empty;
    public ApprovalDecision Decision { get; set; }
    public string? Comments { get; set; }
    public string? Signature { get; set; }
    public DateTime DecisionAt { get; set; } = DateTime.UtcNow;
}

/// <summary>
/// Currency issuance by the central bank
/// </summary>
public class CurrencyIssuance
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public string IssuanceId { get; set; } = string.Empty;
    public IssuanceType Type { get; set; }
    
    // Currency details
    public string CurrencySymbol { get; set; } = string.Empty;
    public decimal Amount { get; set; }
    public string RecipientAccount { get; set; } = string.Empty;
    
    // Backing
    public string? BackingAsset { get; set; }
    public decimal? BackingAmount { get; set; }
    public string? CollateralProofId { get; set; }
    
    // Authorization
    public string AuthorizedBy { get; set; } = string.Empty;
    public string? TreatyReference { get; set; }
    public string? GovernanceProposalId { get; set; }
    
    // Blockchain
    public string? MintTransactionHash { get; set; }
    public ulong? BlockNumber { get; set; }
    
    public IssuanceStatus Status { get; set; } = IssuanceStatus.Pending;
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime? IssuedAt { get; set; }
}

/// <summary>
/// Treasury account/reserve
/// </summary>
public class TreasuryAccount
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public string AccountId { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public TreasuryAccountType Type { get; set; }
    
    // Balances
    public Dictionary<string, decimal> Balances { get; set; } = new();
    
    // Controls
    public decimal DailyLimit { get; set; }
    public decimal MonthlyLimit { get; set; }
    public decimal DailyUsed { get; set; }
    public decimal MonthlyUsed { get; set; }
    
    // Signatories
    public List<string> Signatories { get; set; } = new();
    public int RequiredSignatures { get; set; } = 1;
    
    public bool IsActive { get; set; } = true;
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}

// Enums
public enum TreasuryOperationType
{
    Disbursement,
    Issuance,
    Redemption,
    Transfer,
    Allocation,
    Reserve,
    Investment,
    Fee,
    Dividend,
    Grant,
    Loan,
    Repayment
}

public enum TreasuryOperationStatus
{
    Pending,
    AwaitingApproval,
    Approved,
    Processing,
    Executed,
    Completed,
    Rejected,
    Cancelled,
    Failed
}

public enum ApprovalDecision { Approved, Rejected, Deferred }
public enum IssuanceType { Mint, Burn, Reissue }
public enum IssuanceStatus { Pending, Approved, Minted, Completed, Rejected, Cancelled }
public enum TreasuryAccountType { Reserve, Operating, Investment, Grant, Escrow, Humanitarian }
