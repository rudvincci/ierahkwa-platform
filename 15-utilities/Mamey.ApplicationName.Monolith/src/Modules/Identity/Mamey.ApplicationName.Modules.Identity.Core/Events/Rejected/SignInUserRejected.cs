using Mamey.CQRS.Events;

namespace Mamey.ApplicationName.Modules.Identity.Core.Events.Rejected;

internal record SignInUserRejected(Guid UserId, string Reason) : IRejectedEvent
{
    public string Code { get; } = "sign_in_user_rejected";
}