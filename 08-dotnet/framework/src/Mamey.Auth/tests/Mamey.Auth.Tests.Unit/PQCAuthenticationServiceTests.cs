using System;
using System.Collections;
using System.Collections.Generic;
using FluentAssertions;
using Mamey.Auth;
using Mamey.Security.PostQuantum;
using Mamey.Security.PostQuantum.Interfaces;
using Mamey.Security.PostQuantum.Models;
using Xunit;

namespace Mamey.Auth.Tests.Unit;

public class PQCAuthenticationServiceTests
{
    private sealed class FakePqSigner : IPQSigner
    {
        public SignatureAlgorithm Algorithm => SignatureAlgorithm.ML_DSA_65;

        public int SignatureSize => 32;
        public int PublicKeySize => 32;
        public int PrivateKeySize => 32;

        // Deterministic "signature" = SHA256(data) for testing purposes.
        public byte[] PublicKey { get; } = new byte[32];

        public byte[] Sign(byte[] data)
        {
            using var sha = System.Security.Cryptography.SHA256.Create();
            return sha.ComputeHash(data);
        }

        public bool Verify(byte[] data, byte[] signature, byte[] publicKey)
        {
            using var sha = System.Security.Cryptography.SHA256.Create();
            var expected = sha.ComputeHash(data);
            return StructuralComparisons.StructuralEqualityComparer.Equals(expected, signature);
        }

        public Task<byte[]> SignAsync(byte[] message, CancellationToken cancellationToken = default)
        {
            return Task.FromResult(Sign(message));
        }

        public Task<bool> VerifyAsync(byte[] message, byte[] signature, byte[] publicKey, CancellationToken cancellationToken = default)
        {
            return Task.FromResult(Verify(message, signature, publicKey));
        }
    }

    private static JwtOptions CreateDefaultOptions(bool usePqc = false, bool useHybrid = false)
        => new()
        {
            Secret = "test-secret",
            Issuer = "mamey-test",
            Audience = "mamey-clients",
            ExpirationMinutes = 5,
            UsePostQuantumSigning = usePqc,
            UseHybridSigning = useHybrid,
            SignatureAlgorithm = SignatureAlgorithm.ML_DSA_65
        };

    [Fact]
    public void Classical_Jwt_Generation_And_Validation_Works()
    {
        var options = CreateDefaultOptions(usePqc: false, useHybrid: false);
        var service = new PQCAuthenticationService(options, new FakePqSigner());

        var token = service.GenerateToken("user-1", "admin", new Dictionary<string, string>
        {
            ["custom"] = "value"
        });

        var tokenString = token.ToTokenString();
        tokenString.Split('.').Length.Should().Be(3); // header.payload.classicalSig

        var valid = service.TryValidateToken(tokenString, out var payload);
        valid.Should().BeTrue();
        payload.Should().NotBeNull();
        payload!.Subject.Should().Be("user-1");
        payload.Role.Should().Be("admin");
        payload.Claims.Should().ContainKey("custom");
    }

    [Fact]
    public void Pqc_Jwt_Generation_And_Validation_Works()
    {
        var options = CreateDefaultOptions(usePqc: true, useHybrid: false);
        var service = new PQCAuthenticationService(options, new FakePqSigner());

        var token = service.GenerateToken("user-2", "user");
        var tokenString = token.ToTokenString();

        var segments = tokenString.Split('.');
        segments.Length.Should().Be(4); // header.payload.classicalSig.pqSig

        var headerJson = PostQuantumJwtToken.Base64UrlDecodeToString(segments[0]);
        headerJson.Should().Contain("\"alg\":\"ML-DSA-65\"");

        var valid = service.TryValidateToken(tokenString, out var payload);
        valid.Should().BeTrue();
        payload.Should().NotBeNull();
        payload!.Subject.Should().Be("user-2");
    }

    [Fact]
    public void Hybrid_Jwt_Generation_And_Validation_Works()
    {
        var options = CreateDefaultOptions(usePqc: true, useHybrid: true);
        var service = new PQCAuthenticationService(options, new FakePqSigner());

        var token = service.GenerateToken("user-3", "hybrid-role");
        var tokenString = token.ToTokenString();

        var segments = tokenString.Split('.');
        segments.Length.Should().Be(4); // header.payload.classicalSig.pqSig

        var headerJson = PostQuantumJwtToken.Base64UrlDecodeToString(segments[0]);
        headerJson.Should().Contain("\"alg\":\"HYBRID-RSA-MLDSA65\"");

        var valid = service.TryValidateToken(tokenString, out var payload);
        valid.Should().BeTrue();
        payload.Should().NotBeNull();
        payload!.Subject.Should().Be("user-3");
        payload.Role.Should().Be("hybrid-role");
    }
}


