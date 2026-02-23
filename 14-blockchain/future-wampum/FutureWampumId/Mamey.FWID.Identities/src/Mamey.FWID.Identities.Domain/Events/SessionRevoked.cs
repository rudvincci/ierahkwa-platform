using Mamey.CQRS;
using Mamey.FWID.Identities.Domain.Entities;

namespace Mamey.FWID.Identities.Domain.Events;

/// <summary>
/// Domain event raised when a session is revoked.
/// </summary>
internal record SessionRevoked(SessionId SessionId, IdentityId IdentityId, DateTime RevokedAt) : IDomainEvent;

