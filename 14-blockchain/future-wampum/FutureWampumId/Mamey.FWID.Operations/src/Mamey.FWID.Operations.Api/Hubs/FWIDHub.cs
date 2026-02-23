using Mamey.Auth.Jwt;
using Microsoft.AspNetCore.SignalR;
using Mamey.FWID.Operations.Api.Infrastructure;

namespace Mamey.FWID.Operations.Api.Hubs;

public class FWIDHub : Hub
{
    private readonly IJwtHandler _jwtHandler;

    public FWIDHub(IJwtHandler jwtHandler)
    {
        _jwtHandler = jwtHandler;
    }

    public async Task InitializeAsync(string token)
    {
        if (string.IsNullOrWhiteSpace(token))
        {
            await DisconnectAsync();
        }
        try
        {
            var payload = _jwtHandler.GetTokenPayload(token);

            if (payload is null)
            {
                await DisconnectAsync();

                return;
            }

            var group = Guid.Parse(payload.Subject).ToUserGroup();
            await Groups.AddToGroupAsync(Context.ConnectionId, group);
            await ConnectAsync();
        }
        catch
        {
            await DisconnectAsync();
        }
    }

    private async Task ConnectAsync()
    {
        await Clients.Client(Context.ConnectionId).SendAsync("connected");
    }

    private async Task DisconnectAsync()
    {
        await Clients.Client(Context.ConnectionId).SendAsync("disconnected");
    }
}



