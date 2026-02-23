using Mamey.Exceptions;
using Mamey.Types;

namespace Pupitre.Notifications.Domain.Exceptions;

internal class InvalidNotificationTagsException : DomainException
{
    public override string Code { get; } = "invalid_notification_tags";

    public InvalidNotificationTagsException() : base("Notification tags are invalid.")
    {
    }
}
