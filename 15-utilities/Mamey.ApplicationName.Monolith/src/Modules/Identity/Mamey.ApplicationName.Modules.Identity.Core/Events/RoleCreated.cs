using Mamey.CQRS.Events;

namespace Mamey.ApplicationName.Modules.Identity.Core.Events;

public record RoleCreated(Guid RoleId, string Email) : IEvent;