namespace Mamey.Auth.DecentralizedIdentifiers.Validation;

/// <summary>
/// Contract for pluggable ZK proof backend validation.
/// </summary>
public interface IZkProofBackend
{
    /// <summary>
    /// The proof type supported (e.g. "BbsBlsSignature2020").
    /// </summary>
    string ProofType { get; }

    /// <summary>
    /// Validates the ZKP proof for the given input and backend protocol.
    /// </summary>
    Task<bool> ValidateAsync(
        string credentialOrPresentationJson,
        object proofJson,
        string verificationMethod,
        string protocol,
        CancellationToken cancellationToken);
}