using Mamey.FWID.Identities.Application.Clients;

namespace Mamey.FWID.Identities.Infrastructure.Ledger;

/// <summary>
/// Service for logging identity events to FutureWampumLedger for immutable audit trail.
/// </summary>
internal interface ILedgerAuditService
{
    /// <summary>
    /// Logs an identity creation event.
    /// </summary>
    Task<bool> LogIdentityCreatedAsync(
        Guid identityId,
        string? firstName = null,
        string? lastName = null,
        string? email = null,
        string? correlationId = null,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Logs an identity update event.
    /// </summary>
    Task<bool> LogIdentityUpdatedAsync(
        Guid identityId,
        Dictionary<string, object>? changedAttributes = null,
        string? correlationId = null,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Logs an identity deletion event.
    /// </summary>
    Task<bool> LogIdentityDeletedAsync(
        Guid identityId,
        string? correlationId = null,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Logs a custom identity event.
    /// </summary>
    Task<bool> LogIdentityEventAsync(
        string eventType,
        Guid identityId,
        string? description = null,
        Dictionary<string, object>? metadata = null,
        string? correlationId = null,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Queries the audit trail for an identity.
    /// </summary>
    Task<IReadOnlyList<AuditTrailEntry>> QueryAuditTrailAsync(
        Guid identityId,
        DateTime? fromDate = null,
        DateTime? toDate = null,
        string? eventType = null,
        CancellationToken cancellationToken = default);
}

/// <summary>
/// Audit trail entry from the ledger.
/// </summary>
public class AuditTrailEntry
{
    public string TransactionHash { get; set; } = null!;
    public string TransactionType { get; set; } = null!;
    public Guid EntityId { get; set; }
    public string? Description { get; set; }
    public Dictionary<string, object>? Metadata { get; set; }
    public DateTime Timestamp { get; set; }
    public string? CorrelationId { get; set; }
}
