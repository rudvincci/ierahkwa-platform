using Mamey.CQRS.Events;

namespace Mamey.ApplicationName.Modules.Identity.Core.Events;

internal record AuthenticatorEnabled(Guid UserId, string? Key) : IEvent;