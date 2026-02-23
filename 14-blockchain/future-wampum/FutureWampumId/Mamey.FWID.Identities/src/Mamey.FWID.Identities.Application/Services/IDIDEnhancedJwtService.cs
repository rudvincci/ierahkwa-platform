namespace Mamey.FWID.Identities.Application.Services;

/// <summary>
/// Service interface for JWT token generation with DID claims.
/// Enhances JWT tokens to include DID-related claims when identity has linked DIDs.
/// 
/// TDD Reference: Lines 1594-1703 (Identity), Lines 2207-2336 (DID)
/// BDD Reference: Lines 326-434
/// </summary>
public interface IDIDEnhancedJwtService
{
    /// <summary>
    /// Generates a JWT token with DID claims.
    /// </summary>
    /// <param name="identityId">The identity ID.</param>
    /// <param name="primaryDid">The primary DID (if any).</param>
    /// <param name="additionalClaims">Additional claims to include.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>The generated token result.</returns>
    Task<DIDEnhancedTokenResult> GenerateTokenAsync(
        Guid identityId,
        string? primaryDid = null,
        Dictionary<string, string>? additionalClaims = null,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Refreshes a token, preserving DID claims.
    /// </summary>
    /// <param name="refreshToken">The refresh token.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>The new token result.</returns>
    Task<DIDEnhancedTokenResult> RefreshTokenAsync(
        string refreshToken,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Extracts DID claims from a token.
    /// </summary>
    /// <param name="token">The JWT token.</param>
    /// <returns>The DID claims if present.</returns>
    DIDClaimsInfo? ExtractDIDClaims(string token);

    /// <summary>
    /// Validates that a token has valid DID claims.
    /// </summary>
    /// <param name="token">The JWT token.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>Whether the DID claims are valid.</returns>
    Task<bool> ValidateDIDClaimsAsync(
        string token,
        CancellationToken cancellationToken = default);
}

/// <summary>
/// Result of DID-enhanced token generation.
/// </summary>
public class DIDEnhancedTokenResult
{
    /// <summary>
    /// Whether token generation was successful.
    /// </summary>
    public bool Success { get; set; }

    /// <summary>
    /// The access token.
    /// </summary>
    public string? AccessToken { get; set; }

    /// <summary>
    /// The refresh token.
    /// </summary>
    public string? RefreshToken { get; set; }

    /// <summary>
    /// When the access token expires.
    /// </summary>
    public DateTime? ExpiresAt { get; set; }

    /// <summary>
    /// Token type (always "Bearer").
    /// </summary>
    public string TokenType { get; set; } = "Bearer";

    /// <summary>
    /// DID claims included in the token.
    /// </summary>
    public DIDClaimsInfo? DIDClaims { get; set; }

    /// <summary>
    /// Error message if generation failed.
    /// </summary>
    public string? ErrorMessage { get; set; }
}

/// <summary>
/// DID-related claims in a JWT token.
/// </summary>
public class DIDClaimsInfo
{
    /// <summary>
    /// The primary DID string.
    /// </summary>
    public string? DID { get; set; }

    /// <summary>
    /// The DID method (e.g., "futurewampum", "web", "key").
    /// </summary>
    public string? DIDMethod { get; set; }

    /// <summary>
    /// Whether the DID is verified.
    /// </summary>
    public bool DIDVerified { get; set; }

    /// <summary>
    /// Whether the DID is verified on blockchain.
    /// </summary>
    public bool BlockchainVerified { get; set; }

    /// <summary>
    /// List of all linked DIDs.
    /// </summary>
    public List<string> LinkedDIDs { get; set; } = new();

    /// <summary>
    /// When the DID was last verified.
    /// </summary>
    public DateTime? VerifiedAt { get; set; }
}
