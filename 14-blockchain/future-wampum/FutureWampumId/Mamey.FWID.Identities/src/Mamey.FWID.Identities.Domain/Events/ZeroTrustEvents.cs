using Mamey.CQRS;
using Mamey.FWID.Identities.Domain.Entities;

namespace Mamey.FWID.Identities.Domain.Events;

/// <summary>
/// Event raised when a zero-trust policy is created.
/// </summary>
internal record ZeroTrustPolicyCreated(
    ZeroTrustId ZeroTrustId,
    string EntityId,
    string EntityType,
    TrustLevel TrustLevel,
    DateTime CreatedAt) : IDomainEvent;

/// <summary>
/// Event raised when a security policy is added.
/// </summary>
internal record SecurityPolicyAdded(
    ZeroTrustId ZeroTrustId,
    string PolicyId,
    PolicyType PolicyType,
    int Priority,
    DateTime AddedAt) : IDomainEvent;

/// <summary>
/// Event raised when an access request is evaluated.
/// </summary>
internal record AccessRequestEvaluated(
    ZeroTrustId ZeroTrustId,
    string RequestId,
    AccessResult Result,
    string Reason,
    DateTime EvaluatedAt) : IDomainEvent;

/// <summary>
/// Event raised when a trust signal is recorded.
/// </summary>
internal record TrustSignalRecorded(
    ZeroTrustId ZeroTrustId,
    TrustSignalType SignalType,
    int Confidence,
    string Source,
    DateTime RecordedAt) : IDomainEvent;

/// <summary>
/// Event raised when an isolation zone is assigned.
/// </summary>
internal record IsolationZoneAssigned(
    ZeroTrustId ZeroTrustId,
    string ZoneId,
    string ZoneName,
    SecurityLevel SecurityLevel,
    DateTime AssignedAt) : IDomainEvent;

/// <summary>
/// Event raised when a risk score is updated.
/// </summary>
internal record RiskScoreUpdated(
    ZeroTrustId ZeroTrustId,
    int OldScore,
    int NewScore,
    string Reason,
    DateTime UpdatedAt) : IDomainEvent;

/// <summary>
/// Event raised when a security event is recorded.
/// </summary>
internal record SecurityEventRecorded(
    ZeroTrustId ZeroTrustId,
    SecurityEventType EventType,
    SecuritySeverity Severity,
    string Description,
    DateTime RecordedAt) : IDomainEvent;

/// <summary>
/// Event raised when a zero-trust policy is deactivated.
/// </summary>
internal record ZeroTrustPolicyDeactivated(
    ZeroTrustId ZeroTrustId,
    string Reason,
    DateTime DeactivatedAt) : IDomainEvent;
