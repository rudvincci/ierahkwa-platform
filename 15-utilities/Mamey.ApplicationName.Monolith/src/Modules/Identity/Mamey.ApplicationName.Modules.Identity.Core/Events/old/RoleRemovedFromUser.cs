using Mamey.CQRS.Events;

namespace Mamey.ApplicationName.Modules.Identity.Core.Events;

internal record RoleRemovedFromUser(Guid UserId, string RoleName) : IEvent;