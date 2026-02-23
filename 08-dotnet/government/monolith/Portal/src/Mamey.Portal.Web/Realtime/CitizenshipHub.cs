using Microsoft.AspNetCore.SignalR;

namespace Mamey.Portal.Web.Realtime;

public sealed class CitizenshipHub : Hub
{
    public Task JoinCitizen(string tenantId, string email)
    {
        tenantId = (tenantId ?? string.Empty).Trim().ToLowerInvariant();
        email = (email ?? string.Empty).Trim().ToLowerInvariant();

        if (string.IsNullOrWhiteSpace(tenantId) || string.IsNullOrWhiteSpace(email))
        {
            return Task.CompletedTask;
        }

        return Groups.AddToGroupAsync(Context.ConnectionId, CitizenGroup(tenantId, email));
    }

    public Task LeaveCitizen(string tenantId, string email)
    {
        tenantId = (tenantId ?? string.Empty).Trim().ToLowerInvariant();
        email = (email ?? string.Empty).Trim().ToLowerInvariant();

        if (string.IsNullOrWhiteSpace(tenantId) || string.IsNullOrWhiteSpace(email))
        {
            return Task.CompletedTask;
        }

        return Groups.RemoveFromGroupAsync(Context.ConnectionId, CitizenGroup(tenantId, email));
    }

    public static string CitizenGroup(string tenantId, string email)
        => $"citizen:{tenantId}:{email}";
}




