using System.Text.Json.Serialization;

namespace Mamey.Auth.Decentralized.Core;

/// <summary>
/// Represents a verification method in a DID Document as defined by W3C DID 1.1 specification.
/// Verification methods are used to verify proofs and authenticate the DID subject.
/// </summary>
public class VerificationMethod
{
    /// <summary>
    /// The unique identifier for this verification method
    /// </summary>
    [JsonPropertyName("id")]
    public string Id { get; set; } = string.Empty;
    
    /// <summary>
    /// The type of verification method (e.g., "Ed25519VerificationKey2020", "JsonWebKey2020")
    /// </summary>
    [JsonPropertyName("type")]
    public string Type { get; set; } = string.Empty;
    
    /// <summary>
    /// The DID controller of this verification method
    /// </summary>
    [JsonPropertyName("controller")]
    public string Controller { get; set; } = string.Empty;
    
    /// <summary>
    /// The public key in JWK format
    /// </summary>
    [JsonPropertyName("publicKeyJwk")]
    public Dictionary<string, object>? PublicKeyJwk { get; set; }
    
    /// <summary>
    /// The public key in multibase format
    /// </summary>
    [JsonPropertyName("publicKeyMultibase")]
    public string? PublicKeyMultibase { get; set; }
    
    /// <summary>
    /// Additional properties not defined in the W3C specification
    /// </summary>
    [JsonExtensionData]
    public Dictionary<string, JsonElement> AdditionalProperties { get; set; } = new();
    
    /// <summary>
    /// Validates the verification method according to W3C DID 1.1 specification
    /// </summary>
    /// <returns>True if the verification method is valid, false otherwise</returns>
    public bool IsValid()
    {
        // Check required fields
        if (string.IsNullOrEmpty(Id))
            return false;
        
        if (string.IsNullOrEmpty(Type))
            return false;
        
        if (string.IsNullOrEmpty(Controller))
            return false;
        
        // Check that at least one public key format is provided
        if (PublicKeyJwk == null && string.IsNullOrEmpty(PublicKeyMultibase))
            return false;
        
        // Validate JWK format if provided
        if (PublicKeyJwk != null && !IsValidJwk())
            return false;
        
        // Validate multibase format if provided
        if (!string.IsNullOrEmpty(PublicKeyMultibase) && !IsValidMultibase())
            return false;
        
        return true;
    }
    
    /// <summary>
    /// Validates the JWK format
    /// </summary>
    /// <returns>True if the JWK is valid, false otherwise</returns>
    private bool IsValidJwk()
    {
        if (PublicKeyJwk == null)
            return false;
        
        // Check required JWK fields
        if (!PublicKeyJwk.ContainsKey("kty") || !PublicKeyJwk.ContainsKey("crv"))
            return false;
        
        // Validate key type
        var kty = PublicKeyJwk["kty"]?.ToString();
        if (string.IsNullOrEmpty(kty))
            return false;
        
        // Validate curve for EC keys
        if (kty == "EC")
        {
            var crv = PublicKeyJwk["crv"]?.ToString();
            if (string.IsNullOrEmpty(crv))
                return false;
        }
        
        // Validate algorithm for RSA keys
        if (kty == "RSA")
        {
            if (!PublicKeyJwk.ContainsKey("n") || !PublicKeyJwk.ContainsKey("e"))
                return false;
        }
        
        return true;
    }
    
    /// <summary>
    /// Validates the multibase format
    /// </summary>
    /// <returns>True if the multibase is valid, false otherwise</returns>
    private bool IsValidMultibase()
    {
        if (string.IsNullOrEmpty(PublicKeyMultibase))
            return false;
        
        // Multibase format: <base-encoding-character><base-encoded-data>
        if (PublicKeyMultibase.Length < 2)
            return false;
        
        // Check that the first character is a valid multibase encoding character
        var encodingChar = PublicKeyMultibase[0];
        var validEncodings = new[] { 'z', 'Z', 'm', 'M', 'u', 'U', 'b', 'B' };
        
        return validEncodings.Contains(encodingChar);
    }
    
    /// <summary>
    /// Converts the verification method to JWK format
    /// </summary>
    /// <returns>The public key in JWK format</returns>
    public Dictionary<string, object> ToJwk()
    {
        if (PublicKeyJwk != null)
            return new Dictionary<string, object>(PublicKeyJwk);
        
        // Convert from multibase if needed
        if (!string.IsNullOrEmpty(PublicKeyMultibase))
        {
            // This would require implementing multibase decoding
            // For now, return a basic JWK structure
            return new Dictionary<string, object>
            {
                ["kty"] = Type,
                ["crv"] = "Ed25519", // Default assumption
                ["x"] = PublicKeyMultibase
            };
        }
        
        throw new InvalidOperationException("No public key data available");
    }
    
    /// <summary>
    /// Converts the verification method to multibase format
    /// </summary>
    /// <returns>The public key in multibase format</returns>
    public string ToMultibase()
    {
        if (!string.IsNullOrEmpty(PublicKeyMultibase))
            return PublicKeyMultibase;
        
        // Convert from JWK if needed
        if (PublicKeyJwk != null)
        {
            // This would require implementing multibase encoding
            // For now, return a placeholder
            return $"z{Convert.ToBase64String(Encoding.UTF8.GetBytes(JsonSerializer.Serialize(PublicKeyJwk)))}";
        }
        
        throw new InvalidOperationException("No public key data available");
    }
    
    /// <summary>
    /// Creates a verification method from JWK data
    /// </summary>
    /// <param name="id">The verification method ID</param>
    /// <param name="type">The verification method type</param>
    /// <param name="controller">The DID controller</param>
    /// <param name="publicKeyJwk">The public key in JWK format</param>
    /// <returns>A new VerificationMethod instance</returns>
    public static VerificationMethod FromJwk(string id, string type, string controller, Dictionary<string, object> publicKeyJwk)
    {
        return new VerificationMethod
        {
            Id = id,
            Type = type,
            Controller = controller,
            PublicKeyJwk = publicKeyJwk
        };
    }
    
    /// <summary>
    /// Creates a verification method from multibase data
    /// </summary>
    /// <param name="id">The verification method ID</param>
    /// <param name="type">The verification method type</param>
    /// <param name="controller">The DID controller</param>
    /// <param name="publicKeyMultibase">The public key in multibase format</param>
    /// <returns>A new VerificationMethod instance</returns>
    public static VerificationMethod FromMultibase(string id, string type, string controller, string publicKeyMultibase)
    {
        return new VerificationMethod
        {
            Id = id,
            Type = type,
            Controller = controller,
            PublicKeyMultibase = publicKeyMultibase
        };
    }
}
