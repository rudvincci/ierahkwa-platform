namespace Mamey.Auth.Decentralized.Crypto;

/// <summary>
/// Represents a cryptographic key pair
/// </summary>
public class KeyPair
{
    /// <summary>
    /// The public key
    /// </summary>
    public byte[] PublicKey { get; }
    
    /// <summary>
    /// The private key
    /// </summary>
    public byte[] PrivateKey { get; }
    
    /// <summary>
    /// The key ID
    /// </summary>
    public string KeyId { get; }
    
    /// <summary>
    /// The algorithm used for this key pair
    /// </summary>
    public string Algorithm { get; }
    
    /// <summary>
    /// The curve name for elliptic curve algorithms
    /// </summary>
    public string? CurveName { get; }
    
    /// <summary>
    /// Initializes a new instance of the KeyPair class
    /// </summary>
    /// <param name="publicKey">The public key</param>
    /// <param name="privateKey">The private key</param>
    /// <param name="algorithm">The algorithm used for this key pair</param>
    /// <param name="curveName">The curve name for elliptic curve algorithms</param>
    /// <param name="keyId">Optional key ID</param>
    public KeyPair(byte[] publicKey, byte[] privateKey, string algorithm, string? curveName = null, string? keyId = null)
    {
        PublicKey = publicKey ?? throw new ArgumentNullException(nameof(publicKey));
        PrivateKey = privateKey ?? throw new ArgumentNullException(nameof(privateKey));
        Algorithm = algorithm ?? throw new ArgumentNullException(nameof(algorithm));
        CurveName = curveName;
        KeyId = keyId ?? Guid.NewGuid().ToString();
    }
    
    /// <summary>
    /// Creates a key pair from key material
    /// </summary>
    /// <param name="keyMaterial">The key material</param>
    /// <param name="algorithm">The algorithm used for this key pair</param>
    /// <param name="curveName">The curve name for elliptic curve algorithms</param>
    /// <param name="keyId">Optional key ID</param>
    /// <returns>A new KeyPair instance</returns>
    public static KeyPair FromKeyMaterial(KeyMaterial keyMaterial, string algorithm, string? curveName = null, string? keyId = null)
    {
        if (keyMaterial == null)
            throw new ArgumentNullException(nameof(keyMaterial));
        
        return new KeyPair(keyMaterial.PublicKey, keyMaterial.PrivateKey, algorithm, curveName, keyId);
    }
    
    /// <summary>
    /// Converts the key pair to key material
    /// </summary>
    /// <returns>The key material</returns>
    public KeyMaterial ToKeyMaterial()
    {
        return new KeyMaterial(PublicKey, PrivateKey);
    }
    
    /// <summary>
    /// Determines whether the specified object is equal to the current object
    /// </summary>
    /// <param name="obj">The object to compare with the current object</param>
    /// <returns>True if the objects are equal, false otherwise</returns>
    public override bool Equals(object obj)
    {
        return obj is KeyPair other && 
               PublicKey.SequenceEqual(other.PublicKey) && 
               PrivateKey.SequenceEqual(other.PrivateKey) &&
               Algorithm == other.Algorithm &&
               CurveName == other.CurveName;
    }
    
    /// <summary>
    /// Returns a hash code for the current object
    /// </summary>
    /// <returns>A hash code for the current object</returns>
    public override int GetHashCode()
    {
        return HashCode.Combine(PublicKey, PrivateKey, Algorithm, CurveName);
    }
}
