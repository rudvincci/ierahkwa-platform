using Mamey.FWID.Identities.Domain.Entities;

namespace Mamey.FWID.Identities.Application.Services;

/// <summary>
/// Interface for resilient blockchain operations with retry policies, circuit breaker, and monitoring.
/// Wraps IMameyNodeBankingClient with enterprise-grade resilience patterns.
/// 
/// TDD Reference: Line 1594-1703 (Feature Domain 1: Biometric Identity)
/// BDD Reference: Lines 326-378 (V.2 Biometric Identity Registration)
/// </summary>
internal interface IResilientBlockchainClient
{
    /// <summary>
    /// Creates a blockchain account with exponential backoff retry and circuit breaker.
    /// </summary>
    /// <param name="identityId">The identity identifier.</param>
    /// <param name="currency">The currency for the account (default: USD).</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>The blockchain account address, or null if creation failed after all retries.</returns>
    Task<BlockchainAccountResult> CreateAccountWithRetryAsync(
        string identityId,
        string currency = "USD",
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Retries failed blockchain account creation for an identity.
    /// Used by background sync service.
    /// </summary>
    /// <param name="identity">The identity with failed blockchain account creation.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>Result of the retry attempt.</returns>
    Task<BlockchainAccountResult> RetryFailedAccountCreationAsync(
        Identity identity,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Checks if the circuit breaker is open (blockchain service unavailable).
    /// </summary>
    /// <returns>True if circuit breaker is open, false otherwise.</returns>
    bool IsCircuitBreakerOpen { get; }

    /// <summary>
    /// Gets the current health status of the blockchain connection.
    /// </summary>
    BlockchainHealthStatus GetHealthStatus();

    /// <summary>
    /// Records a failed blockchain account creation for later retry.
    /// </summary>
    /// <param name="identityId">The identity identifier.</param>
    /// <param name="error">The error message.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    Task RecordFailedCreationAsync(
        string identityId,
        string error,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets identities with failed blockchain account creation for retry.
    /// </summary>
    /// <param name="limit">Maximum number of identities to retrieve.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>List of identity IDs with failed blockchain account creation.</returns>
    Task<IReadOnlyList<string>> GetFailedAccountCreationsAsync(
        int limit = 100,
        CancellationToken cancellationToken = default);
}

/// <summary>
/// Result of a blockchain account creation operation.
/// </summary>
internal record BlockchainAccountResult(
    bool Success,
    string? AccountAddress,
    string? ErrorMessage,
    int AttemptCount,
    TimeSpan TotalDuration);

/// <summary>
/// Health status of the blockchain connection.
/// </summary>
internal record BlockchainHealthStatus(
    bool IsHealthy,
    bool CircuitBreakerOpen,
    int ConsecutiveFailures,
    DateTime? LastSuccessfulOperation,
    DateTime? LastFailedOperation,
    string? LastErrorMessage);
