namespace Mamey.Auth.Decentralized.Crypto;

/// <summary>
/// Interface for key generation services
/// </summary>
public interface IKeyGenerator
{
    /// <summary>
    /// Generates a key pair for the specified algorithm
    /// </summary>
    /// <param name="algorithm">The cryptographic algorithm</param>
    /// <param name="curveName">Optional curve name for elliptic curve algorithms</param>
    /// <returns>A new key pair</returns>
    KeyPair GenerateKeyPair(string algorithm, string? curveName = null);
    
    /// <summary>
    /// Generates a key pair for the specified algorithm with a seed
    /// </summary>
    /// <param name="algorithm">The cryptographic algorithm</param>
    /// <param name="seed">The seed for key generation</param>
    /// <param name="curveName">Optional curve name for elliptic curve algorithms</param>
    /// <returns>A new key pair</returns>
    KeyPair GenerateKeyPair(string algorithm, byte[] seed, string? curveName = null);
    
    /// <summary>
    /// Gets the supported algorithms
    /// </summary>
    /// <returns>List of supported algorithm names</returns>
    IEnumerable<string> GetSupportedAlgorithms();
    
    /// <summary>
    /// Gets the supported curves for an algorithm
    /// </summary>
    /// <param name="algorithm">The algorithm</param>
    /// <returns>List of supported curve names</returns>
    IEnumerable<string> GetSupportedCurves(string algorithm);
    
    /// <summary>
    /// Validates a key pair
    /// </summary>
    /// <param name="keyPair">The key pair to validate</param>
    /// <returns>True if the key pair is valid, false otherwise</returns>
    bool ValidateKeyPair(KeyPair keyPair);
    
    /// <summary>
    /// Gets a cryptographic provider for the specified algorithm
    /// </summary>
    /// <param name="algorithm">The algorithm name</param>
    /// <returns>The cryptographic provider</returns>
    ICryptoProvider GetProvider(string algorithm);
}
