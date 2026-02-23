using System.Security.Cryptography;
using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Diagnosers;
using Mamey.Security.PostQuantum.Algorithms.MLKEM;
using Mamey.Security.PostQuantum.Models;

namespace Mamey.Security.PostQuantum.Tests.Performance;

/// <summary>
/// Benchmark suite comparing classical and post-quantum key encapsulation mechanisms.
/// Run with: dotnet run -c Release -- --job short
/// </summary>
[MemoryDiagnoser]
[RankColumn]
public class EncryptionBenchmarks
{
    private byte[] _plaintext = null!;
    
    // ML-KEM encryptor
    private MLKEMEncryptor _mlKemEncryptor = null!;
    
    // ML-KEM key pairs
    private byte[] _mlKem512PublicKey = null!;
    private byte[] _mlKem512PrivateKey = null!;
    private byte[] _mlKem768PublicKey = null!;
    private byte[] _mlKem768PrivateKey = null!;
    private byte[] _mlKem1024PublicKey = null!;
    private byte[] _mlKem1024PrivateKey = null!;

    [GlobalSetup]
    public void Setup()
    {
        // Create a representative plaintext (256 bytes)
        _plaintext = new byte[256];
        RandomNumberGenerator.Fill(_plaintext);
        
        _mlKemEncryptor = new MLKEMEncryptor();
        
        // Generate ML-KEM key pairs (simulated since we need liboqs)
        // In a real benchmark with liboqs, these would be generated via the library
        // For now, we create placeholder keys of correct size
        _mlKem512PublicKey = new byte[800];
        _mlKem512PrivateKey = new byte[1632];
        _mlKem768PublicKey = new byte[1184];
        _mlKem768PrivateKey = new byte[2400];
        _mlKem1024PublicKey = new byte[1568];
        _mlKem1024PrivateKey = new byte[3168];
        
        RandomNumberGenerator.Fill(_mlKem512PublicKey);
        RandomNumberGenerator.Fill(_mlKem512PrivateKey);
        RandomNumberGenerator.Fill(_mlKem768PublicKey);
        RandomNumberGenerator.Fill(_mlKem768PrivateKey);
        RandomNumberGenerator.Fill(_mlKem1024PublicKey);
        RandomNumberGenerator.Fill(_mlKem1024PrivateKey);
    }

    // ==================== KEY GENERATION BENCHMARKS ====================

    [Benchmark(Description = "RSA-2048 KeyGen")]
    public RSA RSA2048_KeyGen()
    {
        return RSA.Create(2048);
    }

    [Benchmark(Description = "ECDH P-256 KeyGen")]
    public ECDiffieHellman ECDH_KeyGen()
    {
        return ECDiffieHellman.Create(ECCurve.NamedCurves.nistP256);
    }

    [Benchmark(Description = "AES-256 KeyGen")]
    public byte[] AES256_KeyGen()
    {
        var key = new byte[32];
        RandomNumberGenerator.Fill(key);
        return key;
    }

    // ==================== AES ENCRYPTION BENCHMARKS ====================

    [Benchmark(Description = "AES-256-GCM Encrypt")]
    public (byte[] ciphertext, byte[] nonce, byte[] tag) AES256GCM_Encrypt()
    {
        var key = new byte[32];
        RandomNumberGenerator.Fill(key);
        
        var nonce = new byte[12];
        RandomNumberGenerator.Fill(nonce);
        
        var tag = new byte[16];
        var ciphertext = new byte[_plaintext.Length];
        
        using var aes = new AesGcm(key, 16);
        aes.Encrypt(nonce, _plaintext, ciphertext, tag);
        
        return (ciphertext, nonce, tag);
    }

    [Benchmark(Description = "AES-256-GCM Decrypt")]
    public byte[] AES256GCM_Decrypt()
    {
        var key = new byte[32];
        RandomNumberGenerator.Fill(key);
        
        var nonce = new byte[12];
        RandomNumberGenerator.Fill(nonce);
        
        var tag = new byte[16];
        var ciphertext = new byte[_plaintext.Length];
        
        using var aesEnc = new AesGcm(key, 16);
        aesEnc.Encrypt(nonce, _plaintext, ciphertext, tag);
        
        var plaintext = new byte[ciphertext.Length];
        using var aesDec = new AesGcm(key, 16);
        aesDec.Decrypt(nonce, ciphertext, tag, plaintext);
        
        return plaintext;
    }

    // ==================== SIZE COMPARISON (not timing) ====================

    [Benchmark(Description = "ML-KEM-512 Key Sizes")]
    public (int pk, int sk, int ct) MLKEM512_Sizes()
    {
        return (800, 1632, 768); // Public key, private key, ciphertext
    }

    [Benchmark(Description = "ML-KEM-768 Key Sizes")]
    public (int pk, int sk, int ct) MLKEM768_Sizes()
    {
        return (1184, 2400, 1088);
    }

    [Benchmark(Description = "ML-KEM-1024 Key Sizes")]
    public (int pk, int sk, int ct) MLKEM1024_Sizes()
    {
        return (1568, 3168, 1568);
    }
}


