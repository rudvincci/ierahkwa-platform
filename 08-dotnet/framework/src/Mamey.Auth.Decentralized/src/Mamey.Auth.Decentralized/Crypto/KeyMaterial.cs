namespace Mamey.Auth.Decentralized.Crypto;

/// <summary>
/// Represents raw key material for cryptographic operations
/// </summary>
public class KeyMaterial
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
    /// Initializes a new instance of the KeyMaterial class
    /// </summary>
    /// <param name="publicKey">The public key</param>
    /// <param name="privateKey">The private key</param>
    public KeyMaterial(byte[] publicKey, byte[] privateKey)
    {
        PublicKey = publicKey ?? throw new ArgumentNullException(nameof(publicKey));
        PrivateKey = privateKey ?? throw new ArgumentNullException(nameof(privateKey));
    }
    
    /// <summary>
    /// Determines whether the specified object is equal to the current object
    /// </summary>
    /// <param name="obj">The object to compare with the current object</param>
    /// <returns>True if the objects are equal, false otherwise</returns>
    public override bool Equals(object obj)
    {
        return obj is KeyMaterial other && 
               PublicKey.SequenceEqual(other.PublicKey) && 
               PrivateKey.SequenceEqual(other.PrivateKey);
    }
    
    /// <summary>
    /// Returns a hash code for the current object
    /// </summary>
    /// <returns>A hash code for the current object</returns>
    public override int GetHashCode()
    {
        return HashCode.Combine(PublicKey, PrivateKey);
    }
}
