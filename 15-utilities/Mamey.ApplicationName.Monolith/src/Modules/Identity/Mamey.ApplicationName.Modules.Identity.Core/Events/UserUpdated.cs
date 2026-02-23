using Mamey.CQRS.Events;
using Mamey.Types;

namespace Mamey.ApplicationName.Modules.Identity.Core.Events;

public record UserUpdated(
    Guid UsedId,
    string Email,
    DateTime OccurredAt = default) : IEvent
{
    public UserUpdated(UserId userId, string changedBy, Guid tenantId)
        : this(userId, changedBy, DateTime.UtcNow) { }
}