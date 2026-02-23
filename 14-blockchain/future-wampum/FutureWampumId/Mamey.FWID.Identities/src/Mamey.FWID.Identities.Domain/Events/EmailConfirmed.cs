using Mamey.CQRS;
using Mamey.FWID.Identities.Domain.Entities;

namespace Mamey.FWID.Identities.Domain.Events;

/// <summary>
/// Domain event raised when an email is confirmed.
/// </summary>
internal record EmailConfirmed(IdentityId IdentityId, DateTime ConfirmedAt) : IDomainEvent;

