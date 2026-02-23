using Mamey.CQRS;
using Mamey.FWID.Identities.Domain.Entities;

namespace Mamey.FWID.Identities.Domain.Events;

/// <summary>
/// Domain event raised when an SMS is confirmed.
/// </summary>
internal record SmsConfirmed(IdentityId IdentityId, DateTime ConfirmedAt) : IDomainEvent;

