using Mamey.CQRS.Events;
using Mamey.MessageBrokers;
using Mamey.Microservice.Abstractions.Messaging;

namespace Mamey.FWID.Identities.Application.Events.Integration.Roles;

/// <summary>
/// Integration event raised when a role is assigned to an identity.
/// </summary>
[Contract]
[Message("auth.role.assigned")]
internal record RoleAssignedIntegrationEvent(
    Guid IdentityId,
    Guid RoleId,
    string RoleName,
    DateTime AssignedAt) : IEvent;

