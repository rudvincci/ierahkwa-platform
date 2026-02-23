using Mamey.CQRS.Events;
using Mamey.MessageBrokers;

namespace Mamey.FWID.Notifications.Application.Events.Integration.DIDs;

/// <summary>
/// Integration event raised when a DID is created in the DIDs service.
/// </summary>
[Message("dids")]
public record DIDCreatedIntegrationEvent(Guid DIDId, Guid IdentityId, string DidString, DateTime CreatedAt) : IEvent;







