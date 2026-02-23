using Mamey.CQRS;
using Mamey.FWID.Identities.Domain.Entities;
using Mamey.FWID.Identities.Domain.ValueObjects;

namespace Mamey.FWID.Identities.Domain.Events;

/// <summary>
/// Domain event raised when an MFA challenge is created.
/// </summary>
internal record MfaChallengeCreated(IdentityId IdentityId, MfaMethod Method, DateTime CreatedAt, DateTime ExpiresAt) : IDomainEvent;

