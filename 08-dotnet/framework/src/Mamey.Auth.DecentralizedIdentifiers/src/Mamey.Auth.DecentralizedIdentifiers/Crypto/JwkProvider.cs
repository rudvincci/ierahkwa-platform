namespace Mamey.Auth.DecentralizedIdentifiers.Crypto;

/// <summary>
/// Utility class for JWK (JSON Web Key) handling and normalization.
/// </summary>
public static class JwkProvider
{
    public static IDictionary<string, object> CreateEd25519(byte[] publicKey)
    {
        return new Dictionary<string, object>
        {
            { "kty", "OKP" },
            { "crv", "Ed25519" },
            { "x", Base64Url.Encode(publicKey) }
        };
    }
    // Add similar for secp256k1, RSA, etc.
}