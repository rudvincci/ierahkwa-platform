using Mamey.CQRS.Events;
using Mamey.MessageBrokers;

namespace Mamey.FWID.Identities.Application.Events.Integration.Credentials;

/// <summary>
/// Integration event raised when a credential is issued in the Credentials service.
/// </summary>
[Message("credentials")]
internal record CredentialIssuedIntegrationEvent(Guid CredentialId, Guid IdentityId, string CredentialType, Guid IssuerId, DateTime IssuedAt) : IEvent;

