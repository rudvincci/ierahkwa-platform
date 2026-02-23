namespace Mamey.Identity.Decentralized.VC;

/// <summary>
/// Service abstraction for verifying Verifiable Credentials (VCs) and Verifiable Presentations (VPs).
/// Supports both JWT and Linked Data Proof (LD-Proof) formats.
/// </summary>
public interface IVerifiableCredentialValidator
{
    /// <summary>
    /// Parses and validates a VC or VP token (JWT or JSON-LD).
    /// </summary>
    /// <param name="vcOrVpToken">The token as string (JWT, JSON, etc).</param>
    /// <param name="cancellationToken">The cancellation token. </param>
    /// <returns>A <see cref="VerifiableCredentialValidationResult"/> including validity, claims, and status.</returns>
    Task<VerifiableCredentialValidationResult> ValidateAsync(string vcOrVpToken, CancellationToken cancellationToken = default);

    /// <summary>
    /// Validates with advanced options (expected issuer, challenge, status, etc.).
    /// </summary>
    /// <param name="request">Structured verification request (recommended).</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>Validation result with claims, issuer, subject, status, etc.</returns>
    Task<VerifiableCredentialValidationResult> ValidateAsync(
        CredentialVerifyRequest request,
        CancellationToken cancellationToken = default);
    
    
}

