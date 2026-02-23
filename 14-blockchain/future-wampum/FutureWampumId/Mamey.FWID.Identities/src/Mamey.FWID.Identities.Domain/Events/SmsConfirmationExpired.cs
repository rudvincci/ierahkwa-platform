using Mamey.CQRS;
using Mamey.FWID.Identities.Domain.Entities;

namespace Mamey.FWID.Identities.Domain.Events;

/// <summary>
/// Domain event raised when an SMS confirmation expires.
/// </summary>
internal record SmsConfirmationExpired(SmsConfirmationId SmsConfirmationId, IdentityId IdentityId, DateTime ExpiredAt) : IDomainEvent;

