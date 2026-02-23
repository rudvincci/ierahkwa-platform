using Mamey.Auth.Decentralized.Core;
using Mamey.Auth.Decentralized.Crypto;
using Mamey.Auth.Decentralized.Utilities;
using Mamey.Auth.Decentralized.Exceptions;

namespace Mamey.Auth.Decentralized.Methods.DidKey;

/// <summary>
/// Utility class for generating DID Documents from DID Key identifiers
/// </summary>
public static class DidKeyGenerator
{
    /// <summary>
    /// Generates a DID Document from a DID Key identifier
    /// </summary>
    /// <param name="identifier">The DID Key identifier</param>
    /// <param name="keyGenerator">The key generator</param>
    /// <param name="options">The DID Key options</param>
    /// <returns>The generated DID Document</returns>
    public static async Task<DidDocument?> GenerateDidDocumentAsync(string identifier, IKeyGenerator keyGenerator, DidKeyOptions options)
    {
        if (string.IsNullOrEmpty(identifier))
            throw new ArgumentException("Identifier cannot be null or empty", nameof(identifier));
        
        if (keyGenerator == null)
            throw new ArgumentNullException(nameof(keyGenerator));
        
        if (options == null)
            throw new ArgumentNullException(nameof(options));
        
        try
        {
            // Parse the key identifier to extract the key type and data
            var keyInfo = ParseKeyIdentifier(identifier);
            if (keyInfo == null)
                return null;
            
            // Get the cryptographic provider for the key type
            var provider = keyGenerator.GetProvider(keyInfo.Algorithm);
            
            // Convert the multibase key to public key bytes
            var publicKeyBytes = provider.MultibaseToPublicKey(keyInfo.MultibaseKey);
            
            // Create the DID
            var did = Did.Create("key", identifier);
            
            // Create the DID Document
            var didDocument = new DidDocument
            {
                Id = did.Value,
                Context = new List<string> { "https://www.w3.org/ns/did/v1" }
            };
            
            // Create the verification method
            var verificationMethod = new VerificationMethod
            {
                Id = $"{did.Value}#{keyInfo.Algorithm.ToLowerInvariant()}",
                Type = GetVerificationMethodType(keyInfo.Algorithm),
                Controller = did.Value,
                PublicKeyMultibase = keyInfo.MultibaseKey
            };
            
            // Add the verification method
            didDocument.AddVerificationMethod(verificationMethod);
            
            // Add verification relationships if enabled
            if (options.IncludeVerificationRelationships)
            {
                didDocument.Authentication.Add(verificationMethod.Id);
                didDocument.AssertionMethod.Add(verificationMethod.Id);
                didDocument.CapabilityInvocation.Add(verificationMethod.Id);
                didDocument.CapabilityDelegation.Add(verificationMethod.Id);
            }
            
            // Add service endpoints if enabled
            if (options.IncludeServiceEndpoints && options.DefaultServiceEndpoints.Any())
            {
                foreach (var serviceConfig in options.DefaultServiceEndpoints)
                {
                    var service = ServiceEndpoint.Create(
                        $"{did.Value}#{serviceConfig.Id}",
                        serviceConfig.Type,
                        serviceConfig.ServiceEndpoint
                    );
                    
                    // Add additional properties
                    foreach (var kvp in serviceConfig.Properties)
                    {
                        service.SetProperty(kvp.Key, kvp.Value);
                    }
                    
                    didDocument.AddService(service);
                }
            }
            
            return didDocument;
        }
        catch (Exception)
        {
            return null;
        }
    }
    
    /// <summary>
    /// Validates a DID Key identifier
    /// </summary>
    /// <param name="identifier">The identifier to validate</param>
    /// <returns>True if the identifier is valid</returns>
    public static bool IsValidKeyIdentifier(string identifier)
    {
        if (string.IsNullOrEmpty(identifier))
            return false;
        
        try
        {
            var keyInfo = ParseKeyIdentifier(identifier);
            return keyInfo != null;
        }
        catch
        {
            return false;
        }
    }
    
    /// <summary>
    /// Parses a DID Key identifier to extract key information
    /// </summary>
    /// <param name="identifier">The identifier to parse</param>
    /// <returns>The key information or null if invalid</returns>
    private static KeyInfo? ParseKeyIdentifier(string identifier)
    {
        if (string.IsNullOrEmpty(identifier))
            return null;
        
        try
        {
            // Decode the multibase key
            var keyBytes = MultibaseUtil.Decode(identifier);
            
            // Check for multicodec prefix
            if (keyBytes.Length < 2)
                return null;
            
            var multicodec = (keyBytes[0] << 8) | keyBytes[1];
            var algorithm = GetAlgorithmFromMulticodec(multicodec);
            
            if (string.IsNullOrEmpty(algorithm))
                return null;
            
            return new KeyInfo
            {
                Algorithm = algorithm,
                MultibaseKey = identifier,
                KeyBytes = keyBytes.Skip(2).ToArray()
            };
        }
        catch
        {
            return null;
        }
    }
    
    /// <summary>
    /// Gets the algorithm name from a multicodec value
    /// </summary>
    /// <param name="multicodec">The multicodec value</param>
    /// <returns>The algorithm name or null if not supported</returns>
    private static string? GetAlgorithmFromMulticodec(int multicodec)
    {
        return multicodec switch
        {
            0xed01 => "Ed25519",      // Ed25519 public key
            0x1205 => "RSA",          // RSA public key
            0xe701 => "Secp256k1",    // Secp256k1 public key
            _ => null
        };
    }
    
    /// <summary>
    /// Gets the verification method type for an algorithm
    /// </summary>
    /// <param name="algorithm">The algorithm name</param>
    /// <returns>The verification method type</returns>
    private static string GetVerificationMethodType(string algorithm)
    {
        return algorithm switch
        {
            "Ed25519" => "Ed25519VerificationKey2020",
            "RSA" => "RsaVerificationKey2018",
            "Secp256k1" => "EcdsaSecp256k1VerificationKey2019",
            _ => "JsonWebKey2020"
        };
    }
    
    /// <summary>
    /// Key information extracted from a DID Key identifier
    /// </summary>
    private class KeyInfo
    {
        public string Algorithm { get; set; } = string.Empty;
        public string MultibaseKey { get; set; } = string.Empty;
        public byte[] KeyBytes { get; set; } = Array.Empty<byte>();
    }
}
