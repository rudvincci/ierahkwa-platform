using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;

namespace Mamey.Casino.Infrastructure.Hubs;

[Authorize]               // only authenticated users may connect
public class NotificationHub : Hub
{
    // You can override these if you want to track connections:
    public override Task OnConnectedAsync()    => base.OnConnectedAsync();
    public override Task OnDisconnectedAsync(Exception? ex) 
        => base.OnDisconnectedAsync(ex);

    // (optional) hub method clients can call
    public Task Ping() => Clients.Caller.SendAsync("Pong");
}