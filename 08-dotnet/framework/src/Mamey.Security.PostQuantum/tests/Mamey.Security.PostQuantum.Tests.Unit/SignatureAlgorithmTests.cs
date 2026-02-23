using FluentAssertions;
using Mamey.Security.PostQuantum;
using Xunit;

namespace Mamey.Security.PostQuantum.Tests.Unit;

public class SignatureAlgorithmTests
{
    [Fact]
    public void SignatureAlgorithm_ShouldContain_AllExpectedValues()
    {
        var values = Enum.GetValues(typeof(SignatureAlgorithm)).Cast<SignatureAlgorithm>().ToList();

        values.Should().Contain(SignatureAlgorithm.RSA_SHA256);
        values.Should().Contain(SignatureAlgorithm.ECDSA_SECP256K1);
        values.Should().Contain(SignatureAlgorithm.ED25519);
        values.Should().Contain(SignatureAlgorithm.ML_DSA_44);
        values.Should().Contain(SignatureAlgorithm.ML_DSA_65);
        values.Should().Contain(SignatureAlgorithm.ML_DSA_87);
        values.Should().Contain(SignatureAlgorithm.SLH_DSA_128F);
        values.Should().Contain(SignatureAlgorithm.SLH_DSA_128S);
        values.Should().Contain(SignatureAlgorithm.SLH_DSA_256F);
        values.Should().Contain(SignatureAlgorithm.HYBRID_RSA_MLDSA65);
        values.Should().Contain(SignatureAlgorithm.HYBRID_ECDSA_MLDSA65);
    }

    [Fact]
    public void KemAlgorithm_ShouldContain_AllExpectedValues()
    {
        var values = Enum.GetValues(typeof(KemAlgorithm)).Cast<KemAlgorithm>().ToList();

        values.Should().Contain(KemAlgorithm.RSA_OAEP);
        values.Should().Contain(KemAlgorithm.ECDH_P256);
        values.Should().Contain(KemAlgorithm.ML_KEM_512);
        values.Should().Contain(KemAlgorithm.ML_KEM_768);
        values.Should().Contain(KemAlgorithm.ML_KEM_1024);
        values.Should().Contain(KemAlgorithm.HYBRID_RSA_MLKEM768);
    }
}



