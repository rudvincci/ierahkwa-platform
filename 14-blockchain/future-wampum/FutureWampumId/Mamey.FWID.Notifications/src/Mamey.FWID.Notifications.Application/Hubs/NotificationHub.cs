using Microsoft.AspNetCore.SignalR;

namespace Mamey.FWID.Notifications.Application.Hubs;

/// <summary>
/// SignalR hub for real-time notifications.
/// </summary>
public class NotificationHub : Hub
{
    /// <summary>
    /// Sends a notification to a specific identity.
    /// </summary>
    public async Task SendToIdentity(Guid identityId, object notification)
    {
        await Clients.Group($"identity-{identityId}").SendAsync("ReceiveNotification", notification);
    }
}







