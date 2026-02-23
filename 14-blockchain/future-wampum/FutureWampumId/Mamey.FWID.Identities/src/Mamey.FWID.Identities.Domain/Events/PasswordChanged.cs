using Mamey.CQRS;
using Mamey.FWID.Identities.Domain.Entities;

namespace Mamey.FWID.Identities.Domain.Events;

/// <summary>
/// Domain event raised when a password is changed.
/// </summary>
internal record PasswordChanged(IdentityId IdentityId, DateTime ChangedAt) : IDomainEvent;

