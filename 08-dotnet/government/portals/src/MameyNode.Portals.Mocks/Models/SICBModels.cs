namespace MameyNode.Portals.Mocks.Models;

// Models based on banking.proto, ledger.proto, and upg.proto for SICB

public enum MonetaryInstrumentType
{
    Currency = 0,
    Bond = 1,
    Credit = 2,
    TreasuryNote = 3
}

public enum ReserveStatus
{
    Unspecified = 0,
    Adequate = 1,
    Low = 2,
    Critical = 3
}

public class MonetaryInstrumentInfo
{
    public string InstrumentId { get; set; } = string.Empty;
    public MonetaryInstrumentType Type { get; set; }
    public string Currency { get; set; } = "USD";
    public string TotalSupply { get; set; } = "0";
    public string CirculatingSupply { get; set; } = "0";
    public DateTime IssuedAt { get; set; }
    public string Issuer { get; set; } = string.Empty;
    public string Status { get; set; } = "Active";
}

public class LedgerReserveInfo
{
    public string ReserveId { get; set; } = string.Empty;
    public string Currency { get; set; } = "USD";
    public string TotalReserves { get; set; } = "0";
    public string RequiredReserves { get; set; } = "0";
    public string ReserveRatio { get; set; } = "0";
    public ReserveStatus Status { get; set; }
    public DateTime LastUpdated { get; set; }
    public string BackingAssets { get; set; } = string.Empty;
}

public class LendingInfo
{
    public string LoanId { get; set; } = string.Empty;
    public string BorrowerId { get; set; } = string.Empty;
    public string Amount { get; set; } = "0";
    public string Currency { get; set; } = "USD";
    public string InterestRate { get; set; } = "0";
    public string Status { get; set; } = "Active";
    public DateTime CreatedAt { get; set; }
    public DateTime? MaturityDate { get; set; }
    public string RemainingBalance { get; set; } = "0";
}

public class MonetaryPolicyInfo
{
    public string PolicyId { get; set; } = string.Empty;
    public string PolicyType { get; set; } = string.Empty;
    public string InterestRate { get; set; } = "0";
    public string ReserveRequirement { get; set; } = "0";
    public DateTime EffectiveDate { get; set; }
    public string Status { get; set; } = "Active";
    public string Description { get; set; } = string.Empty;
}

public class FiscalOperationInfo
{
    public string OperationId { get; set; } = string.Empty;
    public string OperationType { get; set; } = string.Empty;
    public string Amount { get; set; } = "0";
    public string Currency { get; set; } = "USD";
    public string ProgramId { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; }
    public string Status { get; set; } = "Pending";
    public string RecipientId { get; set; } = string.Empty;
}

public class TreasuryProgramInfo
{
    public string ProgramId { get; set; } = string.Empty;
    public string ProgramName { get; set; } = string.Empty;
    public string Budget { get; set; } = "0";
    public string Allocated { get; set; } = "0";
    public string Disbursed { get; set; } = "0";
    public string Currency { get; set; } = "USD";
    public DateTime StartDate { get; set; }
    public DateTime? EndDate { get; set; }
    public string Status { get; set; } = "Active";
}

public class ForeignExchangeInfo
{
    public string ExchangeId { get; set; } = string.Empty;
    public string FromCurrency { get; set; } = string.Empty;
    public string ToCurrency { get; set; } = string.Empty;
    public string ExchangeRate { get; set; } = "1.0";
    public string Amount { get; set; } = "0";
    public string ConvertedAmount { get; set; } = "0";
    public DateTime ExecutedAt { get; set; }
    public string SettlementId { get; set; } = string.Empty;
    public string Status { get; set; } = "Completed";
}

public class ComplianceEnforcementInfo
{
    public string EnforcementId { get; set; } = string.Empty;
    public string EntityId { get; set; } = string.Empty;
    public string EntityType { get; set; } = string.Empty;
    public string ViolationType { get; set; } = string.Empty;
    public string Severity { get; set; } = "Low";
    public string Status { get; set; } = "Open";
    public DateTime ReportedAt { get; set; }
    public string Description { get; set; } = string.Empty;
}

public class CitizenToolInfo
{
    public string ToolId { get; set; } = string.Empty;
    public string ToolName { get; set; } = string.Empty;
    public string ToolType { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public bool IsActive { get; set; }
    public DateTime CreatedAt { get; set; }
    public int UsageCount { get; set; }
}

public class SystemIntegrityInfo
{
    public string CheckId { get; set; } = string.Empty;
    public string CheckType { get; set; } = string.Empty;
    public bool IsHealthy { get; set; }
    public string HealthScore { get; set; } = "100";
    public DateTime CheckedAt { get; set; }
    public List<string> Issues { get; set; } = new();
    public string Status { get; set; } = "OK";
}

public class TreasuryInstrumentInfo
{
    public string InstrumentId { get; set; } = string.Empty;
    public string InstrumentType { get; set; } = string.Empty;
    public string FaceValue { get; set; } = "0";
    public string Currency { get; set; } = "USD";
    public string InterestRate { get; set; } = "0";
    public DateTime IssueDate { get; set; }
    public DateTime MaturityDate { get; set; }
    public string Status { get; set; } = "Active";
}

// UPG Models (from upg.proto)
public enum AdapterStatus
{
    Unspecified = 0,
    Active = 1,
    Inactive = 2,
    Error = 3
}

public enum POSTransactionStatus
{
    Unspecified = 0,
    Pending = 1,
    Authorized = 2,
    Captured = 3,
    Declined = 4,
    Refunded = 5
}

public enum SettlementStatus
{
    Unspecified = 0,
    Pending = 1,
    Processing = 2,
    Completed = 3,
    Failed = 4
}

public class ProtocolAdapterInfo
{
    public string AdapterId { get; set; } = string.Empty;
    public string ProtocolName { get; set; } = string.Empty;
    public string AdapterType { get; set; } = string.Empty;
    public Dictionary<string, string> Configuration { get; set; } = new();
    public DateTime RegisteredAt { get; set; }
    public AdapterStatus Status { get; set; }
}

public class RouteOption
{
    public string Rail { get; set; } = string.Empty;
    public string Fee { get; set; } = "0";
    public string EstimatedTime { get; set; } = string.Empty;
    public double ReliabilityScore { get; set; }
}

public class POSTransactionInfo
{
    public string TransactionId { get; set; } = string.Empty;
    public string MerchantId { get; set; } = string.Empty;
    public string TerminalId { get; set; } = string.Empty;
    public string Amount { get; set; } = "0";
    public string Currency { get; set; } = "USD";
    public string AuthorizationCode { get; set; } = string.Empty;
    public POSTransactionStatus Status { get; set; }
    public DateTime CreatedAt { get; set; }
}

public class MerchantSettlementInfo
{
    public string SettlementId { get; set; } = string.Empty;
    public string MerchantId { get; set; } = string.Empty;
    public string SettlementPeriod { get; set; } = string.Empty;
    public string TotalAmount { get; set; } = "0";
    public string Currency { get; set; } = "USD";
    public int TransactionCount { get; set; }
    public SettlementStatus Status { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? ProcessedAt { get; set; }
}

// ODL Models
public class ODLInfo
{
    public string ODLId { get; set; } = string.Empty;
    public string ProviderId { get; set; } = string.Empty;
    public string FromCurrency { get; set; } = string.Empty;
    public string ToCurrency { get; set; } = string.Empty;
    public string Amount { get; set; } = "0";
    public string ExchangeRate { get; set; } = "1.0";
    public string Status { get; set; } = "Pending";
    public DateTime CreatedAt { get; set; }
    public DateTime? ExecutedAt { get; set; }
}

