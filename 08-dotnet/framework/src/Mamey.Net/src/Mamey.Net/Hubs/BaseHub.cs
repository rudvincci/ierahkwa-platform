using Microsoft.AspNetCore.SignalR;
namespace Mamey.Net.Hubs;

public abstract class BaseHub : Hub
{
    public BaseHub()
    {
    }

    public async Task SendMessageToClientAsync(string userId, string message)
        => await Clients.Client(userId).SendAsync("ReceiveMessage", message);

    public async Task SendToClientAsync<T>(string userId, T obj, CancellationToken cancellationToken = default)
        => await SendToClientAsync($"Receive{nameof(T)}Object", userId, obj, cancellationToken);

    public async Task SendToClientAsync<T>(string method, string userId, T obj, CancellationToken cancellationToken = default)
        => await Clients.Client(userId).SendAsync(method, obj, cancellationToken);

    public async Task SendMessageToAllClientsAsync(string message, CancellationToken cancellationToken = default)
        => await Clients.All.SendAsync("ReceiveGlobalMessage", message, cancellationToken);

    public async Task SendToAllClientsAsync<T>(T obj, CancellationToken cancellationToken = default)
        => await Clients.All.SendAsync($"Receive{nameof(T)}Object", obj, cancellationToken);

    public async IAsyncEnumerable<DateTime> CurrentUtcDateTimeStreamAsync(CancellationToken cancellationToken)
    {
        while (true)
        {
            yield return DateTime.UtcNow;
            await Task.Delay(1000, cancellationToken);
        }
    }
}



