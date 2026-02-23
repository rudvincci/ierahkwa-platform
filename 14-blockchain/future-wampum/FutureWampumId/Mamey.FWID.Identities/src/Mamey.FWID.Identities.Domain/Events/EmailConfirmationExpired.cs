using Mamey.CQRS;
using Mamey.FWID.Identities.Domain.Entities;

namespace Mamey.FWID.Identities.Domain.Events;

/// <summary>
/// Domain event raised when an email confirmation expires.
/// </summary>
internal record EmailConfirmationExpired(EmailConfirmationId EmailConfirmationId, IdentityId IdentityId, DateTime ExpiredAt) : IDomainEvent;

