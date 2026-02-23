using Mamey.CQRS.Events;

namespace Mamey.ApplicationName.Modules.Identity.Core.Events;

internal record RoleAddedToUser(Guid UserId, string RoleName) : IEvent;