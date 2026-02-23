using Mamey.Auth.DecentralizedIdentifiers.Abstractions;
using Mamey.Auth.DecentralizedIdentifiers.Core;
using Mamey.Auth.DecentralizedIdentifiers.Utilities;
using Mamey.Auth.DecentralizedIdentifiers.Caching;
using Microsoft.Extensions.Logging;
using System.Net;
using System.Text.Json;

namespace Mamey.Auth.DecentralizedIdentifiers.Methods.Web;

/// <summary>
/// Enhanced DID Web resolver with caching, domain verification, and better error handling
/// </summary>
public class DidWebResolver : IDidResolver
{
    private readonly HttpClient _httpClient;
    private readonly IDidDocumentCache _cache;
    private readonly ILogger<DidWebResolver> _logger;
    private readonly WebResolverOptions _options;

    public DidWebResolver(
        HttpClient httpClient,
        IDidDocumentCache cache,
        ILogger<DidWebResolver> logger,
        WebResolverOptions options = null)
    {
        _httpClient = httpClient ?? throw new ArgumentNullException(nameof(httpClient));
        _cache = cache ?? throw new ArgumentNullException(nameof(cache));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        _options = options ?? new WebResolverOptions();
    }

    public async Task<DidResolutionResult> ResolveAsync(string did, CancellationToken cancellationToken = default)
    {
        try
        {
            if (!SupportsMethod(DidUtils.GetMethod(did)))
                throw new NotSupportedException("DID is not did:web");

            // Check cache first
            if (_options.EnableCaching)
            {
                var cachedDoc = await _cache.GetAsync(did);
                if (cachedDoc != null)
                {
                    _logger.LogDebug("Resolved did:web from cache: {Did}", did);
                    return new DidResolutionResult
                    {
                        DidDocument = cachedDoc,
                        DocumentMetadata = new Dictionary<string, object>
                        {
                            { "cached", true },
                            { "resolver", "DidWebResolver" }
                        },
                        ResolutionMetadata = new Dictionary<string, object>
                        {
                            { "resolver", "DidWebResolver" },
                            { "method", "web" },
                            { "cached", true },
                            { "resolved_at", DateTime.UtcNow }
                        }
                    };
                }
            }

            // Resolve from web
            var url = ToDidWebUrl(did);
            _logger.LogDebug("Resolving did:web from URL: {Url}", url);

            // Verify domain if required
            if (_options.RequireDomainVerification)
            {
                await VerifyDomainAsync(did, cancellationToken);
            }

            var response = await _httpClient.GetAsync(url, cancellationToken);
            
            if (response.StatusCode == HttpStatusCode.NotFound)
            {
                throw new InvalidOperationException($"DID document not found at {url}");
            }
            
            response.EnsureSuccessStatusCode();

            var json = await response.Content.ReadAsStringAsync(cancellationToken);
            var doc = JsonSerializer.Deserialize<DidDocument>(json, new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            });

            if (doc == null)
            {
                throw new InvalidOperationException($"Failed to parse DID document from {url}");
            }

            // Validate DID document
            ValidateDidDocument(doc, did);

            // Cache the result
            if (_options.EnableCaching)
            {
                await _cache.SetAsync(did, doc, _options.CacheTtlMinutes);
            }

            _logger.LogInformation("Successfully resolved did:web: {Did}", did);

            return new DidResolutionResult
            {
                DidDocument = doc,
                DocumentMetadata = new Dictionary<string, object>
                {
                    { "cached", false },
                    { "resolver", "DidWebResolver" },
                    { "url", url },
                    { "resolved_at", DateTime.UtcNow }
                },
                ResolutionMetadata = new Dictionary<string, object>
                {
                    { "resolver", "DidWebResolver" },
                    { "method", "web" },
                    { "url", url },
                    { "resolved_at", DateTime.UtcNow }
                }
            };
        }
        catch (HttpRequestException ex)
        {
            _logger.LogError(ex, "HTTP error resolving did:web: {Did}", did);
            throw new InvalidOperationException($"Failed to resolve did:web: {did}", ex);
        }
        catch (TaskCanceledException ex)
        {
            _logger.LogError(ex, "Timeout resolving did:web: {Did}", did);
            throw new InvalidOperationException($"Timeout resolving did:web: {did}", ex);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error resolving did:web: {Did}", did);
            throw;
        }
    }

    public bool SupportsMethod(string didMethod) => string.Equals(didMethod, "web", StringComparison.OrdinalIgnoreCase);

    /// <summary>
    /// Verify domain ownership for did:web
    /// </summary>
    private async Task VerifyDomainAsync(string did, CancellationToken cancellationToken)
    {
        try
        {
            var didObj = new Did(did);
            var domain = didObj.MethodSpecificId.Split(':')[0];
            
            // Check if domain is in allowed list
            if (_options.AllowedDomains?.Any() == true && !_options.AllowedDomains.Contains(domain))
            {
                throw new InvalidOperationException($"Domain {domain} is not in the allowed domains list");
            }

            // Check if domain is in blocked list
            if (_options.BlockedDomains?.Contains(domain) == true)
            {
                throw new InvalidOperationException($"Domain {domain} is blocked");
            }

            // Additional domain verification could be added here
            // e.g., DNS TXT record verification, SSL certificate validation, etc.
            
            _logger.LogDebug("Domain verification passed for: {Domain}", domain);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Domain verification failed for did:web: {Did}", did);
            throw;
        }
    }

    /// <summary>
    /// Validate DID document structure and content
    /// </summary>
    private void ValidateDidDocument(DidDocument doc, string expectedDid)
    {
        if (string.IsNullOrEmpty(doc.Id))
            throw new InvalidOperationException("DID document missing ID");

        if (doc.Id != expectedDid)
            throw new InvalidOperationException($"DID document ID mismatch. Expected: {expectedDid}, Found: {doc.Id}");

        if (doc.VerificationMethods == null || !doc.VerificationMethods.Any())
            throw new InvalidOperationException("DID document missing verification methods");

        if (doc.Context == null || !doc.Context.Any())
            throw new InvalidOperationException("DID document missing context");

        _logger.LogDebug("DID document validation passed for: {Did}", expectedDid);
    }

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