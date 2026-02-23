using Mamey.CQRS;
using Mamey.FWID.Identities.Domain.Entities;

namespace Mamey.FWID.Identities.Domain.Events;

/// <summary>
/// Domain event raised when an account is unlocked.
/// </summary>
internal record AccountUnlocked(IdentityId IdentityId, DateTime UnlockedAt) : IDomainEvent;

