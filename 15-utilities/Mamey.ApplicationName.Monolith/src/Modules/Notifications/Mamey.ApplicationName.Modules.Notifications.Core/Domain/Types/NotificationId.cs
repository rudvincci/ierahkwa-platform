using Mamey.Types;

namespace Mamey.ApplicationName.Modules.Notifications.Core.Domain.Types;

public class NotificationId : TypeId
{
    public NotificationId(Guid value) : base(value)
    {
    }

    public static implicit operator NotificationId(Guid id) => new(id);

    public static implicit operator Guid(NotificationId id) => id.Value;
}