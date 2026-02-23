namespace MameyNode.Portals.Mocks.Models;

// Models based on payments.proto and wallet.proto for FutureWampumPay
// Note: PaymentModels.cs already exists with PaymentInfo, MerchantPaymentInfo, MultisigPaymentInfo, DisbursementInfo, etc.

public class PaymentWalletInfo
{
    public string InvoiceId { get; set; } = string.Empty;
    public string FromAccount { get; set; } = string.Empty;
    public string ToAccount { get; set; } = string.Empty;
    public string Amount { get; set; } = "0";
    public string Currency { get; set; } = "USD";
    public string Status { get; set; } = "Pending";
    public DateTime CreatedAt { get; set; }
    public DateTime? DueDate { get; set; }
    public DateTime? PaidAt { get; set; }
    public string Description { get; set; } = string.Empty;
}

public class QRNFCPaymentInfo
{
    public string PaymentId { get; set; } = string.Empty;
    public string PaymentType { get; set; } = string.Empty; // QR, NFC
    public string Amount { get; set; } = "0";
    public string Currency { get; set; } = "USD";
    public string QRCodeData { get; set; } = string.Empty;
    public string NFCData { get; set; } = string.Empty;
    public PaymentStatus Status { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? CompletedAt { get; set; }
}

public class OfflinePaymentInfo
{
    public string PaymentId { get; set; } = string.Empty;
    public string FromAccount { get; set; } = string.Empty;
    public string ToAccount { get; set; } = string.Empty;
    public string Amount { get; set; } = "0";
    public string Currency { get; set; } = "USD";
    public string Status { get; set; } = "Pending";
    public DateTime CreatedAt { get; set; }
    public DateTime? SyncedAt { get; set; }
    public bool IsSynced { get; set; }
    public string Signature { get; set; } = string.Empty;
}

public class CurrencyConversionInfo
{
    public string ConversionId { get; set; } = string.Empty;
    public string FromCurrency { get; set; } = string.Empty;
    public string ToCurrency { get; set; } = string.Empty;
    public string Amount { get; set; } = "0";
    public string ExchangeRate { get; set; } = "1.0";
    public string ConvertedAmount { get; set; } = "0";
    public string Fee { get; set; } = "0";
    public DateTime ExecutedAt { get; set; }
    public string Status { get; set; } = "Completed";
}

public class LedgerSyncInfo
{
    public string SyncId { get; set; } = string.Empty;
    public string TransactionId { get; set; } = string.Empty;
    public string Status { get; set; } = "Pending";
    public DateTime CreatedAt { get; set; }
    public DateTime? SyncedAt { get; set; }
    public string BlockHash { get; set; } = string.Empty;
    public bool IsConfirmed { get; set; }
}

public class ZKPPaymentInfo
{
    public string PaymentId { get; set; } = string.Empty;
    public string ProofId { get; set; } = string.Empty;
    public string Amount { get; set; } = "0";
    public string Currency { get; set; } = "USD";
    public bool IsVerified { get; set; }
    public DateTime CreatedAt { get; set; }
    public string PublicInput { get; set; } = string.Empty;
    public string Status { get; set; } = "Pending";
}

// MultisigPaymentInfo is already defined in PaymentModels.cs

// MultisigPaymentStatus is already defined in PaymentModels.cs

public class DelegationInfo
{
    public string DelegationId { get; set; } = string.Empty;
    public string DelegatorId { get; set; } = string.Empty;
    public string DelegateId { get; set; } = string.Empty;
    public string Permissions { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; }
    public DateTime? ExpiresAt { get; set; }
    public string Status { get; set; } = "Active";
}

public class GeoPolicyInfo
{
    public string PolicyId { get; set; } = string.Empty;
    public string AccountId { get; set; } = string.Empty;
    public List<string> AllowedRegions { get; set; } = new();
    public List<string> BlockedRegions { get; set; } = new();
    public bool IsActive { get; set; }
    public DateTime CreatedAt { get; set; }
}

public class ConsentPolicyInfo
{
    public string PolicyId { get; set; } = string.Empty;
    public string AccountId { get; set; } = string.Empty;
    public string ConsentType { get; set; } = string.Empty;
    public bool IsConsented { get; set; }
    public DateTime ConsentedAt { get; set; }
    public DateTime? ExpiresAt { get; set; }
    public string Status { get; set; } = "Active";
}

public class CredentialAccessInfo
{
    public string AccessId { get; set; } = string.Empty;
    public string AccountId { get; set; } = string.Empty;
    public string CredentialId { get; set; } = string.Empty;
    public string AccessType { get; set; } = string.Empty;
    public DateTime AccessedAt { get; set; }
    public string Purpose { get; set; } = string.Empty;
}

public class EscrowInfo
{
    public string EscrowId { get; set; } = string.Empty;
    public string BuyerAccount { get; set; } = string.Empty;
    public string SellerAccount { get; set; } = string.Empty;
    public string Amount { get; set; } = "0";
    public string Currency { get; set; } = "USD";
    public string Condition { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; }
    public DateTime? ExpiryTime { get; set; }
    public string Status { get; set; } = "Active";
}

public class GroupWalletInfo
{
    public string GroupWalletId { get; set; } = string.Empty;
    public string GroupName { get; set; } = string.Empty;
    public List<string> Members { get; set; } = new();
    public int RequiredSignatures { get; set; }
    public Dictionary<string, string> Balances { get; set; } = new();
    public DateTime CreatedAt { get; set; }
    public bool IsActive { get; set; } = true;
}

public class AutomatedDeductionInfo
{
    public string DeductionId { get; set; } = string.Empty;
    public string FromAccount { get; set; } = string.Empty;
    public string ToAccount { get; set; } = string.Empty;
    public string Amount { get; set; } = "0";
    public string Currency { get; set; } = "USD";
    public string Frequency { get; set; } = "Monthly";
    public DateTime StartDate { get; set; }
    public DateTime? EndDate { get; set; }
    public DateTime NextDeductionDate { get; set; }
    public string Status { get; set; } = "Active";
}

public class DisputeInfo
{
    public string DisputeId { get; set; } = string.Empty;
    public string TransactionId { get; set; } = string.Empty;
    public string InitiatorId { get; set; } = string.Empty;
    public string DisputeType { get; set; } = string.Empty;
    public string Reason { get; set; } = string.Empty;
    public string Status { get; set; } = "Open";
    public DateTime CreatedAt { get; set; }
    public DateTime? ResolvedAt { get; set; }
    public string Resolution { get; set; } = string.Empty;
}

public class VaultInfo
{
    public string VaultId { get; set; } = string.Empty;
    public string AccountId { get; set; } = string.Empty;
    public string VaultType { get; set; } = string.Empty; // Hot, Warm, Cold
    public Dictionary<string, string> Balances { get; set; } = new();
    public DateTime CreatedAt { get; set; }
    public bool IsActive { get; set; } = true;
    public string SecurityLevel { get; set; } = "Standard";
}

// Smart Contract Models (from general.proto)
public class SmartContractInfo
{
    public string ContractAddress { get; set; } = string.Empty;
    public string DeployerAccount { get; set; } = string.Empty;
    public string ContractCodeHash { get; set; } = string.Empty;
    public DateTime DeployedAt { get; set; }
    public Dictionary<string, string> State { get; set; } = new();
    public string ContractType { get; set; } = string.Empty;
}

// Account Abstraction Models
public class SmartWalletInfo
{
    public string WalletId { get; set; } = string.Empty;
    public string OwnerId { get; set; } = string.Empty;
    public string WalletType { get; set; } = string.Empty; // EOA, SmartWallet, etc.
    public Dictionary<string, string> Balances { get; set; } = new();
    public DateTime CreatedAt { get; set; }
    public bool IsActive { get; set; } = true;
}

public class SessionKeyInfo
{
    public string SessionKeyId { get; set; } = string.Empty;
    public string WalletId { get; set; } = string.Empty;
    public string Permissions { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; }
    public DateTime? ExpiresAt { get; set; }
    public bool IsActive { get; set; } = true;
}

// Programmable Payment Models
public class PaymentConditionInfo
{
    public string ConditionId { get; set; } = string.Empty;
    public string ConditionType { get; set; } = string.Empty;
    public string ConditionExpression { get; set; } = string.Empty;
    public bool IsMet { get; set; }
    public DateTime EvaluatedAt { get; set; }
}

public class ExpiringBalanceInfo
{
    public string BalanceId { get; set; } = string.Empty;
    public string AccountId { get; set; } = string.Empty;
    public string Amount { get; set; } = "0";
    public string Currency { get; set; } = "USD";
    public DateTime ExpiresAt { get; set; }
    public bool IsExpired { get; set; }
    public string Status { get; set; } = "Active";
}

// Payment Channel Models
public class PaymentChannelInfo
{
    public string ChannelId { get; set; } = string.Empty;
    public string PartyA { get; set; } = string.Empty;
    public string PartyB { get; set; } = string.Empty;
    public string TotalCapacity { get; set; } = "0";
    public string BalanceA { get; set; } = "0";
    public string BalanceB { get; set; } = "0";
    public string Currency { get; set; } = "USD";
    public string Status { get; set; } = "Open";
    public DateTime CreatedAt { get; set; }
    public DateTime? ClosedAt { get; set; }
}

// Pathfinding Models
public class PathfindingRouteInfo
{
    public string RouteId { get; set; } = string.Empty;
    public string FromCurrency { get; set; } = string.Empty;
    public string ToCurrency { get; set; } = string.Empty;
    public string Amount { get; set; } = "0";
    public string ExpectedOutput { get; set; } = "0";
    public string PriceImpact { get; set; } = "0";
    public List<string> Pools { get; set; } = new();
    public List<string> Tokens { get; set; } = new();
    public string Fee { get; set; } = "0";
}

