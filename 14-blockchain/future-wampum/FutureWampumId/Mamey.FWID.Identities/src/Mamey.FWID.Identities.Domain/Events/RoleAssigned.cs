using Mamey.CQRS;
using Mamey.FWID.Identities.Domain.Entities;

namespace Mamey.FWID.Identities.Domain.Events;

/// <summary>
/// Domain event raised when a role is assigned to an identity.
/// </summary>
internal record RoleAssigned(IdentityId IdentityId, Guid RoleId, DateTime AssignedAt) : IDomainEvent;

