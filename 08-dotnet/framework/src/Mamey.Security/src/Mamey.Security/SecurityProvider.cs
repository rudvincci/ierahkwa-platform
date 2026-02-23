using System.Text.Encodings.Web;
using System.Security.Cryptography.X509Certificates;
using System.Security.Cryptography;
using System.Text;
using Mamey.Security.PostQuantum;
using Mamey.Security.PostQuantum.Interfaces;
using Mamey.Security.PostQuantum.Models;
using Mamey.Security.PostQuantum.Algorithms.MLKEM;

namespace Mamey.Security;

public sealed class SecurityProvider : ISecurityProvider
{
    private readonly IEncryptor _encryptor;
    private readonly IHasher _hasher;
    private readonly IRng _rng;
    private readonly ISigner _signer;
    private readonly IMd5 _md5;
    private readonly UrlEncoder _urlEncoder;
    private readonly SecurityOptions _securityOptions;
    private readonly string _key;

    private readonly IPQSigner? _pqSigner;
    private readonly IPQKeyGenerator? _pqKeyGenerator;
    private readonly IPQEncryptor? _pqEncryptor;

    public SecurityProvider(IEncryptor encryptor, IHasher hasher,
        IRng rng, ISigner signer, IMd5 md5, SecurityOptions securityOptions)
        : this(encryptor, hasher, rng, signer, md5, securityOptions, null, null, null)
    {
    }

    public SecurityProvider(
        IEncryptor encryptor,
        IHasher hasher,
        IRng rng,
        ISigner signer,
        IMd5 md5,
        SecurityOptions securityOptions,
        IPQSigner? pqSigner,
        IPQKeyGenerator? pqKeyGenerator,
        IPQEncryptor? pqEncryptor)
    {
        _encryptor = encryptor ?? throw new ArgumentNullException(nameof(encryptor));
        _hasher = hasher ?? throw new ArgumentNullException(nameof(hasher));
        _rng = rng ?? throw new ArgumentNullException(nameof(rng));
        _signer = signer ?? throw new ArgumentNullException(nameof(signer));
        _md5 = md5 ?? throw new ArgumentNullException(nameof(md5));
        _urlEncoder = UrlEncoder.Create();
        _securityOptions = securityOptions ?? throw new ArgumentNullException(nameof(securityOptions));
        _key = securityOptions.Encryption.Key;

        _pqSigner = pqSigner;
        _pqKeyGenerator = pqKeyGenerator;
        _pqEncryptor = pqEncryptor;
    }

    public string Encrypt(string data)
    {
        if (data == null)
        {
            throw new ArgumentNullException(nameof(data));
        }

        return _securityOptions.Encryption.Enabled ? _encryptor.Encrypt(data, _key) : data;
    }

    public string Decrypt(string data)
    {
        if (data == null)
        {
            throw new ArgumentNullException(nameof(data));
        }

        return _securityOptions.Encryption.Enabled ? _encryptor.Decrypt(data, _key) : data;
    }

    public string Hash(string data)
    {
        if (data is null)
        {
            throw new ArgumentNullException(nameof(data));
        }

        return _hasher.Hash(data);
    }

    public byte[] Hash(byte[] data)
    {
        if (data is null)
        {
            throw new ArgumentNullException(nameof(data));
        }

        return _hasher.Hash(data);
    }

    public string Sign(object data, X509Certificate2 certificate)
    {
        if (data is null)
        {
            throw new ArgumentNullException(nameof(data));
        }

        if (certificate is null)
        {
            throw new ArgumentNullException(nameof(certificate));
        }

        return _signer.Sign(data, certificate);
    }

    public bool Verify(object data, X509Certificate2 certificate, string signature)
    {
        if (data is null)
        {
            throw new ArgumentNullException(nameof(data));
        }

        if (certificate is null)
        {
            throw new ArgumentNullException(nameof(certificate));
        }

        if (string.IsNullOrEmpty(signature))
        {
            throw new ArgumentException($"'{nameof(signature)}' cannot be null or empty.", nameof(signature));
        }

        return _signer.Verify(data, certificate, signature);
    }

    public byte[] Sign(byte[] data, X509Certificate2 certificate)
    {
        if (data is null)
        {
            throw new ArgumentNullException(nameof(data));
        }

        if (certificate is null)
        {
            throw new ArgumentNullException(nameof(certificate));
        }

        return _signer.Sign(data, certificate);
    }

    public bool Verify(byte[] data, X509Certificate2 certificate, byte[] signature)
    {
        if (data is null)
        {
            throw new ArgumentNullException(nameof(data));
        }

        if (certificate is null)
        {
            throw new ArgumentNullException(nameof(certificate));
        }

        if (signature is null)
        {
            throw new ArgumentNullException(nameof(signature));
        }

        return _signer.Verify(data, certificate, signature);
    }

    public string GenerateRandomString(int length = 50, bool removeSpecialChars = true)
        => _rng.Generate(length, removeSpecialChars);

    public string SanitizeUrl(string value)
    {
        if (string.IsNullOrEmpty(value))
        {
            throw new ArgumentException($"'{nameof(value)}' cannot be null or empty.", nameof(value));
        }

        return _urlEncoder.Encode(value);
    }

    public string CalculateMd5(string value)
    {
        if (value == null)
        {
            throw new ArgumentNullException(nameof(value));
        }
        
        if (string.IsNullOrEmpty(value))
        {
            throw new ArgumentException($"'{nameof(value)}' cannot be empty.", nameof(value));
        }

        return _md5.Calculate(value);
    }

    #region Post-quantum operations

    private void EnsurePostQuantumEnabled()
    {
        if (!_securityOptions.EnablePostQuantum)
        {
            throw new InvalidOperationException("Post-quantum cryptography is disabled in SecurityOptions.");
        }

        if (_pqSigner is null || _pqKeyGenerator is null || _pqEncryptor is null)
        {
            throw new InvalidOperationException("Post-quantum services are not configured in the DI container.");
        }
    }

    public byte[] SignPostQuantum(byte[] data, byte[] privateKey, SignatureAlgorithm algorithm)
    {
        if (data is null) throw new ArgumentNullException(nameof(data));
        if (privateKey is null) throw new ArgumentNullException(nameof(privateKey));

        EnsurePostQuantumEnabled();

        if (algorithm != _pqSigner!.Algorithm)
        {
            throw new ArgumentException(
                $"Requested algorithm '{algorithm}' does not match configured signer algorithm '{_pqSigner.Algorithm}'.",
                nameof(algorithm));
        }

        // Current implementation uses the configured PQ signer key pair.
        // The privateKey parameter is reserved for future key-selection logic.
        return _pqSigner.Sign(data);
    }

    public bool VerifyPostQuantum(byte[] data, byte[] signature, byte[] publicKey, SignatureAlgorithm algorithm)
    {
        if (data is null) throw new ArgumentNullException(nameof(data));
        if (signature is null) throw new ArgumentNullException(nameof(signature));
        if (publicKey is null) throw new ArgumentNullException(nameof(publicKey));

        EnsurePostQuantumEnabled();

        if (algorithm != _pqSigner!.Algorithm)
        {
            throw new ArgumentException(
                $"Requested algorithm '{algorithm}' does not match configured signer algorithm '{_pqSigner.Algorithm}'.",
                nameof(algorithm));
        }

        return _pqSigner.Verify(data, signature, publicKey);
    }

    public byte[] EncryptPostQuantum(byte[] data, byte[] recipientPublicKey, KemAlgorithm algorithm)
    {
        if (data is null) throw new ArgumentNullException(nameof(data));
        if (recipientPublicKey is null) throw new ArgumentNullException(nameof(recipientPublicKey));

        EnsurePostQuantumEnabled();

        // Prefer MLKEMEncryptor convenience API when available.
        if (_pqEncryptor is MLKEMEncryptor kemEncryptor)
        {
            var (kemCiphertext, ciphertext, nonce, tag) = kemEncryptor.EncryptMLKEM(algorithm, recipientPublicKey, data);
            return CombineKemAndAes(kemCiphertext.Value, ciphertext, nonce, tag);
        }

        // Generic fallback: use KEM encapsulation plus AES-GCM.
        var kemCt = _pqEncryptor!.Encapsulate(algorithm, recipientPublicKey, out var sharedSecret);
        var (ct, nonce2, tag2) = EncryptWithAesGcm(sharedSecret, data, null);
        return CombineKemAndAes(kemCt.Value, ct, nonce2, tag2);
    }

    public byte[] DecryptPostQuantum(byte[] ciphertext, byte[] recipientPrivateKey, KemAlgorithm algorithm)
    {
        if (ciphertext is null) throw new ArgumentNullException(nameof(ciphertext));
        if (recipientPrivateKey is null) throw new ArgumentNullException(nameof(recipientPrivateKey));

        EnsurePostQuantumEnabled();

        SplitKemAndAes(ciphertext, out var kemBytes, out var ct, out var nonce, out var tag);

        if (_pqEncryptor is MLKEMEncryptor kemEncryptor)
        {
            var kemCiphertext = new PQCiphertext(kemBytes);
            return kemEncryptor.DecryptMLKEM(algorithm, recipientPrivateKey, kemCiphertext, ct, nonce, tag);
        }

        var kemCt = new PQCiphertext(kemBytes);
        var sharedSecret = _pqEncryptor!.Decapsulate(algorithm, kemCt, recipientPrivateKey);
        return DecryptWithAesGcm(sharedSecret, ct, nonce, tag, null);
    }

    private static byte[] CombineKemAndAes(byte[] kemCiphertext, byte[] ciphertext, byte[] nonce, byte[] tag)
    {
        if (kemCiphertext is null) throw new ArgumentNullException(nameof(kemCiphertext));
        if (ciphertext is null) throw new ArgumentNullException(nameof(ciphertext));
        if (nonce is null) throw new ArgumentNullException(nameof(nonce));
        if (tag is null) throw new ArgumentNullException(nameof(tag));

        // Format: [version:1][kemLen:4][nonceLen:4][tagLen:4][kemCt][nonce][tag][ciphertext]
        const byte version = 1;
        var kemLen = (uint)kemCiphertext.Length;
        var nonceLen = (uint)nonce.Length;
        var tagLen = (uint)tag.Length;

        var totalLen = 1 + 4 + 4 + 4 + kemCiphertext.Length + nonce.Length + tag.Length + ciphertext.Length;
        var buffer = new byte[totalLen];
        var offset = 0;

        buffer[offset++] = version;
        System.Buffers.Binary.BinaryPrimitives.WriteUInt32LittleEndian(buffer.AsSpan(offset, 4), kemLen);
        offset += 4;
        System.Buffers.Binary.BinaryPrimitives.WriteUInt32LittleEndian(buffer.AsSpan(offset, 4), nonceLen);
        offset += 4;
        System.Buffers.Binary.BinaryPrimitives.WriteUInt32LittleEndian(buffer.AsSpan(offset, 4), tagLen);
        offset += 4;

        Buffer.BlockCopy(kemCiphertext, 0, buffer, offset, kemCiphertext.Length);
        offset += kemCiphertext.Length;
        Buffer.BlockCopy(nonce, 0, buffer, offset, nonce.Length);
        offset += nonce.Length;
        Buffer.BlockCopy(tag, 0, buffer, offset, tag.Length);
        offset += tag.Length;
        Buffer.BlockCopy(ciphertext, 0, buffer, offset, ciphertext.Length);

        return buffer;
    }

    private static void SplitKemAndAes(byte[] combined, out byte[] kemCiphertext, out byte[] ciphertext, out byte[] nonce, out byte[] tag)
    {
        if (combined is null) throw new ArgumentNullException(nameof(combined));
        if (combined.Length < 1 + 4 + 4 + 4)
        {
            throw new CryptographicException("Invalid post-quantum ciphertext format.");
        }

        var offset = 0;
        var version = combined[offset++];
        if (version != 1)
        {
            throw new CryptographicException("Unsupported post-quantum ciphertext version.");
        }

        var span = combined.AsSpan(offset);
        var kemLen = System.Buffers.Binary.BinaryPrimitives.ReadUInt32LittleEndian(span.Slice(0, 4));
        var nonceLen = System.Buffers.Binary.BinaryPrimitives.ReadUInt32LittleEndian(span.Slice(4, 4));
        var tagLen = System.Buffers.Binary.BinaryPrimitives.ReadUInt32LittleEndian(span.Slice(8, 4));

        offset += 12;

        var expectedLen = 1 + 12 + (int)kemLen + (int)nonceLen + (int)tagLen;
        if (combined.Length < expectedLen)
        {
            throw new CryptographicException("Invalid post-quantum ciphertext length.");
        }

        kemCiphertext = new byte[kemLen];
        Buffer.BlockCopy(combined, offset, kemCiphertext, 0, (int)kemLen);
        offset += (int)kemLen;

        nonce = new byte[nonceLen];
        Buffer.BlockCopy(combined, offset, nonce, 0, (int)nonceLen);
        offset += (int)nonceLen;

        tag = new byte[tagLen];
        Buffer.BlockCopy(combined, offset, tag, 0, (int)tagLen);
        offset += (int)tagLen;

        var remaining = combined.Length - offset;
        if (remaining < 0)
        {
            throw new CryptographicException("Invalid post-quantum ciphertext payload.");
        }

        ciphertext = new byte[remaining];
        Buffer.BlockCopy(combined, offset, ciphertext, 0, remaining);
    }

    private static (byte[] ciphertext, byte[] nonce, byte[] tag) EncryptWithAesGcm(
        byte[] key,
        byte[] plaintext,
        byte[]? associatedData)
    {
        if (key is null) throw new ArgumentNullException(nameof(key));
        if (plaintext is null) throw new ArgumentNullException(nameof(plaintext));

        if (key.Length != 32)
        {
            throw new CryptographicException($"AES-256-GCM requires 32-byte key, got {key.Length}.");
        }

        var nonce = new byte[12];
        var tag = new byte[16];
        var ciphertext = new byte[plaintext.Length];

        RandomNumberGenerator.Fill(nonce);

        using var aes = new AesGcm(key);
        aes.Encrypt(nonce, plaintext, ciphertext, tag, associatedData);

        return (ciphertext, nonce, tag);
    }

    private static byte[] DecryptWithAesGcm(
        byte[] key,
        byte[] ciphertext,
        byte[] nonce,
        byte[] tag,
        byte[]? associatedData)
    {
        if (key is null) throw new ArgumentNullException(nameof(key));
        if (ciphertext is null) throw new ArgumentNullException(nameof(ciphertext));
        if (nonce is null) throw new ArgumentNullException(nameof(nonce));
        if (tag is null) throw new ArgumentNullException(nameof(tag));

        if (key.Length != 32)
        {
            throw new CryptographicException($"AES-256-GCM requires 32-byte key, got {key.Length}.");
        }

        var plaintext = new byte[ciphertext.Length];

        using var aes = new AesGcm(key);
        aes.Decrypt(nonce, ciphertext, tag, plaintext, associatedData);

        return plaintext;
    }

    #endregion

}

public  interface ICertificateProvider<TPrivateKey>
    where TPrivateKey : class, IPrivateKey
{
    string SignWithPrivateKey(object data, IPrivateKey privateKey);
    bool VerifyWithPrivateKey(object data, IPrivateKey privateKey, string signature);
    byte[] SignWithPrivateKey(byte[] data, IPrivateKey privateKey);
    bool VerifyWithPrivateKey(byte[] data, IPrivateKey privateKey, byte[] signature);
    IPrivateKeyResult<TPrivateKey> GeneratePrivateKey(int length = 50, bool pkHasSpecialCharacters = false);
    bool VerifyPrivateKeySignature(TPrivateKey privateKey);
    ICertificateResult<TPrivateKey> Generate(int keyLength = 50, bool pkHasSpecialCharacters = false, string? subject = null);
    ICertificateResult<TPrivateKey> GenerateFromPrivateKey(TPrivateKey privateKey, string? subject = null);
    void ExportToFile(X509Certificate2 certificate, string filePath);
}

public sealed class CertificateProvider<TPrivateKey> : ICertificateProvider<TPrivateKey>
    where TPrivateKey: class, IPrivateKey
{
    private readonly SecurityOptions _securityOptions;
    private readonly ISecurityProvider _securityProvider;
    private readonly IPrivateKeyGenerator<IPrivateKey, IPrivateKeyResult<IPrivateKey>> _privateKeyGenerator;
    private readonly ICertificateGenerator<TPrivateKey, ICertificateResult<TPrivateKey>> _certificateGenerator;

    public CertificateProvider(SecurityOptions securityOptions,
        IPrivateKeyGenerator<IPrivateKey, IPrivateKeyResult<IPrivateKey>> privateKeyGenerator)
    {
        _securityOptions = securityOptions;
        _privateKeyGenerator = privateKeyGenerator;
    }

    public CertificateProvider(SecurityOptions securityOptions, ISecurityProvider securityProvider,
        IPrivateKeyGenerator<IPrivateKey, IPrivateKeyResult<IPrivateKey>> privateKeyGenerator,
        ICertificateGenerator<TPrivateKey, ICertificateResult<TPrivateKey>> certificateGenerator)
    {
        _securityOptions = securityOptions;
        _securityProvider = securityProvider;
        _privateKeyGenerator = privateKeyGenerator;
        _certificateGenerator = certificateGenerator;
    }

    public string SignWithPrivateKey(object data, IPrivateKey privateKey)
    {
        if (data is null)
        {
            throw new ArgumentNullException(nameof(data));
        }

        if (privateKey is null)
        {
            throw new ArgumentNullException(nameof(privateKey));
        }

        var decryptedPrivateKey = _securityProvider.Decrypt(privateKey.EncryptedPrivateKey);
        using var rsa = RSA.Create();
        rsa.ImportFromPem(decryptedPrivateKey);
        var dataBytes = Encoding.UTF8.GetBytes(data.ToString());
        var signature = rsa.SignData(dataBytes, HashAlgorithmName.SHA256, RSASignaturePadding.Pkcs1);
        return Convert.ToBase64String(signature);
    }
    public bool VerifyWithPrivateKey(object data, IPrivateKey privateKey, string signature)
    {
        if (data is null)
        {
            throw new ArgumentNullException(nameof(data));
        }

        if (privateKey is null)
        {
            throw new ArgumentNullException(nameof(privateKey));
        }

        if (string.IsNullOrEmpty(signature))
        {
            throw new ArgumentException($"'{nameof(signature)}' cannot be null or empty.", nameof(signature));
        }

        var decryptedPrivateKey = _securityProvider.Decrypt(privateKey.EncryptedPrivateKey);
        using var rsa = RSA.Create();
        rsa.ImportFromPem(decryptedPrivateKey);
        var dataBytes = Encoding.UTF8.GetBytes(data.ToString());
        var signatureBytes = Convert.FromBase64String(signature);
        return rsa.VerifyData(dataBytes, signatureBytes, HashAlgorithmName.SHA256, RSASignaturePadding.Pkcs1);
    }
    public byte[] SignWithPrivateKey(byte[] data, IPrivateKey privateKey)
    {
        var decryptedPrivateKey = _securityProvider.Decrypt(privateKey.EncryptedPrivateKey);
        using var rsa = RSA.Create();
        rsa.ImportFromPem(decryptedPrivateKey);
        return rsa.SignData(data, HashAlgorithmName.SHA256, RSASignaturePadding.Pkcs1);
    }
    public bool VerifyWithPrivateKey(byte[] data, IPrivateKey privateKey, byte[] signature)
    {
        var decryptedPrivateKey = _securityProvider.Decrypt(privateKey.EncryptedPrivateKey);
        using var rsa = RSA.Create();
        rsa.ImportFromPem(decryptedPrivateKey);
        return rsa.VerifyData(data, signature, HashAlgorithmName.SHA256, RSASignaturePadding.Pkcs1);
    }

    public IPrivateKeyResult<TPrivateKey> GeneratePrivateKey(int length, bool pkHasSpecialCharacters = false)
        => (IPrivateKeyResult<TPrivateKey>)_privateKeyGenerator.Generate(length, pkHasSpecialCharacters);

    public bool VerifyPrivateKeySignature(TPrivateKey privateKey)
        => _privateKeyGenerator.VerifyPrivateKeySignature(privateKey);

    public ICertificateResult<TPrivateKey> Generate(int keyLength = 50, bool pkHasSpecialCharacters = false, string? subject = null)
        => _certificateGenerator.Generate(keyLength, pkHasSpecialCharacters, subject);

    public ICertificateResult<TPrivateKey> GenerateFromPrivateKey(TPrivateKey privateKey, string? subject = null)
        => _certificateGenerator.GenerateFromPrivateKey(privateKey, subject);
    public void ExportToFile(X509Certificate2 certificate, string filePath)
        => _certificateGenerator.ExportToFile(certificate, filePath);
}
