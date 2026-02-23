using Mamey.CQRS;
using Mamey.FWID.Identities.Domain.Entities;

namespace Mamey.FWID.Identities.Domain.Events;

/// <summary>
/// Domain event raised when an email confirmation is sent.
/// </summary>
internal record EmailConfirmationSent(IdentityId IdentityId, string Email, string Token, DateTime SentAt, DateTime ExpiresAt) : IDomainEvent;
