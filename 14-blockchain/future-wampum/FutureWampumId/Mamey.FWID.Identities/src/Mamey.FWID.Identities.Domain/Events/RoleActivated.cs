using Mamey.CQRS;
using Mamey.FWID.Identities.Domain.Entities;
using Mamey.Types;
using RoleId = Mamey.Types.RoleId;

namespace Mamey.FWID.Identities.Domain.Events;

/// <summary>
/// Domain event raised when a role is activated.
/// </summary>
internal record RoleActivated(RoleId RoleId, DateTime ActivatedAt) : IDomainEvent;

