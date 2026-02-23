using Mamey.CQRS.Events;
using Mamey.MessageBrokers;

namespace Mamey.FWID.Notifications.Application.Events.Integration.Credentials;

/// <summary>
/// Integration event raised when a credential is issued in the Credentials service.
/// </summary>
[Message("credentials")]
public record CredentialIssuedIntegrationEvent(Guid CredentialId, Guid IdentityId, string CredentialType, Guid IssuerId, DateTime IssuedAt) : IEvent;







