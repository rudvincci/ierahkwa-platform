using System;
using System.Collections.Generic;

namespace Mamey.Auth.DecentralizedIdentifiers.Audit;

/// <summary>
/// Base class for all audit events
/// </summary>
public abstract class AuditEvent
{
    /// <summary>
    /// Unique identifier for the audit event
    /// </summary>
    public string Id { get; set; } = Guid.NewGuid().ToString();

    /// <summary>
    /// Type of audit event
    /// </summary>
    public string EventType { get; set; } = string.Empty;

    /// <summary>
    /// Timestamp when the event occurred
    /// </summary>
    public DateTime Timestamp { get; set; } = DateTime.UtcNow;

    /// <summary>
    /// DID associated with the event
    /// </summary>
    public string? Did { get; set; }

    /// <summary>
    /// User agent or client information
    /// </summary>
    public string? UserAgent { get; set; }

    /// <summary>
    /// IP address of the client
    /// </summary>
    public string? IpAddress { get; set; }

    /// <summary>
    /// Session identifier
    /// </summary>
    public string? SessionId { get; set; }

    /// <summary>
    /// Request identifier for correlation
    /// </summary>
    public string? RequestId { get; set; }

    /// <summary>
    /// Additional metadata
    /// </summary>
    public Dictionary<string, object> Metadata { get; set; } = new();

    /// <summary>
    /// Severity level of the event
    /// </summary>
    public AuditSeverity Severity { get; set; } = AuditSeverity.Info;

    /// <summary>
    /// Service or component that generated the event
    /// </summary>
    public string? Source { get; set; }

    /// <summary>
    /// Tenant or organization identifier
    /// </summary>
    public string? TenantId { get; set; }
}

/// <summary>
/// Authentication attempt event
/// </summary>
public class AuthenticationAttemptEvent : AuditEvent
{
    public AuthenticationAttemptEvent()
    {
        EventType = "AuthenticationAttempt";
    }

    /// <summary>
    /// Authentication method used
    /// </summary>
    public string AuthenticationMethod { get; set; } = string.Empty;

    /// <summary>
    /// Challenge used for authentication
    /// </summary>
    public string? Challenge { get; set; }

    /// <summary>
    /// Domain binding
    /// </summary>
    public string? Domain { get; set; }

    /// <summary>
    /// Whether the attempt was successful
    /// </summary>
    public bool Success { get; set; }

    /// <summary>
    /// Error message if failed
    /// </summary>
    public string? ErrorMessage { get; set; }

    /// <summary>
    /// Verification method used
    /// </summary>
    public string? VerificationMethod { get; set; }
}

/// <summary>
/// Successful authentication event
/// </summary>
public class AuthenticationSuccessEvent : AuditEvent
{
    public AuthenticationSuccessEvent()
    {
        EventType = "AuthenticationSuccess";
        Severity = AuditSeverity.Info;
    }

    /// <summary>
    /// Authentication method used
    /// </summary>
    public string AuthenticationMethod { get; set; } = string.Empty;

    /// <summary>
    /// Token issued
    /// </summary>
    public string? TokenId { get; set; }

    /// <summary>
    /// Token expiration time
    /// </summary>
    public DateTime? TokenExpiresAt { get; set; }

    /// <summary>
    /// Claims included in the token
    /// </summary>
    public Dictionary<string, string> Claims { get; set; } = new();

    /// <summary>
    /// Verification method used
    /// </summary>
    public string? VerificationMethod { get; set; }

    /// <summary>
    /// Session duration
    /// </summary>
    public TimeSpan? SessionDuration { get; set; }
}

/// <summary>
/// Failed authentication event
/// </summary>
public class AuthenticationFailureEvent : AuditEvent
{
    public AuthenticationFailureEvent()
    {
        EventType = "AuthenticationFailure";
        Severity = AuditSeverity.Warning;
    }

    /// <summary>
    /// Authentication method attempted
    /// </summary>
    public string AuthenticationMethod { get; set; } = string.Empty;

    /// <summary>
    /// Reason for failure
    /// </summary>
    public string FailureReason { get; set; } = string.Empty;

    /// <summary>
    /// Error code
    /// </summary>
    public string? ErrorCode { get; set; }

    /// <summary>
    /// Number of consecutive failures
    /// </summary>
    public int ConsecutiveFailures { get; set; }

    /// <summary>
    /// Whether this triggered a security alert
    /// </summary>
    public bool SecurityAlertTriggered { get; set; }

    /// <summary>
    /// Verification method attempted
    /// </summary>
    public string? VerificationMethod { get; set; }
}

/// <summary>
/// DID resolution event
/// </summary>
public class DidResolutionEvent : AuditEvent
{
    public DidResolutionEvent()
    {
        EventType = "DidResolution";
    }

    /// <summary>
    /// DID that was resolved
    /// </summary>
    public string ResolvedDid { get; set; } = string.Empty;

    /// <summary>
    /// Resolution method used
    /// </summary>
    public string ResolutionMethod { get; set; } = string.Empty;

    /// <summary>
    /// Whether resolution was successful
    /// </summary>
    public bool Success { get; set; }

    /// <summary>
    /// Resolution time in milliseconds
    /// </summary>
    public long ResolutionTimeMs { get; set; }

    /// <summary>
    /// Error message if failed
    /// </summary>
    public string? ErrorMessage { get; set; }

    /// <summary>
    /// Whether result was served from cache
    /// </summary>
    public bool FromCache { get; set; }
}

/// <summary>
/// Credential verification event
/// </summary>
public class CredentialVerificationEvent : AuditEvent
{
    public CredentialVerificationEvent()
    {
        EventType = "CredentialVerification";
    }

    /// <summary>
    /// Credential ID that was verified
    /// </summary>
    public string CredentialId { get; set; } = string.Empty;

    /// <summary>
    /// Issuer DID
    /// </summary>
    public string? IssuerDid { get; set; }

    /// <summary>
    /// Holder DID
    /// </summary>
    public string? HolderDid { get; set; }

    /// <summary>
    /// Whether verification was successful
    /// </summary>
    public bool Success { get; set; }

    /// <summary>
    /// Verification method used
    /// </summary>
    public string VerificationMethod { get; set; } = string.Empty;

    /// <summary>
    /// Error message if failed
    /// </summary>
    public string? ErrorMessage { get; set; }

    /// <summary>
    /// Whether credential is revoked
    /// </summary>
    public bool IsRevoked { get; set; }

    /// <summary>
    /// Whether credential is expired
    /// </summary>
    public bool IsExpired { get; set; }
}

/// <summary>
/// Key rotation event
/// </summary>
public class KeyRotationEvent : AuditEvent
{
    public KeyRotationEvent()
    {
        EventType = "KeyRotation";
        Severity = AuditSeverity.Info;
    }

    /// <summary>
    /// Old key identifier
    /// </summary>
    public string? OldKeyId { get; set; }

    /// <summary>
    /// New key identifier
    /// </summary>
    public string? NewKeyId { get; set; }

    /// <summary>
    /// Key type
    /// </summary>
    public string KeyType { get; set; } = string.Empty;

    /// <summary>
    /// Whether rotation was successful
    /// </summary>
    public bool Success { get; set; }

    /// <summary>
    /// Number of credentials re-issued
    /// </summary>
    public int CredentialsReissued { get; set; }

    /// <summary>
    /// Grace period in days
    /// </summary>
    public int GracePeriodDays { get; set; }
}

/// <summary>
/// Credential issuance event
/// </summary>
public class CredentialIssuanceEvent : AuditEvent
{
    public CredentialIssuanceEvent()
    {
        EventType = "CredentialIssuance";
        Severity = AuditSeverity.Info;
    }

    /// <summary>
    /// Credential ID that was issued
    /// </summary>
    public string CredentialId { get; set; } = string.Empty;

    /// <summary>
    /// Issuer DID
    /// </summary>
    public string IssuerDid { get; set; } = string.Empty;

    /// <summary>
    /// Subject DID
    /// </summary>
    public string SubjectDid { get; set; } = string.Empty;

    /// <summary>
    /// Credential type
    /// </summary>
    public string CredentialType { get; set; } = string.Empty;

    /// <summary>
    /// Whether issuance was successful
    /// </summary>
    public bool Success { get; set; }

    /// <summary>
    /// Expiration date
    /// </summary>
    public DateTime? ExpirationDate { get; set; }

    /// <summary>
    /// Proof type used
    /// </summary>
    public string? ProofType { get; set; }
}

/// <summary>
/// Credential revocation event
/// </summary>
public class CredentialRevocationEvent : AuditEvent
{
    public CredentialRevocationEvent()
    {
        EventType = "CredentialRevocation";
        Severity = AuditSeverity.Warning;
    }

    /// <summary>
    /// Credential ID that was revoked
    /// </summary>
    public string CredentialId { get; set; } = string.Empty;

    /// <summary>
    /// Issuer DID
    /// </summary>
    public string? IssuerDid { get; set; }

    /// <summary>
    /// Subject DID
    /// </summary>
    public string? SubjectDid { get; set; }

    /// <summary>
    /// Reason for revocation
    /// </summary>
    public string RevocationReason { get; set; } = string.Empty;

    /// <summary>
    /// Whether revocation was successful
    /// </summary>
    public bool Success { get; set; }

    /// <summary>
    /// Revocation date
    /// </summary>
    public DateTime RevocationDate { get; set; } = DateTime.UtcNow;
}

/// <summary>
/// DID creation event
/// </summary>
public class DidCreationEvent : AuditEvent
{
    public DidCreationEvent()
    {
        EventType = "DidCreation";
        Severity = AuditSeverity.Info;
    }

    /// <summary>
    /// DID that was created
    /// </summary>
    public string CreatedDid { get; set; } = string.Empty;

    /// <summary>
    /// DID method used
    /// </summary>
    public string DidMethod { get; set; } = string.Empty;

    /// <summary>
    /// Whether creation was successful
    /// </summary>
    public bool Success { get; set; }

    /// <summary>
    /// Number of verification methods added
    /// </summary>
    public int VerificationMethodsCount { get; set; }

    /// <summary>
    /// Number of service endpoints added
    /// </summary>
    public int ServiceEndpointsCount { get; set; }
}

/// <summary>
/// DID update event
/// </summary>
public class DidUpdateEvent : AuditEvent
{
    public DidUpdateEvent()
    {
        EventType = "DidUpdate";
        Severity = AuditSeverity.Info;
    }

    /// <summary>
    /// DID that was updated
    /// </summary>
    public string UpdatedDid { get; set; } = string.Empty;

    /// <summary>
    /// Update operation performed
    /// </summary>
    public string UpdateOperation { get; set; } = string.Empty;

    /// <summary>
    /// Whether update was successful
    /// </summary>
    public bool Success { get; set; }

    /// <summary>
    /// Previous version
    /// </summary>
    public int? PreviousVersion { get; set; }

    /// <summary>
    /// New version
    /// </summary>
    public int? NewVersion { get; set; }
}

/// <summary>
/// DID deactivation event
/// </summary>
public class DidDeactivationEvent : AuditEvent
{
    public DidDeactivationEvent()
    {
        EventType = "DidDeactivation";
        Severity = AuditSeverity.Warning;
    }

    /// <summary>
    /// DID that was deactivated
    /// </summary>
    public string DeactivatedDid { get; set; } = string.Empty;

    /// <summary>
    /// Whether deactivation was successful
    /// </summary>
    public bool Success { get; set; }

    /// <summary>
    /// Reason for deactivation
    /// </summary>
    public string? DeactivationReason { get; set; }

    /// <summary>
    /// Number of credentials revoked
    /// </summary>
    public int CredentialsRevoked { get; set; }
}

/// <summary>
/// Presentation validation event
/// </summary>
public class PresentationValidationEvent : AuditEvent
{
    public PresentationValidationEvent()
    {
        EventType = "PresentationValidation";
    }

    /// <summary>
    /// Presentation ID
    /// </summary>
    public string? PresentationId { get; set; }

    /// <summary>
    /// Holder DID
    /// </summary>
    public string? HolderDid { get; set; }

    /// <summary>
    /// Whether validation was successful
    /// </summary>
    public bool Success { get; set; }

    /// <summary>
    /// Number of credentials in presentation
    /// </summary>
    public int CredentialsCount { get; set; }

    /// <summary>
    /// Challenge used
    /// </summary>
    public string? Challenge { get; set; }

    /// <summary>
    /// Domain binding
    /// </summary>
    public string? Domain { get; set; }

    /// <summary>
    /// Error message if failed
    /// </summary>
    public string? ErrorMessage { get; set; }
}

/// <summary>
/// Security event
/// </summary>
public class SecurityEvent : AuditEvent
{
    public SecurityEvent()
    {
        EventType = "SecurityEvent";
        Severity = AuditSeverity.Critical;
    }

    /// <summary>
    /// Security event type
    /// </summary>
    public string SecurityEventType { get; set; } = string.Empty;

    /// <summary>
    /// Description of the security event
    /// </summary>
    public string Description { get; set; } = string.Empty;

    /// <summary>
    /// Whether the event was resolved
    /// </summary>
    public bool Resolved { get; set; }

    /// <summary>
    /// Actions taken
    /// </summary>
    public List<string> ActionsTaken { get; set; } = new();

    /// <summary>
    /// Risk level
    /// </summary>
    public SecurityRiskLevel RiskLevel { get; set; } = SecurityRiskLevel.Medium;
}

/// <summary>
/// Audit severity levels
/// </summary>
public enum AuditSeverity
{
    Debug,
    Info,
    Warning,
    Error,
    Critical
}

/// <summary>
/// Security risk levels
/// </summary>
public enum SecurityRiskLevel
{
    Low,
    Medium,
    High,
    Critical
}












