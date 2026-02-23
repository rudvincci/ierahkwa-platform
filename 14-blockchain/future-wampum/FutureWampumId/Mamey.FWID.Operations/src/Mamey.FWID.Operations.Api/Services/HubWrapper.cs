using Mamey.FWID.Operations.Api.Hubs;
using Mamey.FWID.Operations.Api.Infrastructure;
using Microsoft.AspNetCore.SignalR;

namespace Mamey.FWID.Operations.Api.Services;

public class HubWrapper : IHubWrapper
{
    private readonly IHubContext<FWIDHub> _hubContext;

    public HubWrapper(IHubContext<FWIDHub> hubContext)
    {
        _hubContext = hubContext;
    }

    public async Task PublishToUserAsync(string userId, string message, object data)
        => await _hubContext.Clients.Group(userId.ToUserGroup()).SendAsync(message, data);

    public async Task PublishToAllAsync(string message, object data)
        => await _hubContext.Clients.All.SendAsync(message, data);
}



