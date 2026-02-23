using Mamey.Auth.Decentralized.Exceptions;

namespace Mamey.Auth.Decentralized.Crypto;

/// <summary>
/// Base class for cryptographic providers
/// </summary>
public abstract class CryptoProviderBase : ICryptoProvider
{
    /// <summary>
    /// Gets the name of the cryptographic algorithm
    /// </summary>
    public abstract string AlgorithmName { get; }
    
    /// <summary>
    /// Gets the curve name for elliptic curve algorithms
    /// </summary>
    public abstract string? CurveName { get; }
    
    /// <summary>
    /// Generates a new key pair
    /// </summary>
    /// <returns>A new key pair</returns>
    public abstract KeyPair GenerateKeyPair();
    
    /// <summary>
    /// Generates a new key pair with a specific seed
    /// </summary>
    /// <param name="seed">The seed for key generation</param>
    /// <returns>A new key pair</returns>
    public abstract KeyPair GenerateKeyPair(byte[] seed);
    
    /// <summary>
    /// Creates a key pair from existing key material
    /// </summary>
    /// <param name="keyMaterial">The key material</param>
    /// <returns>A key pair</returns>
    public virtual KeyPair CreateKeyPair(KeyMaterial keyMaterial)
    {
        if (keyMaterial == null)
            throw new ArgumentNullException(nameof(keyMaterial));
        
        return new KeyPair(keyMaterial.PublicKey, keyMaterial.PrivateKey, AlgorithmName, CurveName);
    }
    
    /// <summary>
    /// Signs data with the private key
    /// </summary>
    /// <param name="data">The data to sign</param>
    /// <param name="privateKey">The private key</param>
    /// <returns>The signature</returns>
    public abstract byte[] Sign(byte[] data, byte[] privateKey);
    
    /// <summary>
    /// Verifies a signature with the public key
    /// </summary>
    /// <param name="data">The original data</param>
    /// <param name="signature">The signature to verify</param>
    /// <param name="publicKey">The public key</param>
    /// <returns>True if the signature is valid, false otherwise</returns>
    public abstract bool Verify(byte[] data, byte[] signature, byte[] publicKey);
    
    /// <summary>
    /// Converts a public key to JWK format
    /// </summary>
    /// <param name="publicKey">The public key</param>
    /// <returns>The public key in JWK format</returns>
    public abstract Dictionary<string, object> PublicKeyToJwk(byte[] publicKey);
    
    /// <summary>
    /// Converts a public key to multibase format
    /// </summary>
    /// <param name="publicKey">The public key</param>
    /// <returns>The public key in multibase format</returns>
    public abstract string PublicKeyToMultibase(byte[] publicKey);
    
    /// <summary>
    /// Converts a JWK to public key bytes
    /// </summary>
    /// <param name="jwk">The JWK</param>
    /// <returns>The public key bytes</returns>
    public abstract byte[] JwkToPublicKey(Dictionary<string, object> jwk);
    
    /// <summary>
    /// Converts a multibase string to public key bytes
    /// </summary>
    /// <param name="multibase">The multibase string</param>
    /// <returns>The public key bytes</returns>
    public abstract byte[] MultibaseToPublicKey(string multibase);
    
    /// <summary>
    /// Validates a key pair
    /// </summary>
    /// <param name="keyPair">The key pair to validate</param>
    /// <returns>True if the key pair is valid, false otherwise</returns>
    public virtual bool ValidateKeyPair(KeyPair keyPair)
    {
        if (keyPair == null)
            return false;
        
        if (keyPair.Algorithm != AlgorithmName)
            return false;
        
        if (keyPair.CurveName != CurveName)
            return false;
        
        // Test signing and verification with a small test message
        try
        {
            var testData = Encoding.UTF8.GetBytes("test");
            var signature = Sign(testData, keyPair.PrivateKey);
            return Verify(testData, signature, keyPair.PublicKey);
        }
        catch
        {
            return false;
        }
    }
    
    /// <summary>
    /// Validates that the provided key has the correct length for the algorithm
    /// </summary>
    /// <param name="key">The key to validate</param>
    /// <param name="expectedLength">The expected key length</param>
    /// <param name="keyType">The type of key (public/private)</param>
    /// <exception cref="CryptoException">Thrown when the key length is invalid</exception>
    protected void ValidateKeyLength(byte[] key, int expectedLength, string keyType)
    {
        if (key == null)
            throw new CryptoException(AlgorithmName, "ValidateKeyLength", $"{keyType} key cannot be null");
        
        if (key.Length != expectedLength)
            throw new CryptoException(AlgorithmName, "ValidateKeyLength", 
                $"{keyType} key length {key.Length} is invalid for {AlgorithmName}. Expected {expectedLength} bytes");
    }
    
    /// <summary>
    /// Validates that the provided data is not null or empty
    /// </summary>
    /// <param name="data">The data to validate</param>
    /// <param name="operation">The operation being performed</param>
    /// <exception cref="CryptoException">Thrown when the data is invalid</exception>
    protected void ValidateData(byte[] data, string operation)
    {
        if (data == null)
            throw new CryptoException(AlgorithmName, operation, "Data cannot be null");
        
        if (data.Length == 0)
            throw new CryptoException(AlgorithmName, operation, "Data cannot be empty");
    }
}
