using Mamey.CQRS.Events;

namespace Mamey.ApplicationName.Modules.Identity.Core.Events;

internal record PasswordResetInitiated(string Email, string ResetUrl) : IEvent;