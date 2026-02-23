using System.Security.Cryptography;
using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Diagnosers;
using Mamey.Security.PostQuantum.Algorithms.MLDSA;
using Mamey.Security.PostQuantum.Hybrid;
using Mamey.Security.PostQuantum.Models;

namespace Mamey.Security.PostQuantum.Tests.Performance;

/// <summary>
/// Benchmark suite comparing classical and post-quantum signature algorithms.
/// Run with: dotnet run -c Release -- --job short
/// </summary>
[MemoryDiagnoser]
[RankColumn]
public class SignatureBenchmarks
{
    private byte[] _message = null!;
    
    // Classical keys
    private RSA _rsa = null!;
    private ECDsa _ecdsa = null!;
    
    // ML-DSA signers
    private MLDSASigner _mlDsa44Signer = null!;
    private MLDSASigner _mlDsa65Signer = null!;
    private MLDSASigner _mlDsa87Signer = null!;
    
    // Hybrid signer
    private HybridSigner _hybridSigner = null!;
    private HybridKeyPair _hybridKeyPair = null!;
    
    // Pre-computed signatures for verification benchmarks
    private byte[] _rsaSignature = null!;
    private byte[] _ecdsaSignature = null!;
    private byte[] _mlDsa65Signature = null!;
    private HybridSignature _hybridSignature = null!;

    [GlobalSetup]
    public void Setup()
    {
        // Create a representative message (1KB)
        _message = new byte[1024];
        RandomNumberGenerator.Fill(_message);
        
        // Initialize classical keys
        _rsa = RSA.Create(2048);
        _ecdsa = ECDsa.Create(ECCurve.NamedCurves.nistP256);
        
        // Initialize ML-DSA signers
        var mlDsa44KeyPair = MLDSASigner.GenerateKeyPair(SignatureAlgorithm.ML_DSA_44);
        var mlDsa65KeyPair = MLDSASigner.GenerateKeyPair(SignatureAlgorithm.ML_DSA_65);
        var mlDsa87KeyPair = MLDSASigner.GenerateKeyPair(SignatureAlgorithm.ML_DSA_87);
        
        _mlDsa44Signer = new MLDSASigner(SignatureAlgorithm.ML_DSA_44, mlDsa44KeyPair);
        _mlDsa65Signer = new MLDSASigner(SignatureAlgorithm.ML_DSA_65, mlDsa65KeyPair);
        _mlDsa87Signer = new MLDSASigner(SignatureAlgorithm.ML_DSA_87, mlDsa87KeyPair);
        
        // Initialize hybrid signer
        _hybridSigner = new HybridSigner(SignatureAlgorithm.HYBRID_ECDSA_MLDSA65);
        _hybridKeyPair = _hybridSigner.GenerateHybridKeyPair(SignatureAlgorithm.HYBRID_ECDSA_MLDSA65);
        
        // Pre-compute signatures for verification benchmarks
        _rsaSignature = _rsa.SignData(_message, HashAlgorithmName.SHA256, RSASignaturePadding.Pkcs1);
        _ecdsaSignature = _ecdsa.SignData(_message, HashAlgorithmName.SHA256);
        _mlDsa65Signature = _mlDsa65Signer.Sign(_message);
        _hybridSignature = _hybridSigner.SignHybrid(_message, _hybridKeyPair.ClassicalPrivateKey, _hybridKeyPair.PostQuantumPrivateKey);
    }

    [GlobalCleanup]
    public void Cleanup()
    {
        _rsa?.Dispose();
        _ecdsa?.Dispose();
    }

    // ==================== SIGNING BENCHMARKS ====================

    [Benchmark(Description = "RSA-2048 Sign")]
    public byte[] RSA2048_Sign()
    {
        return _rsa.SignData(_message, HashAlgorithmName.SHA256, RSASignaturePadding.Pkcs1);
    }

    [Benchmark(Description = "ECDSA P-256 Sign")]
    public byte[] ECDSA_Sign()
    {
        return _ecdsa.SignData(_message, HashAlgorithmName.SHA256);
    }

    [Benchmark(Description = "ML-DSA-44 Sign")]
    public byte[] MLDSA44_Sign()
    {
        return _mlDsa44Signer.Sign(_message);
    }

    [Benchmark(Description = "ML-DSA-65 Sign")]
    public byte[] MLDSA65_Sign()
    {
        return _mlDsa65Signer.Sign(_message);
    }

    [Benchmark(Description = "ML-DSA-87 Sign")]
    public byte[] MLDSA87_Sign()
    {
        return _mlDsa87Signer.Sign(_message);
    }

    [Benchmark(Description = "Hybrid ECDSA+ML-DSA-65 Sign")]
    public HybridSignature Hybrid_ECDSA_MLDSA65_Sign()
    {
        return _hybridSigner.SignHybrid(_message, _hybridKeyPair.ClassicalPrivateKey, _hybridKeyPair.PostQuantumPrivateKey);
    }

    // ==================== VERIFICATION BENCHMARKS ====================

    [Benchmark(Description = "RSA-2048 Verify")]
    public bool RSA2048_Verify()
    {
        return _rsa.VerifyData(_message, _rsaSignature, HashAlgorithmName.SHA256, RSASignaturePadding.Pkcs1);
    }

    [Benchmark(Description = "ECDSA P-256 Verify")]
    public bool ECDSA_Verify()
    {
        return _ecdsa.VerifyData(_message, _ecdsaSignature, HashAlgorithmName.SHA256);
    }

    [Benchmark(Description = "ML-DSA-65 Verify")]
    public bool MLDSA65_Verify()
    {
        return _mlDsa65Signer.Verify(_message, _mlDsa65Signature, _mlDsa65Signer.PublicKey);
    }

    [Benchmark(Description = "Hybrid ECDSA+ML-DSA-65 Verify")]
    public bool Hybrid_ECDSA_MLDSA65_Verify()
    {
        return _hybridSigner.VerifyHybrid(_message, _hybridSignature, _hybridKeyPair.ClassicalPublicKey, _hybridKeyPair.PostQuantumPrivateKey);
    }
}


