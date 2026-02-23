using System.Text.Json;
using Mamey.Identity.Decentralized.Abstractions;

namespace Mamey.Identity.Decentralized.Trust;

/// <summary>
/// Loads trusted DIDs from a remote HTTP endpoint (e.g., webhook, REST API).
/// </summary>
public class HttpTrustRegistry : ITrustRegistry
{
    private readonly string _url;
    private readonly HttpClient _client;
    private readonly HashSet<string> _trustedDids = new();

    /// <summary>
    /// Initializes a new HTTP trust registry.
    /// </summary>
    /// <param name="url">HTTP URL returning a JSON array of DIDs.</param>
    /// <param name="client">HttpClient for requests.</param>
    public HttpTrustRegistry(string url, HttpClient client)
    {
        _url = url;
        _client = client;
    }

    /// <inheritdoc/>
    public void AddTrustedIssuer(string did) => _trustedDids.Add(did);

    /// <inheritdoc/>
    public void RemoveTrustedIssuer(string did) => _trustedDids.Remove(did);

    /// <inheritdoc/>
    public bool IsTrusted(string did) => _trustedDids.Contains(did);

    /// <inheritdoc/>
    public IEnumerable<string> AllTrusted() => _trustedDids;

    /// <inheritdoc/>
    public async Task RefreshAsync(CancellationToken cancellationToken = default)
    {
        var json = await _client.GetStringAsync(_url, cancellationToken);
        var dids = JsonSerializer.Deserialize<List<string>>(json);
        _trustedDids.Clear();
        if (dids != null)
            foreach (var did in dids)
                _trustedDids.Add(did);
    }
}