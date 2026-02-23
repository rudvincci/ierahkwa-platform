using Mamey.CQRS.Events;

namespace Mamey.ApplicationName.Modules.Identity.Core.Events;

internal class PasswordResetConfirmed(Guid UserId) : IEvent;