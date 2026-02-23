using Mamey.CQRS.Events;
using Mamey.MessageBrokers;

namespace Mamey.FWID.Notifications.Application.Events.Integration.AccessControls;

/// <summary>
/// Integration event raised when zone access is granted in the AccessControls service.
/// </summary>
[Message("accesscontrols")]
public record ZoneAccessGrantedIntegrationEvent(Guid AccessControlId, Guid IdentityId, Guid ZoneId, string Permission, DateTime GrantedAt) : IEvent;







