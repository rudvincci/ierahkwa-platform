using Mamey.Auth;
using Mamey.Auth.Jwt;
using Mamey.FWID.Identities.Application.Services;
using Mamey.FWID.Identities.Domain.Entities;
using Mamey.FWID.Identities.Domain.Repositories;
using Mamey.FWID.Identities.Domain.ValueObjects;
using Mamey.Security;
using Microsoft.Extensions.Logging;
using NSubstitute;
using Shouldly;
using Xunit;

namespace Mamey.FWID.Identities.Tests.Unit.Application.Services;

public class AuthenticationServiceTests
{
    private readonly IIdentityRepository _identityRepository;
    private readonly ISessionRepository _sessionRepository;
    private readonly IJwtHandler _jwtHandler;
    private readonly ISecurityProvider _securityProvider;
    private readonly ILogger<AuthenticationService> _logger;
    private readonly AuthenticationService _service;

    public AuthenticationServiceTests()
    {
        _identityRepository = Substitute.For<IIdentityRepository>();
        _sessionRepository = Substitute.For<ISessionRepository>();
        _jwtHandler = Substitute.For<IJwtHandler>();
        _securityProvider = Substitute.For<ISecurityProvider>();
        _logger = Substitute.For<ILogger<AuthenticationService>>();
        _service = new AuthenticationService(
            _identityRepository,
            _sessionRepository,
            _jwtHandler,
            _securityProvider,
            _logger);
    }

    [Fact]
    public async Task SignInAsync_WithValidCredentials_ShouldReturnAuthenticationResult()
    {
        // Arrange
        var identityId = new IdentityId(Guid.NewGuid());
        var username = "testuser";
        var password = "password123";
        var passwordHash = "hashed-password";
        var identity = CreateTestIdentity(identityId, username, passwordHash);
        
        _identityRepository.GetSingleOrDefaultAsync(Arg.Any<System.Linq.Expressions.Expression<Func<Identity, bool>>>(), Arg.Any<CancellationToken>())
            .Returns(identity);
        _securityProvider.Hash(password).Returns(passwordHash);
        
        var accessToken = new JsonWebToken
        {
            AccessToken = "access-token",
            Expires = DateTimeOffset.UtcNow.AddHours(1).ToUnixTimeSeconds()
        };
        _jwtHandler.CreateToken(Arg.Any<string>(), Arg.Any<string>(), Arg.Any<string>(), Arg.Any<IDictionary<string, string>>())
            .Returns(accessToken);
        
        _securityProvider.GenerateRandomString(64).Returns("refresh-token");

        // Act
        var result = await _service.SignInAsync(username, password, null, null);

        // Assert
        result.ShouldNotBeNull();
        result.IdentityId.ShouldBe(identityId);
        result.AccessToken.ShouldBe("access-token");
        result.RefreshToken.ShouldBe("refresh-token");
        await _sessionRepository.Received(1).AddAsync(Arg.Any<Session>(), Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task SignInAsync_WithInvalidCredentials_ShouldThrowException()
    {
        // Arrange
        var username = "testuser";
        var password = "wrongpassword";
        var correctPasswordHash = "correct-hash";
        var wrongPasswordHash = "wrong-hash";
        var identity = CreateTestIdentity(new IdentityId(Guid.NewGuid()), username, correctPasswordHash);
        
        _identityRepository.FindAsync(Arg.Any<System.Linq.Expressions.Expression<Func<Identity, bool>>>(), Arg.Any<CancellationToken>())
            .Returns(new List<Identity> { identity });
        _securityProvider.Hash(password).Returns(wrongPasswordHash);

        // Act & Assert
        await Should.ThrowAsync<InvalidOperationException>(
            () => _service.SignInAsync(username, password, null, null));
    }

    [Fact]
    public async Task SignOutAsync_WithValidSessionId_ShouldRevokeSession()
    {
        // Arrange
        var sessionId = new SessionId();
        var session = new Session(
            sessionId,
            new IdentityId(Guid.NewGuid()),
            "access-token",
            "refresh-token",
            DateTime.UtcNow.AddHours(1));
        
        _sessionRepository.GetAsync(sessionId, Arg.Any<CancellationToken>())
            .Returns(session);

        // Act
        await _service.SignOutAsync(sessionId);

        // Assert
        session.Status.ShouldBe(SessionStatus.Revoked);
        await _sessionRepository.Received(1).UpdateAsync(session, Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task RefreshTokenAsync_WithValidRefreshToken_ShouldReturnNewTokens()
    {
        // Arrange
        var sessionId = new SessionId();
        var identityId = new IdentityId(Guid.NewGuid());
        var refreshToken = "valid-refresh-token";
        var session = new Session(
            sessionId,
            identityId,
            "old-access-token",
            refreshToken,
            DateTime.UtcNow.AddHours(1),
            "192.168.1.1",
            "Mozilla/5.0");
        
        _sessionRepository.GetByRefreshTokenAsync(refreshToken, Arg.Any<CancellationToken>())
            .Returns(session);
        
        // Mock identity repository to return the identity
        var identity = CreateTestIdentity(identityId, "testuser", "hashed-password");
        _identityRepository.GetAsync(identityId, Arg.Any<CancellationToken>())
            .Returns(identity);
        
        var newAccessToken = new JsonWebToken
        {
            AccessToken = "new-access-token",
            Expires = DateTimeOffset.UtcNow.AddHours(1).ToUnixTimeSeconds()
        };
        _jwtHandler.CreateToken(Arg.Any<string>(), Arg.Any<string>(), Arg.Any<string>(), Arg.Any<IDictionary<string, string>>())
            .Returns(newAccessToken);
        
        _securityProvider.GenerateRandomString(64).Returns("new-refresh-token");

        // Act
        var result = await _service.RefreshTokenAsync(refreshToken);

        // Assert
        result.ShouldNotBeNull();
        result.AccessToken.ShouldBe("new-access-token");
        result.RefreshToken.ShouldBe("new-refresh-token");
        session.AccessToken.ShouldBe("new-access-token");
        session.RefreshToken.ShouldBe("new-refresh-token");
    }

    private Identity CreateTestIdentity(IdentityId id, string username, string password)
    {
        // Create a valid BiometricData instance for testing
        var biometricTemplate = new byte[] { 1, 2, 3, 4, 5 };
        var biometricHash = Convert.ToHexString(System.Security.Cryptography.SHA512.HashData(biometricTemplate)).ToLowerInvariant();
        var biometricData = new Mamey.FWID.Identities.Domain.ValueObjects.BiometricData(
            Mamey.FWID.Identities.Domain.ValueObjects.BiometricType.Fingerprint,
            biometricTemplate,
            biometricHash);
        
        var identity = new Identity(
            id,
            new Mamey.Types.Name("Test", "User"),
            new Mamey.FWID.Identities.Domain.ValueObjects.PersonalDetails(
                new DateTime(1990, 1, 1),
                "Location",
                "Male",
                "Clan"),
            new Mamey.FWID.Identities.Domain.ValueObjects.ContactInformation(
                new Mamey.Types.Email("test@example.com"),
                null,
                null),
            biometricData,
            "zone-001",
            null);
        
        identity.SetCredentials(username, password);
        return identity;
    }
}

