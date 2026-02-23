using Mamey.CQRS;
using Mamey.FWID.Identities.Domain.Entities;
using Mamey.FWID.Identities.Domain.ValueObjects;

namespace Mamey.FWID.Identities.Domain.Events;

/// <summary>
/// Domain event raised when an MFA configuration is disabled.
/// </summary>
internal record MfaConfigurationDisabled(MfaConfigurationId MfaConfigurationId, IdentityId IdentityId, MfaMethod Method, DateTime DisabledAt) : IDomainEvent;

