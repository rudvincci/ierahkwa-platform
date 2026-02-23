using Mamey.CQRS;
using Mamey.FWID.Identities.Domain.Entities;

namespace Mamey.FWID.Identities.Domain.Events;

/// <summary>
/// Domain event raised when a session expires.
/// </summary>
internal record SessionExpired(SessionId SessionId, IdentityId IdentityId, DateTime ExpiredAt) : IDomainEvent;

