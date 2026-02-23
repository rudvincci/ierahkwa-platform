using System;
using System.Security.Cryptography;
using System.Threading;
using System.Threading.Tasks;
using Mamey.Security.PostQuantum.Interfaces;
using Mamey.Security.PostQuantum.Models;
using Mamey.Security.PostQuantum.Native;

namespace Mamey.Security.PostQuantum.Algorithms.MLDSA;

/// <summary>
/// ML-DSA (Dilithium) signer implementation backed by liboqs.
/// Supports ML_DSA_44, ML_DSA_65 and ML_DSA_87 variants via the IPQSigner
/// abstraction defined in the Quantum Resistant MameyNode plan.
///
/// This class owns a single key pair instance; use <see cref="GenerateKeyPair"/>
/// to obtain a <see cref="PQKeyPair"/> and construct a signer around it.
/// </summary>
public sealed class MLDSASigner : IPQSigner
{
    private readonly byte[] _publicKey;
    private readonly byte[] _secretKey;

    public SignatureAlgorithm Algorithm { get; }

    public int SignatureSize { get; }
    public int PublicKeySize { get; }
    public int PrivateKeySize { get; }

    public byte[] PublicKey => (byte[])_publicKey.Clone();

    public MLDSASigner(SignatureAlgorithm algorithm, PQKeyPair keyPair)
        : this(algorithm, keyPair.PublicKey, keyPair.PrivateKey)
    {
    }

    public MLDSASigner(SignatureAlgorithm algorithm, byte[] publicKey, byte[] secretKey)
    {
        Algorithm = algorithm;
        (PublicKeySize, PrivateKeySize, SignatureSize) = GetSizes(algorithm);

        if (publicKey is null) throw new ArgumentNullException(nameof(publicKey));
        if (secretKey is null) throw new ArgumentNullException(nameof(secretKey));

        if (publicKey.Length != PublicKeySize)
        {
            throw new CryptographicException($"Invalid ML-DSA public key length: {publicKey.Length}, expected {PublicKeySize}.");
        }
        if (secretKey.Length != PrivateKeySize)
        {
            throw new CryptographicException($"Invalid ML-DSA private key length: {secretKey.Length}, expected {PrivateKeySize}.");
        }

        _publicKey = (byte[])publicKey.Clone();
        _secretKey = (byte[])secretKey.Clone();
    }

    /// <summary>
    /// Generate a new ML-DSA key pair for the given algorithm using liboqs.
    /// </summary>
    public static PQKeyPair GenerateKeyPair(SignatureAlgorithm algorithm)
    {
        (var pkSize, var skSize, _) = GetSizes(algorithm);

        var publicKey = new byte[pkSize];
        var secretKey = new byte[skSize];

        int status = algorithm switch
        {
            SignatureAlgorithm.ML_DSA_44 => LibOQS.OQS_SIG_dilithium_2_keypair(publicKey, secretKey),
            SignatureAlgorithm.ML_DSA_65 => LibOQS.OQS_SIG_dilithium_3_keypair(publicKey, secretKey),
            SignatureAlgorithm.ML_DSA_87 => LibOQS.OQS_SIG_dilithium_5_keypair(publicKey, secretKey),
            _ => throw new ArgumentOutOfRangeException(nameof(algorithm), algorithm, "Unsupported ML-DSA algorithm")
        };

        LibOQS.EnsureSuccess(status, $"OQS_SIG_{algorithm}_keypair");
        return new PQKeyPair(publicKey, secretKey);
    }

    public byte[] Sign(byte[] message)
    {
        if (message is null) throw new ArgumentNullException(nameof(message));
        if (_secretKey.Length != PrivateKeySize)
        {
            throw new CryptographicException("Invalid private key length for ML-DSA signer.");
        }

        var sigBuffer = new byte[SignatureSize];
        ulong sigLen = (ulong)sigBuffer.Length;

        int status = Algorithm switch
        {
            SignatureAlgorithm.ML_DSA_44 => LibOQS.OQS_SIG_dilithium_2_sign(sigBuffer, ref sigLen, message, (ulong)message.Length, _secretKey),
            SignatureAlgorithm.ML_DSA_65 => LibOQS.OQS_SIG_dilithium_3_sign(sigBuffer, ref sigLen, message, (ulong)message.Length, _secretKey),
            SignatureAlgorithm.ML_DSA_87 => LibOQS.OQS_SIG_dilithium_5_sign(sigBuffer, ref sigLen, message, (ulong)message.Length, _secretKey),
            _ => throw new ArgumentOutOfRangeException(nameof(Algorithm), Algorithm, "Unsupported ML-DSA algorithm")
        };

        LibOQS.EnsureSuccess(status, $"OQS_SIG_{Algorithm}_sign");

        // Trim to actual signature length if liboqs used fewer bytes
        if (sigLen == (ulong)sigBuffer.Length)
        {
            return sigBuffer;
        }

        var signature = new byte[sigLen];
        Buffer.BlockCopy(sigBuffer, 0, signature, 0, (int)sigLen);
        return signature;
    }

    public async Task<byte[]> SignAsync(byte[] message, CancellationToken cancellationToken = default)
    {
        // For now, use synchronous implementation wrapped in Task to keep semantics simple.
        // liboqs itself is CPU-bound and thread-safe when used with separate key instances.
        return await Task.Run(() => Sign(message), cancellationToken).ConfigureAwait(false);
    }

    public bool Verify(byte[] message, byte[] signature, byte[] publicKey)
    {
        if (message is null) throw new ArgumentNullException(nameof(message));
        if (signature is null) throw new ArgumentNullException(nameof(signature));
        if (publicKey is null) throw new ArgumentNullException(nameof(publicKey));

        (var pkSize, _, _) = GetSizes(Algorithm);
        if (publicKey.Length != pkSize)
        {
            throw new CryptographicException($"Invalid ML-DSA public key length for verification: {publicKey.Length}, expected {pkSize}.");
        }

        ulong sigLen = (ulong)signature.Length;

        int status = Algorithm switch
        {
            SignatureAlgorithm.ML_DSA_44 => LibOQS.OQS_SIG_dilithium_2_verify(message, (ulong)message.Length, signature, sigLen, publicKey),
            SignatureAlgorithm.ML_DSA_65 => LibOQS.OQS_SIG_dilithium_3_verify(message, (ulong)message.Length, signature, sigLen, publicKey),
            SignatureAlgorithm.ML_DSA_87 => LibOQS.OQS_SIG_dilithium_5_verify(message, (ulong)message.Length, signature, sigLen, publicKey),
            _ => throw new ArgumentOutOfRangeException(nameof(Algorithm), Algorithm, "Unsupported ML-DSA algorithm")
        };

        // For verification we return bool instead of throwing; non-zero means verification failed.
        return status == 0;
    }

    public async Task<bool> VerifyAsync(byte[] message, byte[] signature, byte[] publicKey, CancellationToken cancellationToken = default)
    {
        return await Task.Run(() => Verify(message, signature, publicKey), cancellationToken).ConfigureAwait(false);
    }

    private static (int publicKeySize, int privateKeySize, int signatureSize) GetSizes(SignatureAlgorithm algorithm)
    {
        return algorithm switch
        {
            // Values aligned with liboqs constants (FIPS 204 / ML-DSA)
            SignatureAlgorithm.ML_DSA_44 => (1312, 2528, 2420),
            SignatureAlgorithm.ML_DSA_65 => (1952, 4000, 3293),
            SignatureAlgorithm.ML_DSA_87 => (2592, 4864, 4595),
            _ => throw new ArgumentOutOfRangeException(nameof(algorithm), algorithm, "Algorithm is not an ML-DSA variant")
        };
    }
}


