using Mamey.CQRS.Events;

namespace Mamey.ApplicationName.Modules.Identity.Core.Events.Rejected;

internal record UserEmailConfirmationRejected(Guid UserId, string Reason) : IRejectedEvent
{
    public string Code { get; } = "email_confirmation_rejected";
}