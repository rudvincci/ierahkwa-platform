using Mamey.CQRS.Events;

namespace Mamey.ApplicationName.Modules.Identity.Core.Events;

internal record SmsTwoFactorVerified(Guid UserId) : IEvent;