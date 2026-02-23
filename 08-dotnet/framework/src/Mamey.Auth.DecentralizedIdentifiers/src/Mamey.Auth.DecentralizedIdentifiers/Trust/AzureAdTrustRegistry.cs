using System.Text.Json;
using Mamey.Auth.DecentralizedIdentifiers.Abstractions;

namespace Mamey.Auth.DecentralizedIdentifiers.Trust;

/// <summary>
/// Loads trusted DIDs from Azure Active Directory Group (requires Graph API, Group.Read.All).
/// </summary>
public class AzureAdTrustRegistry : ITrustRegistry
{
    private readonly string _graphApiUrl;
    private readonly HttpClient _client;
    private readonly HashSet<string> _trustedDids = new();

    /// <summary>
    /// Initializes a new Azure AD trust registry.
    /// </summary>
    /// <param name="graphApiUrl">MS Graph API endpoint (e.g., /groups/{group-id}/members).</param>
    /// <param name="client">Authenticated HttpClient (Bearer token with correct scopes).</param>
    public AzureAdTrustRegistry(string graphApiUrl, HttpClient client)
    {
        _graphApiUrl = graphApiUrl;
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
        var json = await _client.GetStringAsync(_graphApiUrl, cancellationToken);
        var doc = JsonDocument.Parse(json);
        _trustedDids.Clear();

        // Example: { "value": [ {"userPrincipalName": "...", "onPremisesExtensionAttributes": { "extension_DID": "did:..." } } ] }
        if (doc.RootElement.TryGetProperty("value", out var valueArr) && valueArr.ValueKind == JsonValueKind.Array)
        {
            foreach (var member in valueArr.EnumerateArray())
            {
                if (member.TryGetProperty("onPremisesExtensionAttributes", out var extAttrs) &&
                    extAttrs.TryGetProperty("extension_DID", out var didProp))
                {
                    var did = didProp.GetString();
                    if (!string.IsNullOrWhiteSpace(did))
                        _trustedDids.Add(did);
                }
            }
        }
    }
}