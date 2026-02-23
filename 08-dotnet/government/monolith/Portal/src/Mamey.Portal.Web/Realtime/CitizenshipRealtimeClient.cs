using Microsoft.AspNetCore.SignalR.Client;
using Mamey.Portal.Citizenship.Application.Models;
using System.Reactive.Subjects;

namespace Mamey.Portal.Web.Realtime;

public interface ICitizenshipRealtimeClient : IAsyncDisposable
{
    IObservable<Guid> ApplicationUpdated { get; }
    IObservable<(Guid ApplicationId, Guid IssuedDocumentId)> IssuedDocumentCreated { get; }

    Task StartAsync(Uri baseUri, string tenantId, string email, CancellationToken ct = default);
}

public sealed class CitizenshipRealtimeClient : ICitizenshipRealtimeClient
{
    private readonly Subject<Guid> _applicationUpdated = new();
    private readonly Subject<(Guid ApplicationId, Guid IssuedDocumentId)> _issuedDocumentCreated = new();

    private HubConnection? _connection;

    public IObservable<Guid> ApplicationUpdated => _applicationUpdated;
    public IObservable<(Guid ApplicationId, Guid IssuedDocumentId)> IssuedDocumentCreated => _issuedDocumentCreated;

    public async Task StartAsync(Uri baseUri, string tenantId, string email, CancellationToken ct = default)
    {
        tenantId = (tenantId ?? string.Empty).Trim();
        email = (email ?? string.Empty).Trim();

        if (string.IsNullOrWhiteSpace(tenantId) || string.IsNullOrWhiteSpace(email))
        {
            throw new ArgumentException("tenantId and email are required.");
        }

        var hubUrl = new Uri(baseUri, "/hubs/citizenship");

        _connection = new HubConnectionBuilder()
            .WithUrl(hubUrl)
            .WithAutomaticReconnect()
            .Build();

        _connection.On<Guid>(CitizenshipRealtimeEvents.ApplicationUpdated, appId =>
        {
            _applicationUpdated.OnNext(appId);
        });

        _connection.On<object>(CitizenshipRealtimeEvents.IssuedDocumentCreated, payload =>
        {
            // payload arrives as a JSON object; easiest is to re-serialize via System.Text.Json
            try
            {
                var json = System.Text.Json.JsonSerializer.Serialize(payload);
                var obj = System.Text.Json.JsonSerializer.Deserialize<IssuedDocPayload>(json);
                if (obj is not null)
                {
                    _issuedDocumentCreated.OnNext((obj.applicationId, obj.issuedDocumentId));
                }
            }
            catch
            {
                // ignore malformed payload
            }
        });

        await _connection.StartAsync(ct);
        await _connection.InvokeAsync("JoinCitizen", tenantId, email, ct);
    }

    public async ValueTask DisposeAsync()
    {
        _applicationUpdated.OnCompleted();
        _issuedDocumentCreated.OnCompleted();
        _applicationUpdated.Dispose();
        _issuedDocumentCreated.Dispose();

        if (_connection is not null)
        {
            try
            {
                await _connection.DisposeAsync();
            }
            catch
            {
                // ignore
            }
        }
    }

    private sealed record IssuedDocPayload(Guid applicationId, Guid issuedDocumentId);
}


