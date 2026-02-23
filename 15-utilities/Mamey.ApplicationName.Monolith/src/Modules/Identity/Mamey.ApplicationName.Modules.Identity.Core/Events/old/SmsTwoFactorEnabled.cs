using Mamey.CQRS.Events;

namespace Mamey.ApplicationName.Modules.Identity.Core.Events;

internal record SmsTwoFactorEnabled(Guid UserId, string PhoneNumber) : IEvent;