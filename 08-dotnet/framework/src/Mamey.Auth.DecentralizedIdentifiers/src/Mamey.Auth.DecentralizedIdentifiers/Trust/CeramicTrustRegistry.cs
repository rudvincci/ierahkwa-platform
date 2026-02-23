using System.Text.Json;
using Mamey.Auth.DecentralizedIdentifiers.Abstractions;

namespace Mamey.Auth.DecentralizedIdentifiers.Trust;

public class CeramicTrustRegistry : ITrustRegistry
{
    private readonly string _ceramicApiUrl; // e.g., "https://gateway.ceramic.network/api/v0/streams/<streamId>"
    private readonly HttpClient _client;
    private readonly HashSet<string> _trustedIssuers = new();

    public CeramicTrustRegistry(string ceramicApiUrl, HttpClient client)
    {
        _ceramicApiUrl = ceramicApiUrl;
        _client = client;
    }

    public void AddTrustedIssuer(string did) => _trustedIssuers.Add(did);
    public void RemoveTrustedIssuer(string did) => _trustedIssuers.Remove(did);
    public bool IsTrusted(string did) => _trustedIssuers.Contains(did);
    public IEnumerable<string> AllTrusted() => _trustedIssuers;

    public async Task RefreshAsync(CancellationToken cancellationToken = default)
    {
        var json = await _client.GetStringAsync(_ceramicApiUrl, cancellationToken);
        var doc = JsonDocument.Parse(json);
        if (doc.RootElement.TryGetProperty("content", out var contentProp) &&
            contentProp.ValueKind == JsonValueKind.Array)
        {
            _trustedIssuers.Clear();
            foreach (var did in contentProp.EnumerateArray())
                _trustedIssuers.Add(did.GetString());
        }
    }
}