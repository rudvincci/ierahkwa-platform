using Mamey.CQRS;
using Mamey.FWID.Identities.Domain.Entities;
using Mamey.Types;

namespace Mamey.FWID.Identities.Domain.Events;

/// <summary>
/// Domain event raised when a permission is deactivated.
/// </summary>
internal record PermissionDeactivated(PermissionId PermissionId, DateTime DeactivatedAt) : IDomainEvent;

