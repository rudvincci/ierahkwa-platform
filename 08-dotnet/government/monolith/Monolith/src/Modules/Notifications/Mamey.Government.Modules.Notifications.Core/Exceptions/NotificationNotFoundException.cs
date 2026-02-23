using Mamey.Exceptions;

namespace Mamey.Government.Modules.Notifications.Core.Exceptions
{
    public class NotificationNotFoundException : MameyException
    {
        public Guid Id { get; }

        public NotificationNotFoundException(Guid id) : base($"Notification with id '{id} was not found.'")
            => Id = id;
    }
}