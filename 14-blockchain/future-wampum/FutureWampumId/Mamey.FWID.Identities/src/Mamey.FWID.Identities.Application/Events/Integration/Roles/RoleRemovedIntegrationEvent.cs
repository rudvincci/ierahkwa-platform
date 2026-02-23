using Mamey.CQRS.Events;
using Mamey.MessageBrokers;
using Mamey.Microservice.Abstractions.Messaging;

namespace Mamey.FWID.Identities.Application.Events.Integration.Roles;

/// <summary>
/// Integration event raised when a role is removed from an identity.
/// </summary>
[Contract]
[Message("auth.role.removed")]
internal record RoleRemovedIntegrationEvent(
    Guid IdentityId,
    Guid RoleId,
    string RoleName,
    DateTime RemovedAt) : IEvent;

