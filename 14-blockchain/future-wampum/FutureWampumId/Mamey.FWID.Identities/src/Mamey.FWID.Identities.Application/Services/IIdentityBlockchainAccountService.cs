using Mamey.FWID.Identities.Application.Clients;

namespace Mamey.FWID.Identities.Application.Services;

/// <summary>
/// Service interface for blockchain account operations related to identities.
/// Provides account retrieval, balance checking, and caching.
/// 
/// TDD Reference: Lines 354-407 (Identity domain methods)
/// BDD Reference: Lines 326-378 (V.2 Biometric Identity)
/// </summary>
public interface IIdentityBlockchainAccountService
{
    /// <summary>
    /// Gets the blockchain account information for an identity.
    /// Results are cached with configurable TTL.
    /// </summary>
    /// <param name="identityId">The identity ID.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>Account information or null if not found.</returns>
    Task<BlockchainAccountDetails?> GetBlockchainAccountAsync(
        Guid identityId,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets the blockchain balance for an identity's account.
    /// Results are cached with configurable TTL.
    /// </summary>
    /// <param name="identityId">The identity ID.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>Balance information or null if not found.</returns>
    Task<BlockchainBalanceDetails?> GetBlockchainBalanceAsync(
        Guid identityId,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets the transaction history for an identity's blockchain account.
    /// </summary>
    /// <param name="identityId">The identity ID.</param>
    /// <param name="limit">Maximum number of transactions.</param>
    /// <param name="offset">Offset for pagination.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>Transaction history or empty list.</returns>
    Task<BlockchainTransactionHistory> GetTransactionHistoryAsync(
        Guid identityId,
        int limit = 20,
        int offset = 0,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Invalidates the cached account information for an identity.
    /// Call this after account-modifying operations.
    /// </summary>
    /// <param name="identityId">The identity ID.</param>
    void InvalidateCache(Guid identityId);
}

/// <summary>
/// Detailed blockchain account information.
/// </summary>
public class BlockchainAccountDetails
{
    /// <summary>Identity ID associated with the account.</summary>
    public Guid IdentityId { get; set; }

    /// <summary>The blockchain account address (hex-encoded).</summary>
    public string BlockchainAddress { get; set; } = string.Empty;

    /// <summary>Current balance.</summary>
    public string Balance { get; set; } = "0";

    /// <summary>Account currency.</summary>
    public string Currency { get; set; } = "USD";

    /// <summary>Account status (Active, Suspended, Closed, Frozen).</summary>
    public string Status { get; set; } = "Active";

    /// <summary>When the account was created.</summary>
    public DateTime? CreatedAt { get; set; }

    /// <summary>Whether the retrieval was successful.</summary>
    public bool Success { get; set; }

    /// <summary>Error message if retrieval failed.</summary>
    public string? ErrorMessage { get; set; }

    /// <summary>When this data was retrieved from cache/blockchain.</summary>
    public DateTime RetrievedAt { get; set; } = DateTime.UtcNow;

    /// <summary>Whether this data was served from cache.</summary>
    public bool FromCache { get; set; }
}

/// <summary>
/// Blockchain balance details.
/// </summary>
public class BlockchainBalanceDetails
{
    /// <summary>Identity ID associated with the account.</summary>
    public Guid IdentityId { get; set; }

    /// <summary>Current balance as string (to handle large numbers).</summary>
    public string Balance { get; set; } = "0";

    /// <summary>Currency code.</summary>
    public string Currency { get; set; } = "USD";

    /// <summary>Whether the retrieval was successful.</summary>
    public bool Success { get; set; }

    /// <summary>Error message if retrieval failed.</summary>
    public string? ErrorMessage { get; set; }

    /// <summary>When this balance was retrieved.</summary>
    public DateTime RetrievedAt { get; set; } = DateTime.UtcNow;

    /// <summary>Whether this data was served from cache.</summary>
    public bool FromCache { get; set; }
}

/// <summary>
/// Blockchain transaction history for an identity.
/// </summary>
public class BlockchainTransactionHistory
{
    /// <summary>Identity ID associated with the account.</summary>
    public Guid IdentityId { get; set; }

    /// <summary>List of transactions.</summary>
    public List<BlockchainTransactionInfo> Transactions { get; set; } = new();

    /// <summary>Total count of transactions (for pagination).</summary>
    public int TotalCount { get; set; }

    /// <summary>Whether the retrieval was successful.</summary>
    public bool Success { get; set; }

    /// <summary>Error message if retrieval failed.</summary>
    public string? ErrorMessage { get; set; }
}

/// <summary>
/// Individual transaction information.
/// </summary>
public class BlockchainTransactionInfo
{
    /// <summary>Transaction ID.</summary>
    public string TransactionId { get; set; } = string.Empty;

    /// <summary>Source account.</summary>
    public string FromAccount { get; set; } = string.Empty;

    /// <summary>Destination account.</summary>
    public string ToAccount { get; set; } = string.Empty;

    /// <summary>Transaction amount.</summary>
    public string Amount { get; set; } = "0";

    /// <summary>Currency code.</summary>
    public string Currency { get; set; } = string.Empty;

    /// <summary>Transaction status.</summary>
    public string Status { get; set; } = string.Empty;

    /// <summary>Block hash if confirmed.</summary>
    public string? BlockHash { get; set; }

    /// <summary>Transaction timestamp.</summary>
    public DateTime Timestamp { get; set; }
}
