using System;
using System.Security.Cryptography;
using Mamey.Security.PostQuantum.Interfaces;
using Mamey.Security.PostQuantum.Models;
using Mamey.Security.PostQuantum.Native;

namespace Mamey.Security.PostQuantum.Algorithms.MLKEM;

/// <summary>
/// ML-KEM (Kyber) encryptor implementation backed by liboqs.
/// Supports ML_KEM_512, ML_KEM_768 and ML_KEM_1024 variants through the
/// <see cref="IPQEncryptor"/> abstraction and exposes convenience methods
/// for hybrid data encryption using AES-256-GCM.
/// </summary>
public sealed class MLKEMEncryptor : IPQEncryptor
{
    /// <summary>
    /// Perform KEM encapsulation for the given algorithm and recipient public key.
    /// Returns the KEM ciphertext and outputs the shared secret.
    /// </summary>
    public PQCiphertext Encapsulate(KemAlgorithm algorithm, byte[] recipientPublicKey, out byte[] sharedSecret)
    {
        if (recipientPublicKey is null) throw new ArgumentNullException(nameof(recipientPublicKey));

        var (pkSize, _, ctSize, ssSize) = GetSizes(algorithm);
        if (recipientPublicKey.Length != pkSize)
        {
            throw new CryptographicException($"Invalid ML-KEM public key length: {recipientPublicKey.Length}, expected {pkSize}.");
        }

        var ciphertext = new byte[ctSize];
        sharedSecret = new byte[ssSize];

        int status = algorithm switch
        {
            KemAlgorithm.ML_KEM_512 => LibOQS.OQS_KEM_kyber_512_encaps(ciphertext, sharedSecret, recipientPublicKey),
            KemAlgorithm.ML_KEM_768 => LibOQS.OQS_KEM_kyber_768_encaps(ciphertext, sharedSecret, recipientPublicKey),
            KemAlgorithm.ML_KEM_1024 => LibOQS.OQS_KEM_kyber_1024_encaps(ciphertext, sharedSecret, recipientPublicKey),
            _ => throw new ArgumentOutOfRangeException(nameof(algorithm), algorithm, "Unsupported ML-KEM algorithm")
        };

        LibOQS.EnsureSuccess(status, $"OQS_KEM_{algorithm}_encaps");
        return new PQCiphertext(ciphertext);
    }

    /// <summary>
    /// Perform KEM decapsulation for the given algorithm, ciphertext and recipient private key.
    /// Returns the recovered shared secret.
    /// </summary>
    public byte[] Decapsulate(KemAlgorithm algorithm, PQCiphertext ciphertext, byte[] recipientPrivateKey)
    {
        if (ciphertext is null) throw new ArgumentNullException(nameof(ciphertext));
        if (recipientPrivateKey is null) throw new ArgumentNullException(nameof(recipientPrivateKey));

        var (pkSize, skSize, ctSize, ssSize) = GetSizes(algorithm);

        if (ciphertext.Value.Length != ctSize)
        {
            throw new CryptographicException($"Invalid ML-KEM ciphertext length: {ciphertext.Value.Length}, expected {ctSize}.");
        }

        if (recipientPrivateKey.Length != skSize)
        {
            throw new CryptographicException($"Invalid ML-KEM private key length: {recipientPrivateKey.Length}, expected {skSize}.");
        }

        var sharedSecret = new byte[ssSize];
        int status = algorithm switch
        {
            KemAlgorithm.ML_KEM_512 => LibOQS.OQS_KEM_kyber_512_decaps(sharedSecret, ciphertext.Value, recipientPrivateKey),
            KemAlgorithm.ML_KEM_768 => LibOQS.OQS_KEM_kyber_768_decaps(sharedSecret, ciphertext.Value, recipientPrivateKey),
            KemAlgorithm.ML_KEM_1024 => LibOQS.OQS_KEM_kyber_1024_decaps(sharedSecret, ciphertext.Value, recipientPrivateKey),
            _ => throw new ArgumentOutOfRangeException(nameof(algorithm), algorithm, "Unsupported ML-KEM algorithm")
        };

        LibOQS.EnsureSuccess(status, $"OQS_KEM_{algorithm}_decaps");
        return sharedSecret;
    }

    /// <summary>
    /// Convenience wrapper that performs KEM encapsulation and then uses the
    /// derived shared secret as an AES-256-GCM key to encrypt the given
    /// plaintext. Returns the KEM ciphertext plus AES ciphertext, nonce and tag.
    /// </summary>
    public (PQCiphertext kemCiphertext, byte[] ciphertext, byte[] nonce, byte[] tag) EncryptMLKEM(
        KemAlgorithm algorithm,
        byte[] recipientPublicKey,
        byte[] plaintext,
        byte[]? associatedData = null)
    {
        if (plaintext is null) throw new ArgumentNullException(nameof(plaintext));

        var kemCiphertext = Encapsulate(algorithm, recipientPublicKey, out var sharedSecret);
        var (ciphertext, nonce, tag) = EncryptWithAesGcm(sharedSecret, plaintext, associatedData);
        return (kemCiphertext, ciphertext, nonce, tag);
    }

    /// <summary>
    /// Convenience wrapper that performs KEM decapsulation and then uses the
    /// recovered shared secret as an AES-256-GCM key to decrypt the given
    /// ciphertext. Throws <see cref="CryptographicException"/> if tag
    /// validation fails.
    /// </summary>
    public byte[] DecryptMLKEM(
        KemAlgorithm algorithm,
        byte[] recipientPrivateKey,
        PQCiphertext kemCiphertext,
        byte[] ciphertext,
        byte[] nonce,
        byte[] tag,
        byte[]? associatedData = null)
    {
        if (ciphertext is null) throw new ArgumentNullException(nameof(ciphertext));
        if (nonce is null) throw new ArgumentNullException(nameof(nonce));
        if (tag is null) throw new ArgumentNullException(nameof(tag));

        var sharedSecret = Decapsulate(algorithm, kemCiphertext, recipientPrivateKey);
        return DecryptWithAesGcm(sharedSecret, ciphertext, nonce, tag, associatedData);
    }

    private static (byte[] ciphertext, byte[] nonce, byte[] tag) EncryptWithAesGcm(
        byte[] key,
        byte[] plaintext,
        byte[]? associatedData)
    {
        if (key is null) throw new ArgumentNullException(nameof(key));
        if (plaintext is null) throw new ArgumentNullException(nameof(plaintext));

        // AES-256-GCM expects a 32-byte key
        if (key.Length != 32)
        {
            throw new CryptographicException($"AES-256-GCM requires 32-byte key, got {key.Length}.");
        }

        var nonce = new byte[12]; // 96-bit nonce as recommended for GCM
        var tag = new byte[16];   // 128-bit tag
        var ciphertext = new byte[plaintext.Length];

        RandomNumberGenerator.Fill(nonce);

        using var aes = new AesGcm(key, tagSizeInBytes: 16);
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

        using var aes = new AesGcm(key, tagSizeInBytes: 16);
        aes.Decrypt(nonce, ciphertext, tag, plaintext, associatedData);

        return plaintext;
    }

    private static (int publicKeySize, int privateKeySize, int ciphertextSize, int sharedSecretSize) GetSizes(KemAlgorithm algorithm)
    {
        return algorithm switch
        {
            // Values aligned with acceptance criteria from T-MN-QTM-004
            KemAlgorithm.ML_KEM_512 => (800, 1632, 768, 32),
            KemAlgorithm.ML_KEM_768 => (1184, 2400, 1088, 32),
            KemAlgorithm.ML_KEM_1024 => (1568, 3168, 1568, 32),
            _ => throw new ArgumentOutOfRangeException(nameof(algorithm), algorithm, "Algorithm is not an ML-KEM variant")
        };
    }
}


