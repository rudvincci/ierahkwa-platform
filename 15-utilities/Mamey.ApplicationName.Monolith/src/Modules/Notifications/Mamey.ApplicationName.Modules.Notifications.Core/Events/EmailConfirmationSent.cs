using Mamey.CQRS.Events;

namespace Mamey.ApplicationName.Modules.Notifications.Core.Events;

internal record EmailConfirmationSent(Guid UserId, string Email) : IEvent;