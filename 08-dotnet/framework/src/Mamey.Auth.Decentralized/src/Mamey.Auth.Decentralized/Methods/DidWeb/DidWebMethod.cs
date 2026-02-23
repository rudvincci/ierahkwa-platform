using Mamey.Auth.Decentralized.Core;
using Mamey.Auth.Decentralized.Exceptions;
using Microsoft.Extensions.Logging;
using System.Text.Json;

namespace Mamey.Auth.Decentralized.Methods.DidWeb;

/// <summary>
/// DID Web method implementation
/// </summary>
public class DidWebMethod : MethodBase
{
    private readonly DidWebOptions _options;
    private readonly HttpClient _httpClient;
    
    /// <summary>
    /// Gets the name of the DID method
    /// </summary>
    public override string MethodName => "web";
    
    /// <summary>
    /// Initializes a new instance of the DidWebMethod class
    /// </summary>
    /// <param name="options">The DID Web options</param>
    /// <param name="httpClient">The HTTP client</param>
    /// <param name="logger">The logger</param>
    public DidWebMethod(DidWebOptions options, HttpClient httpClient, ILogger<DidWebMethod> logger) : base(logger)
    {
        _options = options ?? throw new ArgumentNullException(nameof(options));
        _httpClient = httpClient ?? throw new ArgumentNullException(nameof(httpClient));
    }
    
    /// <summary>
    /// Resolves a DID to its DID Document
    /// </summary>
    /// <param name="did">The DID to resolve</param>
    /// <param name="options">Optional resolution options</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>The resolution result</returns>
    public override async Task<DidResolutionResult> ResolveAsync(string did, DidResolutionOptions? options = null, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrEmpty(did))
            throw new ArgumentException("DID cannot be null or empty", nameof(did));
        
        Logger.LogDebug("Resolving DID Web: {Did}", did);
        
        try
        {
            var parsedDid = Did.Parse(did);
            if (parsedDid.Method != MethodName)
            {
                return DidResolutionResult.MethodNotSupported(did);
            }
            
            if (!ValidateIdentifier(parsedDid.Identifier))
            {
                return DidResolutionResult.InvalidDid(did);
            }
            
            // Convert DID to HTTPS URL
            var url = DidWebUrlConverter.DidToHttpsUrl(parsedDid.Identifier, _options);
            
            // Fetch the DID Document from the URL
            var response = await _httpClient.GetAsync(url, cancellationToken);
            
            if (!response.IsSuccessStatusCode)
            {
                if (response.StatusCode == System.Net.HttpStatusCode.NotFound)
                {
                    return DidResolutionResult.NotFound(did);
                }
                
                return DidResolutionResult.Failed(did, "httpError", $"HTTP {response.StatusCode}: {response.ReasonPhrase}");
            }
            
            var json = await response.Content.ReadAsStringAsync(cancellationToken);
            var didDocument = JsonSerializer.Deserialize<DidDocument>(json, new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            });
            
            if (didDocument == null)
            {
                return DidResolutionResult.Failed(did, "parseError", "Failed to parse DID Document");
            }
            
            // Validate the DID Document
            if (!didDocument.ValidateW3cCompliance())
            {
                return DidResolutionResult.Failed(did, "validationError", "DID Document does not comply with W3C DID 1.1 specification");
            }
            
            // Ensure the DID in the document matches
            if (didDocument.Id != did)
            {
                return DidResolutionResult.Failed(did, "mismatchError", "DID Document ID does not match the requested DID");
            }
            
            var result = DidResolutionResult.Success(did, didDocument);
            result.ResolutionMetadata.ContentType = response.Content.Headers.ContentType?.ToString();
            result.ResolutionMetadata.Method = MethodName;
            
            Logger.LogDebug("Successfully resolved DID Web: {Did}", did);
            return result;
        }
        catch (HttpRequestException ex)
        {
            Logger.LogWarning("HTTP error resolving DID Web: {Did}, Error: {Error}", did, ex.Message);
            return DidResolutionResult.Failed(did, "httpError", ex.Message);
        }
        catch (JsonException ex)
        {
            Logger.LogWarning("JSON parse error resolving DID Web: {Did}, Error: {Error}", did, ex.Message);
            return DidResolutionResult.Failed(did, "parseError", ex.Message);
        }
        catch (Exception ex)
        {
            Logger.LogError(ex, "Error resolving DID Web: {Did}", did);
            return DidResolutionResult.Failed(did, "resolutionError", ex.Message);
        }
    }
    
    /// <summary>
    /// Dereferences a DID URL to its content
    /// </summary>
    /// <param name="didUrl">The DID URL to dereference</param>
    /// <param name="options">Optional dereferencing options</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>The dereferencing result</returns>
    public override async Task<DidDereferencingResult> DereferenceAsync(string didUrl, DidDereferencingOptions? options = null, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrEmpty(didUrl))
            throw new ArgumentException("DID URL cannot be null or empty", nameof(didUrl));
        
        Logger.LogDebug("Dereferencing DID Web URL: {DidUrl}", didUrl);
        
        try
        {
            var parsedDidUrl = DidUrl.Parse(didUrl);
            if (parsedDidUrl.Did.Method != MethodName)
            {
                return DidDereferencingResult.Failed("methodNotSupported", $"DID method '{parsedDidUrl.Did.Method}' is not supported");
            }
            
            if (!ValidateIdentifier(parsedDidUrl.Did.Identifier))
            {
                return DidDereferencingResult.Failed("invalidDid", "Invalid DID identifier");
            }
            
            // Convert DID URL to HTTPS URL
            var url = DidWebUrlConverter.DidUrlToHttpsUrl(parsedDidUrl, _options);
            
            // Fetch the content from the URL
            var response = await _httpClient.GetAsync(url, cancellationToken);
            
            if (!response.IsSuccessStatusCode)
            {
                if (response.StatusCode == System.Net.HttpStatusCode.NotFound)
                {
                    return DidDereferencingResult.Failed("notFound", "Content not found");
                }
                
                return DidDereferencingResult.Failed("httpError", $"HTTP {response.StatusCode}: {response.ReasonPhrase}");
            }
            
            var content = await response.Content.ReadAsByteArrayAsync(cancellationToken);
            var contentType = response.Content.Headers.ContentType?.ToString();
            
            var contentStream = new DidDereferencingContent
            {
                Data = content,
                Text = contentType?.StartsWith("text/") == true ? System.Text.Encoding.UTF8.GetString(content) : null
            };
            
            var result = DidDereferencingResult.Success(contentStream);
            result.DereferencingMetadata.ContentType = contentType;
            result.DereferencingMetadata.Method = MethodName;
            
            Logger.LogDebug("Successfully dereferenced DID Web URL: {DidUrl}", didUrl);
            return result;
        }
        catch (HttpRequestException ex)
        {
            Logger.LogWarning("HTTP error dereferencing DID Web URL: {DidUrl}, Error: {Error}", didUrl, ex.Message);
            return DidDereferencingResult.Failed("httpError", ex.Message);
        }
        catch (Exception ex)
        {
            Logger.LogError(ex, "Error dereferencing DID Web URL: {DidUrl}", didUrl);
            return DidDereferencingResult.Failed("dereferencingError", ex.Message);
        }
    }
    
    /// <summary>
    /// Validates a DID identifier for this method
    /// </summary>
    /// <param name="identifier">The identifier to validate</param>
    /// <returns>True if the identifier is valid for this method</returns>
    public override bool ValidateIdentifier(string identifier)
    {
        if (string.IsNullOrEmpty(identifier))
            return false;
        
        try
        {
            // DID Web identifiers should be valid domain names
            var uri = new Uri($"https://{identifier}");
            return uri.Host == identifier && !string.IsNullOrEmpty(uri.Host);
        }
        catch
        {
            return false;
        }
    }
}
