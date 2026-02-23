using Mamey.CQRS;
using Mamey.FWID.Identities.Domain.Entities;

namespace Mamey.FWID.Identities.Domain.Events;

/// <summary>
/// Domain event raised when a permission is assigned to an identity.
/// </summary>
internal record PermissionAssigned(IdentityId IdentityId, Guid PermissionId, DateTime AssignedAt) : IDomainEvent;

