using Mamey.Exceptions;

namespace Mamey.Government.Modules.Notifications.Core.Exceptions
{
    public sealed class NotificationAlreadyExistsException : MameyException
    {
        public Guid Id { get; }

        public NotificationAlreadyExistsException(Guid id) : base($"Notification with id: '{id}' already exists.")
            => Id = id;
    }
}