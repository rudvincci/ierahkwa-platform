using Mamey.CQRS.Events;

namespace Mamey.ApplicationName.Modules.Identity.Core.Events;

internal record TwoFactorDisabled(Guid UserId) : IEvent;