using Mamey.CQRS;
using Mamey.FWID.Identities.Domain.Entities;

namespace Mamey.FWID.Identities.Domain.Events;

/// <summary>
/// Domain event raised when an SMS confirmation is sent.
/// </summary>
internal record SmsConfirmationSent(IdentityId IdentityId, string PhoneNumber, string Code, DateTime SentAt, DateTime ExpiresAt) : IDomainEvent;

