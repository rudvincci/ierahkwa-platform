using System.Threading;
using System.Threading.Tasks;
using Mamey.Security.PostQuantum.Models;

namespace Mamey.Security.PostQuantum.Interfaces;

/// <summary>
/// Generic post-quantum signer interface (ML-DSA / SLH-DSA).
/// Mirrors the Quantum Resistant MameyNode plan.
/// </summary>
public interface IPQSigner
{
    /// <summary>
    /// Gets the public key associated with this signer instance.
    /// </summary>
    byte[] PublicKey { get; }

    byte[] Sign(byte[] message);
    bool Verify(byte[] message, byte[] signature, byte[] publicKey);
    Task<byte[]> SignAsync(byte[] message, CancellationToken cancellationToken = default);
    Task<bool> VerifyAsync(byte[] message, byte[] signature, byte[] publicKey, CancellationToken cancellationToken = default);

    SignatureAlgorithm Algorithm { get; }
    int SignatureSize { get; }
    int PublicKeySize { get; }
    int PrivateKeySize { get; }
}

/// <summary>
/// Key generation interface for PQC algorithms (signatures and KEMs).
/// </summary>
public interface IPQKeyGenerator
{
    PQKeyPair GenerateKeyPair(SignatureAlgorithm algorithm);
    PQKeyPair GenerateKeyPair(KemAlgorithm algorithm);
    Task<PQKeyPair> GenerateKeyPairAsync(SignatureAlgorithm algorithm, CancellationToken cancellationToken = default);
    bool ValidateKeyPair(PQKeyPair keyPair);
}

/// <summary>
/// Hybrid crypto provider for transition mode (classical + PQ signatures/keys).
/// </summary>
public interface IHybridCryptoProvider
{
    HybridSignature SignHybrid(byte[] message, byte[] classicalPrivateKey, byte[] pqPrivateKey);
    bool VerifyHybrid(byte[] message, HybridSignature signature, byte[] classicalPublicKey, byte[] pqPublicKey);
    HybridKeyPair GenerateHybridKeyPair(SignatureAlgorithm hybridAlgorithm);
}

/// <summary>
/// Placeholder for PQ KEM-based encryptor interface (ML-KEM).
/// Detailed implementation is handled in follow-up tasks.
/// </summary>
public interface IPQEncryptor
{
    PQCiphertext Encapsulate(KemAlgorithm algorithm, byte[] recipientPublicKey, out byte[] sharedSecret);
    byte[] Decapsulate(KemAlgorithm algorithm, PQCiphertext ciphertext, byte[] recipientPrivateKey);
}



