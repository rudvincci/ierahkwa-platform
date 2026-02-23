namespace Mamey.Blockchain.Banking;

/// <summary>
/// Send transaction result
/// </summary>
public class SendTransactionResult
{
    public string TransactionId { get; set; } = string.Empty;
    public TransactionStatus Status { get; set; }
    public string BlockHash { get; set; } = string.Empty;
    public string ErrorMessage { get; set; } = string.Empty;
}

/// <summary>
/// Balance result
/// </summary>
public class BalanceResult
{
    public string Balance { get; set; } = string.Empty;
    public string Currency { get; set; } = string.Empty;
    public bool Success { get; set; }
}

/// <summary>
/// Account information result
/// </summary>
public class AccountInfoResult
{
    public string AccountId { get; set; } = string.Empty;
    public string BlockchainAccount { get; set; } = string.Empty;
    public string Balance { get; set; } = string.Empty;
    public string Currency { get; set; } = string.Empty;
    public AccountStatus Status { get; set; }
    public bool Success { get; set; }
}

/// <summary>
/// Transaction status result
/// </summary>
public class TransactionStatusResult
{
    public string TransactionId { get; set; } = string.Empty;
    public TransactionStatus Status { get; set; }
    public string FromAccount { get; set; } = string.Empty;
    public string ToAccount { get; set; } = string.Empty;
    public string Amount { get; set; } = string.Empty;
    public string Currency { get; set; } = string.Empty;
    public string BlockHash { get; set; } = string.Empty;
    public bool Success { get; set; }
}

/// <summary>
/// Create account result
/// </summary>
public class CreateAccountResult
{
    public string AccountId { get; set; } = string.Empty;
    public string BlockchainAccount { get; set; } = string.Empty;
    public bool Success { get; set; }
    public string ErrorMessage { get; set; } = string.Empty;
}

/// <summary>
/// Transaction list result
/// </summary>
public class TransactionListResult
{
    public List<TransactionInfo> Transactions { get; set; } = new();
    public int TotalCount { get; set; }
    public bool Success { get; set; }
    public string ErrorMessage { get; set; } = string.Empty;
}

/// <summary>
/// Transaction information
/// </summary>
public class TransactionInfo
{
    public string TransactionId { get; set; } = string.Empty;
    public string FromAccount { get; set; } = string.Empty;
    public string ToAccount { get; set; } = string.Empty;
    public string Amount { get; set; } = string.Empty;
    public string Currency { get; set; } = string.Empty;
    public TransactionStatus Status { get; set; }
    public string BlockHash { get; set; } = string.Empty;
    public DateTime Timestamp { get; set; }
}

/// <summary>
/// Transaction status enum
/// </summary>
public enum TransactionStatus
{
    Pending = 0,
    Processing = 1,
    Confirmed = 2,
    Failed = 3
}

/// <summary>
/// Account status enum
/// </summary>
public enum AccountStatus
{
    Active = 0,
    Suspended = 1,
    Closed = 2
}


























