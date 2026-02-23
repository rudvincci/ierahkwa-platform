using Mamey.CQRS.Events;
using Mamey.MessageBrokers;
using Mamey.Microservice.Abstractions.Messaging;

namespace Mamey.FWID.Identities.Application.Events.Integration.Auth;

/// <summary>
/// Integration event raised when an identity signs out.
/// </summary>
[Contract]
[Message("auth.sign-out")]
internal record SignOutIntegrationEvent(
    Guid IdentityId,
    Guid SessionId,
    DateTime SignedOutAt) : IEvent;

