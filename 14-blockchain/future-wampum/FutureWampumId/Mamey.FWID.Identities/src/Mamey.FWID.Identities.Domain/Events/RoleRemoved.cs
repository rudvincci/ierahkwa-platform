using Mamey.CQRS;
using Mamey.FWID.Identities.Domain.Entities;

namespace Mamey.FWID.Identities.Domain.Events;

/// <summary>
/// Domain event raised when a role is removed from an identity.
/// </summary>
internal record RoleRemoved(IdentityId IdentityId, Guid RoleId, DateTime RemovedAt) : IDomainEvent;

