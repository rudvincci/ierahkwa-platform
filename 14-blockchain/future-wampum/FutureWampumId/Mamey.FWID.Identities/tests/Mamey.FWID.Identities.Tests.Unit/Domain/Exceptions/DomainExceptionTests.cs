using Mamey.FWID.Identities.Domain.Entities;
using Mamey.FWID.Identities.Domain.Exceptions;
using Shouldly;
using Xunit;

namespace Mamey.FWID.Identities.Tests.Unit.Domain.Exceptions;

public class DomainExceptionTests
{
    [Fact]
    public void BiometricVerificationFailedException_ShouldContainCorrectProperties()
    {
        // Arrange
        var identityId = new IdentityId(Guid.NewGuid());
        var matchScore = 0.85;
        var threshold = 0.95;

        // Act
        var exception = new BiometricVerificationFailedException(identityId, matchScore, threshold);

        // Assert
        exception.ShouldNotBeNull();
        exception.IdentityId.ShouldBe(identityId);
        exception.MatchScore.ShouldBe(matchScore);
        exception.Threshold.ShouldBe(threshold);
        exception.Code.ShouldBe("biometric_verification_failed");
        exception.Message.ShouldContain(identityId.Value.ToString());
        exception.Message.ShouldContain(matchScore.ToString("F2"));
        exception.Message.ShouldContain(threshold.ToString("F2"));
    }
}

