using Mamey.CQRS;
using Mamey.FWID.Identities.Domain.Entities;
using Mamey.Types;
using RoleId = Mamey.Types.RoleId;

namespace Mamey.FWID.Identities.Domain.Events;

/// <summary>
/// Domain event raised when a role is updated.
/// </summary>
internal record RoleUpdated(RoleId RoleId, string Name, DateTime UpdatedAt) : IDomainEvent;

