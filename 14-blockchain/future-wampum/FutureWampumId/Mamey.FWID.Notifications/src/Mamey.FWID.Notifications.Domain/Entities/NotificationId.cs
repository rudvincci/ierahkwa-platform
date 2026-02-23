using System;
using Mamey.FWID.Identities.Domain.Entities;
using Mamey.Types;

namespace Mamey.FWID.Notifications.Domain.Entities;

/// <summary>
/// Notification identifier that includes the IdentityId.
/// </summary>
internal class NotificationId : AggregateId<Guid>
{
    public NotificationId(IdentityId identityId) : base(Guid.NewGuid())
        => IdentityId = identityId;

    public NotificationId(Guid value, IdentityId identityId) : base(value)
        => IdentityId = identityId;

    public IdentityId IdentityId { get; }
}







