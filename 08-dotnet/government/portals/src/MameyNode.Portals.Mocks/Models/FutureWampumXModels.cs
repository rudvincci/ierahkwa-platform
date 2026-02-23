namespace MameyNode.Portals.Mocks.Models;

// Models based on dex.proto, crypto_exchange.proto, and bridge.proto for FutureWampumX
// Note: AMMModel, PoolStatus, PoolInfo, and SwapRoute are already defined in DexModels.cs

public class SwapInfo
{
    public string SwapId { get; set; } = string.Empty;
    public string PoolId { get; set; } = string.Empty;
    public string TokenIn { get; set; } = string.Empty;
    public string TokenOut { get; set; } = string.Empty;
    public string AmountIn { get; set; } = "0";
    public string AmountOut { get; set; } = "0";
    public string PriceImpact { get; set; } = "0";
    public string FeePaid { get; set; } = "0";
    public List<string> RouteTaken { get; set; } = new();
    public DateTime ExecutedAt { get; set; }
    public string Status { get; set; } = "Completed";
}

public class MultiCurrencyWalletInfo
{
    public string WalletId { get; set; } = string.Empty;
    public string AccountId { get; set; } = string.Empty;
    public Dictionary<string, string> Balances { get; set; } = new();
    public DateTime CreatedAt { get; set; }
    public bool IsActive { get; set; } = true;
}

public class ExchangeOrderInfo
{
    public string OrderId { get; set; } = string.Empty;
    public string TradingPair { get; set; } = string.Empty;
    public string OrderType { get; set; } = string.Empty; // buy, sell
    public string Price { get; set; } = "0";
    public string Quantity { get; set; } = "0";
    public string FilledQuantity { get; set; } = "0";
    public OrderStatus Status { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? FilledAt { get; set; }
}

public enum OrderStatus
{
    Unspecified = 0,
    Open = 1,
    PartiallyFilled = 2,
    Filled = 3,
    Cancelled = 4
}

public class TradingPairInfo
{
    public string TradingPair { get; set; } = string.Empty;
    public string BaseCurrency { get; set; } = string.Empty;
    public string QuoteCurrency { get; set; } = string.Empty;
    public string LastPrice { get; set; } = "0";
    public string Volume24h { get; set; } = "0";
    public string MinTradeAmount { get; set; } = "0";
    public string TickSize { get; set; } = "0.01";
}

public class ExchangeRateOracleInfo
{
    public string OracleId { get; set; } = string.Empty;
    public string FromCurrency { get; set; } = string.Empty;
    public string ToCurrency { get; set; } = string.Empty;
    public string ExchangeRate { get; set; } = "1.0";
    public DateTime UpdatedAt { get; set; }
    public string Source { get; set; } = string.Empty;
    public double Confidence { get; set; } = 1.0;
}

// Crypto Exchange Models (from crypto_exchange.proto)
public class CryptoOrderInfo
{
    public string OrderId { get; set; } = string.Empty;
    public string TradingPair { get; set; } = string.Empty;
    public string OrderType { get; set; } = string.Empty;
    public string Price { get; set; } = "0";
    public string Quantity { get; set; } = "0";
    public string FilledQuantity { get; set; } = "0";
    public OrderStatus Status { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? FilledAt { get; set; }
}

public class CustodyAccountInfo
{
    public string CustodyAccountId { get; set; } = string.Empty;
    public string AccountId { get; set; } = string.Empty;
    public string Currency { get; set; } = string.Empty;
    public string Balance { get; set; } = "0";
    public CustodyAccountType Type { get; set; }
    public DateTime CreatedAt { get; set; }
}

public enum CustodyAccountType
{
    Hot = 0,
    Cold = 1
}

public class StakingInfo
{
    public string AccountId { get; set; } = string.Empty;
    public string TotalStaked { get; set; } = "0";
    public string Currency { get; set; } = string.Empty;
    public string RewardsEarned { get; set; } = "0";
    public DateTime StakingStartDate { get; set; }
}

// Bridge Models (from bridge.proto)
public class AccountMappingInfo
{
    public string MappingId { get; set; } = string.Empty;
    public string BankingAccountId { get; set; } = string.Empty;
    public string BlockchainAccount { get; set; } = string.Empty;
    public string AccountType { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; }
    public MappingStatus Status { get; set; }
}

public enum MappingStatus
{
    Unspecified = 0,
    Active = 1,
    Suspended = 2,
    Removed = 3
}

public class BridgedIdentityInfo
{
    public string BridgeId { get; set; } = string.Empty;
    public string SourceSystem { get; set; } = string.Empty;
    public string SourceIdentityId { get; set; } = string.Empty;
    public string TargetSystem { get; set; } = string.Empty;
    public string TargetIdentityId { get; set; } = string.Empty;
    public Dictionary<string, string> IdentityData { get; set; } = new();
    public DateTime BridgedAt { get; set; }
    public DateTime? LastSyncedAt { get; set; }
    public BridgeStatus Status { get; set; }
}

public class BridgedTransactionInfo
{
    public string BridgeId { get; set; } = string.Empty;
    public string SourceSystem { get; set; } = string.Empty;
    public string SourceTransactionId { get; set; } = string.Empty;
    public string TargetSystem { get; set; } = string.Empty;
    public string TargetTransactionId { get; set; } = string.Empty;
    public string FromAccount { get; set; } = string.Empty;
    public string ToAccount { get; set; } = string.Empty;
    public string Amount { get; set; } = "0";
    public string Currency { get; set; } = "USD";
    public DateTime BridgedAt { get; set; }
    public BridgeStatus Status { get; set; }
}

public enum BridgeStatus
{
    Unspecified = 0,
    Pending = 1,
    Completed = 2,
    Failed = 3,
    Syncing = 4
}

// Travel Rule Models
public class TravelRuleInfo
{
    public string TravelRuleId { get; set; } = string.Empty;
    public string TransactionId { get; set; } = string.Empty;
    public string SenderVASP { get; set; } = string.Empty;
    public string ReceiverVASP { get; set; } = string.Empty;
    public string IVMS101Data { get; set; } = string.Empty;
    public string Status { get; set; } = "Pending";
    public DateTime CreatedAt { get; set; }
    public DateTime? ProcessedAt { get; set; }
}

public class VASPDirectoryInfo
{
    public string VASPId { get; set; } = string.Empty;
    public string VASPName { get; set; } = string.Empty;
    public string VASPAddress { get; set; } = string.Empty;
    public string PublicKey { get; set; } = string.Empty;
    public bool IsActive { get; set; }
    public DateTime RegisteredAt { get; set; }
}

// Trust Lines Models
public class TrustLineInfo
{
    public string TrustLineId { get; set; } = string.Empty;
    public string AccountA { get; set; } = string.Empty;
    public string AccountB { get; set; } = string.Empty;
    public string Currency { get; set; } = string.Empty;
    public string Limit { get; set; } = "0";
    public string Balance { get; set; } = "0";
    public bool IsActive { get; set; }
    public DateTime CreatedAt { get; set; }
}

