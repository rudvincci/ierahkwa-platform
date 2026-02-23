using Mamey.CQRS.Events;
using Mamey.MessageBrokers;

namespace Mamey.FWID.Identities.Application.Events.Integration.AccessControls;

/// <summary>
/// Integration event raised when zone access is granted in the AccessControls service.
/// </summary>
[Message("accesscontrols")]
internal record ZoneAccessGrantedIntegrationEvent(Guid AccessControlId, Guid IdentityId, Guid ZoneId, string Permission, DateTime GrantedAt) : IEvent;
