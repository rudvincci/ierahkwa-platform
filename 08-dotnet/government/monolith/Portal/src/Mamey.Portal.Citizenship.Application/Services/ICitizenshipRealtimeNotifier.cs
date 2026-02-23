namespace Mamey.Portal.Citizenship.Application.Services;

// App-layer abstraction so Infrastructure can publish domain events without depending on Web/SignalR.
public interface ICitizenshipRealtimeNotifier
{
    Task NotifyApplicationUpdatedAsync(string tenantId, string email, Guid applicationId, CancellationToken ct = default);
    Task NotifyIssuedDocumentCreatedAsync(string tenantId, string email, Guid applicationId, Guid issuedDocumentId, CancellationToken ct = default);
}




