namespace Mamey.Blockchain.DID;

/// <summary>
/// Client interface for DID operations on MameyNode blockchain.
/// Implements W3C DID Core specification for did:futurewampum method.
/// </summary>
public interface IDIDClient
{
    /// <summary>
    /// Issues a new DID on the blockchain.
    /// </summary>
    /// <param name="controller">The controller (owner) of the DID.</param>
    /// <param name="verificationMethods">Initial verification methods.</param>
    /// <param name="services">Initial service endpoints.</param>
    /// <param name="metadata">Additional metadata.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>The result containing the new DID and DID Document.</returns>
    Task<IssueDIDResult> IssueDIDAsync(
        string controller,
        IReadOnlyList<DIDVerificationMethod>? verificationMethods = null,
        IReadOnlyList<DIDService>? services = null,
        Dictionary<string, string>? metadata = null,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Resolves a DID to its DID Document.
    /// </summary>
    /// <param name="did">The DID to resolve.</param>
    /// <param name="version">Specific version to resolve (0 = latest).</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>The DID Document and resolution metadata.</returns>
    Task<ResolveDIDResult> ResolveDIDAsync(
        string did,
        ulong version = 0,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Updates a DID Document.
    /// </summary>
    /// <param name="did">The DID to update.</param>
    /// <param name="didDocument">The new DID Document as JSON.</param>
    /// <param name="proof">Cryptographic proof of authorization.</param>
    /// <param name="reason">Reason for the update.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>The update result.</returns>
    Task<UpdateDIDResult> UpdateDIDDocumentAsync(
        string did,
        string didDocument,
        string proof,
        string? reason = null,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Revokes (deactivates) a DID.
    /// </summary>
    /// <param name="did">The DID to revoke.</param>
    /// <param name="proof">Cryptographic proof of authorization.</param>
    /// <param name="reason">Reason for revocation.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>The revocation result.</returns>
    Task<RevokeDIDResult> RevokeDIDAsync(
        string did,
        string proof,
        string? reason = null,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets the history of a DID (all versions).
    /// </summary>
    /// <param name="did">The DID to get history for.</param>
    /// <param name="limit">Maximum number of entries to return.</param>
    /// <param name="offset">Offset for pagination.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>The DID history.</returns>
    Task<DIDHistoryResult> GetDIDHistoryAsync(
        string did,
        int limit = 100,
        int offset = 0,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Verifies ownership of a DID.
    /// </summary>
    /// <param name="did">The DID to verify.</param>
    /// <param name="challenge">Challenge string to sign.</param>
    /// <param name="signature">Signature of the challenge.</param>
    /// <param name="verificationMethodId">Which verification method to use.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>The verification result.</returns>
    Task<VerifyOwnershipResult> VerifyDIDOwnershipAsync(
        string did,
        string challenge,
        string signature,
        string? verificationMethodId = null,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Lists DIDs for a controller.
    /// </summary>
    /// <param name="controller">The controller to list DIDs for.</param>
    /// <param name="includeDeactivated">Include deactivated DIDs.</param>
    /// <param name="limit">Maximum number of entries to return.</param>
    /// <param name="offset">Offset for pagination.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>The list of DIDs.</returns>
    Task<ListDIDsResult> ListDIDsAsync(
        string controller,
        bool includeDeactivated = false,
        int limit = 100,
        int offset = 0,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Adds a verification method to a DID Document.
    /// </summary>
    Task<ModifyDIDResult> AddVerificationMethodAsync(
        string did,
        DIDVerificationMethod verificationMethod,
        string proof,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Removes a verification method from a DID Document.
    /// </summary>
    Task<ModifyDIDResult> RemoveVerificationMethodAsync(
        string did,
        string verificationMethodId,
        string proof,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Adds a service endpoint to a DID Document.
    /// </summary>
    Task<ModifyDIDResult> AddServiceAsync(
        string did,
        DIDService service,
        string proof,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Removes a service endpoint from a DID Document.
    /// </summary>
    Task<ModifyDIDResult> RemoveServiceAsync(
        string did,
        string serviceId,
        string proof,
        CancellationToken cancellationToken = default);
}
