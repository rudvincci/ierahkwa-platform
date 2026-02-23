using Mamey.Exceptions;
using System;
using Mamey.Types;

namespace Pupitre.Notifications.Domain.Exceptions;

internal class MissingNotificationNameException : DomainException
{
    public MissingNotificationNameException()
        : base("Notification name is missing.")
    {
    }

    public override string Code => "missing_notification_name";
}
