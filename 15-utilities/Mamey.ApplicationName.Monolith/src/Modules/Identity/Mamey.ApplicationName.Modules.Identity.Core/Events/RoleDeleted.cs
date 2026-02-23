using Mamey.CQRS.Events;

namespace Mamey.ApplicationName.Modules.Identity.Core.Events;

public record RoleDeleted(Guid RoleId) : IEvent;