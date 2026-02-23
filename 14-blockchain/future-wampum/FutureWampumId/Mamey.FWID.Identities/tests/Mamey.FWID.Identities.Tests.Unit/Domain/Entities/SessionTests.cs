using Mamey.FWID.Identities.Domain.Entities;
using Mamey.FWID.Identities.Domain.Events;
using Mamey.FWID.Identities.Domain.ValueObjects;
using Shouldly;
using Xunit;

namespace Mamey.FWID.Identities.Tests.Unit.Domain.Entities;

public class SessionTests
{
    [Fact]
    public void Constructor_WithValidParameters_ShouldCreateSession()
    {
        // Arrange
        var sessionId = new SessionId();
        var identityId = new IdentityId(Guid.NewGuid());
        var accessToken = "access-token-123";
        var refreshToken = "refresh-token-456";
        var expiresAt = DateTime.UtcNow.AddHours(24);
        var ipAddress = "192.168.1.1";
        var userAgent = "Mozilla/5.0";

        // Act
        var session = new Session(sessionId, identityId, accessToken, refreshToken, expiresAt, ipAddress, userAgent);

        // Assert
        session.ShouldNotBeNull();
        session.Id.ShouldBe(sessionId);
        session.IdentityId.ShouldBe(identityId);
        session.AccessToken.ShouldBe(accessToken);
        session.RefreshToken.ShouldBe(refreshToken);
        session.ExpiresAt.ShouldBe(expiresAt);
        session.Status.ShouldBe(SessionStatus.Active);
        session.IpAddress.ShouldBe(ipAddress);
        session.UserAgent.ShouldBe(userAgent);
        session.Events.ShouldContain(e => e is SessionCreated);
    }

    [Fact]
    public void UpdateTokens_WithValidTokens_ShouldUpdateTokens()
    {
        // Arrange
        var session = CreateTestSession();
        var newAccessToken = "new-access-token";
        var newRefreshToken = "new-refresh-token";
        var originalLastAccessed = session.LastAccessedAt;

        // Act
        session.UpdateTokens(newAccessToken, newRefreshToken);

        // Assert
        session.AccessToken.ShouldBe(newAccessToken);
        session.RefreshToken.ShouldBe(newRefreshToken);
        session.LastAccessedAt.ShouldBeGreaterThan(originalLastAccessed);
    }

    [Fact]
    public void UpdateTokens_WithNullAccessToken_ShouldThrowException()
    {
        // Arrange
        var session = CreateTestSession();

        // Act & Assert
        Should.Throw<ArgumentNullException>(() => session.UpdateTokens(null!, "refresh-token"));
    }

    [Fact]
    public void Revoke_ShouldSetStatusToRevoked()
    {
        // Arrange
        var session = CreateTestSession();

        // Act
        session.Revoke();

        // Assert
        session.Status.ShouldBe(SessionStatus.Revoked);
        session.RevokedAt.ShouldNotBeNull();
        session.Events.ShouldContain(e => e is SessionRevoked);
    }

    [Fact]
    public void Expire_ShouldSetStatusToExpired()
    {
        // Arrange
        var session = CreateTestSession();

        // Act
        session.Expire();

        // Assert
        session.Status.ShouldBe(SessionStatus.Expired);
        session.Events.ShouldContain(e => e is SessionExpired);
    }

    [Fact]
    public void IsValid_WhenActiveAndNotExpired_ShouldReturnTrue()
    {
        // Arrange
        var session = new Session(
            new SessionId(),
            new IdentityId(Guid.NewGuid()),
            "access-token",
            "refresh-token",
            DateTime.UtcNow.AddHours(1),
            "192.168.1.1",
            "Mozilla/5.0");

        // Act
        var isValid = session.IsValid();

        // Assert
        isValid.ShouldBeTrue();
    }

    [Fact]
    public void IsValid_WhenExpired_ShouldReturnFalse()
    {
        // Arrange
        var session = new Session(
            new SessionId(),
            new IdentityId(Guid.NewGuid()),
            "access-token",
            "refresh-token",
            DateTime.UtcNow.AddHours(-1),
            "192.168.1.1",
            "Mozilla/5.0");

        // Act
        var isValid = session.IsValid();

        // Assert
        isValid.ShouldBeFalse();
    }

    [Fact]
    public void IsValid_WhenRevoked_ShouldReturnFalse()
    {
        // Arrange
        var session = CreateTestSession();
        session.Revoke();

        // Act
        var isValid = session.IsValid();

        // Assert
        isValid.ShouldBeFalse();
    }

    [Fact]
    public void UpdateLastAccessed_ShouldUpdateLastAccessedAt()
    {
        // Arrange
        var session = CreateTestSession();
        var originalLastAccessed = session.LastAccessedAt;
        Thread.Sleep(10); // Small delay to ensure time difference

        // Act
        session.UpdateLastAccessed();

        // Assert
        session.LastAccessedAt.ShouldBeGreaterThan(originalLastAccessed);
    }

    private Session CreateTestSession()
    {
        return new Session(
            new SessionId(),
            new IdentityId(Guid.NewGuid()),
            "access-token",
            "refresh-token",
            DateTime.UtcNow.AddHours(24),
            "192.168.1.1",
            "Mozilla/5.0");
    }
}

