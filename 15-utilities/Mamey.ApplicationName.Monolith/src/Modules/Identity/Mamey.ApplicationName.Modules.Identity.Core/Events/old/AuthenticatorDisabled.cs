using Mamey.CQRS.Events;

namespace Mamey.ApplicationName.Modules.Identity.Core.Events;

internal record AuthenticatorDisabled(Guid UserId) : IEvent;