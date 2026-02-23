using Mamey.CQRS;
using Mamey.FWID.Identities.Domain.Entities;

namespace Mamey.FWID.Identities.Domain.Events;

/// <summary>
/// Domain event raised when multi-factor authentication is disabled.
/// </summary>
internal record MfaDisabled(IdentityId IdentityId, DateTime DisabledAt) : IDomainEvent;

