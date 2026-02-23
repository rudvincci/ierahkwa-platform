using Mamey.CQRS;
using Mamey.FWID.Identities.Domain.Entities;

namespace Mamey.FWID.Identities.Domain.Events;

/// <summary>
/// Event raised when an audit event is recorded.
/// </summary>
internal record AuditEventRecorded(
    AuditTrailId AuditTrailId,
    string EventType,
    string EntityType,
    string EntityId,
    string Action,
    string? UserId,
    DateTime Timestamp) : IDomainEvent;

/// <summary>
/// Event raised when an audit event is archived.
/// </summary>
internal record AuditEventArchived(
    AuditTrailId AuditTrailId,
    string ArchiveLocation,
    DateTime ArchivedAt) : IDomainEvent;
