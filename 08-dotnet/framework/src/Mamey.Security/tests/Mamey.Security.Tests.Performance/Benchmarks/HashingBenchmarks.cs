using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Jobs;
using Mamey.Security;
using Mamey.Security.Tests.Shared.Fixtures;
using Mamey.Security.Tests.Shared.Helpers;

namespace Mamey.Security.Tests.Performance.Benchmarks;

/// <summary>
/// Performance benchmarks for hashing operations.
/// </summary>
[SimpleJob(RuntimeMoniker.Net90)]
[MemoryDiagnoser]
public class HashingBenchmarks
{
    private readonly ISecurityProvider _securityProvider;
    private readonly IHasher _hasher;
    private readonly IMd5 _md5;
    private readonly string _testData;
    private readonly string _largeData;
    private readonly byte[] _testBytes;
    private readonly byte[] _largeBytes;

    public HashingBenchmarks()
    {
        var fixture = new SecurityTestFixture();
        _securityProvider = fixture.SecurityProvider;
        _hasher = fixture.Hasher;
        _md5 = fixture.Md5;
        _testData = "Test data for hashing benchmark";
        _largeData = TestDataGenerator.GenerateLargeString();
        _testBytes = TestDataGenerator.GenerateRandomBytes(1000);
        _largeBytes = TestDataGenerator.GenerateRandomBytes(1024 * 1024); // 1MB
    }

    #region SHA-512 Benchmarks

    [Benchmark(Baseline = true)]
    public string Sha512_Hash_String()
    {
        return _hasher.Hash(_testData);
    }

    [Benchmark]
    public string Sha512_Hash_LargeData()
    {
        return _hasher.Hash(_largeData);
    }

    [Benchmark]
    public byte[] Sha512_Hash_ByteArray()
    {
        return _hasher.Hash(_testBytes);
    }

    [Benchmark]
    public byte[] Sha512_Hash_LargeByteArray()
    {
        return _hasher.Hash(_largeBytes);
    }

    [Benchmark]
    public byte[] Sha512_HashToBytes()
    {
        return _hasher.HashToBytes(_testData);
    }

    [Benchmark]
    [Arguments(10)]
    [Arguments(100)]
    [Arguments(1000)]
    public string Sha512_ConcurrentHashing(int iterations)
    {
        var result = _testData;
        for (int i = 0; i < iterations; i++)
        {
            result = _hasher.Hash(result);
        }
        return result;
    }

    #endregion

    #region MD5 Benchmarks

    [Benchmark]
    public string Md5_Calculate_String()
    {
        return _md5.Calculate(_testData);
    }

    [Benchmark]
    public string Md5_Calculate_LargeData()
    {
        return _md5.Calculate(_largeData);
    }

    [Benchmark]
    public string Md5_Calculate_Stream()
    {
        var stream = new MemoryStream(_testBytes);
        return _md5.Calculate(stream);
    }

    [Benchmark]
    public string Md5_Calculate_LargeStream()
    {
        var stream = new MemoryStream(_largeBytes);
        return _md5.Calculate(stream);
    }

    [Benchmark]
    [Arguments(10)]
    [Arguments(100)]
    [Arguments(1000)]
    public string Md5_ConcurrentHashing(int iterations)
    {
        var result = _testData;
        for (int i = 0; i < iterations; i++)
        {
            result = _md5.Calculate(result);
        }
        return result;
    }

    #endregion

    #region SecurityProvider Benchmarks

    [Benchmark]
    public string SecurityProvider_Hash()
    {
        return _securityProvider.Hash(_testData);
    }

    [Benchmark]
    public byte[] SecurityProvider_Hash_ByteArray()
    {
        return _securityProvider.Hash(_testBytes);
    }

    [Benchmark]
    public string SecurityProvider_CalculateMd5()
    {
        return _securityProvider.CalculateMd5(_testData);
    }

    #endregion
}



