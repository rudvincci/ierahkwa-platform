using Mamey.CQRS.Events;
using Mamey.MessageBrokers;

namespace Mamey.FWID.Notifications.Application.Events.Integration.Identities;

/// <summary>
/// Integration event raised when an identity is created in the Identities service.
/// </summary>
[Message("identities")]
public record IdentityCreatedIntegrationEvent(Guid IdentityId, string Name, DateTime CreatedAt, string? Zone) : IEvent;







