using Mamey.CQRS.Events;

namespace Mamey.ApplicationName.Modules.Identity.Core.Events.Rejected;

internal record PasswordResetConfirmationRejected(Guid UserId, string Reason) : IRejectedEvent
{
    public string Code { get; } = "password_reset_confirmation";
}