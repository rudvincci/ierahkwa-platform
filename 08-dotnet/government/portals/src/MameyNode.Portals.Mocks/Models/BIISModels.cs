namespace MameyNode.Portals.Mocks.Models;

// Models based on banking.proto and ledger.proto for BIIS

public enum TransactionStatus
{
    Unspecified = 0,
    Pending = 1,
    Processing = 2,
    Confirmed = 3,
    Failed = 4
}

public enum AccountStatus
{
    Unspecified = 0,
    Active = 1,
    Suspended = 2,
    Closed = 3,
    Frozen = 4
}

// PoolStatus is already defined in DexModels.cs

public class LiquidityPoolInfo
{
    public string PoolId { get; set; } = string.Empty;
    public string TreatyId { get; set; } = string.Empty;
    public string TotalLiquidity { get; set; } = "0";
    public string Currency { get; set; } = "USD";
    public PoolStatus Status { get; set; }
    public DateTime CreatedAt { get; set; }
    public string SourceBank { get; set; } = string.Empty;
    public string TargetBank { get; set; } = string.Empty;
    public string AvailableLiquidity { get; set; } = "0";
    public string ReservedLiquidity { get; set; } = "0";
}

public class CurrencyExchangeInfo
{
    public string ExchangeId { get; set; } = string.Empty;
    public string FromCurrency { get; set; } = string.Empty;
    public string ToCurrency { get; set; } = string.Empty;
    public string ExchangeRate { get; set; } = "1.0";
    public string Amount { get; set; } = "0";
    public string ConvertedAmount { get; set; } = "0";
    public TransactionStatus Status { get; set; }
    public DateTime CreatedAt { get; set; }
    public string TreatyId { get; set; } = string.Empty;
}

public class CrossBorderSettlementInfo
{
    public string SettlementId { get; set; } = string.Empty;
    public string SourceBank { get; set; } = string.Empty;
    public string TargetBank { get; set; } = string.Empty;
    public string Amount { get; set; } = "0";
    public string Currency { get; set; } = "USD";
    public string Status { get; set; } = "Pending";
    public DateTime CreatedAt { get; set; }
    public DateTime? ProcessedAt { get; set; }
    public string TransactionId { get; set; } = string.Empty;
    public string TreatyId { get; set; } = string.Empty;
}

public class InterbankChannelInfo
{
    public string ChannelId { get; set; } = string.Empty;
    public string BankA { get; set; } = string.Empty;
    public string BankB { get; set; } = string.Empty;
    public string ChannelType { get; set; } = string.Empty;
    public bool IsActive { get; set; }
    public DateTime EstablishedAt { get; set; }
    public string Capacity { get; set; } = "0";
    public string CurrentUsage { get; set; } = "0";
}

public class BlockchainTransparencyInfo
{
    public string BlockHash { get; set; } = string.Empty;
    public string TransactionId { get; set; } = string.Empty;
    public string FromAccount { get; set; } = string.Empty;
    public string ToAccount { get; set; } = string.Empty;
    public string Amount { get; set; } = "0";
    public string Currency { get; set; } = "USD";
    public DateTime Timestamp { get; set; }
    public ulong BlockHeight { get; set; }
    public bool IsConfirmed { get; set; }
}

public class AssetCollateralizationInfo
{
    public string CollateralId { get; set; } = string.Empty;
    public string AssetId { get; set; } = string.Empty;
    public string AssetType { get; set; } = string.Empty;
    public string CollateralValue { get; set; } = "0";
    public string Currency { get; set; } = "USD";
    public string LoanAmount { get; set; } = "0";
    public string CollateralRatio { get; set; } = "0";
    public DateTime CreatedAt { get; set; }
    public string Status { get; set; } = "Active";
}

public class IdentityComplianceInfo
{
    public string ComplianceId { get; set; } = string.Empty;
    public string AccountId { get; set; } = string.Empty;
    public string ComplianceType { get; set; } = string.Empty;
    public string Status { get; set; } = "Pending";
    public DateTime VerifiedAt { get; set; }
    public string TreatyId { get; set; } = string.Empty;
    public bool IsCompliant { get; set; }
    public List<string> Violations { get; set; } = new();
}

public class ZKPPrivacyInfo
{
    public string ProofId { get; set; } = string.Empty;
    public string TransactionId { get; set; } = string.Empty;
    public string ProofType { get; set; } = string.Empty;
    public bool IsVerified { get; set; }
    public DateTime CreatedAt { get; set; }
    public string PublicInput { get; set; } = string.Empty;
}

public class TreatyEnforcementInfo
{
    public string EnforcementId { get; set; } = string.Empty;
    public string TreatyId { get; set; } = string.Empty;
    public string ViolationType { get; set; } = string.Empty;
    public string Severity { get; set; } = "Low";
    public string Status { get; set; } = "Open";
    public DateTime ReportedAt { get; set; }
    public DateTime? ResolvedAt { get; set; }
    public string Description { get; set; } = string.Empty;
}

public class LiquidityRiskInfo
{
    public string RiskId { get; set; } = string.Empty;
    public string PoolId { get; set; } = string.Empty;
    public string RiskLevel { get; set; } = "Low";
    public string RiskScore { get; set; } = "0";
    public DateTime AssessedAt { get; set; }
    public List<string> RiskFactors { get; set; } = new();
    public string Recommendation { get; set; } = string.Empty;
}

