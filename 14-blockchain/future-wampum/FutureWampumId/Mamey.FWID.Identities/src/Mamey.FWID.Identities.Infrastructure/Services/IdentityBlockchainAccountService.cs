using Mamey.FWID.Identities.Application.Clients;
using Mamey.FWID.Identities.Application.Services;
using Mamey.FWID.Identities.Infrastructure.Metrics;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace Mamey.FWID.Identities.Infrastructure.Services;

/// <summary>
/// Service implementation for blockchain account operations.
/// Provides caching, metrics, and error handling for blockchain queries.
/// 
/// TDD Reference: Lines 354-407 (Identity domain methods)
/// BDD Reference: Lines 326-378 (V.2 Biometric Identity)
/// </summary>
internal sealed class IdentityBlockchainAccountService : IIdentityBlockchainAccountService
{
    private readonly IMameyNodeBankingClient _bankingClient;
    private readonly IMemoryCache _cache;
    private readonly ILogger<IdentityBlockchainAccountService> _logger;
    private readonly TimeSpan _cacheTtl;
    private readonly bool _enabled;

    private const string CacheKeyPrefix = "blockchain_account_";
    private const string BalanceCacheKeyPrefix = "blockchain_balance_";

    public IdentityBlockchainAccountService(
        IMameyNodeBankingClient bankingClient,
        IMemoryCache cache,
        ILogger<IdentityBlockchainAccountService> logger,
        IConfiguration configuration)
    {
        _bankingClient = bankingClient ?? throw new ArgumentNullException(nameof(bankingClient));
        _cache = cache ?? throw new ArgumentNullException(nameof(cache));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));

        _enabled = configuration.GetValue<bool>("mameyNode:enabled", false);
        _cacheTtl = configuration.GetValue<TimeSpan>("mameyNode:banking:cacheTtl", TimeSpan.FromMinutes(5));

        _logger.LogInformation(
            "Initialized {Service} with CacheTTL={CacheTTL}, Enabled={Enabled}",
            nameof(IdentityBlockchainAccountService), _cacheTtl, _enabled);
    }

    /// <inheritdoc />
    public async Task<BlockchainAccountDetails?> GetBlockchainAccountAsync(
        Guid identityId,
        CancellationToken cancellationToken = default)
    {
        if (!_enabled)
        {
            _logger.LogDebug("MameyNode integration is disabled");
            return new BlockchainAccountDetails
            {
                IdentityId = identityId,
                Success = false,
                ErrorMessage = "MameyNode integration is disabled"
            };
        }

        var cacheKey = $"{CacheKeyPrefix}{identityId}";
        var startTime = DateTime.UtcNow;

        // Try cache first
        if (_cache.TryGetValue(cacheKey, out BlockchainAccountDetails? cachedAccount) && cachedAccount != null)
        {
            _logger.LogDebug("Cache hit for blockchain account: IdentityId={IdentityId}", identityId);
            cachedAccount.FromCache = true;
            BlockchainMetrics.OperationsTotal.WithLabels("identities", "get_account", "cache_hit").Inc();
            return cachedAccount;
        }

        try
        {
            _logger.LogDebug("Fetching blockchain account: IdentityId={IdentityId}", identityId);

            var accountInfo = await _bankingClient.GetAccountInfoAsync(
                identityId.ToString(),
                cancellationToken);

            var durationSeconds = (DateTime.UtcNow - startTime).TotalSeconds;

            if (accountInfo != null && accountInfo.Success)
            {
                var details = new BlockchainAccountDetails
                {
                    IdentityId = identityId,
                    BlockchainAddress = accountInfo.BlockchainAccount,
                    Balance = accountInfo.Balance,
                    Currency = accountInfo.Currency,
                    Status = accountInfo.Status,
                    Success = true,
                    FromCache = false
                };

                // Cache the result
                _cache.Set(cacheKey, details, _cacheTtl);

                BlockchainMetrics.RecordOperationSuccess("identities", "get_account", durationSeconds);
                _logger.LogInformation(
                    "Retrieved blockchain account: IdentityId={IdentityId}, Address={Address}",
                    identityId, accountInfo.BlockchainAccount);

                return details;
            }

            BlockchainMetrics.RecordOperationFailure("identities", "get_account", durationSeconds);
            
            return new BlockchainAccountDetails
            {
                IdentityId = identityId,
                Success = false,
                ErrorMessage = accountInfo?.ErrorMessage ?? "Account not found"
            };
        }
        catch (Exception ex)
        {
            var durationSeconds = (DateTime.UtcNow - startTime).TotalSeconds;
            BlockchainMetrics.RecordOperationFailure("identities", "get_account", durationSeconds);

            _logger.LogError(ex, "Error fetching blockchain account: IdentityId={IdentityId}", identityId);

            return new BlockchainAccountDetails
            {
                IdentityId = identityId,
                Success = false,
                ErrorMessage = $"Error: {ex.Message}"
            };
        }
    }

    /// <inheritdoc />
    public async Task<BlockchainBalanceDetails?> GetBlockchainBalanceAsync(
        Guid identityId,
        CancellationToken cancellationToken = default)
    {
        if (!_enabled)
        {
            _logger.LogDebug("MameyNode integration is disabled");
            return new BlockchainBalanceDetails
            {
                IdentityId = identityId,
                Success = false,
                ErrorMessage = "MameyNode integration is disabled"
            };
        }

        var cacheKey = $"{BalanceCacheKeyPrefix}{identityId}";
        var startTime = DateTime.UtcNow;

        // Try cache first (shorter TTL for balance)
        var balanceCacheTtl = TimeSpan.FromMinutes(1); // Balance changes more frequently
        if (_cache.TryGetValue(cacheKey, out BlockchainBalanceDetails? cachedBalance) && cachedBalance != null)
        {
            _logger.LogDebug("Cache hit for blockchain balance: IdentityId={IdentityId}", identityId);
            cachedBalance.FromCache = true;
            BlockchainMetrics.OperationsTotal.WithLabels("identities", "get_balance", "cache_hit").Inc();
            return cachedBalance;
        }

        try
        {
            _logger.LogDebug("Fetching blockchain balance: IdentityId={IdentityId}", identityId);

            var balance = await _bankingClient.GetBalanceAsync(
                identityId.ToString(),
                cancellationToken);

            var durationSeconds = (DateTime.UtcNow - startTime).TotalSeconds;

            if (balance != null && balance.Success)
            {
                var details = new BlockchainBalanceDetails
                {
                    IdentityId = identityId,
                    Balance = balance.Balance,
                    Currency = balance.Currency,
                    Success = true,
                    FromCache = false
                };

                // Cache with shorter TTL
                _cache.Set(cacheKey, details, balanceCacheTtl);

                BlockchainMetrics.RecordOperationSuccess("identities", "get_balance", durationSeconds);
                _logger.LogDebug(
                    "Retrieved blockchain balance: IdentityId={IdentityId}, Balance={Balance} {Currency}",
                    identityId, balance.Balance, balance.Currency);

                return details;
            }

            BlockchainMetrics.RecordOperationFailure("identities", "get_balance", durationSeconds);

            return new BlockchainBalanceDetails
            {
                IdentityId = identityId,
                Success = false,
                ErrorMessage = balance?.ErrorMessage ?? "Balance not available"
            };
        }
        catch (Exception ex)
        {
            var durationSeconds = (DateTime.UtcNow - startTime).TotalSeconds;
            BlockchainMetrics.RecordOperationFailure("identities", "get_balance", durationSeconds);

            _logger.LogError(ex, "Error fetching blockchain balance: IdentityId={IdentityId}", identityId);

            return new BlockchainBalanceDetails
            {
                IdentityId = identityId,
                Success = false,
                ErrorMessage = $"Error: {ex.Message}"
            };
        }
    }

    /// <inheritdoc />
    public async Task<BlockchainTransactionHistory> GetTransactionHistoryAsync(
        Guid identityId,
        int limit = 20,
        int offset = 0,
        CancellationToken cancellationToken = default)
    {
        if (!_enabled)
        {
            _logger.LogDebug("MameyNode integration is disabled");
            return new BlockchainTransactionHistory
            {
                IdentityId = identityId,
                Success = false,
                ErrorMessage = "MameyNode integration is disabled"
            };
        }

        var startTime = DateTime.UtcNow;

        try
        {
            _logger.LogDebug(
                "Fetching transaction history: IdentityId={IdentityId}, Limit={Limit}, Offset={Offset}",
                identityId, limit, offset);

            // Note: IMameyNodeBankingClient doesn't have ListTransactions yet
            // This is a placeholder for when it's implemented
            var durationSeconds = (DateTime.UtcNow - startTime).TotalSeconds;
            
            _logger.LogWarning(
                "Transaction history not yet implemented for IdentityId={IdentityId}",
                identityId);

            return new BlockchainTransactionHistory
            {
                IdentityId = identityId,
                Transactions = new List<BlockchainTransactionInfo>(),
                TotalCount = 0,
                Success = false,
                ErrorMessage = "Transaction history not yet implemented"
            };
        }
        catch (Exception ex)
        {
            var durationSeconds = (DateTime.UtcNow - startTime).TotalSeconds;
            BlockchainMetrics.RecordOperationFailure("identities", "get_transactions", durationSeconds);

            _logger.LogError(ex, "Error fetching transaction history: IdentityId={IdentityId}", identityId);

            return new BlockchainTransactionHistory
            {
                IdentityId = identityId,
                Success = false,
                ErrorMessage = $"Error: {ex.Message}"
            };
        }
    }

    /// <inheritdoc />
    public void InvalidateCache(Guid identityId)
    {
        var accountCacheKey = $"{CacheKeyPrefix}{identityId}";
        var balanceCacheKey = $"{BalanceCacheKeyPrefix}{identityId}";

        _cache.Remove(accountCacheKey);
        _cache.Remove(balanceCacheKey);

        _logger.LogDebug("Invalidated blockchain cache for IdentityId={IdentityId}", identityId);
    }
}
