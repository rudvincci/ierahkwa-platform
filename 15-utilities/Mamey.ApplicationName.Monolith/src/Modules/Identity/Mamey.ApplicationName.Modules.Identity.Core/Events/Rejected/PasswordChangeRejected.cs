using Mamey.CQRS.Events;

namespace Mamey.ApplicationName.Modules.Identity.Core.Events.Rejected;

internal record PasswordChangeRejected(Guid UserId, string Reason) : IRejectedEvent
{
    public string Code { get; } = "user_authenicator_password_change";
}