using System;
using FluentAssertions;
using Mamey.Blockchain.Node;
using Mamey.Security.PostQuantum;
using Mamey.Security.PostQuantum.Models;
using Xunit;

namespace Mamey.Blockchain.Node.Tests.Unit;

public class MameyTransactionTests
{
    [Fact]
    public void Serialize_And_Deserialize_Classical_Version1_RoundTrips()
    {
        var tx = new MameyTransaction
        {
            Version = 1,
            Data = new byte[200],
            SignatureAlgorithm = SignatureAlgorithm.ML_DSA_65, // algorithm value is ignored for V1
            Signature = new byte[71]
        };

        var bytes = tx.Serialize();
        bytes.Should().NotBeNull();
        bytes.Length.Should().BeGreaterThan(0);

        var deserialized = MameyTransaction.Deserialize(bytes);

        deserialized.Version.Should().Be(1);
        deserialized.Data.Length.Should().Be(200);
        deserialized.Signature.Length.Should().Be(71);
        deserialized.IsHybrid.Should().BeFalse();
        deserialized.IsQuantumResistant.Should().BeFalse();
        deserialized.ClassicalSignature.Should().NotBeNull();
        deserialized.PQSignature.Should().BeNull();
    }

    [Fact]
    public void Serialize_And_Deserialize_Pqc_Version2_RoundTrips()
    {
        var tx = new MameyTransaction
        {
            Version = 2,
            Data = new byte[200],
            SignatureAlgorithm = SignatureAlgorithm.ML_DSA_65,
            Signature = new byte[2420],
            IsQuantumResistant = true
        };

        var bytes = tx.Serialize();
        bytes.Should().NotBeNull();
        bytes.Length.Should().BeGreaterThan(0);

        var deserialized = MameyTransaction.Deserialize(bytes);

        deserialized.Version.Should().Be(2);
        deserialized.Data.Length.Should().Be(200);
        deserialized.Signature.Length.Should().Be(2420);
        deserialized.SignatureAlgorithm.Should().Be(SignatureAlgorithm.ML_DSA_65);
        deserialized.IsQuantumResistant.Should().BeTrue();
    }

    [Fact]
    public void Serialize_And_Deserialize_Hybrid_Version2_RoundTrips_Metadata()
    {
        var tx = new MameyTransaction
        {
            Version = 2,
            Data = new byte[200],
            SignatureAlgorithm = SignatureAlgorithm.HYBRID_RSA_MLDSA65,
            Signature = new byte[71 + 2420],
            IsHybrid = true,
            ClassicalSignature = new byte[71],
            PQSignature = new byte[2420],
            IsQuantumResistant = true
        };

        var bytes = tx.Serialize();
        var deserialized = MameyTransaction.Deserialize(bytes);

        deserialized.Version.Should().Be(2);
        deserialized.Data.Length.Should().Be(200);
        deserialized.Signature.Length.Should().Be(71 + 2420);
        deserialized.SignatureAlgorithm.Should().Be(SignatureAlgorithm.HYBRID_RSA_MLDSA65);
        deserialized.IsQuantumResistant.Should().BeTrue();
    }
}


