using Mamey.CQRS.Events;
using Mamey.MessageBrokers;
using Mamey.Microservice.Abstractions.Messaging;

namespace Mamey.FWID.Identities.Application.Events.Integration.Confirmations;

/// <summary>
/// Integration event raised when an SMS/phone number is confirmed.
/// </summary>
[Contract]
[Message("auth.sms.confirmed")]
internal record SmsConfirmedIntegrationEvent(
    Guid IdentityId,
    string PhoneNumber,
    DateTime ConfirmedAt) : IEvent;

