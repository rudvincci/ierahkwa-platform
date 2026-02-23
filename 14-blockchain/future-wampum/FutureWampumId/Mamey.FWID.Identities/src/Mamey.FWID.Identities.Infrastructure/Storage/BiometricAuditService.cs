using System.Text.Json;
using Mamey.Contexts;
using Mamey.FWID.Identities.Domain.ValueObjects;
using Mamey.Microservice.Infrastructure;
using Microsoft.Extensions.Logging;

namespace Mamey.FWID.Identities.Infrastructure.Storage;

/// <summary>
/// Service for auditing biometric data access.
/// Logs all access operations for compliance and security monitoring.
/// </summary>
internal class BiometricAuditService : IBiometricAuditService
{
    private readonly ILogger<BiometricAuditService> _logger;
    private readonly IContext _context;
    private readonly IBiometricAuditRepository? _auditRepository;

    public BiometricAuditService(
        ILogger<BiometricAuditService> logger,
        IContext context,
        IBiometricAuditRepository? auditRepository = null)
    {
        _logger = logger;
        _context = context;
        _auditRepository = auditRepository;
    }

    public async Task LogAccessAsync(
        IdentityId identityId,
        BiometricType biometricType,
        string operation,
        string? objectName = null,
        bool success = true,
        string? errorMessage = null,
        CancellationToken cancellationToken = default)
    {
        var auditEvent = new BiometricAccessAuditEvent
        {
            IdentityId = identityId,
            BiometricType = biometricType,
            Operation = operation,
            ObjectName = objectName,
            Timestamp = DateTime.UtcNow,
            Success = success,
            ErrorMessage = errorMessage,
            CorrelationId = _context.CorrelationId.ToString()
        };

        // Log to structured logging
        _logger.LogInformation(
            "Biometric access audit: IdentityId={IdentityId}, Type={BiometricType}, Operation={Operation}, ObjectName={ObjectName}, Success={Success}, CorrelationId={CorrelationId}",
            identityId.Value,
            biometricType,
            operation,
            objectName,
            success,
            auditEvent.CorrelationId);

        // Store in audit repository if available
        if (_auditRepository != null)
        {
            try
            {
                await _auditRepository.StoreAsync(auditEvent, cancellationToken);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to store biometric access audit event");
                // Don't throw - audit logging failures shouldn't break the operation
            }
        }
    }
}

/// <summary>
/// Repository for storing biometric access audit events.
/// </summary>
internal interface IBiometricAuditRepository
{
    Task StoreAsync(BiometricAccessAuditEvent auditEvent, CancellationToken cancellationToken = default);
}
