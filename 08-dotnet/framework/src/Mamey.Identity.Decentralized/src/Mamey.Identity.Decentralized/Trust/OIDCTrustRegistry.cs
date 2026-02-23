using System.Text.Json;
using Mamey.Identity.Decentralized.Abstractions;

namespace Mamey.Identity.Decentralized.Trust;

public class OIDCTrustRegistry : ITrustRegistry
{
    private readonly string _discoveryUrl;
    private readonly HashSet<string> _trustedIssuers = new();
    private readonly HttpClient _client;

    public OIDCTrustRegistry(string discoveryUrl, HttpClient client)
    {
        _discoveryUrl = discoveryUrl;
        _client = client;
    }

    public void AddTrustedIssuer(string did) => _trustedIssuers.Add(did);
    public void RemoveTrustedIssuer(string did) => _trustedIssuers.Remove(did);
    public bool IsTrusted(string did) => _trustedIssuers.Contains(did);
    public IEnumerable<string> AllTrusted() => _trustedIssuers;

    public async Task RefreshAsync(CancellationToken cancellationToken = default)
    {
        var json = await _client.GetStringAsync(_discoveryUrl, cancellationToken);
        var doc = JsonDocument.Parse(json);
        if (doc.RootElement.TryGetProperty("issuer", out var issuerProp))
            _trustedIssuers.Add(issuerProp.GetString());
    }
}