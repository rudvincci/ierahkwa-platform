using Mamey.CQRS.Events;
using Mamey.MessageBrokers;
using Mamey.Microservice.Abstractions.Messaging;

namespace Mamey.FWID.Identities.Application.Events.Integration.Confirmations;

/// <summary>
/// Integration event raised when an email is confirmed.
/// </summary>
[Contract]
[Message("auth.email.confirmed")]
internal record EmailConfirmedIntegrationEvent(
    Guid IdentityId,
    string Email,
    DateTime ConfirmedAt) : IEvent;

