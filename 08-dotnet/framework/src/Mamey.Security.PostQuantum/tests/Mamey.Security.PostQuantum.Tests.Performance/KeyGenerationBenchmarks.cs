using System.Security.Cryptography;
using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Diagnosers;
using Mamey.Security.PostQuantum.Algorithms.MLDSA;
using Mamey.Security.PostQuantum.Hybrid;
using Mamey.Security.PostQuantum.Models;

namespace Mamey.Security.PostQuantum.Tests.Performance;

/// <summary>
/// Benchmark suite for key generation operations across classical and PQC algorithms.
/// Run with: dotnet run -c Release -- --job short
/// </summary>
[MemoryDiagnoser]
[RankColumn]
public class KeyGenerationBenchmarks
{
    // ==================== CLASSICAL KEY GENERATION ====================

    [Benchmark(Description = "RSA-2048 KeyPair")]
    public (byte[] pub, byte[] priv) RSA2048_KeyPair()
    {
        using var rsa = RSA.Create(2048);
        return (rsa.ExportSubjectPublicKeyInfo(), rsa.ExportPkcs8PrivateKey());
    }

    [Benchmark(Description = "RSA-4096 KeyPair")]
    public (byte[] pub, byte[] priv) RSA4096_KeyPair()
    {
        using var rsa = RSA.Create(4096);
        return (rsa.ExportSubjectPublicKeyInfo(), rsa.ExportPkcs8PrivateKey());
    }

    [Benchmark(Description = "ECDSA P-256 KeyPair")]
    public (byte[] pub, byte[] priv) ECDSA_P256_KeyPair()
    {
        using var ecdsa = ECDsa.Create(ECCurve.NamedCurves.nistP256);
        return (ecdsa.ExportSubjectPublicKeyInfo(), ecdsa.ExportPkcs8PrivateKey());
    }

    [Benchmark(Description = "ECDSA P-384 KeyPair")]
    public (byte[] pub, byte[] priv) ECDSA_P384_KeyPair()
    {
        using var ecdsa = ECDsa.Create(ECCurve.NamedCurves.nistP384);
        return (ecdsa.ExportSubjectPublicKeyInfo(), ecdsa.ExportPkcs8PrivateKey());
    }

    [Benchmark(Description = "Ed25519 KeyPair")]
    public (byte[] pub, byte[] priv) Ed25519_KeyPair()
    {
        using var ed25519 = DSA.Create(); // .NET 9 has Ed25519 support
        return (ed25519.ExportSubjectPublicKeyInfo(), ed25519.ExportPkcs8PrivateKey());
    }

    // ==================== ML-DSA KEY GENERATION ====================

    [Benchmark(Description = "ML-DSA-44 KeyPair")]
    public PQKeyPair MLDSA44_KeyPair()
    {
        return MLDSASigner.GenerateKeyPair(SignatureAlgorithm.ML_DSA_44);
    }

    [Benchmark(Description = "ML-DSA-65 KeyPair")]
    public PQKeyPair MLDSA65_KeyPair()
    {
        return MLDSASigner.GenerateKeyPair(SignatureAlgorithm.ML_DSA_65);
    }

    [Benchmark(Description = "ML-DSA-87 KeyPair")]
    public PQKeyPair MLDSA87_KeyPair()
    {
        return MLDSASigner.GenerateKeyPair(SignatureAlgorithm.ML_DSA_87);
    }

    // ==================== HYBRID KEY GENERATION ====================

    [Benchmark(Description = "Hybrid RSA+ML-DSA-65 KeyPair")]
    public HybridKeyPair Hybrid_RSA_MLDSA65_KeyPair()
    {
        var hybridSigner = new HybridSigner(SignatureAlgorithm.HYBRID_RSA_MLDSA65);
        return hybridSigner.GenerateHybridKeyPair(SignatureAlgorithm.HYBRID_RSA_MLDSA65);
    }

    [Benchmark(Description = "Hybrid ECDSA+ML-DSA-65 KeyPair")]
    public HybridKeyPair Hybrid_ECDSA_MLDSA65_KeyPair()
    {
        var hybridSigner = new HybridSigner(SignatureAlgorithm.HYBRID_ECDSA_MLDSA65);
        return hybridSigner.GenerateHybridKeyPair(SignatureAlgorithm.HYBRID_ECDSA_MLDSA65);
    }
}


