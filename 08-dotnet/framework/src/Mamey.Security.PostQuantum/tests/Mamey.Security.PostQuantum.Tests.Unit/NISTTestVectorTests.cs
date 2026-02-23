using System;
using System.Text.Json;
using FluentAssertions;
using Xunit;

namespace Mamey.Security.PostQuantum.Tests.Unit;

/// <summary>
/// Skeleton tests for NIST ML-DSA / ML-KEM test vectors.
/// These are marked as skipped in this environment because they
/// require official vector files and a native liboqs runtime.
/// </summary>
public class NISTTestVectorTests
{
    [Fact(Skip = "Requires official NIST ML-DSA vectors and native liboqs runtime.")]
    public void MLDSA_Vectors_ShouldVerifyCorrectly()
    {
        const string sampleVector = "{\"tcId\":1,\"msg\":\"AA==\",\"pk\":\"AA==\",\"sk\":\"AA==\",\"sig\":\"AA==\"}";
        using var doc = JsonDocument.Parse(sampleVector);
        var root = doc.RootElement;

        root.GetProperty("tcId").GetInt32().Should().Be(1);
        root.GetProperty("msg").GetString().Should().NotBeNull();
    }

    [Fact(Skip = "Requires official NIST ML-KEM vectors and native liboqs runtime.")]
    public void MLKEM_Vectors_ShouldEncapsulateAndDecapsulate()
    {
        const string sampleVector = "{\"tcId\":1,\"pk\":\"AA==\",\"sk\":\"AA==\",\"ct\":\"AA==\",\"ss\":\"AA==\"}";
        using var doc = JsonDocument.Parse(sampleVector);
        var root = doc.RootElement;

        root.GetProperty("tcId").GetInt32().Should().Be(1);
        root.GetProperty("pk").GetString().Should().NotBeNull();
    }
}


