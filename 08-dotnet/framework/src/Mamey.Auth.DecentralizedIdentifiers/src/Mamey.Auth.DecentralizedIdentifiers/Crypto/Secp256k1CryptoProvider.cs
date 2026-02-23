using System.Security.Cryptography;
using NBitcoin.Secp256k1;

namespace Mamey.Auth.DecentralizedIdentifiers.Crypto;

/// <summary>
/// Provides secp256k1 keypair operations for Ethereum/Bitcoin-based DID methods.
/// </summary>
public class Secp256k1CryptoProvider : IKeyPairCryptoProvider
{
    public string KeyType => "secp256k1";
    public string VerificationMethodType => "EcdsaSecp256k1VerificationKey2019";

    public async System.Threading.Tasks.Task<(byte[] privateKey, byte[] publicKey)> GenerateKeyPairAsync(string keyType)
    {
        if (!string.Equals(keyType, KeyType, StringComparison.OrdinalIgnoreCase))
            throw new NotSupportedException($"Key type {keyType} not supported by Secp256k1 provider.");

        var priv = new byte[32];
        using (var rnd = RandomNumberGenerator.Create())
        {
            do
            {
                rnd.GetBytes(priv);
            } while (!ECPrivKey.TryCreate(priv, out _));
        }

        var privKey = ECPrivKey.Create(priv);
        var pubKey = privKey.CreatePubKey().ToBytes(); // compressed (33 bytes)
        return (priv, pubKey);
    }

    public byte[] Sign(string keyType, byte[] privateKey, byte[] data)
    {
        if (!string.Equals(keyType, KeyType, StringComparison.OrdinalIgnoreCase))
            throw new NotSupportedException($"Key type {keyType} not supported by Secp256k1 provider.");

        if (!ECPrivKey.TryCreate(privateKey, out var privKey))
            throw new ArgumentException("Invalid secp256k1 private key.");

        var hash = SHA256Util.Hash(data);
        privKey.TrySignECDSA(hash, out SecpECDSASignature sig); // note the out var type

        return sig.ToDER();
    }

    public bool Verify(string keyType, byte[] publicKey, byte[] data, byte[] signature)
    {
        if (!string.Equals(keyType, KeyType, StringComparison.OrdinalIgnoreCase))
            throw new NotSupportedException($"Key type {keyType} not supported by Secp256k1 provider.");

        if (!ECPubKey.TryCreate(publicKey, Context.Instance, out bool compressed, out var pubKey))
            throw new ArgumentException("Invalid secp256k1 public key.");

        var hash = SHA256Util.Hash(data);
        if (!SecpECDSASignature.TryCreateFromDer(signature, out var sig))
            return false;

        return pubKey.SigVerify(sig, hash);
    }

    public IDictionary<string, object> ExportJwk(byte[] publicKey)
    {
        if (!ECPubKey.TryCreate(publicKey, Context.Instance, out bool compressed, out var pubKey))
            throw new ArgumentException("Invalid secp256k1 public key.");

        var uncompressed = pubKey.ToBytes(false);
        if (uncompressed.Length != 65 || uncompressed[0] != 0x04)
            throw new Exception("Failed to get uncompressed public key.");

        var x = new byte[32];
        var y = new byte[32];
        Array.Copy(uncompressed, 1, x, 0, 32);
        Array.Copy(uncompressed, 33, y, 0, 32);

        return new Dictionary<string, object>
        {
            { "kty", "EC" },
            { "crv", "secp256k1" },
            { "x", Base64Url.Encode(x) },
            { "y", Base64Url.Encode(y) }
        };
    }

    public string ExportBase58(byte[] publicKey) => Base58Util.Encode(publicKey);

    public string ExportMultibase(byte[] publicKey) => MultibaseUtil.Encode(publicKey, MultibaseUtil.Base.Base58Btc);

    public byte[] ImportJwk(IDictionary<string, object> jwk)
    {
        if (jwk.TryGetValue("x", out var x) && jwk.TryGetValue("y", out var y))
        {
            var xb = Base64Url.Decode(x.ToString());
            var yb = Base64Url.Decode(y.ToString());

            // Compose uncompressed key: 0x04 | x | y
            var uncompressed = new byte[65];
            uncompressed[0] = 0x04;
            Buffer.BlockCopy(xb, 0, uncompressed, 1, 32);
            Buffer.BlockCopy(yb, 0, uncompressed, 33, 32);

            if (!ECPubKey.TryCreate(uncompressed, Context.Instance, out bool compressed, out var pubKey))
                throw new Exception("Failed to create ECPubKey from JWK coordinates.");

            return pubKey.ToBytes(); // compressed
        }
        throw new ArgumentException("Invalid secp256k1 JWK; missing 'x' or 'y'.");
    }
}