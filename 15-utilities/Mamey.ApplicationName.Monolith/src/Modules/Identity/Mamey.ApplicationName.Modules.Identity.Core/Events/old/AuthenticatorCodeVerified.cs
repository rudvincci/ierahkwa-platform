using Mamey.CQRS.Events;

namespace Mamey.ApplicationName.Modules.Identity.Core.Events;

internal record AuthenticatorCodeVerified(Guid UsedId) : IEvent;