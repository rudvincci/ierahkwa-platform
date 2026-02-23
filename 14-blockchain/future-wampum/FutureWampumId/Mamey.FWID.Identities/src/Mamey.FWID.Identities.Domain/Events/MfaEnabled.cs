using Mamey.CQRS;
using Mamey.FWID.Identities.Domain.Entities;
using Mamey.FWID.Identities.Domain.ValueObjects;

namespace Mamey.FWID.Identities.Domain.Events;

/// <summary>
/// Domain event raised when multi-factor authentication is enabled.
/// </summary>
internal record MfaEnabled(IdentityId IdentityId, MfaMethod Method, DateTime EnabledAt) : IDomainEvent;

