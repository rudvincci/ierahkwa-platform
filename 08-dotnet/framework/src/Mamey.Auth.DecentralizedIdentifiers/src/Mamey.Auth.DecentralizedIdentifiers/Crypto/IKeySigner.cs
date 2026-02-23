namespace Mamey.Auth.DecentralizedIdentifiers.Crypto;

/// <summary>
/// Signs data using a private key for the specified key type.
/// </summary>
public interface IKeySigner
{
    /// <summary>
    /// Signs the provided data with the given private key.
    /// </summary>
    /// <param name="keyType">The type of key.</param>
    /// <param name="privateKey">Private key bytes.</param>
    /// <param name="data">Data to sign.</param>
    /// <returns>Signature as a byte array.</returns>
    byte[] Sign(string keyType, byte[] privateKey, byte[] data);
}