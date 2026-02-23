using Mamey.CQRS.Events;
using Mamey.MessageBrokers;
using Mamey.Microservice.Abstractions.Messaging;

namespace Mamey.FWID.Identities.Application.Events.Integration.Permissions;

/// <summary>
/// Integration event raised when a permission is removed from an identity.
/// </summary>
[Contract]
[Message("auth.permission.removed")]
internal record PermissionRemovedIntegrationEvent(
    Guid IdentityId,
    Guid PermissionId,
    string PermissionName,
    DateTime RemovedAt) : IEvent;

