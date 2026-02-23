namespace Mamey.Identity.Decentralized.Abstractions;

/// <summary>
/// Defines a contract for signing and verifying JSON-LD Linked Data Proofs,
/// including canonicalization, signature creation, and proof verification.
/// </summary>
public interface IProofService
{
    /// <summary>
    /// Signs a JSON-LD input using the specified verification method, creating a Linked Data Proof.
    /// </summary>
    /// <param name="jsonLd">The JSON-LD object to sign.</param>
    /// <param name="verificationMethodId">The ID of the verification method (e.g., DID#key).</param>
    /// <param name="privateKey">The private key bytes.</param>
    /// <param name="proofPurpose">Purpose (e.g., assertionMethod).</param>
    /// <param name="type">Signature type (e.g., Ed25519Signature2020).</param>
    /// <param name="created">Optional creation time for proof.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>The JSON-LD proof object.</returns>
    Task<object> CreateProofAsync(
        string jsonLd,
        string verificationMethodId,
        byte[] privateKey,
        string proofPurpose,
        string type,
        string created = null,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Verifies the Linked Data Proof of a JSON-LD object.
    /// </summary>
    /// <param name="jsonLd">The JSON-LD object to verify.</param>
    /// <param name="proof">The proof object extracted from JSON-LD.</param>
    /// <param name="publicKey">The public key bytes.</param>
    /// <param name="type">Signature type (e.g., Ed25519Signature2020).</param>
    /// <param name="proofPurpose">Purpose (e.g., assertionMethod).</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>True if the proof is valid, otherwise false.</returns>
    Task<bool> VerifyProofAsync(
        string jsonLd,
        object proof,
        byte[] publicKey,
        string type,
        string proofPurpose,
        CancellationToken cancellationToken = default);

    Task<bool> IsRevokedAsync(string statusListCredentialUrl, int statusListIndex,
        CancellationToken cancellationToken = default);
}