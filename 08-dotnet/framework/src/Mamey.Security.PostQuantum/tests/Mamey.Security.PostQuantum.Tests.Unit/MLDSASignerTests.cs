using System;
using System.Security.Cryptography;
using System.Threading.Tasks;
using FluentAssertions;
using Mamey.Security.PostQuantum.Algorithms.MLDSA;
using Mamey.Security.PostQuantum.Models;
using Xunit;

namespace Mamey.Security.PostQuantum.Tests.Unit;

public class MLDSASignerTests
{
    [Theory]
    [InlineData(SignatureAlgorithm.ML_DSA_44, 1312, 2528, 2420)]
    [InlineData(SignatureAlgorithm.ML_DSA_65, 1952, 4000, 3293)]
    [InlineData(SignatureAlgorithm.ML_DSA_87, 2592, 4864, 4595)]
    public void Ctor_ShouldSetSizes_FromAlgorithm(
        SignatureAlgorithm algorithm,
        int expectedPk,
        int expectedSk,
        int expectedSig)
    {
        var pk = new byte[expectedPk];
        var sk = new byte[expectedSk];
        var keyPair = new PQKeyPair(pk, sk);

        var signer = new MLDSASigner(algorithm, keyPair);

        signer.Algorithm.Should().Be(algorithm);
        signer.PublicKeySize.Should().Be(expectedPk);
        signer.PrivateKeySize.Should().Be(expectedSk);
        signer.SignatureSize.Should().Be(expectedSig);
    }

    [Fact]
    public void Ctor_ShouldThrow_ForUnsupportedAlgorithm()
    {
        var pk = new byte[16];
        var sk = new byte[16];
        var keyPair = new PQKeyPair(pk, sk);

        Action act = () => _ = new MLDSASigner(SignatureAlgorithm.RSA_SHA256, keyPair);
        act.Should().Throw<ArgumentOutOfRangeException>();
    }

    [Fact]
    public void Sign_ShouldThrow_WhenMessageIsNull()
    {
        var keyPair = new PQKeyPair(new byte[1312], new byte[2528]);
        var signer = new MLDSASigner(SignatureAlgorithm.ML_DSA_44, keyPair);

        Action act = () => signer.Sign(null!);
        act.Should().Throw<ArgumentNullException>();
    }

    [Fact]
    public void Verify_ShouldThrow_WhenArgumentsAreNull()
    {
        var keyPair = new PQKeyPair(new byte[1312], new byte[2528]);
        var signer = new MLDSASigner(SignatureAlgorithm.ML_DSA_44, keyPair);

        Action act1 = () => signer.Verify(null!, Array.Empty<byte>(), new byte[1312]);
        Action act2 = () => signer.Verify(Array.Empty<byte>(), null!, new byte[1312]);
        Action act3 = () => signer.Verify(Array.Empty<byte>(), Array.Empty<byte>(), null!);

        act1.Should().Throw<ArgumentNullException>();
        act2.Should().Throw<ArgumentNullException>();
        act3.Should().Throw<ArgumentNullException>();
    }

    [Fact]
    public void Verify_ShouldThrow_WhenPublicKeyLengthInvalid()
    {
        var keyPair = new PQKeyPair(new byte[1312], new byte[2528]);
        var signer = new MLDSASigner(SignatureAlgorithm.ML_DSA_44, keyPair);

        var invalidPk = new byte[1311];
        Action act = () => signer.Verify(Array.Empty<byte>(), Array.Empty<byte>(), invalidPk);
        act.Should().Throw<CryptographicException>();
    }

    [Fact(Skip = "Requires native liboqs runtime for key generation and sign/verify.")]
    public async Task Sign_And_Verify_ShouldRoundtrip_ForAllVariants()
    {
        foreach (var algorithm in new[]
                 {
                     SignatureAlgorithm.ML_DSA_44,
                     SignatureAlgorithm.ML_DSA_65,
                     SignatureAlgorithm.ML_DSA_87
                 })
        {
            var keyPair = MLDSASigner.GenerateKeyPair(algorithm);
            var signer = new MLDSASigner(algorithm, keyPair);
            var message = new byte[] { 1, 2, 3, 4, 5 };

            var signature = await signer.SignAsync(message);
            signature.Length.Should().BeGreaterThan(0);

            var verified = await signer.VerifyAsync(message, signature, signer.PublicKey);
            verified.Should().BeTrue();
        }
    }
}

