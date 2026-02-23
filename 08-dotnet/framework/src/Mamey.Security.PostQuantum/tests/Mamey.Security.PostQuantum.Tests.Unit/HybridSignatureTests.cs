using System;
using FluentAssertions;
using Mamey.Security.PostQuantum.Models;
using Xunit;

namespace Mamey.Security.PostQuantum.Tests.Unit;

public class HybridSignatureTests
{
    [Fact]
    public void CombinedSignature_ShouldSerializeAndParse_Roundtrip()
    {
        var classical = new byte[] { 1, 2, 3 };
        var pq = new byte[] { 4, 5 };

        var hybrid = new HybridSignature(classical, pq);
        var combined = hybrid.CombinedSignature;

        HybridSignature.TryParse(combined, out var parsed).Should().BeTrue();
        parsed.Should().NotBeNull();
        parsed!.ClassicalSignature.Should().Equal(classical);
        parsed.PostQuantumSignature.Should().Equal(pq);
    }

    [Fact]
    public void TryParse_ShouldReturnFalse_ForInvalidVersion()
    {
        var classical = new byte[] { 1 };
        var pq = new byte[] { 2 };
        var hybrid = new HybridSignature(classical, pq);
        var combined = hybrid.CombinedSignature;

        combined[0] = (byte)(HybridSignature.CurrentVersion + 1); // corrupt version

        HybridSignature.TryParse(combined, out var parsed).Should().BeFalse();
        parsed.Should().BeNull();
    }
}


