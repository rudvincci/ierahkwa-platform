using Mamey.CQRS;
using Mamey.FWID.Identities.Domain.Entities;
using Mamey.Types;

namespace Mamey.FWID.Identities.Domain.Events;

/// <summary>
/// Domain event raised when a permission is activated.
/// </summary>
internal record PermissionActivated(PermissionId PermissionId, DateTime ActivatedAt) : IDomainEvent;

