using Mamey.CQRS;
using Mamey.FWID.Identities.Domain.Entities;
using Mamey.Types;
using RoleId = Mamey.Types.RoleId;

namespace Mamey.FWID.Identities.Domain.Events;

/// <summary>
/// Domain event raised when a permission is added to a role.
/// </summary>
internal record PermissionAddedToRole(RoleId RoleId, PermissionId PermissionId, DateTime AddedAt) : IDomainEvent;

