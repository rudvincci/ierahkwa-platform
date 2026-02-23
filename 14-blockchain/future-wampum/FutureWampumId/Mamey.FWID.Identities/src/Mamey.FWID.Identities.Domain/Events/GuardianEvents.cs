using Mamey.CQRS;
using Mamey.FWID.Identities.Domain.Entities;

namespace Mamey.FWID.Identities.Domain.Events;

/// <summary>
/// Event raised when a guardian relationship is created.
/// </summary>
internal record GuardianCreated(
    GuardianId GuardianId,
    IdentityId GuardianIdentityId,
    IdentityId DependentIdentityId,
    string RelationshipType,
    DateTime CreatedAt) : IDomainEvent;

/// <summary>
/// Event raised when a guardian relationship is terminated.
/// </summary>
internal record GuardianTerminated(
    GuardianId GuardianId,
    string Reason,
    DateTime TerminatedAt) : IDomainEvent;

/// <summary>
/// Event raised when a delegation is granted.
/// </summary>
internal record DelegationGranted(
    GuardianId GuardianId,
    DelegationId DelegationId,
    string Scope,
    DateTime? ExpiresAt,
    string? Conditions) : IDomainEvent;

/// <summary>
/// Event raised when a delegation is revoked.
/// </summary>
internal record DelegationRevoked(
    GuardianId GuardianId,
    DelegationId DelegationId,
    string Reason) : IDomainEvent;
