using NSec.Cryptography;

namespace Mamey.Identity.Decentralized.Crypto;

/// <summary>
/// Provides Ed25519 keypair generation, signing, verifying, and export for did:key and related methods.
/// </summary>
public class Ed25519CryptoProvider : IKeyPairCryptoProvider
{
    public string KeyType => "Ed25519";
    public string VerificationMethodType => "Ed25519VerificationKey2018";

    private static readonly SignatureAlgorithm Ed25519Alg = SignatureAlgorithm.Ed25519;

    public async System.Threading.Tasks.Task<(byte[] privateKey, byte[] publicKey)> GenerateKeyPairAsync(string keyType)
    {
        if (!string.Equals(keyType, KeyType, StringComparison.OrdinalIgnoreCase))
            throw new NotSupportedException($"Key type {keyType} not supported by Ed25519 provider.");

        using var key = Key.Create(Ed25519Alg,
            new KeyCreationParameters { ExportPolicy = KeyExportPolicies.AllowPlaintextExport });
        var publicKey = key.PublicKey.Export(KeyBlobFormat.RawPublicKey);
        var privateKey = key.Export(KeyBlobFormat.RawPrivateKey);
        return (privateKey, publicKey);
    }

    public byte[] Sign(string keyType, byte[] privateKey, byte[] data)
    {
        if (!string.Equals(keyType, KeyType, StringComparison.OrdinalIgnoreCase))
            throw new NotSupportedException($"Key type {keyType} not supported by Ed25519 provider.");

        using var key = Key.Import(Ed25519Alg, privateKey, KeyBlobFormat.RawPrivateKey);
        return Ed25519Alg.Sign(key, data);
    }

    public bool Verify(string keyType, byte[] publicKey, byte[] data, byte[] signature)
    {
        if (!string.Equals(keyType, KeyType, StringComparison.OrdinalIgnoreCase))
            throw new NotSupportedException($"Key type {keyType} not supported by Ed25519 provider.");

        var pubKey = PublicKey.Import(Ed25519Alg, publicKey, KeyBlobFormat.RawPublicKey);
        return Ed25519Alg.Verify(pubKey, data, signature);
    }

    public IDictionary<string, object> ExportJwk(byte[] publicKey)
    {
        return new Dictionary<string, object>
        {
            { "kty", "OKP" },
            { "crv", "Ed25519" },
            { "x", Base64Url.Encode(publicKey) }
        };
    }

    public string ExportBase58(byte[] publicKey) => Base58Util.Encode(publicKey);

    public string ExportMultibase(byte[] publicKey) => MultibaseUtil.Encode(publicKey, MultibaseUtil.Base.Base58Btc);

    public byte[] ImportJwk(IDictionary<string, object> jwk)
    {
        if (jwk.TryGetValue("x", out var x))
            return Base64Url.Decode(x.ToString());
        throw new ArgumentException("Invalid Ed25519 JWK; missing 'x'.");
    }
}