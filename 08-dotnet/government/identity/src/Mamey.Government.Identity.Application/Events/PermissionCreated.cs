using Mamey.CQRS.Events;
using Mamey.Government.Identity.Domain.Entities;
using Mamey.Types;

namespace Mamey.Government.Identity.Application.Events;

internal record PermissionCreated(PermissionId PermissionId, string Name, string Resource, string Action, DateTime CreatedAt) : IEvent;
