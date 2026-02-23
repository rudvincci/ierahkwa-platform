using Mamey.CQRS.Events;
using Mamey.MessageBrokers;
using Mamey.Microservice.Abstractions.Messaging;

namespace Mamey.FWID.Identities.Application.Events.Integration.Permissions;

/// <summary>
/// Integration event raised when a permission is assigned to an identity.
/// </summary>
[Contract]
[Message("auth.permission.assigned")]
internal record PermissionAssignedIntegrationEvent(
    Guid IdentityId,
    Guid PermissionId,
    string PermissionName,
    DateTime AssignedAt) : IEvent;

