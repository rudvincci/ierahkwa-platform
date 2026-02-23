using Mamey.Identity.Decentralized.Abstractions;
using Mamey.Identity.Decentralized.Core;
using Mamey.Identity.Decentralized.Utilities;

namespace Mamey.Identity.Decentralized.Methods.Web;

public class DidWebResolver : IDidResolver
{
    private readonly HttpClient _httpClient;

    public DidWebResolver(HttpClient httpClient)
    {
        _httpClient = httpClient ?? throw new ArgumentNullException(nameof(httpClient));
    }

    public async Task<DidResolutionResult> ResolveAsync(string did, CancellationToken cancellationToken = default)
    {
        if (!SupportsMethod(DidUtils.GetMethod(did)))
            throw new NotSupportedException("DID is not did:web");

        // did:web:example.com:users:alice  →  https://example.com/users/alice/did.json
        var url = ToDidWebUrl(did);

        var resp = await _httpClient.GetAsync(url, cancellationToken);
        resp.EnsureSuccessStatusCode();

        var json = await resp.Content.ReadAsStringAsync(cancellationToken);
        var doc = DidDocument.Parse(json);

        return new DidResolutionResult
        {
            DidDocument = doc,
            DocumentMetadata = new System.Collections.Generic.Dictionary<string, object>(),
            ResolutionMetadata = new System.Collections.Generic.Dictionary<string, object> { { "resolver", "DidWebResolver" } }
        };
    }

    public bool SupportsMethod(string didMethod) => string.Equals(didMethod, "web", StringComparison.OrdinalIgnoreCase);

    private static string ToDidWebUrl(string did)
    {
        var didObj = new Did(did);
        var segments = didObj.MethodSpecificId.Split(':');
        var host = segments[0];
        var path = segments.Length > 1
            ? "/" + string.Join("/", segments.Skip(1))
            : string.Empty;
        var url = $"https://{host}{(path == string.Empty ? "/.well-known" : path)}/did.json";
        return url;
    }
}