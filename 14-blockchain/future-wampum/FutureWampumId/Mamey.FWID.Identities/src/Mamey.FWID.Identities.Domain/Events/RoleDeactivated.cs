using Mamey.CQRS;
using Mamey.FWID.Identities.Domain.Entities;
using Mamey.Types;
using RoleId = Mamey.Types.RoleId;

namespace Mamey.FWID.Identities.Domain.Events;

/// <summary>
/// Domain event raised when a role is deactivated.
/// </summary>
internal record RoleDeactivated(RoleId RoleId, DateTime DeactivatedAt) : IDomainEvent;

