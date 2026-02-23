using Mamey.CQRS;
using Mamey.FWID.Identities.Domain.Entities;

namespace Mamey.FWID.Identities.Domain.Events;

/// <summary>
/// Domain event raised when a permission is removed from an identity.
/// </summary>
internal record PermissionRemoved(IdentityId IdentityId, Guid PermissionId, DateTime RemovedAt) : IDomainEvent;

