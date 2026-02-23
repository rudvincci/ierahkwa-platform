using System;
using System.Collections.Generic;
using FluentAssertions;
using Mamey.Blockchain.Node;
using Mamey.Blockchain.Node.Consensus;
using Mamey.Security.PostQuantum;
using Mamey.Security.PostQuantum.Models;
using Xunit;

namespace Mamey.Blockchain.Node.Tests.Unit;

public class PQCValidatorTests
{
    private sealed class FakeBlockSignatureVerifier : IBlockSignatureVerifier
    {
        public bool NextClassicalResult { get; set; } = true;
        public bool NextPqcResult { get; set; } = true;

        public bool VerifyClassical(ReadOnlySpan<byte> data, ReadOnlySpan<byte> signature, ReadOnlySpan<byte> publicKey)
            => NextClassicalResult;

        public bool VerifyPostQuantum(ReadOnlySpan<byte> data, ReadOnlySpan<byte> signature, ReadOnlySpan<byte> publicKey,
            SignatureAlgorithm algorithm)
            => NextPqcResult;
    }

    [Fact]
    public void ClassicalOnly_Mode_Accepts_Classical_And_Rejects_Pqc()
    {
        var verifier = new FakeBlockSignatureVerifier { NextClassicalResult = true };
        var validator = new PQCValidator(verifier);
        var config = new ValidatorNodeConfiguration
        {
            RequireQuantumResistantSignatures = false,
            BaseValidationLevel = PQCValidationLevel.ClassicalOnly,
            AcceptClassicalTransactions = true
        };
        var now = DateTime.UtcNow;

        var classicalTx = new MameyTransaction
        {
            Data = new byte[10],
            Signature = new byte[71],
            IsQuantumResistant = false,
            IsHybrid = false
        };

        var classicalResult = validator.ValidateTransaction(classicalTx, config, Array.Empty<byte>(), now, 0);
        classicalResult.IsValid.Should().BeTrue();
        classicalResult.ClassicalValid.Should().BeTrue();
        classicalResult.PostQuantumValid.Should().BeFalse();

        var pqcTx = new MameyTransaction
        {
            Data = new byte[10],
            Signature = new byte[2420],
            IsQuantumResistant = true,
            IsHybrid = false,
            SignatureAlgorithm = SignatureAlgorithm.ML_DSA_65
        };

        var pqcResult = validator.ValidateTransaction(pqcTx, config, Array.Empty<byte>(), now, 0);
        pqcResult.IsValid.Should().BeFalse();
    }

    [Fact]
    public void Hybrid_Mode_Accepts_Both_Classical_And_Pqc()
    {
        var verifier = new FakeBlockSignatureVerifier { NextClassicalResult = true, NextPqcResult = true };
        var validator = new PQCValidator(verifier);
        var config = new ValidatorNodeConfiguration
        {
            RequireQuantumResistantSignatures = false,
            BaseValidationLevel = PQCValidationLevel.Hybrid,
            AcceptClassicalTransactions = true,
            AcceptHybridTransactions = true
        };
        var now = DateTime.UtcNow;

        var classicalTx = new MameyTransaction
        {
            Data = new byte[10],
            Signature = new byte[71],
            IsQuantumResistant = false,
            IsHybrid = false
        };

        var classicalResult = validator.ValidateTransaction(classicalTx, config, Array.Empty<byte>(), now, 0);
        classicalResult.IsValid.Should().BeTrue();

        var pqcTx = new MameyTransaction
        {
            Data = new byte[10],
            Signature = new byte[2420],
            IsQuantumResistant = true,
            IsHybrid = false,
            SignatureAlgorithm = SignatureAlgorithm.ML_DSA_65
        };

        var pqcResult = validator.ValidateTransaction(pqcTx, config, Array.Empty<byte>(), now, 0);
        pqcResult.IsValid.Should().BeTrue();
    }

    [Fact]
    public void HybridRequired_Mode_Requires_Both_Signatures()
    {
        var verifier = new FakeBlockSignatureVerifier { NextClassicalResult = true, NextPqcResult = true };
        var validator = new PQCValidator(verifier);
        var config = new ValidatorNodeConfiguration
        {
            RequireQuantumResistantSignatures = false,
            BaseValidationLevel = PQCValidationLevel.HybridRequired,
            AcceptHybridTransactions = true
        };
        var now = DateTime.UtcNow;

        var missingComponentTx = new MameyTransaction
        {
            Data = new byte[10],
            IsHybrid = true,
            IsQuantumResistant = true,
            ClassicalSignature = new byte[71],
            PQSignature = Array.Empty<byte>(),
            Signature = new byte[71 + 2420],
            SignatureAlgorithm = SignatureAlgorithm.HYBRID_RSA_MLDSA65
        };

        var missingResult = validator.ValidateTransaction(missingComponentTx, config, Array.Empty<byte>(), now, 0);
        missingResult.IsValid.Should().BeFalse();

        var fullHybridTx = new MameyTransaction
        {
            Data = new byte[10],
            IsHybrid = true,
            IsQuantumResistant = true,
            ClassicalSignature = new byte[71],
            PQSignature = new byte[2420],
            Signature = new byte[71 + 2420],
            SignatureAlgorithm = SignatureAlgorithm.HYBRID_RSA_MLDSA65
        };

        var fullResult = validator.ValidateTransaction(fullHybridTx, config, Array.Empty<byte>(), now, 0);
        fullResult.IsValid.Should().BeTrue();
        fullResult.ClassicalValid.Should().BeTrue();
        fullResult.PostQuantumValid.Should().BeTrue();
    }

    [Fact]
    public void QuantumResistantOnly_Mode_Rejects_ClassicalAfterDeadline()
    {
        var verifier = new FakeBlockSignatureVerifier { NextPqcResult = true };
        var validator = new PQCValidator(verifier);
        var now = DateTime.UtcNow;
        var config = new ValidatorNodeConfiguration
        {
            RequireQuantumResistantSignatures = true,
            QuantumResistantMandatoryDate = now.AddSeconds(-1),
            BaseValidationLevel = PQCValidationLevel.Hybrid
        };

        var classicalTx = new MameyTransaction
        {
            Data = new byte[10],
            Signature = new byte[71],
            IsQuantumResistant = false,
            IsHybrid = false
        };

        var classicalResult = validator.ValidateTransaction(classicalTx, config, Array.Empty<byte>(), now, 0);
        classicalResult.IsValid.Should().BeFalse();

        var pqcTx = new MameyTransaction
        {
            Data = new byte[10],
            Signature = new byte[2420],
            IsQuantumResistant = true,
            IsHybrid = false,
            SignatureAlgorithm = SignatureAlgorithm.ML_DSA_65
        };

        var pqcResult = validator.ValidateTransaction(pqcTx, config, Array.Empty<byte>(), now, 0);
        pqcResult.IsValid.Should().BeTrue();
    }

    [Fact]
    public void GetEffectiveValidationLevel_Transitions_From_Hybrid_To_QuantumResistantOnly()
    {
        var config = new ValidatorNodeConfiguration
        {
            RequireQuantumResistantSignatures = true,
            QuantumResistantMandatoryDate = DateTime.UtcNow.AddMinutes(10),
            BaseValidationLevel = PQCValidationLevel.Hybrid
        };

        var before = config.GetEffectiveValidationLevel(config.QuantumResistantMandatoryDate.AddMinutes(-1), 0);
        var after = config.GetEffectiveValidationLevel(config.QuantumResistantMandatoryDate.AddMinutes(1), 0);

        before.Should().Be(PQCValidationLevel.Hybrid);
        after.Should().Be(PQCValidationLevel.QuantumResistantOnly);
    }

    [Fact]
    public void ValidateBlockSize_Enforces_MaxBlockSize()
    {
        var verifier = new FakeBlockSignatureVerifier();
        var validator = new PQCValidator(verifier);
        var config = new ValidatorNodeConfiguration
        {
            MaxBlockSizeBytes = 10 * 1024 * 1024
        };

        var smallTx = new MameyTransaction
        {
            Data = new byte[1024],
            Signature = new byte[256]
        };

        var manySmall = new List<MameyTransaction>();
        for (var i = 0; i < 100; i++)
        {
            manySmall.Add(smallTx);
        }

        var ok = validator.ValidateBlockSize(manySmall, config, out var size);
        ok.Should().BeTrue();
        size.Should().BeLessThanOrEqualTo(config.MaxBlockSizeBytes);

        var hugeTx = new MameyTransaction
        {
            Data = new byte[config.MaxBlockSizeBytes + 1],
            Signature = Array.Empty<byte>()
        };

        var tooBig = validator.ValidateBlockSize(new[] { hugeTx }, config, out var bigSize);
        tooBig.Should().BeFalse();
        bigSize.Should().BeGreaterThan(config.MaxBlockSizeBytes);
    }

    [Fact]
    public void OrderTransactionPool_Prioritises_Pqc_After_Deadline()
    {
        var verifier = new FakeBlockSignatureVerifier();
        var validator = new PQCValidator(verifier);
        var now = DateTime.UtcNow;

        var config = new ValidatorNodeConfiguration
        {
            RequireQuantumResistantSignatures = true,
            QuantumResistantMandatoryDate = now.AddSeconds(-1)
        };

        var classicalTx = new MameyTransaction { IsQuantumResistant = false };
        var pqcTx = new MameyTransaction { IsQuantumResistant = true };
        var pool = new List<MameyTransaction> { classicalTx, pqcTx };

        var ordered = validator.OrderTransactionPool(pool, config, now);
        ordered[0].Should().BeSameAs(pqcTx);
        ordered[1].Should().BeSameAs(classicalTx);
    }

    [Fact]
    public void Tampered_Pqc_Signature_Is_Rejected()
    {
        var verifier = new FakeBlockSignatureVerifier { NextPqcResult = false };
        var validator = new PQCValidator(verifier);
        var now = DateTime.UtcNow;
        var config = new ValidatorNodeConfiguration
        {
            RequireQuantumResistantSignatures = true,
            QuantumResistantMandatoryDate = now.AddSeconds(-1),
            BaseValidationLevel = PQCValidationLevel.QuantumResistantOnly
        };

        var pqcTx = new MameyTransaction
        {
            Data = new byte[10],
            Signature = new byte[2420],
            IsQuantumResistant = true,
            IsHybrid = false,
            SignatureAlgorithm = SignatureAlgorithm.ML_DSA_65
        };

        var result = validator.ValidateTransaction(pqcTx, config, Array.Empty<byte>(), now, 0);
        result.IsValid.Should().BeFalse();
        result.PostQuantumValid.Should().BeFalse();
    }
}


