namespace Mamey.Auth.DecentralizedIdentifiers.Validation;

/// <summary>
/// Service for validating zero-knowledge proofs in verifiable credentials, presentations, or any supported ZKP protocol.
/// </summary>
public interface IZkProofValidator
{
    /// <summary>
    /// Validates a ZKP proof for a given VC/VP document using the provided verification method and protocol.
    /// </summary>
    /// <param name="credentialOrPresentationJson">The VC/VP as JSON string.</param>
    /// <param name="proofJson">The extracted ZKP proof section as JSON string or object.</param>
    /// <param name="verificationMethod">The DID verification method to use (or null for default).</param>
    /// <param name="proofType">The ZKP proof type (e.g., "BbsBlsSignature2020", "Groth16", etc).</param>
    /// <param name="protocol">Optional: the ZKP protocol family or backend to use.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>Returns true if proof is valid, false otherwise.</returns>
    Task<bool> ValidateAsync(
        string credentialOrPresentationJson,
        object proofJson,
        string verificationMethod,
        string proofType,
        string protocol = null,
        CancellationToken cancellationToken = default
    );
}