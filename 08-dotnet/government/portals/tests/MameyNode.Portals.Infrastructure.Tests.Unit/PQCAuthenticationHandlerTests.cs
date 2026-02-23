using System;
using System.Collections.Generic;
using FluentAssertions;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging.Abstractions;
using MameyNode.Portals.Infrastructure.Authentication;
using Xunit;

namespace MameyNode.Portals.Infrastructure.Tests.Unit;

public class PQCAuthenticationHandlerTests
{
    private static IConfiguration BuildConfiguration(IDictionary<string, string?> values)
        => new ConfigurationBuilder().AddInMemoryCollection(values).Build();

    [Fact]
    public void InvalidAlgorithm_ThrowsArgumentException()
    {
        var cfg = BuildConfiguration(new Dictionary<string, string?>
        {
            ["multiAuth:postQuantum:enabled"] = "true",
            ["multiAuth:postQuantum:defaultAlgorithm"] = "INVALID-ALG"
        });

        Action act = () => _ = new PQCAuthenticationHandler(cfg, NullLogger<PQCAuthenticationHandler>.Instance);
        act.Should().Throw<ArgumentException>();
    }

    [Fact]
    public void InvalidMandatoryDate_ThrowsFormatException()
    {
        var cfg = BuildConfiguration(new Dictionary<string, string?>
        {
            ["multiAuth:postQuantum:enabled"] = "true",
            ["multiAuth:postQuantum:mandatoryDate"] = "not-a-date"
        });

        Action act = () => _ = new PQCAuthenticationHandler(cfg, NullLogger<PQCAuthenticationHandler>.Instance);
        act.Should().Throw<FormatException>();
    }

    private static string CreateToken(string alg, bool pqc)
    {
        // Minimal header/payload for testing policy decisions only.
        string HeaderJson = $"{{\"alg\":\"{alg}\",\"typ\":\"JWT\"}}";
        string PayloadJson = "{\"sub\":\"user\",\"exp\":4102444800}"; // 2100-01-01

        static string B64Url(string s)
        {
            var bytes = System.Text.Encoding.UTF8.GetBytes(s);
            var base64 = Convert.ToBase64String(bytes).TrimEnd('=');
            return base64.Replace('+', '-').Replace('/', '_');
        }

        var header = B64Url(HeaderJson);
        var payload = B64Url(PayloadJson);
        var classicalSig = "c2ln"; // "sig" base64url
        var pqSig = pqc ? "cHFj" : null; // "pqc"

        return pqc
            ? string.Join('.', header, payload, classicalSig, pqSig)
            : string.Join('.', header, payload, classicalSig);
    }

    [Fact]
    public void BeforeMandatoryDate_ClassicalAndPqcTokensAreAccepted()
    {
        var cfg = BuildConfiguration(new Dictionary<string, string?>
        {
            ["multiAuth:postQuantum:enabled"] = "true",
            ["multiAuth:postQuantum:requirePQC"] = "true",
            ["multiAuth:postQuantum:mandatoryDate"] = "2030-01-01T00:00:00Z",
            ["multiAuth:postQuantum:defaultAlgorithm"] = "ML-DSA-65"
        });

        var handler = new PQCAuthenticationHandler(cfg, NullLogger<PQCAuthenticationHandler>.Instance);
        var now = new DateTimeOffset(2029, 1, 1, 0, 0, 0, TimeSpan.Zero);

        var classicalToken = CreateToken("HS256", pqc: false);
        var classicalAllowed = handler.IsTokenAllowed(classicalToken, now, out var isPqc1, out var isHybrid1);

        classicalAllowed.Should().BeTrue();
        isPqc1.Should().BeFalse();
        isHybrid1.Should().BeFalse();

        var pqcToken = CreateToken("ML-DSA-65", pqc: true);
        var pqcAllowed = handler.IsTokenAllowed(pqcToken, now, out var isPqc2, out var isHybrid2);

        pqcAllowed.Should().BeTrue();
        isPqc2.Should().BeTrue();
        isHybrid2.Should().BeFalse();
    }

    [Fact]
    public void AfterMandatoryDate_ClassicalRejected_PqcAccepted()
    {
        var cfg = BuildConfiguration(new Dictionary<string, string?>
        {
            ["multiAuth:postQuantum:enabled"] = "true",
            ["multiAuth:postQuantum:requirePQC"] = "true",
            ["multiAuth:postQuantum:mandatoryDate"] = "2030-01-01T00:00:00Z",
            ["multiAuth:postQuantum:defaultAlgorithm"] = "ML-DSA-65"
        });

        var handler = new PQCAuthenticationHandler(cfg, NullLogger<PQCAuthenticationHandler>.Instance);
        var now = new DateTimeOffset(2031, 1, 1, 0, 0, 0, TimeSpan.Zero);

        var classicalToken = CreateToken("HS256", pqc: false);
        var classicalAllowed = handler.IsTokenAllowed(classicalToken, now, out _, out _);
        classicalAllowed.Should().BeFalse();

        var pqcToken = CreateToken("ML-DSA-65", pqc: true);
        var pqcAllowed = handler.IsTokenAllowed(pqcToken, now, out var isPqc, out _);
        pqcAllowed.Should().BeTrue();
        isPqc.Should().BeTrue();
    }

    [Fact]
    public void HybridTokensHonourAcceptHybridFlag()
    {
        var cfg = BuildConfiguration(new Dictionary<string, string?>
        {
            ["multiAuth:postQuantum:enabled"] = "true",
            ["multiAuth:postQuantum:requirePQC"] = "true",
            ["multiAuth:postQuantum:mandatoryDate"] = "2030-01-01T00:00:00Z",
            ["multiAuth:postQuantum:defaultAlgorithm"] = "HYBRID-RSA-MLDSA65",
            ["multiAuth:postQuantum:acceptHybrid"] = "false"
        });

        var handler = new PQCAuthenticationHandler(cfg, NullLogger<PQCAuthenticationHandler>.Instance);
        var now = new DateTimeOffset(2031, 1, 1, 0, 0, 0, TimeSpan.Zero);

        var hybridToken = CreateToken("HYBRID-RSA-MLDSA65", pqc: true);
        var allowed = handler.IsTokenAllowed(hybridToken, now, out var isPqc, out var isHybrid);

        isPqc.Should().BeTrue();
        isHybrid.Should().BeTrue();
        allowed.Should().BeFalse();
    }
}


