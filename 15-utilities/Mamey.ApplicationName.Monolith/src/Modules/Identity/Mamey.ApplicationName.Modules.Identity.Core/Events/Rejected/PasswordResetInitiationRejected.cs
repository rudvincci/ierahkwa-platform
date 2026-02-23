using Mamey.CQRS.Events;

namespace Mamey.ApplicationName.Modules.Identity.Core.Events.Rejected;

internal record PasswordResetInitiationRejected(string Email, string Reason) : IRejectedEvent
{
    public string Code { get; } = "user_authenicator_password_reset_initiation";
}