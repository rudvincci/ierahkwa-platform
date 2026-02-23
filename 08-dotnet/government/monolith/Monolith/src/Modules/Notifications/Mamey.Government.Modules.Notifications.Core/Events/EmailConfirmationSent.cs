using Mamey.CQRS.Events;

namespace Mamey.Government.Modules.Notifications.Core.Events;

internal record EmailConfirmationSent(Guid UserId, string Email) : IEvent;