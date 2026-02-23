using Mamey.ApplicationName.Modules.Notifications.Core.Services;
using Mamey.CQRS.Events;
using Mamey.MicroMonolith.Abstractions.Messaging;

namespace Mamey.ApplicationName.Modules.Notifications.Core.Events.External.Users.Handlers;

internal sealed class TwoFactorEnabledHandler : IEventHandler<TwoFactorEnabled>
{
    private readonly IUserService _userService;
    private readonly INotificationService _notificationService;
    private readonly IMessageBroker _messageBroker;
    public Task HandleAsync(TwoFactorEnabled @event, CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException();
    }
}