using System.Text.Json;
using Mamey.Identity.Decentralized.Abstractions;

namespace Mamey.Identity.Decentralized.Trust;

public class IPFSTrustRegistry : ITrustRegistry
{
    private readonly string _gatewayUrl; // e.g., "https://ipfs.io/ipfs/"
    private readonly string _cid;
    private readonly HttpClient _client;
    private readonly HashSet<string> _trustedIssuers = new();

    public IPFSTrustRegistry(string gatewayUrl, string cid, HttpClient client)
    {
        _gatewayUrl = gatewayUrl.TrimEnd('/');
        _cid = cid;
        _client = client;
    }

    public void AddTrustedIssuer(string did) => _trustedIssuers.Add(did);
    public void RemoveTrustedIssuer(string did) => _trustedIssuers.Remove(did);
    public bool IsTrusted(string did) => _trustedIssuers.Contains(did);
    public IEnumerable<string> AllTrusted() => _trustedIssuers;

    public async Task RefreshAsync(CancellationToken cancellationToken = default)
    {
        var url = $"{_gatewayUrl}/{_cid}";
        var json = await _client.GetStringAsync(url, cancellationToken);
        var dids = JsonSerializer.Deserialize<List<string>>(json);
        _trustedIssuers.Clear();
        if (dids != null)
            foreach (var did in dids)
                _trustedIssuers.Add(did);
    }
}