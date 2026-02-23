using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Jobs;
using Mamey.Security;
using Mamey.Security.Tests.Shared.Fixtures;
using Mamey.Security.Tests.Shared.Helpers;
using Mamey.Security.Tests.Shared.Utilities;

namespace Mamey.Security.Tests.Performance.Benchmarks;

/// <summary>
/// Performance benchmarks for encryption operations.
/// </summary>
[SimpleJob(RuntimeMoniker.Net90)]
[MemoryDiagnoser]
public class EncryptionBenchmarks
{
    private readonly ISecurityProvider _securityProvider;
    private readonly IEncryptor _encryptor;
    private readonly string _testData;
    private readonly string _largeData;
    private readonly string _key;

    public EncryptionBenchmarks()
    {
        var fixture = new SecurityTestFixture();
        _securityProvider = fixture.SecurityProvider;
        _encryptor = fixture.Encryptor;
        _testData = "Test data for encryption benchmark";
        _largeData = TestDataGenerator.GenerateLargeString();
        _key = TestKeys.ValidAesKey;
    }

    #region AES-256 Benchmarks

    [Benchmark]
    public string Aes256_Encrypt_String()
    {
        return _encryptor.Encrypt(_testData, _key, EncryptionAlgorithms.AES);
    }

    [Benchmark]
    public string Aes256_Decrypt_String()
    {
        var encrypted = _encryptor.Encrypt(_testData, _key, EncryptionAlgorithms.AES);
        return _encryptor.Decrypt(encrypted, _key, EncryptionAlgorithms.AES);
    }

    [Benchmark]
    public string Aes256_Encrypt_LargeData()
    {
        return _encryptor.Encrypt(_largeData, _key, EncryptionAlgorithms.AES);
    }

    [Benchmark]
    public string Aes256_Decrypt_LargeData()
    {
        var encrypted = _encryptor.Encrypt(_largeData, _key, EncryptionAlgorithms.AES);
        return _encryptor.Decrypt(encrypted, _key, EncryptionAlgorithms.AES);
    }

    [Benchmark]
    public byte[] Aes256_Encrypt_ByteArray()
    {
        var data = TestDataGenerator.GenerateRandomBytes(1000);
        var iv = TestKeys.ValidAesIv;
        var key = TestKeys.ValidAesKeyBytes;
        return _encryptor.Encrypt(data, iv, key, EncryptionAlgorithms.AES);
    }

    [Benchmark]
    public byte[] Aes256_Decrypt_ByteArray()
    {
        var data = TestDataGenerator.GenerateRandomBytes(1000);
        var iv = TestKeys.ValidAesIv;
        var key = TestKeys.ValidAesKeyBytes;
        var encrypted = _encryptor.Encrypt(data, iv, key, EncryptionAlgorithms.AES);
        return _encryptor.Decrypt(encrypted, iv, key, EncryptionAlgorithms.AES);
    }

    [Benchmark(Baseline = true)]
    public string Aes256_RoundTrip()
    {
        var encrypted = _encryptor.Encrypt(_testData, _key, EncryptionAlgorithms.AES);
        return _encryptor.Decrypt(encrypted, _key, EncryptionAlgorithms.AES);
    }

    [Benchmark]
    [Arguments(10)]
    [Arguments(100)]
    [Arguments(1000)]
    public string Aes256_ConcurrentEncryption(int iterations)
    {
        var result = _testData;
        for (int i = 0; i < iterations; i++)
        {
            result = _encryptor.Encrypt(result, _key, EncryptionAlgorithms.AES);
        }
        return result;
    }

    #endregion

    #region TripleDES Benchmarks

    [Benchmark]
    public string TripleDes_Encrypt_String()
    {
        var key = TestKeys.ValidTripleDesKey;
        return _encryptor.Encrypt(_testData, key, EncryptionAlgorithms.TripleDes);
    }

    [Benchmark]
    public string TripleDes_Decrypt_String()
    {
        var key = TestKeys.ValidTripleDesKey;
        var encrypted = _encryptor.Encrypt(_testData, key, EncryptionAlgorithms.TripleDes);
        return _encryptor.Decrypt(encrypted, key, EncryptionAlgorithms.TripleDes);
    }

    [Benchmark]
    public string TripleDes_RoundTrip()
    {
        var key = TestKeys.ValidTripleDesKey;
        var encrypted = _encryptor.Encrypt(_testData, key, EncryptionAlgorithms.TripleDes);
        return _encryptor.Decrypt(encrypted, key, EncryptionAlgorithms.TripleDes);
    }

    [Benchmark]
    public byte[] TripleDes_Encrypt_ByteArray()
    {
        var data = TestDataGenerator.GenerateRandomBytes(1000);
        var iv = TestKeys.ValidTripleDesIv;
        var key = TestKeys.ValidTripleDesKeyBytes;
        return _encryptor.Encrypt(data, iv, key, EncryptionAlgorithms.TripleDes);
    }

    [Benchmark]
    public byte[] TripleDes_Decrypt_ByteArray()
    {
        var data = TestDataGenerator.GenerateRandomBytes(1000);
        var iv = TestKeys.ValidTripleDesIv;
        var key = TestKeys.ValidTripleDesKeyBytes;
        var encrypted = _encryptor.Encrypt(data, iv, key, EncryptionAlgorithms.TripleDes);
        return _encryptor.Decrypt(encrypted, iv, key, EncryptionAlgorithms.TripleDes);
    }

    #endregion

    #region RSA Benchmarks

    [Benchmark]
    public string Rsa_Encrypt_String()
    {
        using var rsa = System.Security.Cryptography.RSA.Create(2048);
        var key = rsa.ToXmlString(true);
        return _encryptor.Encrypt(_testData, key, EncryptionAlgorithms.RSA);
    }

    [Benchmark]
    public string Rsa_Decrypt_String()
    {
        using var rsa = System.Security.Cryptography.RSA.Create(2048);
        var key = rsa.ToXmlString(true);
        var encrypted = _encryptor.Encrypt(_testData, key, EncryptionAlgorithms.RSA);
        return _encryptor.Decrypt(encrypted, key, EncryptionAlgorithms.RSA);
    }

    [Benchmark]
    public string Rsa_RoundTrip()
    {
        using var rsa = System.Security.Cryptography.RSA.Create(2048);
        var key = rsa.ToXmlString(true);
        var encrypted = _encryptor.Encrypt(_testData, key, EncryptionAlgorithms.RSA);
        return _encryptor.Decrypt(encrypted, key, EncryptionAlgorithms.RSA);
    }

    [Benchmark]
    public byte[] Rsa_Encrypt_ByteArray()
    {
        using var rsa = System.Security.Cryptography.RSA.Create(2048);
        var publicKey = rsa.ExportRSAPublicKey();
        var privateKey = rsa.ExportRSAPrivateKey();
        var data = TestDataGenerator.GenerateRandomBytes(100);
        var iv = TestKeys.ValidAesIv;
        return _encryptor.Encrypt(data, iv, publicKey, EncryptionAlgorithms.RSA);
    }

    [Benchmark]
    public byte[] Rsa_Decrypt_ByteArray()
    {
        using var rsa = System.Security.Cryptography.RSA.Create(2048);
        var publicKey = rsa.ExportRSAPublicKey();
        var privateKey = rsa.ExportRSAPrivateKey();
        var data = TestDataGenerator.GenerateRandomBytes(100);
        var iv = TestKeys.ValidAesIv;
        var encrypted = _encryptor.Encrypt(data, iv, publicKey, EncryptionAlgorithms.RSA);
        return _encryptor.Decrypt(encrypted, iv, privateKey, EncryptionAlgorithms.RSA);
    }

    #endregion

    #region SecurityProvider Benchmarks

    [Benchmark]
    public string SecurityProvider_Encrypt()
    {
        return _securityProvider.Encrypt(_testData);
    }

    [Benchmark]
    public string SecurityProvider_Decrypt()
    {
        var encrypted = _securityProvider.Encrypt(_testData);
        return _securityProvider.Decrypt(encrypted);
    }

    [Benchmark]
    public string SecurityProvider_RoundTrip()
    {
        var encrypted = _securityProvider.Encrypt(_testData);
        return _securityProvider.Decrypt(encrypted);
    }

    #endregion
}



