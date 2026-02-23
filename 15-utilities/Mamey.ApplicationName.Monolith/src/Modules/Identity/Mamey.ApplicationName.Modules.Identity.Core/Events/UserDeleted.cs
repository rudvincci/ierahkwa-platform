using Mamey.CQRS.Events;

namespace Mamey.ApplicationName.Modules.Identity.Core.Events;

public record UserDeleted(Guid UsedId) : IEvent;