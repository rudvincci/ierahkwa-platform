using Mamey.CQRS.Events;

namespace Mamey.ApplicationName.Modules.Identity.Core.Events;

public record RoleUpdated(Guid RoleId, string Email) : IEvent;