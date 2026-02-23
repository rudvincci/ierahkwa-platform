using Mamey.CQRS.Commands;
using Mamey.Microservice.Abstractions.Messaging;

namespace Mamey.FWID.Identities.Contracts.Commands;

/// <summary>
/// Command to authenticate a user using their DID.
/// Alternative to password-based authentication.
/// 
/// TDD Reference: Lines 1594-1703 (Identity Service)
/// BDD Reference: Lines 326-378 (V.2 Biometric Identity)
/// </summary>
[Contract]
public record AuthenticateWithDID(
    /// <summary>
    /// The DID being used for authentication.
    /// Must be linked to an existing identity.
    /// </summary>
    string DID,
    
    /// <summary>
    /// The challenge ID that was issued.
    /// </summary>
    string ChallengeId,
    
    /// <summary>
    /// The signed challenge (signature over the challenge nonce).
    /// </summary>
    string SignedChallenge,
    
    /// <summary>
    /// The nonce from the challenge.
    /// </summary>
    string Nonce,
    
    /// <summary>
    /// Optional: The verification method ID used (from DID Document).
    /// </summary>
    string? VerificationMethodId = null,
    
    /// <summary>
    /// Optional: The signature algorithm used.
    /// </summary>
    string? Algorithm = null
) : ICommand;

/// <summary>
/// Result of DID authentication.
/// </summary>
public record DIDAuthenticationResult
{
    /// <summary>
    /// Whether authentication was successful.
    /// </summary>
    public bool Success { get; init; }
    
    /// <summary>
    /// The identity ID that was authenticated.
    /// </summary>
    public Guid? IdentityId { get; init; }
    
    /// <summary>
    /// The DID that was used.
    /// </summary>
    public string DID { get; init; } = string.Empty;
    
    /// <summary>
    /// JWT access token for the authenticated session.
    /// </summary>
    public string? AccessToken { get; init; }
    
    /// <summary>
    /// JWT refresh token.
    /// </summary>
    public string? RefreshToken { get; init; }
    
    /// <summary>
    /// When the access token expires.
    /// </summary>
    public DateTime? ExpiresAt { get; init; }
    
    /// <summary>
    /// User roles from the identity.
    /// </summary>
    public List<string> Roles { get; init; } = new();
    
    /// <summary>
    /// Error message if authentication failed.
    /// </summary>
    public string? ErrorMessage { get; init; }
    
    /// <summary>
    /// Error code for programmatic handling.
    /// </summary>
    public string? ErrorCode { get; init; }
}

/// <summary>
/// Event published when DID authentication succeeds.
/// </summary>
[Contract]
public record DIDAuthenticationSucceeded(
    Guid IdentityId,
    string DID,
    string DIDMethod,
    DateTime AuthenticatedAt,
    string? DeviceInfo = null,
    string? IpAddress = null
);

/// <summary>
/// Event published when DID authentication fails.
/// </summary>
[Contract]
public record DIDAuthenticationFailed(
    string DID,
    string FailureReason,
    string? ErrorCode,
    DateTime FailedAt,
    string? DeviceInfo = null,
    string? IpAddress = null
);
