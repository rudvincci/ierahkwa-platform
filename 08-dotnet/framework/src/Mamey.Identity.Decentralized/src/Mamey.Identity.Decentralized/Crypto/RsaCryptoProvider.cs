using System.Security.Cryptography;

namespace Mamey.Identity.Decentralized.Crypto;

/// <summary>
/// Provides RSA (RS256) keypair operations for DID methods supporting RSA.
/// </summary>
public class RsaCryptoProvider : IKeyPairCryptoProvider
{
    public string KeyType => "RSA";
    public string VerificationMethodType => "RsaVerificationKey2018";

    public async System.Threading.Tasks.Task<(byte[] privateKey, byte[] publicKey)> GenerateKeyPairAsync(string keyType)
    {
        if (!string.Equals(keyType, KeyType, StringComparison.OrdinalIgnoreCase))
            throw new NotSupportedException($"Key type {keyType} not supported by RSA provider.");

        using var rsa = RSA.Create(2048);
        return (rsa.ExportRSAPrivateKey(), rsa.ExportRSAPublicKey());
    }

    public byte[] Sign(string keyType, byte[] privateKey, byte[] data)
    {
        if (!string.Equals(keyType, KeyType, StringComparison.OrdinalIgnoreCase))
            throw new NotSupportedException($"Key type {keyType} not supported by RSA provider.");

        using var rsa = RSA.Create();
        rsa.ImportRSAPrivateKey(privateKey, out _);
        return rsa.SignData(data, HashAlgorithmName.SHA256, RSASignaturePadding.Pkcs1);
    }

    public bool Verify(string keyType, byte[] publicKey, byte[] data, byte[] signature)
    {
        if (!string.Equals(keyType, KeyType, StringComparison.OrdinalIgnoreCase))
            throw new NotSupportedException($"Key type {keyType} not supported by RSA provider.");

        using var rsa = RSA.Create();
        rsa.ImportRSAPublicKey(publicKey, out _);
        return rsa.VerifyData(data, signature, HashAlgorithmName.SHA256, RSASignaturePadding.Pkcs1);
    }

    public IDictionary<string, object> ExportJwk(byte[] publicKey)
    {
        using var rsa = RSA.Create();
        rsa.ImportRSAPublicKey(publicKey, out _);
        var parameters = rsa.ExportParameters(false);
        return new Dictionary<string, object>
        {
            { "kty", "RSA" },
            { "n", Base64Url.Encode(parameters.Modulus) },
            { "e", Base64Url.Encode(parameters.Exponent) }
        };
    }

    public string ExportBase58(byte[] publicKey) => Base58Util.Encode(publicKey);

    public string ExportMultibase(byte[] publicKey) => MultibaseUtil.Encode(publicKey, MultibaseUtil.Base.Base58Btc);

    public byte[] ImportJwk(IDictionary<string, object> jwk)
    {
        if (jwk.TryGetValue("n", out var n) && jwk.TryGetValue("e", out var e))
        {
            var parameters = new RSAParameters
            {
                Modulus = Base64Url.Decode(n.ToString()),
                Exponent = Base64Url.Decode(e.ToString())
            };
            using var rsa = RSA.Create();
            rsa.ImportParameters(parameters);
            return rsa.ExportRSAPublicKey();
        }
        throw new ArgumentException("Invalid RSA JWK; missing 'n' or 'e'.");
    }
}