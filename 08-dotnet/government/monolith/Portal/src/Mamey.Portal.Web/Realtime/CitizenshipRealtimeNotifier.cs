using Microsoft.AspNetCore.SignalR;
using Mamey.Portal.Citizenship.Application.Models;
using Mamey.Portal.Citizenship.Application.Services;
using Microsoft.Extensions.Logging;

namespace Mamey.Portal.Web.Realtime;

public sealed class CitizenshipRealtimeNotifier : ICitizenshipRealtimeNotifier
{
    private readonly IHubContext<CitizenshipHub> _hub;
    private readonly ILogger<CitizenshipRealtimeNotifier> _logger;

    public CitizenshipRealtimeNotifier(IHubContext<CitizenshipHub> hub, ILogger<CitizenshipRealtimeNotifier> logger)
    {
        _hub = hub;
        _logger = logger;
    }

    public Task NotifyApplicationUpdatedAsync(string tenantId, string email, Guid applicationId, CancellationToken ct = default)
    {
        var group = CitizenshipHub.CitizenGroup(tenantId.ToLowerInvariant(), email.ToLowerInvariant());
        _logger.LogInformation("Citizenship realtime: ApplicationUpdated -> group={Group} applicationId={ApplicationId}", group, applicationId);
        return _hub.Clients.Group(group).SendAsync(CitizenshipRealtimeEvents.ApplicationUpdated, applicationId, cancellationToken: ct);
    }

    public Task NotifyIssuedDocumentCreatedAsync(string tenantId, string email, Guid applicationId, Guid issuedDocumentId, CancellationToken ct = default)
    {
        var group = CitizenshipHub.CitizenGroup(tenantId.ToLowerInvariant(), email.ToLowerInvariant());
        _logger.LogInformation(
            "Citizenship realtime: IssuedDocumentCreated -> group={Group} applicationId={ApplicationId} issuedDocumentId={IssuedDocumentId}",
            group,
            applicationId,
            issuedDocumentId);
        return _hub.Clients.Group(group).SendAsync(CitizenshipRealtimeEvents.IssuedDocumentCreated, new { applicationId, issuedDocumentId }, cancellationToken: ct);
    }
}


