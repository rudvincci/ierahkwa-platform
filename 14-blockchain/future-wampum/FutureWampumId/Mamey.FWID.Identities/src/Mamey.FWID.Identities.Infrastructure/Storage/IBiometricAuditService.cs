using Mamey.FWID.Identities.Domain.ValueObjects;

namespace Mamey.FWID.Identities.Infrastructure.Storage;

/// <summary>
/// Service for auditing biometric data access.
/// Logs all access operations for compliance and security monitoring.
/// </summary>
internal interface IBiometricAuditService
{
    /// <summary>
    /// Logs a biometric access event.
    /// </summary>
    Task LogAccessAsync(
        IdentityId identityId,
        BiometricType biometricType,
        string operation,
        string? objectName = null,
        bool success = true,
        string? errorMessage = null,
        CancellationToken cancellationToken = default);
}

/// <summary>
/// Biometric access audit event.
/// </summary>
public class BiometricAccessAuditEvent
{
    public IdentityId IdentityId { get; set; } = null!;
    public BiometricType BiometricType { get; set; }
    public string Operation { get; set; } = null!;
    public string? ObjectName { get; set; }
    public DateTime Timestamp { get; set; } = DateTime.UtcNow;
    public bool Success { get; set; }
    public string? ErrorMessage { get; set; }
    public string? CorrelationId { get; set; }
}
