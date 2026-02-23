using Mamey.CQRS.Events;
using Mamey.Types;

namespace Mamey.ApplicationName.Modules.Identity.Core.Events;

/// <summary>
/// Published when a new user is successfully signed in.
/// </summary>
public sealed record UserSignedIn(Guid UsedId, Guid TenantId, DateTime OccurredAt = default) : IEvent
{
    public UserSignedIn(UserId userId, Guid tenantId)
        : this(userId, tenantId, DateTime.UtcNow) { }
}