namespace MameyNode.Portals.Mocks.Models;

// Models based on ledger.proto and ledger_integration.proto

public class TransactionInfo
{
    public string TransactionId { get; set; } = string.Empty;
    public string FromAccount { get; set; } = string.Empty;
    public string ToAccount { get; set; } = string.Empty;
    public string Amount { get; set; } = "0";
    public string Currency { get; set; } = "USD";
    public TransactionStatus Status { get; set; }
    public string BlockHash { get; set; } = string.Empty;
    public DateTime Timestamp { get; set; }
    public string TransactionType { get; set; } = string.Empty;
    public Dictionary<string, string> Metadata { get; set; } = new();
}

public class LedgerBlockInfo
{
    public string BlockHash { get; set; } = string.Empty;
    public string Previous { get; set; } = string.Empty;
    public string Account { get; set; } = string.Empty;
    public DateTime Timestamp { get; set; }
    public ulong BlockHeight { get; set; }
    public bool IsConfirmed { get; set; }
    public string BlockType { get; set; } = string.Empty;
    public List<string> Transactions { get; set; } = new();
}

public class TransparencyDashboardInfo
{
    public string TotalTransactions { get; set; } = "0";
    public string TotalVolume { get; set; } = "0";
    public int ActiveAccounts { get; set; }
    public List<CurrencyVolumeInfo> CurrencyVolumes { get; set; } = new();
    public DateTime LastUpdated { get; set; }
}

public class CurrencyVolumeInfo
{
    public string Currency { get; set; } = string.Empty;
    public string Volume { get; set; } = "0";
    public int TransactionCount { get; set; }
}

public class AuditTrailInfo
{
    public string EntryId { get; set; } = string.Empty;
    public string EntityType { get; set; } = string.Empty;
    public string EntityId { get; set; } = string.Empty;
    public string Action { get; set; } = string.Empty;
    public string Actor { get; set; } = string.Empty;
    public DateTime Timestamp { get; set; }
    public Dictionary<string, string> Details { get; set; } = new();
}

public class SynchronizationInfo
{
    public string SyncId { get; set; } = string.Empty;
    public string SourceSystem { get; set; } = string.Empty;
    public string TargetSystem { get; set; } = string.Empty;
    public string Status { get; set; } = "Pending";
    public DateTime StartedAt { get; set; }
    public DateTime? CompletedAt { get; set; }
    public int TotalRecords { get; set; }
    public int SyncedRecords { get; set; }
    public int FailedRecords { get; set; }
}

// Ledger Integration Models (from ledger_integration.proto)
public enum TransactionEventType
{
    Unknown = 0,
    Created = 1,
    Initiated = 2,
    Executing = 3,
    Executed = 4,
    Confirming = 5,
    Confirmed = 6,
    Failed = 7,
    Cancelled = 8,
    Reversed = 9,
    Settled = 10
}

public enum TransactionCategory
{
    Unknown = 0,
    Payment = 1,
    Transfer = 2,
    Deposit = 3,
    Withdrawal = 4,
    Lending = 5,
    Settlement = 6,
    Custody = 7,
    Treasury = 8,
    Fee = 9,
    Refund = 10
}

public class TransactionLogInfo
{
    public string LogId { get; set; } = string.Empty;
    public string TransactionId { get; set; } = string.Empty;
    public string BlockchainHash { get; set; } = string.Empty;
    public TransactionEventType EventType { get; set; }
    public string FromAccount { get; set; } = string.Empty;
    public string ToAccount { get; set; } = string.Empty;
    public string Amount { get; set; } = "0";
    public string Currency { get; set; } = "USD";
    public TransactionCategory Category { get; set; }
    public Dictionary<string, string> Metadata { get; set; } = new();
    public string Description { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; }
    public DateTime? ConfirmedAt { get; set; }
    public ComplianceStatus ComplianceStatus { get; set; }
    public string ComplianceReason { get; set; } = string.Empty;
}

public class TransactionFlagInfo
{
    public string FlagId { get; set; } = string.Empty;
    public string TransactionId { get; set; } = string.Empty;
    public string FlagType { get; set; } = string.Empty;
    public string Severity { get; set; } = "Low";
    public string Reason { get; set; } = string.Empty;
    public FlagStatus Status { get; set; }
    public DateTime CreatedAt { get; set; }
}

public class CurrencyInfo
{
    public string CurrencyId { get; set; } = string.Empty;
    public string CurrencyCode { get; set; } = string.Empty;
    public string CurrencyName { get; set; } = string.Empty;
    public string Symbol { get; set; } = string.Empty;
    public int Decimals { get; set; }
    public string Issuer { get; set; } = string.Empty;
    public DateTime RegisteredAt { get; set; }
    public CurrencyStatus Status { get; set; }
    public Dictionary<string, string> Metadata { get; set; } = new();
}

public enum CurrencyStatus
{
    Unspecified = 0,
    Active = 1,
    Suspended = 2,
    Deprecated = 3
}

public class CreditEntryInfo
{
    public string CreditId { get; set; } = string.Empty;
    public string AccountId { get; set; } = string.Empty;
    public string CreditType { get; set; } = string.Empty;
    public string Amount { get; set; } = "0";
    public string Currency { get; set; } = "USD";
    public string Description { get; set; } = string.Empty;
    public DateTime Timestamp { get; set; }
}

public class CreditSummaryInfo
{
    public string AccountId { get; set; } = string.Empty;
    public string TotalCredit { get; set; } = "0";
    public string AvailableCredit { get; set; } = "0";
    public string UsedCredit { get; set; } = "0";
    public string Currency { get; set; } = "USD";
    public DateTime LastUpdated { get; set; }
}

