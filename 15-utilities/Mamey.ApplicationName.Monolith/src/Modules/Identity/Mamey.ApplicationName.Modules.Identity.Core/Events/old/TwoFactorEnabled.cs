using Mamey.CQRS.Events;

namespace Mamey.ApplicationName.Modules.Identity.Core.Events;

internal record TwoFactorEnabled(Guid UserId) : IEvent;