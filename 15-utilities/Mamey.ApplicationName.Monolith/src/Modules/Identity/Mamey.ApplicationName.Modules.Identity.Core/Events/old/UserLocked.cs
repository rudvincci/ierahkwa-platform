using Mamey.CQRS.Events;

namespace Mamey.ApplicationName.Modules.Identity.Core.Events;

internal record UserLocked(Guid UserId, DateTimeOffset? LockoutEnd) : IEvent;