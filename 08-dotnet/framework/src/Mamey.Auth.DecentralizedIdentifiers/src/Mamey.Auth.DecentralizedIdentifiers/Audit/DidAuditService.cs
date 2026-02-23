using System.Text.Json;
using Microsoft.Extensions.Logging;
using Mamey.Types;

namespace Mamey.Auth.DecentralizedIdentifiers.Audit;

/// <summary>
/// Service for logging DID-related audit events using the existing AuditLog infrastructure.
/// </summary>
public class DidAuditService : IDidAuditService
{
    private readonly ILogger<DidAuditService> _logger;
    private readonly IAuditRepository _auditRepository;

    /// <summary>
    /// Initializes a new instance of the <see cref="DidAuditService"/> class.
    /// </summary>
    /// <param name="logger">The logger instance.</param>
    /// <param name="auditRepository">The audit repository for persisting audit logs.</param>
    public DidAuditService(ILogger<DidAuditService> logger, IAuditRepository auditRepository)
    {
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        _auditRepository = auditRepository ?? throw new ArgumentNullException(nameof(auditRepository));
    }

    /// <inheritdoc />
    public async Task LogDidOperationAsync(
        string activityType,
        string category,
        string status,
        string did,
        string correlationId,
        UserId? userId = null,
        Email? email = null,
        Name? name = null,
        Dictionary<string, object>? metadata = null,
        CancellationToken cancellationToken = default)
    {
        await LogEventAsync(
            activityType,
            category,
            status,
            did,
            correlationId,
            userId,
            email,
            name,
            metadata,
            cancellationToken);
    }

    /// <inheritdoc />
    public async Task LogCredentialOperationAsync(
        string activityType,
        string category,
        string status,
        string credentialId,
        string correlationId,
        UserId? userId = null,
        Email? email = null,
        Name? name = null,
        Dictionary<string, object>? metadata = null,
        CancellationToken cancellationToken = default)
    {
        await LogEventAsync(
            activityType,
            category,
            status,
            credentialId,
            correlationId,
            userId,
            email,
            name,
            metadata,
            cancellationToken);
    }

    /// <inheritdoc />
    public async Task LogPresentationOperationAsync(
        string activityType,
        string category,
        string status,
        string presentationId,
        string correlationId,
        UserId? userId = null,
        Email? email = null,
        Name? name = null,
        Dictionary<string, object>? metadata = null,
        CancellationToken cancellationToken = default)
    {
        await LogEventAsync(
            activityType,
            category,
            status,
            presentationId,
            correlationId,
            userId,
            email,
            name,
            metadata,
            cancellationToken);
    }

    /// <inheritdoc />
    public async Task LogAuthenticationEventAsync(
        string activityType,
        string category,
        string status,
        string subject,
        string correlationId,
        UserId? userId = null,
        Email? email = null,
        Name? name = null,
        Dictionary<string, object>? metadata = null,
        CancellationToken cancellationToken = default)
    {
        await LogEventAsync(
            activityType,
            category,
            status,
            subject,
            correlationId,
            userId,
            email,
            name,
            metadata,
            cancellationToken);
    }

    /// <inheritdoc />
    public async Task LogEventAsync(
        string activityType,
        string category,
        string status,
        string subject,
        string correlationId,
        UserId? userId = null,
        Email? email = null,
        Name? name = null,
        Dictionary<string, object>? metadata = null,
        CancellationToken cancellationToken = default)
    {
        try
        {
            // Create audit log entry
            var auditLog = new AuditLog(
                id: Guid.NewGuid(),
                requestId: correlationId,
                logDate: DateTime.UtcNow,
                activityType: activityType,
                correlationId: correlationId,
                category: category,
                status: status,
                userId: userId ?? UserId.Empty,
                email: email ?? new Email("system@mamey.io"),
                name: name ?? new Name("System", "User"),
                type: GetAuditTypeFromCategory(category)
            );

            // Add metadata to the audit log if provided
            if (metadata != null && metadata.Count > 0)
            {
                var metadataJson = JsonSerializer.Serialize(metadata);
                _logger.LogDebug("Audit event metadata: {Metadata}", metadataJson);
            }

            // Persist the audit log
            await _auditRepository.SaveAsync(auditLog, cancellationToken);

            _logger.LogDebug(
                "Audit event logged: {ActivityType} - {Category} - {Status} - {Subject}",
                activityType,
                category,
                status,
                subject);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex,
                "Failed to log audit event: {ActivityType} - {Category} - {Status} - {Subject}",
                activityType,
                category,
                status,
                subject);
            
            // Don't rethrow to avoid breaking the main operation
        }
    }

    /// <summary>
    /// Maps audit categories to AuditType enum values.
    /// </summary>
    /// <param name="category">The audit category.</param>
    /// <returns>The corresponding AuditType.</returns>
    private static AuditType GetAuditTypeFromCategory(string category)
    {
        return category switch
        {
            DidAuditCategories.AUTHENTICATION => AuditType.Authentication,
            DidAuditCategories.SECURITY => AuditType.Security,
            DidAuditCategories.KEY_MANAGEMENT => AuditType.Security,
            DidAuditCategories.PROOF_OPERATIONS => AuditType.Security,
            DidAuditCategories.DID_OPERATIONS => AuditType.System,
            DidAuditCategories.CREDENTIAL_OPERATIONS => AuditType.System,
            _ => AuditType.Other
        };
    }
}

/// <summary>
/// Repository interface for persisting audit logs.
/// </summary>
public interface IAuditRepository
{
    /// <summary>
    /// Saves an audit log entry.
    /// </summary>
    /// <param name="auditLog">The audit log to save.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>A task representing the async operation.</returns>
    Task SaveAsync(AuditLog auditLog, CancellationToken cancellationToken = default);
}







