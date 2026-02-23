using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace Mamey.Auth.DecentralizedIdentifiers.Audit;

/// <summary>
/// Default implementation of audit trail service
/// </summary>
public class AuditTrailService : IAuditTrailService
{
    private readonly IAuditEventStore _eventStore;
    private readonly IAuditEventPublisher _eventPublisher;
    private readonly ILogger<AuditTrailService> _logger;
    private readonly AuditTrailOptions _options;

    public AuditTrailService(
        IAuditEventStore eventStore,
        IAuditEventPublisher eventPublisher,
        ILogger<AuditTrailService> logger,
        IOptions<AuditTrailOptions> options)
    {
        _eventStore = eventStore ?? throw new ArgumentNullException(nameof(eventStore));
        _eventPublisher = eventPublisher ?? throw new ArgumentNullException(nameof(eventPublisher));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        _options = options?.Value ?? throw new ArgumentNullException(nameof(options));
    }

    public async Task LogAuthenticationAttemptAsync(AuthenticationAttemptEvent auditEvent)
    {
        try
        {
            EnrichEvent(auditEvent);
            await _eventStore.StoreAsync(auditEvent);
            await _eventPublisher.PublishAsync(auditEvent);
            
            _logger.LogDebug("Logged authentication attempt for DID: {Did}, Success: {Success}", 
                auditEvent.Did, auditEvent.Success);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to log authentication attempt for DID: {Did}", auditEvent.Did);
        }
    }

    public async Task LogAuthenticationSuccessAsync(AuthenticationSuccessEvent auditEvent)
    {
        try
        {
            EnrichEvent(auditEvent);
            await _eventStore.StoreAsync(auditEvent);
            await _eventPublisher.PublishAsync(auditEvent);
            
            _logger.LogInformation("Logged successful authentication for DID: {Did}, Method: {Method}", 
                auditEvent.Did, auditEvent.AuthenticationMethod);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to log authentication success for DID: {Did}", auditEvent.Did);
        }
    }

    public async Task LogAuthenticationFailureAsync(AuthenticationFailureEvent auditEvent)
    {
        try
        {
            EnrichEvent(auditEvent);
            await _eventStore.StoreAsync(auditEvent);
            await _eventPublisher.PublishAsync(auditEvent);
            
            _logger.LogWarning("Logged authentication failure for DID: {Did}, Reason: {Reason}, ConsecutiveFailures: {ConsecutiveFailures}", 
                auditEvent.Did, auditEvent.FailureReason, auditEvent.ConsecutiveFailures);

            // Check for suspicious activity
            if (auditEvent.ConsecutiveFailures >= _options.SuspiciousActivityThreshold)
            {
                await LogSecurityEventAsync(new SecurityEvent
                {
                    Did = auditEvent.Did,
                    SecurityEventType = "SuspiciousAuthenticationActivity",
                    Description = $"Multiple consecutive authentication failures: {auditEvent.ConsecutiveFailures}",
                    RiskLevel = SecurityRiskLevel.High,
                    Metadata = new Dictionary<string, object>
                    {
                        ["ConsecutiveFailures"] = auditEvent.ConsecutiveFailures,
                        ["LastFailureReason"] = auditEvent.FailureReason,
                        ["IpAddress"] = auditEvent.IpAddress ?? "Unknown"
                    }
                });
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to log authentication failure for DID: {Did}", auditEvent.Did);
        }
    }

    public async Task LogDidResolutionAsync(DidResolutionEvent auditEvent)
    {
        try
        {
            EnrichEvent(auditEvent);
            await _eventStore.StoreAsync(auditEvent);
            await _eventPublisher.PublishAsync(auditEvent);
            
            _logger.LogDebug("Logged DID resolution for: {Did}, Method: {Method}, Success: {Success}, Time: {TimeMs}ms", 
                auditEvent.ResolvedDid, auditEvent.ResolutionMethod, auditEvent.Success, auditEvent.ResolutionTimeMs);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to log DID resolution for: {Did}", auditEvent.ResolvedDid);
        }
    }

    public async Task LogCredentialVerificationAsync(CredentialVerificationEvent auditEvent)
    {
        try
        {
            EnrichEvent(auditEvent);
            await _eventStore.StoreAsync(auditEvent);
            await _eventPublisher.PublishAsync(auditEvent);
            
            _logger.LogDebug("Logged credential verification for: {CredentialId}, Success: {Success}", 
                auditEvent.CredentialId, auditEvent.Success);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to log credential verification for: {CredentialId}", auditEvent.CredentialId);
        }
    }

    public async Task LogKeyRotationAsync(KeyRotationEvent auditEvent)
    {
        try
        {
            EnrichEvent(auditEvent);
            await _eventStore.StoreAsync(auditEvent);
            await _eventPublisher.PublishAsync(auditEvent);
            
            _logger.LogInformation("Logged key rotation for DID: {Did}, OldKey: {OldKeyId}, NewKey: {NewKeyId}, CredentialsReissued: {CredentialsReissued}", 
                auditEvent.Did, auditEvent.OldKeyId, auditEvent.NewKeyId, auditEvent.CredentialsReissued);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to log key rotation for DID: {Did}", auditEvent.Did);
        }
    }

    public async Task LogCredentialIssuanceAsync(CredentialIssuanceEvent auditEvent)
    {
        try
        {
            EnrichEvent(auditEvent);
            await _eventStore.StoreAsync(auditEvent);
            await _eventPublisher.PublishAsync(auditEvent);
            
            _logger.LogInformation("Logged credential issuance: {CredentialId}, Issuer: {IssuerDid}, Subject: {SubjectDid}, Type: {CredentialType}", 
                auditEvent.CredentialId, auditEvent.IssuerDid, auditEvent.SubjectDid, auditEvent.CredentialType);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to log credential issuance: {CredentialId}", auditEvent.CredentialId);
        }
    }

    public async Task LogCredentialRevocationAsync(CredentialRevocationEvent auditEvent)
    {
        try
        {
            EnrichEvent(auditEvent);
            await _eventStore.StoreAsync(auditEvent);
            await _eventPublisher.PublishAsync(auditEvent);
            
            _logger.LogWarning("Logged credential revocation: {CredentialId}, Reason: {Reason}", 
                auditEvent.CredentialId, auditEvent.RevocationReason);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to log credential revocation: {CredentialId}", auditEvent.CredentialId);
        }
    }

    public async Task LogDidCreationAsync(DidCreationEvent auditEvent)
    {
        try
        {
            EnrichEvent(auditEvent);
            await _eventStore.StoreAsync(auditEvent);
            await _eventPublisher.PublishAsync(auditEvent);
            
            _logger.LogInformation("Logged DID creation: {Did}, Method: {Method}, Success: {Success}", 
                auditEvent.CreatedDid, auditEvent.DidMethod, auditEvent.Success);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to log DID creation: {Did}", auditEvent.CreatedDid);
        }
    }

    public async Task LogDidUpdateAsync(DidUpdateEvent auditEvent)
    {
        try
        {
            EnrichEvent(auditEvent);
            await _eventStore.StoreAsync(auditEvent);
            await _eventPublisher.PublishAsync(auditEvent);
            
            _logger.LogInformation("Logged DID update: {Did}, Operation: {Operation}, Success: {Success}", 
                auditEvent.UpdatedDid, auditEvent.UpdateOperation, auditEvent.Success);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to log DID update: {Did}", auditEvent.UpdatedDid);
        }
    }

    public async Task LogDidDeactivationAsync(DidDeactivationEvent auditEvent)
    {
        try
        {
            EnrichEvent(auditEvent);
            await _eventStore.StoreAsync(auditEvent);
            await _eventPublisher.PublishAsync(auditEvent);
            
            _logger.LogWarning("Logged DID deactivation: {Did}, Reason: {Reason}, CredentialsRevoked: {CredentialsRevoked}", 
                auditEvent.DeactivatedDid, auditEvent.DeactivationReason, auditEvent.CredentialsRevoked);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to log DID deactivation: {Did}", auditEvent.DeactivatedDid);
        }
    }

    public async Task LogPresentationValidationAsync(PresentationValidationEvent auditEvent)
    {
        try
        {
            EnrichEvent(auditEvent);
            await _eventStore.StoreAsync(auditEvent);
            await _eventPublisher.PublishAsync(auditEvent);
            
            _logger.LogDebug("Logged presentation validation: {PresentationId}, Success: {Success}, CredentialsCount: {CredentialsCount}", 
                auditEvent.PresentationId, auditEvent.Success, auditEvent.CredentialsCount);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to log presentation validation: {PresentationId}", auditEvent.PresentationId);
        }
    }

    public async Task LogSecurityEventAsync(SecurityEvent auditEvent)
    {
        try
        {
            EnrichEvent(auditEvent);
            await _eventStore.StoreAsync(auditEvent);
            await _eventPublisher.PublishAsync(auditEvent);
            
            _logger.LogCritical("Logged security event: {EventType}, DID: {Did}, RiskLevel: {RiskLevel}, Description: {Description}", 
                auditEvent.SecurityEventType, auditEvent.Did, auditEvent.RiskLevel, auditEvent.Description);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to log security event: {EventType}", auditEvent.SecurityEventType);
        }
    }

    public async Task<IEnumerable<AuditEvent>> GetAuditEventsForDidAsync(string did, int limit = 100)
    {
        try
        {
            return await _eventStore.GetEventsForDidAsync(did, limit);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to get audit events for DID: {Did}", did);
            return Enumerable.Empty<AuditEvent>();
        }
    }

    public async Task<IEnumerable<AuditEvent>> GetAuditEventsAsync(DateTime from, DateTime to, int limit = 1000)
    {
        try
        {
            return await _eventStore.GetEventsAsync(from, to, limit);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to get audit events from {From} to {To}", from, to);
            return Enumerable.Empty<AuditEvent>();
        }
    }

    public async Task<IEnumerable<AuditEvent>> GetAuditEventsByTypeAsync(string eventType, int limit = 100)
    {
        try
        {
            return await _eventStore.GetEventsByTypeAsync(eventType, limit);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to get audit events by type: {EventType}", eventType);
            return Enumerable.Empty<AuditEvent>();
        }
    }

    public async Task<AuditStatistics> GetAuditStatisticsAsync(DateTime from, DateTime to)
    {
        try
        {
            return await _eventStore.GetStatisticsAsync(from, to);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to get audit statistics from {From} to {To}", from, to);
            return new AuditStatistics();
        }
    }

    public async Task<AuditSearchResult> SearchAuditEventsAsync(AuditSearchRequest request)
    {
        try
        {
            return await _eventStore.SearchEventsAsync(request);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to search audit events");
            return new AuditSearchResult();
        }
    }

    private void EnrichEvent(AuditEvent auditEvent)
    {
        // Set source if not already set
        if (string.IsNullOrEmpty(auditEvent.Source))
        {
            auditEvent.Source = "Mamey.Auth.DecentralizedIdentifiers";
        }

        // Add correlation ID if not present
        if (string.IsNullOrEmpty(auditEvent.RequestId))
        {
            auditEvent.RequestId = Guid.NewGuid().ToString();
        }

        // Add timestamp if not set
        if (auditEvent.Timestamp == default)
        {
            auditEvent.Timestamp = DateTime.UtcNow;
        }

        // Add environment information
        auditEvent.Metadata["Environment"] = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") ?? "Unknown";
        auditEvent.Metadata["MachineName"] = Environment.MachineName;
        auditEvent.Metadata["ProcessId"] = Environment.ProcessId;
    }
}











