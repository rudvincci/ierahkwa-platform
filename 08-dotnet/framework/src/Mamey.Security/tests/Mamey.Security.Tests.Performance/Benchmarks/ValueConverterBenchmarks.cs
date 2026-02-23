using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Jobs;
using Mamey.Security;
using Mamey.Security.EntityFramework.ValueConverters;
using Mamey.Security.MongoDB.Serializers;
using Mamey.Security.Redis.Serializers;
using Mamey.Security.Tests.Shared.Fixtures;
using Mamey.Security.Tests.Shared.Helpers;
using MongoDB.Bson;
using MongoDB.Bson.IO;
using MongoDB.Bson.Serialization;
using Shouldly;
using StackExchange.Redis;
using Xunit;

namespace Mamey.Security.Tests.Performance.Benchmarks;

/// <summary>
/// Performance benchmarks for value converter operations.
/// </summary>
[SimpleJob(RuntimeMoniker.Net90)]
[MemoryDiagnoser]
public class ValueConverterBenchmarks
{
    private readonly ISecurityProvider _securityProvider;
    private readonly EncryptedValueConverter _efEncryptedConverter;
    private readonly HashedValueConverter _efHashedConverter;
    private readonly EncryptedStringSerializer _mongoEncryptedSerializer;
    private readonly HashedStringSerializer _mongoHashedSerializer;
    private readonly EncryptedRedisSerializer _redisEncryptedSerializer;
    private readonly HashedRedisSerializer _redisHashedSerializer;
    private readonly string _testData;
    private readonly string _encryptedData;
    private readonly string _hashedData;

    public ValueConverterBenchmarks()
    {
        var fixture = new SecurityTestFixture();
        _securityProvider = fixture.SecurityProvider;
        _efEncryptedConverter = new EncryptedValueConverter(_securityProvider);
        _efHashedConverter = new HashedValueConverter(_securityProvider);
        _mongoEncryptedSerializer = new EncryptedStringSerializer(_securityProvider);
        _mongoHashedSerializer = new HashedStringSerializer(_securityProvider);
        _redisEncryptedSerializer = new EncryptedRedisSerializer(_securityProvider);
        _redisHashedSerializer = new HashedRedisSerializer(_securityProvider);
        _testData = "Test data for converter benchmark";
        _encryptedData = _securityProvider.Encrypt(_testData);
        _hashedData = _securityProvider.Hash(_testData);
    }

    #region EF Core Value Converter Benchmarks

    [Benchmark]
    public string EfCore_EncryptedValueConverter_ConvertToProvider()
    {
        return (string)_efEncryptedConverter.ConvertToProvider(_testData);
    }

    [Benchmark]
    public string EfCore_EncryptedValueConverter_ConvertFromProvider()
    {
        return (string)_efEncryptedConverter.ConvertFromProvider(_encryptedData);
    }

    [Benchmark(Baseline = true)]
    public string EfCore_EncryptedValueConverter_RoundTrip()
    {
        var encrypted = (string)_efEncryptedConverter.ConvertToProvider(_testData);
        return (string)_efEncryptedConverter.ConvertFromProvider(encrypted);
    }

    [Benchmark]
    public string EfCore_HashedValueConverter_ConvertToProvider()
    {
        return (string)_efHashedConverter.ConvertToProvider(_testData);
    }

    [Benchmark]
    public string EfCore_HashedValueConverter_ConvertFromProvider()
    {
        return (string)_efHashedConverter.ConvertFromProvider(_hashedData);
    }

    [Benchmark]
    [Arguments(10)]
    [Arguments(100)]
    [Arguments(1000)]
    public string EfCore_BulkOperations(int iterations)
    {
        var result = _testData;
        for (int i = 0; i < iterations; i++)
        {
            result = (string)_efEncryptedConverter.ConvertToProvider(result);
            result = (string)_efEncryptedConverter.ConvertFromProvider(result);
        }
        return result;
    }

    #endregion

    #region MongoDB Serializer Benchmarks

    [Benchmark]
    public void MongoDb_EncryptedStringSerializer_Serialize()
    {
        var stream = new MemoryStream();
        var writer = new BsonBinaryWriter(stream);
        var context = BsonSerializationContext.CreateRoot(writer);
        _mongoEncryptedSerializer.Serialize(context, new BsonSerializationArgs { NominalType = typeof(string) }, _testData);
    }

    [Benchmark]
    public string MongoDb_EncryptedStringSerializer_Deserialize()
    {
        var stream = new MemoryStream();
        var writer = new BsonBinaryWriter(stream);
        writer.WriteString(_encryptedData);
        writer.Flush();
        stream.Position = 0;
        var reader = new BsonBinaryReader(stream);
        var context = BsonDeserializationContext.CreateRoot(reader);
        return _mongoEncryptedSerializer.Deserialize(context, new BsonDeserializationArgs { NominalType = typeof(string) });
    }

    [Benchmark]
    public string MongoDb_EncryptedStringSerializer_RoundTrip()
    {
        var stream = new MemoryStream();
        var writer = new BsonBinaryWriter(stream);
        var serializationContext = BsonSerializationContext.CreateRoot(writer);
        _mongoEncryptedSerializer.Serialize(serializationContext, new BsonSerializationArgs { NominalType = typeof(string) }, _testData);
        writer.Flush();
        stream.Position = 0;
        var reader = new BsonBinaryReader(stream);
        var deserializationContext = BsonDeserializationContext.CreateRoot(reader);
        return _mongoEncryptedSerializer.Deserialize(deserializationContext, new BsonDeserializationArgs { NominalType = typeof(string) });
    }

    [Benchmark]
    public void MongoDb_HashedStringSerializer_Serialize()
    {
        var stream = new MemoryStream();
        var writer = new BsonBinaryWriter(stream);
        var context = BsonSerializationContext.CreateRoot(writer);
        _mongoHashedSerializer.Serialize(context, new BsonSerializationArgs { NominalType = typeof(string) }, _testData);
    }

    [Benchmark]
    public string MongoDb_HashedStringSerializer_Deserialize()
    {
        var stream = new MemoryStream();
        var writer = new BsonBinaryWriter(stream);
        writer.WriteString(_hashedData);
        writer.Flush();
        stream.Position = 0;
        var reader = new BsonBinaryReader(stream);
        var context = BsonDeserializationContext.CreateRoot(reader);
        return _mongoHashedSerializer.Deserialize(context, new BsonDeserializationArgs { NominalType = typeof(string) });
    }

    #endregion

    #region Redis Serializer Benchmarks

    [Benchmark]
    public RedisValue Redis_EncryptedRedisSerializer_Serialize()
    {
        return _redisEncryptedSerializer.Serialize(_testData);
    }

    [Benchmark]
    public string Redis_EncryptedRedisSerializer_Deserialize()
    {
        var redisValue = _redisEncryptedSerializer.Serialize(_testData);
        return _redisEncryptedSerializer.Deserialize<string>(redisValue)!;
    }

    [Benchmark]
    public string Redis_EncryptedRedisSerializer_RoundTrip()
    {
        var redisValue = _redisEncryptedSerializer.Serialize(_testData);
        return _redisEncryptedSerializer.Deserialize<string>(redisValue)!;
    }

    [Benchmark]
    public RedisValue Redis_HashedRedisSerializer_Serialize()
    {
        return _redisHashedSerializer.Serialize(_testData);
    }

    [Benchmark]
    public string Redis_HashedRedisSerializer_Deserialize()
    {
        var redisValue = _redisHashedSerializer.Serialize(_testData);
        return _redisHashedSerializer.Deserialize<string>(redisValue)!;
    }

    [Benchmark]
    [Arguments(10)]
    [Arguments(100)]
    [Arguments(1000)]
    public string Redis_BulkOperations(int iterations)
    {
        var result = _testData;
        for (int i = 0; i < iterations; i++)
        {
            var redisValue = _redisEncryptedSerializer.Serialize(result);
            result = _redisEncryptedSerializer.Deserialize<string>(redisValue)!;
        }
        return result;
    }

    #endregion
}



