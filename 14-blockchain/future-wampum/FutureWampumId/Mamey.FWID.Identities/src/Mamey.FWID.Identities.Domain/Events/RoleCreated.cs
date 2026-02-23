using Mamey.CQRS;
using Mamey.FWID.Identities.Domain.Entities;
using Mamey.Types;
using RoleId = Mamey.Types.RoleId;

namespace Mamey.FWID.Identities.Domain.Events;

/// <summary>
/// Domain event raised when a role is created.
/// </summary>
internal record RoleCreated(RoleId RoleId, string Name, DateTime CreatedAt) : IDomainEvent;


