using Mamey.CQRS.Events;
using Mamey.Government.Identity.Domain.Entities;
using Mamey.Types;

namespace Mamey.Government.Identity.Application.Events;

internal record RoleCreated(RoleId RoleId, string Name, string Description, DateTime CreatedAt) : IEvent;
