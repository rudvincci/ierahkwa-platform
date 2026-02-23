using Mamey.Auth.DecentralizedIdentifiers.Abstractions;

namespace Mamey.Auth.DecentralizedIdentifiers.Core;

/// <summary>
/// Encapsulates normalized key material for use in DID Documents and verification.
/// </summary>
public class DidKeyMaterial : IKeyMaterial
{
    public string KeyType { get; }
    public byte[] KeyBytes { get; }
    public string EncodedKey { get; }
    public string Jwk { get; }

    public DidKeyMaterial(string keyType, byte[] keyBytes, string encodedKey, string jwk = null)
    {
        KeyType = keyType;
        KeyBytes = keyBytes;
        EncodedKey = encodedKey;
        Jwk = jwk;
    }
}