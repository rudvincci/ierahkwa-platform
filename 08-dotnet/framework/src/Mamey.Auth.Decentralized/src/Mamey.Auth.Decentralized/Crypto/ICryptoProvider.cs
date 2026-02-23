namespace Mamey.Auth.Decentralized.Crypto;

/// <summary>
/// Interface for cryptographic providers that support key generation, signing, and verification
/// </summary>
public interface ICryptoProvider
{
    /// <summary>
    /// Gets the name of the cryptographic algorithm
    /// </summary>
    string AlgorithmName { get; }
    
    /// <summary>
    /// Gets the curve name for elliptic curve algorithms
    /// </summary>
    string? CurveName { get; }
    
    /// <summary>
    /// Generates a new key pair
    /// </summary>
    /// <returns>A new key pair</returns>
    KeyPair GenerateKeyPair();
    
    /// <summary>
    /// Generates a new key pair with a specific seed
    /// </summary>
    /// <param name="seed">The seed for key generation</param>
    /// <returns>A new key pair</returns>
    KeyPair GenerateKeyPair(byte[] seed);
    
    /// <summary>
    /// Creates a key pair from existing key material
    /// </summary>
    /// <param name="keyMaterial">The key material</param>
    /// <returns>A key pair</returns>
    KeyPair CreateKeyPair(KeyMaterial keyMaterial);
    
    /// <summary>
    /// Signs data with the private key
    /// </summary>
    /// <param name="data">The data to sign</param>
    /// <param name="privateKey">The private key</param>
    /// <returns>The signature</returns>
    byte[] Sign(byte[] data, byte[] privateKey);
    
    /// <summary>
    /// Verifies a signature with the public key
    /// </summary>
    /// <param name="data">The original data</param>
    /// <param name="signature">The signature to verify</param>
    /// <param name="publicKey">The public key</param>
    /// <returns>True if the signature is valid, false otherwise</returns>
    bool Verify(byte[] data, byte[] signature, byte[] publicKey);
    
    /// <summary>
    /// Converts a public key to JWK format
    /// </summary>
    /// <param name="publicKey">The public key</param>
    /// <returns>The public key in JWK format</returns>
    Dictionary<string, object> PublicKeyToJwk(byte[] publicKey);
    
    /// <summary>
    /// Converts a public key to multibase format
    /// </summary>
    /// <param name="publicKey">The public key</param>
    /// <returns>The public key in multibase format</returns>
    string PublicKeyToMultibase(byte[] publicKey);
    
    /// <summary>
    /// Converts a JWK to public key bytes
    /// </summary>
    /// <param name="jwk">The JWK</param>
    /// <returns>The public key bytes</returns>
    byte[] JwkToPublicKey(Dictionary<string, object> jwk);
    
    /// <summary>
    /// Converts a multibase string to public key bytes
    /// </summary>
    /// <param name="multibase">The multibase string</param>
    /// <returns>The public key bytes</returns>
    byte[] MultibaseToPublicKey(string multibase);
    
    /// <summary>
    /// Validates a key pair
    /// </summary>
    /// <param name="keyPair">The key pair to validate</param>
    /// <returns>True if the key pair is valid, false otherwise</returns>
    bool ValidateKeyPair(KeyPair keyPair);
}
