using Mamey.CQRS;
using Mamey.FWID.Identities.Domain.Entities;
using Mamey.Types;

namespace Mamey.FWID.Identities.Domain.Events;

/// <summary>
/// Domain event raised when a permission is updated.
/// </summary>
internal record PermissionUpdated(PermissionId PermissionId, string Name, DateTime UpdatedAt) : IDomainEvent;

