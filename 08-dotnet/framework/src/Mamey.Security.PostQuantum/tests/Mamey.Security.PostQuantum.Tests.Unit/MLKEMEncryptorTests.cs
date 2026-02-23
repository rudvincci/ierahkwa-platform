using System;
using System.Security.Cryptography;
using FluentAssertions;
using Mamey.Security.PostQuantum;
using Mamey.Security.PostQuantum.Algorithms.MLKEM;
using Mamey.Security.PostQuantum.Models;
using Xunit;

namespace Mamey.Security.PostQuantum.Tests.Unit;

public class MLKEMEncryptorTests
{
    [Theory]
    [InlineData(KemAlgorithm.ML_KEM_512, 800)]
    [InlineData(KemAlgorithm.ML_KEM_768, 1184)]
    [InlineData(KemAlgorithm.ML_KEM_1024, 1568)]
    public void Encapsulate_ShouldThrow_ForInvalidPublicKeyLength(KemAlgorithm algorithm, int expectedPk)
    {
        var encryptor = new MLKEMEncryptor();
        var invalidPk = new byte[expectedPk + 1];

        Action act = () => encryptor.Encapsulate(algorithm, invalidPk, out _);

        act.Should().Throw<CryptographicException>();
    }

    [Theory]
    [InlineData(KemAlgorithm.ML_KEM_512, 1632, 768)]
    [InlineData(KemAlgorithm.ML_KEM_768, 2400, 1088)]
    [InlineData(KemAlgorithm.ML_KEM_1024, 3168, 1568)]
    public void Decapsulate_ShouldThrow_ForInvalidKeyOrCiphertextLength(
        KemAlgorithm algorithm,
        int expectedSk,
        int expectedCt)
    {
        var encryptor = new MLKEMEncryptor();
        var ct = new PQCiphertext(new byte[expectedCt + 1]);
        var sk = new byte[expectedSk];

        Action act1 = () => encryptor.Decapsulate(algorithm, ct, sk);
        act1.Should().Throw<CryptographicException>();

        var ctValid = new PQCiphertext(new byte[expectedCt]);
        var skInvalid = new byte[expectedSk + 1];
        Action act2 = () => encryptor.Decapsulate(algorithm, ctValid, skInvalid);
        act2.Should().Throw<CryptographicException>();
    }
}


