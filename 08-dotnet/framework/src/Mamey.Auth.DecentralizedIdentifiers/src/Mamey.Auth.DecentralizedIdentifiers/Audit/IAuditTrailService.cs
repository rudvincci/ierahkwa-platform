using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Mamey.Auth.DecentralizedIdentifiers.Audit;

/// <summary>
/// Interface for audit trail service to track DID authentication events
/// </summary>
public interface IAuditTrailService
{
    /// <summary>
    /// Log an authentication attempt
    /// </summary>
    Task LogAuthenticationAttemptAsync(AuthenticationAttemptEvent auditEvent);

    /// <summary>
    /// Log a successful authentication
    /// </summary>
    Task LogAuthenticationSuccessAsync(AuthenticationSuccessEvent auditEvent);

    /// <summary>
    /// Log a failed authentication
    /// </summary>
    Task LogAuthenticationFailureAsync(AuthenticationFailureEvent auditEvent);

    /// <summary>
    /// Log a DID resolution event
    /// </summary>
    Task LogDidResolutionAsync(DidResolutionEvent auditEvent);

    /// <summary>
    /// Log a credential verification event
    /// </summary>
    Task LogCredentialVerificationAsync(CredentialVerificationEvent auditEvent);

    /// <summary>
    /// Log a key rotation event
    /// </summary>
    Task LogKeyRotationAsync(KeyRotationEvent auditEvent);

    /// <summary>
    /// Log a credential issuance event
    /// </summary>
    Task LogCredentialIssuanceAsync(CredentialIssuanceEvent auditEvent);

    /// <summary>
    /// Log a credential revocation event
    /// </summary>
    Task LogCredentialRevocationAsync(CredentialRevocationEvent auditEvent);

    /// <summary>
    /// Log a DID creation event
    /// </summary>
    Task LogDidCreationAsync(DidCreationEvent auditEvent);

    /// <summary>
    /// Log a DID update event
    /// </summary>
    Task LogDidUpdateAsync(DidUpdateEvent auditEvent);

    /// <summary>
    /// Log a DID deactivation event
    /// </summary>
    Task LogDidDeactivationAsync(DidDeactivationEvent auditEvent);

    /// <summary>
    /// Log a presentation validation event
    /// </summary>
    Task LogPresentationValidationAsync(PresentationValidationEvent auditEvent);

    /// <summary>
    /// Log a security event (suspicious activity, etc.)
    /// </summary>
    Task LogSecurityEventAsync(SecurityEvent auditEvent);

    /// <summary>
    /// Get audit events for a specific DID
    /// </summary>
    Task<IEnumerable<AuditEvent>> GetAuditEventsForDidAsync(string did, int limit = 100);

    /// <summary>
    /// Get audit events for a specific time range
    /// </summary>
    Task<IEnumerable<AuditEvent>> GetAuditEventsAsync(DateTime from, DateTime to, int limit = 1000);

    /// <summary>
    /// Get audit events by event type
    /// </summary>
    Task<IEnumerable<AuditEvent>> GetAuditEventsByTypeAsync(string eventType, int limit = 100);

    /// <summary>
    /// Get audit statistics
    /// </summary>
    Task<AuditStatistics> GetAuditStatisticsAsync(DateTime from, DateTime to);

    /// <summary>
    /// Search audit events with filters
    /// </summary>
    Task<AuditSearchResult> SearchAuditEventsAsync(AuditSearchRequest request);
}











