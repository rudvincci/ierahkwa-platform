using System.Text.RegularExpressions;
using Mamey.Auth.Decentralized.Exceptions;

namespace Mamey.Auth.Decentralized.Core;

/// <summary>
/// Represents a DID URL as defined by W3C DID 1.1 specification.
/// Format: did:method:identifier/path?query#fragment
/// </summary>
public class DidUrl
{
    private static readonly Regex DidUrlRegex = new(@"^did:([a-z0-9]+):([^?#]+)(?:/([^?#]*))?(?:\?([^#]*))?(?:#(.*))?$", RegexOptions.Compiled);
    
    /// <summary>
    /// The DID part of the URL
    /// </summary>
    public Did Did { get; }
    
    /// <summary>
    /// The path component of the URL
    /// </summary>
    public string? Path { get; }
    
    /// <summary>
    /// The query parameters of the URL
    /// </summary>
    public Dictionary<string, string> Query { get; }
    
    /// <summary>
    /// The fragment component of the URL
    /// </summary>
    public string? Fragment { get; }
    
    /// <summary>
    /// The complete DID URL string
    /// </summary>
    public string Value { get; }
    
    private DidUrl(Did did, string? path = null, Dictionary<string, string>? query = null, string? fragment = null)
    {
        Did = did ?? throw new ArgumentNullException(nameof(did));
        Path = path;
        Query = query ?? new Dictionary<string, string>();
        Fragment = fragment;
        
        var url = did.Value;
        if (!string.IsNullOrEmpty(path))
            url += "/" + path;
        if (Query.Any())
            url += "?" + string.Join("&", Query.Select(kvp => $"{Uri.EscapeDataString(kvp.Key)}={Uri.EscapeDataString(kvp.Value)}"));
        if (!string.IsNullOrEmpty(fragment))
            url += "#" + Uri.EscapeDataString(fragment);
        
        Value = url;
    }
    
    /// <summary>
    /// Parses a DID URL string into a DidUrl object
    /// </summary>
    /// <param name="didUrlString">The DID URL string to parse</param>
    /// <returns>A DidUrl object</returns>
    /// <exception cref="InvalidDidException">Thrown when the DID URL string is invalid</exception>
    public static DidUrl Parse(string didUrlString)
    {
        if (string.IsNullOrWhiteSpace(didUrlString))
            throw new InvalidDidException("DID URL string cannot be null or empty");
        
        var match = DidUrlRegex.Match(didUrlString.Trim());
        if (!match.Success)
            throw new InvalidDidException($"Invalid DID URL format: {didUrlString}");
        
        var method = match.Groups[1].Value;
        var identifier = match.Groups[2].Value;
        var path = match.Groups[3].Success ? match.Groups[3].Value : null;
        var queryString = match.Groups[4].Success ? match.Groups[4].Value : null;
        var fragment = match.Groups[5].Success ? match.Groups[5].Value : null;
        
        var did = Did.Create(method, identifier);
        var query = ParseQueryString(queryString);
        
        return new DidUrl(did, path, query, fragment);
    }
    
    /// <summary>
    /// Attempts to parse a DID URL string into a DidUrl object
    /// </summary>
    /// <param name="didUrlString">The DID URL string to parse</param>
    /// <param name="didUrl">The parsed DidUrl object if successful</param>
    /// <returns>True if parsing was successful, false otherwise</returns>
    public static bool TryParse(string didUrlString, out DidUrl didUrl)
    {
        didUrl = null;
        
        if (string.IsNullOrWhiteSpace(didUrlString))
            return false;
        
        var match = DidUrlRegex.Match(didUrlString.Trim());
        if (!match.Success)
            return false;
        
        var method = match.Groups[1].Value;
        var identifier = match.Groups[2].Value;
        var path = match.Groups[3].Success ? match.Groups[3].Value : null;
        var queryString = match.Groups[4].Success ? match.Groups[4].Value : null;
        var fragment = match.Groups[5].Success ? match.Groups[5].Value : null;
        
        if (!Did.TryParse($"did:{method}:{identifier}", out var did))
            return false;
        
        var query = ParseQueryString(queryString);
        
        try
        {
            didUrl = new DidUrl(did, path, query, fragment);
            return true;
        }
        catch
        {
            return false;
        }
    }
    
    /// <summary>
    /// Creates a DID URL from components
    /// </summary>
    /// <param name="did">The DID</param>
    /// <param name="path">Optional path component</param>
    /// <param name="query">Optional query parameters</param>
    /// <param name="fragment">Optional fragment component</param>
    /// <returns>A DidUrl object</returns>
    public static DidUrl Create(Did did, string? path = null, Dictionary<string, string>? query = null, string? fragment = null)
    {
        return new DidUrl(did, path, query, fragment);
    }
    
    /// <summary>
    /// Parses a query string into a dictionary
    /// </summary>
    /// <param name="queryString">The query string to parse</param>
    /// <returns>A dictionary of query parameters</returns>
    private static Dictionary<string, string> ParseQueryString(string? queryString)
    {
        var query = new Dictionary<string, string>();
        
        if (string.IsNullOrEmpty(queryString))
            return query;
        
        var pairs = queryString.Split('&', StringSplitOptions.RemoveEmptyEntries);
        foreach (var pair in pairs)
        {
            var equalIndex = pair.IndexOf('=');
            if (equalIndex > 0)
            {
                var key = Uri.UnescapeDataString(pair.Substring(0, equalIndex));
                var value = Uri.UnescapeDataString(pair.Substring(equalIndex + 1));
                query[key] = value;
            }
            else
            {
                var key = Uri.UnescapeDataString(pair);
                query[key] = string.Empty;
            }
        }
        
        return query;
    }
    
    /// <summary>
    /// Gets a query parameter value
    /// </summary>
    /// <param name="key">The parameter key</param>
    /// <returns>The parameter value if found, null otherwise</returns>
    public string? GetQueryParameter(string key)
    {
        return Query.TryGetValue(key, out var value) ? value : null;
    }
    
    /// <summary>
    /// Sets a query parameter value
    /// </summary>
    /// <param name="key">The parameter key</param>
    /// <param name="value">The parameter value</param>
    public void SetQueryParameter(string key, string value)
    {
        Query[key] = value;
    }
    
    /// <summary>
    /// Removes a query parameter
    /// </summary>
    /// <param name="key">The parameter key to remove</param>
    /// <returns>True if the parameter was removed, false if not found</returns>
    public bool RemoveQueryParameter(string key)
    {
        return Query.Remove(key);
    }
    
    /// <summary>
    /// Validates the DID URL format according to W3C DID 1.1 specification
    /// </summary>
    /// <returns>True if the DID URL is valid, false otherwise</returns>
    public bool IsValid()
    {
        return Did.IsValid() && DidUrlRegex.IsMatch(Value);
    }
    
    /// <summary>
    /// Returns the DID URL as a string
    /// </summary>
    /// <returns>The complete DID URL string</returns>
    public override string ToString() => Value;
    
    /// <summary>
    /// Determines whether the specified object is equal to the current object
    /// </summary>
    /// <param name="obj">The object to compare with the current object</param>
    /// <returns>True if the objects are equal, false otherwise</returns>
    public override bool Equals(object obj)
    {
        return obj is DidUrl other && Value.Equals(other.Value, StringComparison.Ordinal);
    }
    
    /// <summary>
    /// Returns a hash code for the current object
    /// </summary>
    /// <returns>A hash code for the current object</returns>
    public override int GetHashCode()
    {
        return Value.GetHashCode();
    }
    
    /// <summary>
    /// Determines whether two DidUrl objects are equal
    /// </summary>
    /// <param name="left">The first DidUrl object</param>
    /// <param name="right">The second DidUrl object</param>
    /// <returns>True if the objects are equal, false otherwise</returns>
    public static bool operator ==(DidUrl left, DidUrl right)
    {
        if (ReferenceEquals(left, right)) return true;
        if (left is null || right is null) return false;
        return left.Value.Equals(right.Value, StringComparison.Ordinal);
    }
    
    /// <summary>
    /// Determines whether two DidUrl objects are not equal
    /// </summary>
    /// <param name="left">The first DidUrl object</param>
    /// <param name="right">The second DidUrl object</param>
    /// <returns>True if the objects are not equal, false otherwise</returns>
    public static bool operator !=(DidUrl left, DidUrl right)
    {
        return !(left == right);
    }
    
    /// <summary>
    /// Implicitly converts a DidUrl object to a string
    /// </summary>
    /// <param name="didUrl">The DidUrl object to convert</param>
    /// <returns>The DID URL string</returns>
    public static implicit operator string(DidUrl didUrl) => didUrl?.Value ?? string.Empty;
    
    /// <summary>
    /// Implicitly converts a string to a DidUrl object
    /// </summary>
    /// <param name="didUrlString">The DID URL string to convert</param>
    /// <returns>A DidUrl object</returns>
    public static implicit operator DidUrl(string didUrlString) => Parse(didUrlString);
}
