using System.Security.Cryptography;
using Mamey.Auth.Decentralized.Exceptions;
using Mamey.Auth.Decentralized.Utilities;

namespace Mamey.Auth.Decentralized.Crypto;

/// <summary>
/// RSA cryptographic provider using BouncyCastle
/// </summary>
public class RsaProvider : CryptoProviderBase
{
    private const int MinKeySize = 2048;
    private const int DefaultKeySize = 2048;
    
    /// <summary>
    /// Gets the name of the cryptographic algorithm
    /// </summary>
    public override string AlgorithmName => "RSA";
    
    /// <summary>
    /// Gets the curve name for elliptic curve algorithms
    /// </summary>
    public override string? CurveName => null;
    
    /// <summary>
    /// Generates a new key pair
    /// </summary>
    /// <returns>A new key pair</returns>
    public override KeyPair GenerateKeyPair()
    {
        return GenerateKeyPair(DefaultKeySize);
    }
    
    /// <summary>
    /// Generates a new key pair with a specific key size
    /// </summary>
    /// <param name="keySize">The key size in bits</param>
    /// <returns>A new key pair</returns>
    public KeyPair GenerateKeyPair(int keySize)
    {
        if (keySize < MinKeySize)
            throw new CryptoException(AlgorithmName, "GenerateKeyPair", 
                $"RSA key size {keySize} is too small. Minimum size is {MinKeySize} bits");
        
        try
        {
            using var rsa = RSA.Create(keySize);
            var publicKey = rsa.ExportRSAPublicKey();
            var privateKey = rsa.ExportRSAPrivateKey();
            
            return new KeyPair(publicKey, privateKey, AlgorithmName);
        }
        catch (Exception ex)
        {
            throw new CryptoException(AlgorithmName, "GenerateKeyPair", "Failed to generate RSA key pair", ex);
        }
    }
    
    /// <summary>
    /// Generates a new key pair with a specific seed
    /// </summary>
    /// <param name="seed">The seed for key generation</param>
    /// <returns>A new key pair</returns>
    public override KeyPair GenerateKeyPair(byte[] seed)
    {
        if (seed == null)
            throw new ArgumentNullException(nameof(seed));
        
        // Use seed to determine key size (minimum 2048)
        var keySize = Math.Max(MinKeySize, seed.Length * 8);
        return GenerateKeyPair(keySize);
    }
    
    /// <summary>
    /// Signs data with the private key
    /// </summary>
    /// <param name="data">The data to sign</param>
    /// <param name="privateKey">The private key</param>
    /// <returns>The signature</returns>
    public override byte[] Sign(byte[] data, byte[] privateKey)
    {
        ValidateData(data, "Sign");
        
        if (privateKey == null)
            throw new ArgumentNullException(nameof(privateKey));
        
        try
        {
            using var rsa = RSA.Create();
            rsa.ImportRSAPrivateKey(privateKey, out _);
            
            return rsa.SignData(data, HashAlgorithmName.SHA256, RSASignaturePadding.Pkcs1);
        }
        catch (Exception ex)
        {
            throw new CryptoException(AlgorithmName, "Sign", "Failed to sign data with RSA", ex);
        }
    }
    
    /// <summary>
    /// Verifies a signature with the public key
    /// </summary>
    /// <param name="data">The original data</param>
    /// <param name="signature">The signature to verify</param>
    /// <param name="publicKey">The public key</param>
    /// <returns>True if the signature is valid, false otherwise</returns>
    public override bool Verify(byte[] data, byte[] signature, byte[] publicKey)
    {
        ValidateData(data, "Verify");
        
        if (publicKey == null)
            return false;
        
        if (signature == null)
            return false;
        
        try
        {
            using var rsa = RSA.Create();
            rsa.ImportRSAPublicKey(publicKey, out _);
            
            return rsa.VerifyData(data, signature, HashAlgorithmName.SHA256, RSASignaturePadding.Pkcs1);
        }
        catch
        {
            return false;
        }
    }
    
    /// <summary>
    /// Converts a public key to JWK format
    /// </summary>
    /// <param name="publicKey">The public key</param>
    /// <returns>The public key in JWK format</returns>
    public override Dictionary<string, object> PublicKeyToJwk(byte[] publicKey)
    {
        if (publicKey == null)
            throw new ArgumentNullException(nameof(publicKey));
        
        try
        {
            using var rsa = RSA.Create();
            rsa.ImportRSAPublicKey(publicKey, out _);
            
            var parameters = rsa.ExportParameters(false);
            
            return new Dictionary<string, object>
            {
                ["kty"] = "RSA",
                ["n"] = Base64UrlUtil.Encode(parameters.Modulus!),
                ["e"] = Base64UrlUtil.Encode(parameters.Exponent!)
            };
        }
        catch (Exception ex)
        {
            throw new CryptoException(AlgorithmName, "PublicKeyToJwk", "Failed to convert RSA public key to JWK", ex);
        }
    }
    
    /// <summary>
    /// Converts a public key to multibase format
    /// </summary>
    /// <param name="publicKey">The public key</param>
    /// <returns>The public key in multibase format</returns>
    public override string PublicKeyToMultibase(byte[] publicKey)
    {
        if (publicKey == null)
            throw new ArgumentNullException(nameof(publicKey));
        
        // RSA public key in multibase format with RSA public key multicodec
        var multicodecPrefix = new byte[] { 0x12, 0x05 }; // RSA public key multicodec
        var prefixedKey = multicodecPrefix.Concat(publicKey).ToArray();
        return MultibaseUtil.Encode(prefixedKey);
    }
    
    /// <summary>
    /// Converts a JWK to public key bytes
    /// </summary>
    /// <param name="jwk">The JWK</param>
    /// <returns>The public key bytes</returns>
    public override byte[] JwkToPublicKey(Dictionary<string, object> jwk)
    {
        if (jwk == null)
            throw new ArgumentNullException(nameof(jwk));
        
        if (!jwk.TryGetValue("kty", out var kty) || kty?.ToString() != "RSA")
            throw new CryptoException(AlgorithmName, "JwkToPublicKey", "Invalid key type for RSA");
        
        if (!jwk.TryGetValue("n", out var n) || n == null)
            throw new CryptoException(AlgorithmName, "JwkToPublicKey", "Missing 'n' parameter in JWK");
        
        if (!jwk.TryGetValue("e", out var e) || e == null)
            throw new CryptoException(AlgorithmName, "JwkToPublicKey", "Missing 'e' parameter in JWK");
        
        try
        {
            var modulus = Base64UrlUtil.Decode(n.ToString()!);
            var exponent = Base64UrlUtil.Decode(e.ToString()!);
            
            using var rsa = RSA.Create();
            var parameters = new RSAParameters
            {
                Modulus = modulus,
                Exponent = exponent
            };
            
            rsa.ImportParameters(parameters);
            return rsa.ExportRSAPublicKey();
        }
        catch (Exception ex)
        {
            throw new CryptoException(AlgorithmName, "JwkToPublicKey", "Failed to decode RSA public key from JWK", ex);
        }
    }
    
    /// <summary>
    /// Converts a multibase string to public key bytes
    /// </summary>
    /// <param name="multibase">The multibase string</param>
    /// <returns>The public key bytes</returns>
    public override byte[] MultibaseToPublicKey(string multibase)
    {
        if (string.IsNullOrEmpty(multibase))
            throw new ArgumentException("Multibase string cannot be null or empty", nameof(multibase));
        
        try
        {
            var decoded = MultibaseUtil.Decode(multibase);
            
            // Check for RSA public key multicodec prefix
            if (decoded.Length < 2 || decoded[0] != 0x12 || decoded[1] != 0x05)
                throw new CryptoException(AlgorithmName, "MultibaseToPublicKey", "Invalid RSA multicodec prefix");
            
            return decoded.Skip(2).ToArray();
        }
        catch (Exception ex)
        {
            throw new CryptoException(AlgorithmName, "MultibaseToPublicKey", "Failed to decode RSA public key from multibase", ex);
        }
    }
}
