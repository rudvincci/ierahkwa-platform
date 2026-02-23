using Mamey.CQRS.Commands;
using Mamey.Microservice.Abstractions.Messaging;

namespace Mamey.FWID.Identities.Contracts.Commands;

/// <summary>
/// Command to verify DID-based MFA (second factor).
/// Used after successful password authentication.
/// 
/// TDD Reference: Lines 1594-1703 (Identity)
/// BDD Reference: Lines 326-378 (V.2 Biometric Identity)
/// </summary>
[Contract]
public record VerifyDIDMFA(
    /// <summary>
    /// The identity ID that initiated MFA.
    /// </summary>
    Guid IdentityId,
    
    /// <summary>
    /// The MFA session ID (from initial auth).
    /// </summary>
    string MfaSessionId,
    
    /// <summary>
    /// The DID being used for MFA.
    /// </summary>
    string DID,
    
    /// <summary>
    /// The signed challenge.
    /// </summary>
    string SignedChallenge,
    
    /// <summary>
    /// Optional: Device DID for trusted device bypass.
    /// </summary>
    string? DeviceDID = null,
    
    /// <summary>
    /// Optional: Trust this device for future MFA bypass.
    /// </summary>
    bool TrustDevice = false
) : ICommand;

/// <summary>
/// Result of DID MFA verification.
/// </summary>
public record DIDMFAResult
{
    /// <summary>Whether MFA verification was successful.</summary>
    public bool Success { get; init; }
    
    /// <summary>The identity ID.</summary>
    public Guid? IdentityId { get; init; }
    
    /// <summary>Access token (issued after successful MFA).</summary>
    public string? AccessToken { get; init; }
    
    /// <summary>Refresh token.</summary>
    public string? RefreshToken { get; init; }
    
    /// <summary>Token expiration.</summary>
    public DateTime? ExpiresAt { get; init; }
    
    /// <summary>Whether device was registered as trusted.</summary>
    public bool DeviceTrusted { get; init; }
    
    /// <summary>Device trust token (for future MFA bypass).</summary>
    public string? DeviceTrustToken { get; init; }
    
    /// <summary>Error message.</summary>
    public string? ErrorMessage { get; init; }
    
    /// <summary>Error code.</summary>
    public string? ErrorCode { get; init; }
}

/// <summary>
/// Event published when MFA is enabled for an identity.
/// </summary>
[Contract]
public record MfaEnabledIntegrationEvent(
    Guid IdentityId,
    string MfaType,
    string? DID,
    DateTime EnabledAt
);

/// <summary>
/// Event published when MFA verification succeeds.
/// </summary>
[Contract]
public record MfaVerifiedIntegrationEvent(
    Guid IdentityId,
    string MfaType,
    string? DID,
    string? DeviceDID,
    DateTime VerifiedAt
);

/// <summary>
/// Event published when MFA verification fails.
/// </summary>
[Contract]
public record MfaVerificationFailedIntegrationEvent(
    Guid IdentityId,
    string MfaType,
    string FailureReason,
    DateTime FailedAt
);
