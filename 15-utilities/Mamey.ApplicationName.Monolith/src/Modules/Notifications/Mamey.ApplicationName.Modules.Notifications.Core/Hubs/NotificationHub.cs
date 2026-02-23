using System.Collections.Concurrent;
using Mamey.ApplicationName.Modules.Notifications.Core.Domain.Entities;
using Microsoft.AspNetCore.SignalR;

namespace Mamey.ApplicationName.Modules.Notifications.Core.Hubs;

internal class NotificationHub: Hub
{
    private static readonly ConcurrentDictionary<string, string> ConnectedUsers = new();

    public override Task OnConnectedAsync()
    {
        string userId = Context.UserIdentifier;
        if (!string.IsNullOrEmpty(userId))
        {
            ConnectedUsers[userId] = Context.ConnectionId;
        }
        return base.OnConnectedAsync();
    }

    public override Task OnDisconnectedAsync(Exception? exception)
    {
        string userId = Context.UserIdentifier;
        if (!string.IsNullOrEmpty(userId))
        {
            ConnectedUsers.TryRemove(userId, out _);
        }
        return base.OnDisconnectedAsync(exception);
    }

    public async Task SendNotificationToUser(string userId, Notification notification)
    {
        if (ConnectedUsers.TryGetValue(userId, out var connectionId))
        {
            await Clients.Client(connectionId).SendAsync("ReceiveNotification", notification);
        }
    }

    public async Task BroadcastNotification(Notification notification)
    {
        await Clients.All.SendAsync("ReceiveNotification", notification);
    }
}