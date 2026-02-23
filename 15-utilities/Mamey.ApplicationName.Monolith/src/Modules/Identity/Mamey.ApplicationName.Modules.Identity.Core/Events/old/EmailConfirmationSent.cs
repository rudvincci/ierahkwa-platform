using Mamey.CQRS.Events;

namespace Mamey.ApplicationName.Modules.Identity.Core.Events;

internal record EmailConfirmationSent(Guid UserId, string Email) : IEvent;