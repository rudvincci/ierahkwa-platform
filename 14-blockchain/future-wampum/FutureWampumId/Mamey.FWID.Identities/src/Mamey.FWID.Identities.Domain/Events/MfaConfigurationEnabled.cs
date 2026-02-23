using Mamey.CQRS;
using Mamey.FWID.Identities.Domain.Entities;
using Mamey.FWID.Identities.Domain.ValueObjects;

namespace Mamey.FWID.Identities.Domain.Events;

/// <summary>
/// Domain event raised when an MFA configuration is enabled.
/// </summary>
internal record MfaConfigurationEnabled(MfaConfigurationId MfaConfigurationId, IdentityId IdentityId, MfaMethod Method, DateTime EnabledAt) : IDomainEvent;

