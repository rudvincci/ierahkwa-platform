using Mamey.CQRS;
using Mamey.FWID.Identities.Domain.Entities;

namespace Mamey.FWID.Identities.Domain.Events;

/// <summary>
/// Event raised when a disclosure is created.
/// </summary>
internal record DisclosureCreated(
    DisclosureId DisclosureId,
    IdentityId IdentityId,
    string TemplateId,
    string Purpose,
    List<string> Recipients,
    DateTime CreatedAt) : IDomainEvent;

/// <summary>
/// Event raised when a disclosure is approved.
/// </summary>
internal record DisclosureApproved(
    DisclosureId DisclosureId,
    string VerifiablePresentation,
    DateTime IssuedAt,
    DateTime ExpiresAt) : IDomainEvent;

/// <summary>
/// Event raised when a disclosure is rejected.
/// </summary>
internal record DisclosureRejected(
    DisclosureId DisclosureId,
    string Reason,
    DateTime RejectedAt) : IDomainEvent;

/// <summary>
/// Event raised when a disclosure is revoked.
/// </summary>
internal record DisclosureRevoked(
    DisclosureId DisclosureId,
    string Reason,
    DateTime RevokedAt) : IDomainEvent;
