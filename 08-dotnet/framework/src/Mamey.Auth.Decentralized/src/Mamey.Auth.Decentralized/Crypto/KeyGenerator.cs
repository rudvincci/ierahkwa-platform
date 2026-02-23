using Mamey.Auth.Decentralized.Exceptions;

namespace Mamey.Auth.Decentralized.Crypto;

/// <summary>
/// Key generation service that supports multiple cryptographic algorithms
/// </summary>
public class KeyGenerator : IKeyGenerator
{
    private readonly Dictionary<string, ICryptoProvider> _providers;
    
    /// <summary>
    /// Initializes a new instance of the KeyGenerator class
    /// </summary>
    public KeyGenerator()
    {
        _providers = new Dictionary<string, ICryptoProvider>
        {
            ["Ed25519"] = new Ed25519Provider(),
            ["RSA"] = new RsaProvider(),
            ["Secp256k1"] = new Secp256k1Provider()
        };
    }
    
    /// <summary>
    /// Initializes a new instance of the KeyGenerator class with custom providers
    /// </summary>
    /// <param name="providers">Custom cryptographic providers</param>
    public KeyGenerator(IEnumerable<ICryptoProvider> providers)
    {
        _providers = providers.ToDictionary(p => p.AlgorithmName, p => p);
    }
    
    /// <summary>
    /// Generates a key pair for the specified algorithm
    /// </summary>
    /// <param name="algorithm">The cryptographic algorithm</param>
    /// <param name="curveName">Optional curve name for elliptic curve algorithms</param>
    /// <returns>A new key pair</returns>
    public KeyPair GenerateKeyPair(string algorithm, string? curveName = null)
    {
        if (string.IsNullOrEmpty(algorithm))
            throw new ArgumentException("Algorithm cannot be null or empty", nameof(algorithm));
        
        if (!_providers.TryGetValue(algorithm, out var provider))
            throw new CryptoException(algorithm, "GenerateKeyPair", $"Unsupported algorithm: {algorithm}");
        
        if (!string.IsNullOrEmpty(curveName) && provider.CurveName != curveName)
            throw new CryptoException(algorithm, "GenerateKeyPair", 
                $"Algorithm {algorithm} does not support curve {curveName}. Supported curve: {provider.CurveName}");
        
        return provider.GenerateKeyPair();
    }
    
    /// <summary>
    /// Generates a key pair for the specified algorithm with a seed
    /// </summary>
    /// <param name="algorithm">The cryptographic algorithm</param>
    /// <param name="seed">The seed for key generation</param>
    /// <param name="curveName">Optional curve name for elliptic curve algorithms</param>
    /// <returns>A new key pair</returns>
    public KeyPair GenerateKeyPair(string algorithm, byte[] seed, string? curveName = null)
    {
        if (string.IsNullOrEmpty(algorithm))
            throw new ArgumentException("Algorithm cannot be null or empty", nameof(algorithm));
        
        if (seed == null)
            throw new ArgumentNullException(nameof(seed));
        
        if (!_providers.TryGetValue(algorithm, out var provider))
            throw new CryptoException(algorithm, "GenerateKeyPair", $"Unsupported algorithm: {algorithm}");
        
        if (!string.IsNullOrEmpty(curveName) && provider.CurveName != curveName)
            throw new CryptoException(algorithm, "GenerateKeyPair", 
                $"Algorithm {algorithm} does not support curve {curveName}. Supported curve: {provider.CurveName}");
        
        return provider.GenerateKeyPair(seed);
    }
    
    /// <summary>
    /// Gets the supported algorithms
    /// </summary>
    /// <returns>List of supported algorithm names</returns>
    public IEnumerable<string> GetSupportedAlgorithms()
    {
        return _providers.Keys;
    }
    
    /// <summary>
    /// Gets the supported curves for an algorithm
    /// </summary>
    /// <param name="algorithm">The algorithm</param>
    /// <returns>List of supported curve names</returns>
    public IEnumerable<string> GetSupportedCurves(string algorithm)
    {
        if (string.IsNullOrEmpty(algorithm))
            throw new ArgumentException("Algorithm cannot be null or empty", nameof(algorithm));
        
        if (!_providers.TryGetValue(algorithm, out var provider))
            throw new CryptoException(algorithm, "GetSupportedCurves", $"Unsupported algorithm: {algorithm}");
        
        if (string.IsNullOrEmpty(provider.CurveName))
            return Enumerable.Empty<string>();
        
        return new[] { provider.CurveName };
    }
    
    /// <summary>
    /// Validates a key pair
    /// </summary>
    /// <param name="keyPair">The key pair to validate</param>
    /// <returns>True if the key pair is valid, false otherwise</returns>
    public bool ValidateKeyPair(KeyPair keyPair)
    {
        if (keyPair == null)
            return false;
        
        if (!_providers.TryGetValue(keyPair.Algorithm, out var provider))
            return false;
        
        return provider.ValidateKeyPair(keyPair);
    }
    
    /// <summary>
    /// Gets a cryptographic provider for the specified algorithm
    /// </summary>
    /// <param name="algorithm">The algorithm name</param>
    /// <returns>The cryptographic provider</returns>
    public ICryptoProvider GetProvider(string algorithm)
    {
        if (string.IsNullOrEmpty(algorithm))
            throw new ArgumentException("Algorithm cannot be null or empty", nameof(algorithm));
        
        if (!_providers.TryGetValue(algorithm, out var provider))
            throw new CryptoException(algorithm, "GetProvider", $"Unsupported algorithm: {algorithm}");
        
        return provider;
    }
    
    /// <summary>
    /// Adds a custom cryptographic provider
    /// </summary>
    /// <param name="provider">The provider to add</param>
    public void AddProvider(ICryptoProvider provider)
    {
        if (provider == null)
            throw new ArgumentNullException(nameof(provider));
        
        _providers[provider.AlgorithmName] = provider;
    }
    
    /// <summary>
    /// Removes a cryptographic provider
    /// </summary>
    /// <param name="algorithm">The algorithm name</param>
    /// <returns>True if the provider was removed, false if not found</returns>
    public bool RemoveProvider(string algorithm)
    {
        if (string.IsNullOrEmpty(algorithm))
            throw new ArgumentException("Algorithm cannot be null or empty", nameof(algorithm));
        
        return _providers.Remove(algorithm);
    }
}
