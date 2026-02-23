using Mamey.CQRS.Events;
using Mamey.FWID.Identities.Domain.ValueObjects;
using Mamey.MessageBrokers;
using Mamey.Microservice.Abstractions.Messaging;

namespace Mamey.FWID.Identities.Application.Events.Integration.Mfa;

/// <summary>
/// Integration event raised when MFA is successfully verified.
/// </summary>
[Contract]
[Message("auth.mfa.verified")]
internal record MfaVerifiedIntegrationEvent(
    Guid IdentityId,
    string MfaMethod,
    DateTime VerifiedAt) : IEvent;

