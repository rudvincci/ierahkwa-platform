using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Jobs;
using Mamey.Security;
using Mamey.Security.Tests.Shared.Fixtures;
using Mamey.Security.Tests.Shared.Helpers;
using Mamey.Security.Tests.Shared.Utilities;
using System.Security.Cryptography.X509Certificates;

namespace Mamey.Security.Tests.Performance.Benchmarks;

/// <summary>
/// Performance benchmarks for digital signature operations.
/// </summary>
[SimpleJob(RuntimeMoniker.Net90)]
[MemoryDiagnoser]
public class SignatureBenchmarks
{
    private readonly ISecurityProvider _securityProvider;
    private readonly ISigner _signer;
    private readonly X509Certificate2 _certificate;
    private readonly object _testObject;
    private readonly byte[] _testBytes;
    private readonly byte[] _largeBytes;

    public SignatureBenchmarks()
    {
        var fixture = new SecurityTestFixture();
        _securityProvider = fixture.SecurityProvider;
        _signer = fixture.Signer;
        _certificate = TestCertificates.CreateSelfSignedCertificate();
        _testObject = new { Name = "Test", Value = 123 };
        _testBytes = TestDataGenerator.GenerateRandomBytes(1000);
        _largeBytes = TestDataGenerator.GenerateRandomBytes(1024 * 1024); // 1MB
    }

    #region RSA Signing Benchmarks

    [Benchmark(Baseline = true)]
    public string Rsa_Sign_Object()
    {
        return _signer.Sign(_testObject, _certificate);
    }

    [Benchmark]
    public bool Rsa_Verify_Object()
    {
        var signature = _signer.Sign(_testObject, _certificate);
        return _signer.Verify(_testObject, _certificate, signature);
    }

    [Benchmark]
    public string Rsa_Sign_Verify_RoundTrip()
    {
        var signature = _signer.Sign(_testObject, _certificate);
        _signer.Verify(_testObject, _certificate, signature);
        return signature;
    }

    [Benchmark]
    public byte[] Rsa_Sign_ByteArray()
    {
        return _signer.Sign(_testBytes, _certificate);
    }

    [Benchmark]
    public bool Rsa_Verify_ByteArray()
    {
        var signature = _signer.Sign(_testBytes, _certificate);
        return _signer.Verify(_testBytes, _certificate, signature);
    }

    [Benchmark]
    public byte[] Rsa_Sign_LargeData()
    {
        return _signer.Sign(_largeBytes, _certificate);
    }

    [Benchmark]
    public bool Rsa_Verify_LargeData()
    {
        var signature = _signer.Sign(_largeBytes, _certificate);
        return _signer.Verify(_largeBytes, _certificate, signature);
    }

    [Benchmark]
    [Arguments(10)]
    [Arguments(100)]
    [Arguments(1000)]
    public string Rsa_ConcurrentSigning(int iterations)
    {
        var signature = "";
        for (int i = 0; i < iterations; i++)
        {
            signature = _signer.Sign(_testObject, _certificate);
        }
        return signature;
    }

    #endregion

    #region SecurityProvider Benchmarks

    [Benchmark]
    public string SecurityProvider_Sign()
    {
        return _securityProvider.Sign(_testObject, _certificate);
    }

    [Benchmark]
    public bool SecurityProvider_Verify()
    {
        var signature = _securityProvider.Sign(_testObject, _certificate);
        return _securityProvider.Verify(_testObject, _certificate, signature);
    }

    [Benchmark]
    public string SecurityProvider_Sign_Verify_RoundTrip()
    {
        var signature = _securityProvider.Sign(_testObject, _certificate);
        _securityProvider.Verify(_testObject, _certificate, signature);
        return signature;
    }

    [Benchmark]
    public byte[] SecurityProvider_Sign_ByteArray()
    {
        return _securityProvider.Sign(_testBytes, _certificate);
    }

    [Benchmark]
    public bool SecurityProvider_Verify_ByteArray()
    {
        var signature = _securityProvider.Sign(_testBytes, _certificate);
        return _securityProvider.Verify(_testBytes, _certificate, signature);
    }

    #endregion
}



