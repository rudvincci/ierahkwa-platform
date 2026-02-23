using Mamey.CQRS.Events;
using Mamey.MicroMonolith.Abstractions.Contracts;
using Mamey.MicroMonolith.Abstractions.Messaging;

namespace Mamey.ApplicationName.Modules.Notifications.Core.Events.External.Users;

internal record TwoFactorEnabled(Guid UserId, string Email, string Role, string FullName, string ConfirmUrl) : IEvent;

[Message("users")]
internal class TwoFactorEnabledContract : Contract<TwoFactorEnabled>
{
    public TwoFactorEnabledContract()
    {
        RequireAll();
    }
}