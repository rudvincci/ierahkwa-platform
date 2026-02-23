using Mamey.CQRS.Events;
using Mamey.FWID.Identities.Domain.ValueObjects;
using Mamey.MessageBrokers;
using Mamey.Microservice.Abstractions.Messaging;

namespace Mamey.FWID.Identities.Application.Events.Integration.Mfa;

/// <summary>
/// Integration event raised when MFA is enabled for an identity.
/// </summary>
[Contract]
[Message("auth.mfa.enabled")]
internal record MfaEnabledIntegrationEvent(
    Guid IdentityId,
    string MfaMethod,
    DateTime EnabledAt) : IEvent;

