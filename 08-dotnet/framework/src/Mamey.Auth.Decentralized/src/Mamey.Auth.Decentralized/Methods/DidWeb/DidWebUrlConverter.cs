using Mamey.Auth.Decentralized.Core;

namespace Mamey.Auth.Decentralized.Methods.DidWeb;

/// <summary>
/// Utility class for converting DID Web identifiers to HTTPS URLs
/// </summary>
public static class DidWebUrlConverter
{
    /// <summary>
    /// Converts a DID Web identifier to an HTTPS URL
    /// </summary>
    /// <param name="identifier">The DID Web identifier</param>
    /// <param name="options">The DID Web options</param>
    /// <returns>The HTTPS URL</returns>
    public static string DidToHttpsUrl(string identifier, DidWebOptions options)
    {
        if (string.IsNullOrEmpty(identifier))
            throw new ArgumentException("Identifier cannot be null or empty", nameof(identifier));
        
        if (options == null)
            throw new ArgumentNullException(nameof(options));
        
        var scheme = options.UseHttps ? "https" : "http";
        var port = options.Port.HasValue ? $":{options.Port.Value}" : "";
        var path = options.DidDocumentPath;
        
        if (!string.IsNullOrEmpty(options.BaseUrl))
        {
            return $"{options.BaseUrl.TrimEnd('/')}{path}";
        }
        
        return $"{scheme}://{identifier}{port}{path}";
    }
    
    /// <summary>
    /// Converts a DID Web URL to an HTTPS URL
    /// </summary>
    /// <param name="didUrl">The DID Web URL</param>
    /// <param name="options">The DID Web options</param>
    /// <returns>The HTTPS URL</returns>
    public static string DidUrlToHttpsUrl(DidUrl didUrl, DidWebOptions options)
    {
        if (didUrl == null)
            throw new ArgumentNullException(nameof(didUrl));
        
        if (options == null)
            throw new ArgumentNullException(nameof(options));
        
        var baseUrl = DidToHttpsUrl(didUrl.Did.Identifier, options);
        
        // Remove the default DID Document path if we have a specific path
        if (baseUrl.EndsWith(options.DidDocumentPath))
        {
            baseUrl = baseUrl.Substring(0, baseUrl.Length - options.DidDocumentPath.Length);
        }
        
        // Add the path from the DID URL
        if (!string.IsNullOrEmpty(didUrl.Path))
        {
            baseUrl += "/" + didUrl.Path.TrimStart('/');
        }
        
        // Add query parameters if present
        if (didUrl.Query.Any())
        {
            var queryString = string.Join("&", didUrl.Query.Select(kvp => $"{Uri.EscapeDataString(kvp.Key)}={Uri.EscapeDataString(kvp.Value)}"));
            baseUrl += "?" + queryString;
        }
        
        // Add fragment if present
        if (!string.IsNullOrEmpty(didUrl.Fragment))
        {
            baseUrl += "#" + Uri.EscapeDataString(didUrl.Fragment);
        }
        
        return baseUrl;
    }
    
    /// <summary>
    /// Converts an HTTPS URL back to a DID Web identifier
    /// </summary>
    /// <param name="httpsUrl">The HTTPS URL</param>
    /// <param name="options">The DID Web options</param>
    /// <returns>The DID Web identifier</returns>
    public static string? HttpsUrlToDidIdentifier(string httpsUrl, DidWebOptions options)
    {
        if (string.IsNullOrEmpty(httpsUrl))
            return null;
        
        if (options == null)
            return null;
        
        try
        {
            var uri = new Uri(httpsUrl);
            var scheme = options.UseHttps ? "https" : "http";
            
            if (uri.Scheme != scheme)
                return null;
            
            var port = options.Port.HasValue ? $":{options.Port.Value}" : "";
            var expectedHost = uri.Host + port;
            
            if (uri.Port != (options.Port ?? (options.UseHttps ? 443 : 80)))
                return null;
            
            // Check if the path matches the expected DID Document path
            if (uri.AbsolutePath == options.DidDocumentPath)
            {
                return uri.Host;
            }
            
            return null;
        }
        catch
        {
            return null;
        }
    }
}
