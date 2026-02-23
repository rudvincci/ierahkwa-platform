using Mamey.Auth.DecentralizedIdentifiers.Core;

namespace Mamey.Auth.DecentralizedIdentifiers.Abstractions;

/// <summary>
/// Enhanced interface for checking the current status of Verifiable Credentials using various revocation mechanisms.
/// </summary>
public interface ICredentialStatusService
{
    /// <summary>
    /// Returns true if the credential is revoked per StatusList2021 or other mechanism.
    /// </summary>
    Task<bool> IsRevokedAsync(
        string statusListCredentialUrl,
        int statusListIndex,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Checks if the credential referenced by status is active (not revoked or suspended).
    /// </summary>
    /// <param name="status">The credentialStatus object from the VC.</param>
    /// <param name="cancellationToken">Token for cancellation.</param>
    /// <returns>true if active; false if revoked or suspended.</returns>
    Task<bool> IsCredentialActiveAsync(CredentialStatus status, CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets the full status (e.g. "active", "revoked", "suspended", "unknown").
    /// </summary>
    Task<string> GetCredentialStatusAsync(CredentialStatus status, CancellationToken cancellationToken = default);

    /// <summary>
    /// Checks the status of a credential via its status list entry or status ID URI.
    /// </summary>
    /// <param name="statusId">A URI or identifier pointing to the credential status entry (usually from the credentialStatus field).</param>
    /// <param name="cancellationToken">Cancellation support.</param>
    /// <returns>CredentialStatusResult describing status and details.</returns>
    Task<CredentialStatusResult> CheckStatusAsync(string statusId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Checks the status of multiple credentials in batch.
    /// </summary>
    /// <param name="statusIds">Collection of status IDs to check.</param>
    /// <param name="cancellationToken">Cancellation support.</param>
    /// <returns>Dictionary mapping status IDs to their results.</returns>
    Task<Dictionary<string, CredentialStatusResult>> CheckStatusBatchAsync(
        IEnumerable<string> statusIds, 
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Checks if multiple credentials are revoked in batch.
    /// </summary>
    /// <param name="credentials">Collection of credential status information.</param>
    /// <param name="cancellationToken">Cancellation support.</param>
    /// <returns>Dictionary mapping credential keys to revocation status.</returns>
    Task<Dictionary<string, bool>> IsRevokedBatchAsync(
        IEnumerable<(string statusListCredentialUrl, int statusListIndex)> credentials,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Invalidates cached status for a specific credential.
    /// </summary>
    /// <param name="statusId">The status ID to invalidate.</param>
    void InvalidateStatusCache(string statusId);

    /// <summary>
    /// Clears all cached status information.
    /// </summary>
    void ClearStatusCache();
}