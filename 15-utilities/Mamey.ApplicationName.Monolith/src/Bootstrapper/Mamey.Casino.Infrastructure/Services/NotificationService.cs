using Mamey.Casino.Infrastructure.Hubs;
using Microsoft.AspNetCore.SignalR;

namespace Mamey.Casino.Infrastructure.Services;
public class NotificationService : INotificationService
{
    private readonly IHubContext<NotificationHub> _hub;

    public NotificationService(IHubContext<NotificationHub> hub)
    {
        _hub = hub;
    }

    public Task NotifyUserAsync(Guid userId, string message)
    {
        // userId.ToString() must match the NameIdentifier claim on the connection
        return _hub.Clients
            .User(userId.ToString())
            .SendAsync("ReceiveNotification", message);
    }
}
public interface INotificationService
{
    Task NotifyUserAsync(Guid userId, string message);
}