using Mamey.Identity.Decentralized.Core;

namespace Mamey.Identity.Decentralized.Abstractions;

/// <summary>
/// Checks the current status of a Verifiable Credential using its credentialStatus field.
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
}