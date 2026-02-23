using System.Text.RegularExpressions;
using Mamey.Auth.Decentralized.Exceptions;

namespace Mamey.Auth.Decentralized.Core;

/// <summary>
/// Represents a Decentralized Identifier (DID) as defined by W3C DID 1.1 specification.
/// Format: did:method-name:method-specific-id
/// </summary>
public class Did
{
    private static readonly Regex DidRegex = new(@"^did:([a-z0-9]+):(.+)$", RegexOptions.Compiled);
    
    /// <summary>
    /// The method name part of the DID (e.g., "web", "key", "ethr")
    /// </summary>
    public string Method { get; }
    
    /// <summary>
    /// The method-specific identifier part of the DID
    /// </summary>
    public string Identifier { get; }
    
    /// <summary>
    /// The complete DID string
    /// </summary>
    public string Value { get; }
    
    private Did(string method, string identifier)
    {
        Method = method ?? throw new ArgumentNullException(nameof(method));
        Identifier = identifier ?? throw new ArgumentNullException(nameof(identifier));
        Value = $"did:{method}:{identifier}";
    }
    
    /// <summary>
    /// Parses a DID string into a Did object
    /// </summary>
    /// <param name="didString">The DID string to parse</param>
    /// <returns>A Did object</returns>
    /// <exception cref="InvalidDidException">Thrown when the DID string is invalid</exception>
    public static Did Parse(string didString)
    {
        if (string.IsNullOrWhiteSpace(didString))
            throw new InvalidDidException("DID string cannot be null or empty");
        
        var match = DidRegex.Match(didString.Trim());
        if (!match.Success)
            throw new InvalidDidException($"Invalid DID format: {didString}");
        
        var method = match.Groups[1].Value;
        var identifier = match.Groups[2].Value;
        
        if (string.IsNullOrEmpty(method))
            throw new InvalidDidException("DID method cannot be empty");
        
        if (string.IsNullOrEmpty(identifier))
            throw new InvalidDidException("DID identifier cannot be empty");
        
        return new Did(method, identifier);
    }
    
    /// <summary>
    /// Attempts to parse a DID string into a Did object
    /// </summary>
    /// <param name="didString">The DID string to parse</param>
    /// <param name="did">The parsed Did object if successful</param>
    /// <returns>True if parsing was successful, false otherwise</returns>
    public static bool TryParse(string didString, out Did did)
    {
        did = null;
        
        if (string.IsNullOrWhiteSpace(didString))
            return false;
        
        var match = DidRegex.Match(didString.Trim());
        if (!match.Success)
            return false;
        
        var method = match.Groups[1].Value;
        var identifier = match.Groups[2].Value;
        
        if (string.IsNullOrEmpty(method) || string.IsNullOrEmpty(identifier))
            return false;
        
        try
        {
            did = new Did(method, identifier);
            return true;
        }
        catch
        {
            return false;
        }
    }
    
    /// <summary>
    /// Creates a DID from method and identifier components
    /// </summary>
    /// <param name="method">The DID method</param>
    /// <param name="identifier">The method-specific identifier</param>
    /// <returns>A Did object</returns>
    public static Did Create(string method, string identifier)
    {
        if (string.IsNullOrWhiteSpace(method))
            throw new ArgumentException("Method cannot be null or empty", nameof(method));
        
        if (string.IsNullOrWhiteSpace(identifier))
            throw new ArgumentException("Identifier cannot be null or empty", nameof(identifier));
        
        return new Did(method, identifier);
    }
    
    /// <summary>
    /// Validates the DID format according to W3C DID 1.1 specification
    /// </summary>
    /// <returns>True if the DID is valid, false otherwise</returns>
    public bool IsValid()
    {
        return DidRegex.IsMatch(Value) && 
               !string.IsNullOrEmpty(Method) && 
               !string.IsNullOrEmpty(Identifier);
    }
    
    /// <summary>
    /// Returns the DID as a string
    /// </summary>
    /// <returns>The complete DID string</returns>
    public override string ToString() => Value;
    
    /// <summary>
    /// Determines whether the specified object is equal to the current object
    /// </summary>
    /// <param name="obj">The object to compare with the current object</param>
    /// <returns>True if the objects are equal, false otherwise</returns>
    public override bool Equals(object obj)
    {
        return obj is Did other && Value.Equals(other.Value, StringComparison.Ordinal);
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
    /// Determines whether two Did objects are equal
    /// </summary>
    /// <param name="left">The first Did object</param>
    /// <param name="right">The second Did object</param>
    /// <returns>True if the objects are equal, false otherwise</returns>
    public static bool operator ==(Did left, Did right)
    {
        if (ReferenceEquals(left, right)) return true;
        if (left is null || right is null) return false;
        return left.Value.Equals(right.Value, StringComparison.Ordinal);
    }
    
    /// <summary>
    /// Determines whether two Did objects are not equal
    /// </summary>
    /// <param name="left">The first Did object</param>
    /// <param name="right">The second Did object</param>
    /// <returns>True if the objects are not equal, false otherwise</returns>
    public static bool operator !=(Did left, Did right)
    {
        return !(left == right);
    }
    
    /// <summary>
    /// Implicitly converts a Did object to a string
    /// </summary>
    /// <param name="did">The Did object to convert</param>
    /// <returns>The DID string</returns>
    public static implicit operator string(Did did) => did?.Value;
    
    /// <summary>
    /// Implicitly converts a string to a Did object
    /// </summary>
    /// <param name="didString">The DID string to convert</param>
    /// <returns>A Did object</returns>
    public static implicit operator Did(string didString) => Parse(didString);
}
