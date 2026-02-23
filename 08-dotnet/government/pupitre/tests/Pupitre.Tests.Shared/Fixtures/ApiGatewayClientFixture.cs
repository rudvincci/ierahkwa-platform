using System.Net.Http.Headers;
using Xunit;

namespace Pupitre.Tests.Shared.Fixtures;

/// <summary>
/// Thin wrapper around HttpClient that reuses a running API Gateway instance in integration tests.
/// </summary>
public sealed class ApiGatewayClientFixture : IAsyncLifetime
{
    private HttpClient? _client;

    public HttpClient Client => _client ?? throw new InvalidOperationException("Fixture not initialized yet");

    public Task InitializeAsync()
    {
        _client = new HttpClient
        {
            BaseAddress = new Uri(Environment.GetEnvironmentVariable("PUPITRE_API_BASE") ?? "http://localhost:60000")
        };

        var token = Environment.GetEnvironmentVariable("PUPITRE_TEST_TOKEN");
        if (!string.IsNullOrWhiteSpace(token))
        {
            _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
        }

        return Task.CompletedTask;
    }

    public Task DisposeAsync()
    {
        _client?.Dispose();
        return Task.CompletedTask;
    }
}
