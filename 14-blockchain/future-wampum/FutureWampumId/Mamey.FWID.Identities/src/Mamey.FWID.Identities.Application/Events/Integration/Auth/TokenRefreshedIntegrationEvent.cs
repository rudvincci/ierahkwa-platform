using Mamey.CQRS.Events;
using Mamey.MessageBrokers;
using Mamey.Microservice.Abstractions.Messaging;

namespace Mamey.FWID.Identities.Application.Events.Integration.Auth;

/// <summary>
/// Integration event raised when an access token is refreshed.
/// </summary>
[Contract]
[Message("auth.token-refreshed")]
internal record TokenRefreshedIntegrationEvent(
    Guid IdentityId,
    Guid SessionId,
    DateTime RefreshedAt) : IEvent;

