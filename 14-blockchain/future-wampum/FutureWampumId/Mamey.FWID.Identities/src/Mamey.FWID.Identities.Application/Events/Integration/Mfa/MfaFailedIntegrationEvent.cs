using Mamey.CQRS.Events;
using Mamey.FWID.Identities.Domain.ValueObjects;
using Mamey.MessageBrokers;
using Mamey.Microservice.Abstractions.Messaging;

namespace Mamey.FWID.Identities.Application.Events.Integration.Mfa;

/// <summary>
/// Integration event raised when MFA verification fails.
/// </summary>
[Contract]
[Message("auth.mfa.failed")]
internal record MfaFailedIntegrationEvent(
    Guid IdentityId,
    string MfaMethod,
    string Reason,
    DateTime FailedAt) : IEvent;

