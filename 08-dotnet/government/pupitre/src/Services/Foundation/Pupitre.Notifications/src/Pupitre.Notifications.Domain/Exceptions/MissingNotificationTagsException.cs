using Mamey.Exceptions;

namespace Pupitre.Notifications.Domain.Exceptions;

internal class MissingNotificationTagsException : DomainException
{
    public MissingNotificationTagsException()
        : base("Notification tags are missing.")
    {
    }

    public override string Code => "missing_notification_tags";
}