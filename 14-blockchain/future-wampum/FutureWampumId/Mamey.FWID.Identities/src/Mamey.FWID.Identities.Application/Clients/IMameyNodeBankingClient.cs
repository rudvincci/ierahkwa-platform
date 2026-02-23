namespace Mamey.FWID.Identities.Application.Clients;

/// <summary>
/// Client for integrating with MameyNode Banking Service via gRPC
/// Provides blockchain account management for Identity creation
/// </summary>
internal interface IMameyNodeBankingClient
{
    /// <summary>
    /// Creates a new blockchain account for an Identity
    /// </summary>
    /// <param name="accountId">The account ID (typically the Identity identifier)</param>
    /// <param name="currency">Currency code (default: USD)</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Blockchain account address if successful, null otherwise</returns>
    Task<string?> CreateAccountAsync(string accountId, string currency = "USD", CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets the balance for a blockchain account
    /// </summary>
    /// <param name="accountId">The account ID</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Account balance information</returns>
    Task<AccountBalance?> GetBalanceAsync(string accountId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets account information from MameyNode
    /// </summary>
    /// <param name="accountId">The account ID</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Account information if found, null otherwise</returns>
    Task<AccountInfo?> GetAccountInfoAsync(string accountId, CancellationToken cancellationToken = default);
}

/// <summary>
/// Account balance information
/// </summary>
public class AccountBalance
{
    public string Balance { get; set; } = string.Empty;
    public string Currency { get; set; } = string.Empty;
    public bool Success { get; set; }
    public string? ErrorMessage { get; set; }
}

/// <summary>
/// Account information from MameyNode
/// </summary>
public class AccountInfo
{
    public string AccountId { get; set; } = string.Empty;
    public string BlockchainAccount { get; set; } = string.Empty;
    public string Balance { get; set; } = string.Empty;
    public string Currency { get; set; } = string.Empty;
    public string Status { get; set; } = string.Empty;
    public bool Success { get; set; }
    public string? ErrorMessage { get; set; }
}




