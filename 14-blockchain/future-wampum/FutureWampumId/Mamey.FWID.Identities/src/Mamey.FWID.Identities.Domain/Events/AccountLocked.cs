using Mamey.CQRS;
using Mamey.FWID.Identities.Domain.Entities;

namespace Mamey.FWID.Identities.Domain.Events;

/// <summary>
/// Domain event raised when an account is locked.
/// </summary>
internal record AccountLocked(IdentityId IdentityId, DateTime LockedUntil, DateTime LockedAt) : IDomainEvent;

