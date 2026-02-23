using Mamey.CQRS;
using Mamey.FWID.Identities.Domain.Entities;
using Mamey.Types;
using RoleId = Mamey.Types.RoleId;

namespace Mamey.FWID.Identities.Domain.Events;

/// <summary>
/// Domain event raised when a permission is removed from a role.
/// </summary>
internal record PermissionRemovedFromRole(RoleId RoleId, PermissionId PermissionId, DateTime RemovedAt) : IDomainEvent;

