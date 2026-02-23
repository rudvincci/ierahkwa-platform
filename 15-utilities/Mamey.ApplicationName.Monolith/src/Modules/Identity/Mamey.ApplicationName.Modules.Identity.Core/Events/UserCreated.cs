using Mamey.CQRS.Events;
using Mamey.Types;

namespace Mamey.ApplicationName.Modules.Identity.Core.Events;

/// <summary>
/// Published when a new user is successfully created.
/// </summary>
public sealed record UserCreated(Guid UsedId, string Email,  DateTime OccurredAt = default) : IEvent
{
    public UserCreated(UserId userId, string email)
        : this(userId, email,  DateTime.UtcNow) { }
}