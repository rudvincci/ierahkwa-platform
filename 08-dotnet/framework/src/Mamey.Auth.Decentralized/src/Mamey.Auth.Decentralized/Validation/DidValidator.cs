using Mamey.Auth.Decentralized.Core;
using Mamey.Auth.Decentralized.Exceptions;
using System.Text.RegularExpressions;

namespace Mamey.Auth.Decentralized.Validation;

/// <summary>
/// Validates DID format and structure according to W3C DID specification.
/// </summary>
public class DidValidator
{
    private static readonly Regex DidRegex = new(
        @"^did:[a-z0-9]+:[a-zA-Z0-9._-]+$",
        RegexOptions.Compiled | RegexOptions.IgnoreCase);

    private static readonly Regex DidUrlRegex = new(
        @"^did:[a-z0-9]+:[a-zA-Z0-9._-]+(/[^?#]*)?(\?[^#]*)?(#.*)?$",
        RegexOptions.Compiled | RegexOptions.IgnoreCase);

    /// <summary>
    /// Validates a DID string.
    /// </summary>
    /// <param name="did">The DID to validate.</param>
    /// <returns>True if the DID is valid; otherwise, false.</returns>
    public static bool IsValidDid(string did)
    {
        if (string.IsNullOrWhiteSpace(did))
            return false;

        return DidRegex.IsMatch(did);
    }

    /// <summary>
    /// Validates a DID URL string.
    /// </summary>
    /// <param name="didUrl">The DID URL to validate.</param>
    /// <returns>True if the DID URL is valid; otherwise, false.</returns>
    public static bool IsValidDidUrl(string didUrl)
    {
        if (string.IsNullOrWhiteSpace(didUrl))
            return false;

        return DidUrlRegex.IsMatch(didUrl);
    }

    /// <summary>
    /// Validates a DID and throws an exception if invalid.
    /// </summary>
    /// <param name="did">The DID to validate.</param>
    /// <exception cref="InvalidDidException">Thrown when the DID is invalid.</exception>
    public static void ValidateDid(string did)
    {
        if (!IsValidDid(did))
        {
            throw new InvalidDidException("Invalid DID format", new ArgumentException("Invalid DID format"));
        }
    }

    /// <summary>
    /// Validates a DID URL and throws an exception if invalid.
    /// </summary>
    /// <param name="didUrl">The DID URL to validate.</param>
    /// <exception cref="InvalidDidException">Thrown when the DID URL is invalid.</exception>
    public static void ValidateDidUrl(string didUrl)
    {
        if (!IsValidDidUrl(didUrl))
        {
            throw new InvalidDidException("Invalid DID URL format", new ArgumentException("Invalid DID URL format"));
        }
    }

    /// <summary>
    /// Parses and validates a DID.
    /// </summary>
    /// <param name="did">The DID to parse and validate.</param>
    /// <returns>A parsed DID object.</returns>
    /// <exception cref="InvalidDidException">Thrown when the DID is invalid.</exception>
    public static Did ParseDid(string did)
    {
        ValidateDid(did);
        return Did.Parse(did);
    }

    /// <summary>
    /// Parses and validates a DID URL.
    /// </summary>
    /// <param name="didUrl">The DID URL to parse and validate.</param>
    /// <returns>A parsed DID URL object.</returns>
    /// <exception cref="InvalidDidException">Thrown when the DID URL is invalid.</exception>
    public static DidUrl ParseDidUrl(string didUrl)
    {
        ValidateDidUrl(didUrl);
        return DidUrl.Parse(didUrl);
    }

    /// <summary>
    /// Validates the DID method.
    /// </summary>
    /// <param name="method">The DID method to validate.</param>
    /// <param name="supportedMethods">The list of supported methods.</param>
    /// <returns>True if the method is supported; otherwise, false.</returns>
    public static bool IsSupportedMethod(string method, IEnumerable<string> supportedMethods)
    {
        if (string.IsNullOrWhiteSpace(method))
            return false;

        return supportedMethods.Contains(method, StringComparer.OrdinalIgnoreCase);
    }

    /// <summary>
    /// Validates the DID method and throws an exception if not supported.
    /// </summary>
    /// <param name="method">The DID method to validate.</param>
    /// <param name="supportedMethods">The list of supported methods.</param>
    /// <exception cref="UnsupportedDidMethodException">Thrown when the method is not supported.</exception>
    public static void ValidateMethod(string method, IEnumerable<string> supportedMethods)
    {
        if (!IsSupportedMethod(method, supportedMethods))
        {
            throw new UnsupportedDidMethodException($"DID method '{method}' is not supported", new ArgumentException($"DID method '{method}' is not supported"));
        }
    }

    /// <summary>
    /// Validates the DID identifier.
    /// </summary>
    /// <param name="identifier">The identifier to validate.</param>
    /// <returns>True if the identifier is valid; otherwise, false.</returns>
    public static bool IsValidIdentifier(string identifier)
    {
        if (string.IsNullOrWhiteSpace(identifier))
            return false;

        // Check length
        if (identifier.Length > 1024)
            return false;

        // Check for valid characters
        return identifier.All(c => char.IsLetterOrDigit(c) || c == '.' || c == '_' || c == '-');
    }

    /// <summary>
    /// Validates the DID identifier and throws an exception if invalid.
    /// </summary>
    /// <param name="identifier">The identifier to validate.</param>
    /// <exception cref="InvalidDidException">Thrown when the identifier is invalid.</exception>
    public static void ValidateIdentifier(string identifier)
    {
        if (!IsValidIdentifier(identifier))
        {
            throw new InvalidDidException("Invalid DID identifier format", new ArgumentException("Invalid DID identifier format"));
        }
    }

    /// <summary>
    /// Validates the DID fragment.
    /// </summary>
    /// <param name="fragment">The fragment to validate.</param>
    /// <returns>True if the fragment is valid; otherwise, false.</returns>
    public static bool IsValidFragment(string fragment)
    {
        if (string.IsNullOrWhiteSpace(fragment))
            return false;

        // Check length
        if (fragment.Length > 256)
            return false;

        // Check for valid characters (fragment can contain more characters than identifier)
        return fragment.All(c => char.IsLetterOrDigit(c) || c == '.' || c == '_' || c == '-' || c == ':' || c == '/');
    }

    /// <summary>
    /// Validates the DID fragment and throws an exception if invalid.
    /// </summary>
    /// <param name="fragment">The fragment to validate.</param>
    /// <exception cref="InvalidDidException">Thrown when the fragment is invalid.</exception>
    public static void ValidateFragment(string fragment)
    {
        if (!IsValidFragment(fragment))
        {
            throw new InvalidDidException("Invalid DID fragment format", new ArgumentException("Invalid DID fragment format"));
        }
    }

    /// <summary>
    /// Validates the DID query parameters.
    /// </summary>
    /// <param name="query">The query string to validate.</param>
    /// <returns>True if the query is valid; otherwise, false.</returns>
    public static bool IsValidQuery(string query)
    {
        if (string.IsNullOrWhiteSpace(query))
            return true; // Empty query is valid

        // Check length
        if (query.Length > 1024)
            return false;

        // Basic validation - should be URL-encoded
        try
        {
            var decoded = Uri.UnescapeDataString(query);
            return decoded.All(c => char.IsLetterOrDigit(c) || c == '.' || c == '_' || c == '-' || c == ':' || c == '/' || c == '=' || c == '&');
        }
        catch
        {
            return false;
        }
    }

    /// <summary>
    /// Validates the DID query parameters and throws an exception if invalid.
    /// </summary>
    /// <param name="query">The query string to validate.</param>
    /// <exception cref="InvalidDidException">Thrown when the query is invalid.</exception>
    public static void ValidateQuery(string query)
    {
        if (!IsValidQuery(query))
        {
            throw new InvalidDidException("Invalid DID query format", new ArgumentException("Invalid DID query format"));
        }
    }
}
