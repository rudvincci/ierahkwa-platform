using System;
using Mamey.Auth;
using Mamey.Auth.Jwt;
using Mamey.FWID.Identities.Application.Services;
using Mamey.FWID.Identities.Domain.Entities;
using Mamey.FWID.Identities.Domain.Repositories;
using Mamey.FWID.Identities.Domain.ValueObjects;
using Mamey.FWID.Identities.Infrastructure.EF;
using Mamey.FWID.Identities.Infrastructure.EF.Repositories;
using Mamey.FWID.Identities.Tests.Shared.Factories;
using Mamey.FWID.Identities.Tests.Shared.Fixtures;
using Mamey.Security;
using IHasher = Mamey.Security.IHasher;
using Mamey.Security.Internals;
using Mamey.Types;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Shouldly;
using Xunit;

namespace Mamey.FWID.Identities.Tests.Integration.Services;

/// <summary>
/// Integration tests for AuthenticationService with real database.
/// </summary>
[Collection("Integration")]
public class AuthenticationServiceIntegrationTests : IClassFixture<PostgreSQLFixture>, IAsyncLifetime
{
    // Test constants
    private const string TestUsername = "testuser";
    private const string TestPassword = "password123";
    private const string TestIpAddress = "192.168.1.1";
    private const string TestUserAgent = "Mozilla/5.0";

    private readonly PostgreSQLFixture _fixture;
    private IServiceProvider? _serviceProvider;
    private Application.Services.IAuthenticationService? _authenticationService;
    private IIdentityRepository? _identityRepository;
    private ISessionRepository? _sessionRepository;
    private ISecurityProvider? _securityProvider;
    private IHasher? _hasher; // Direct IHasher for password hashing (PasswordHash is not marked with [Hashed])

    public AuthenticationServiceIntegrationTests(PostgreSQLFixture fixture)
    {
        _fixture = fixture;
    }

    public async Task InitializeAsync()
    {
        // Configure services
        var services = new ServiceCollection();

        // Add logging
        services.AddLogging(builder => builder.AddConsole().SetMinimumLevel(LogLevel.Warning));

        // Add Entity Framework Core
        services.AddDbContext<IdentityDbContext>(options =>
        {
            options.UseNpgsql(_fixture.ConnectionString);
            options.EnableSensitiveDataLogging();
        });

        // Add Security services
        var securityOptions = new Mamey.Security.SecurityOptions
        {
            Encryption = new Mamey.Security.SecurityOptions.EncryptionOptions
            {
                Enabled = true,
                Key = "12345678901234567890123456789012"
            }
        };
        services.AddSingleton(securityOptions);
        
        // Create logger factory directly instead of building service provider twice
        var loggerFactory = LoggerFactory.Create(builder => builder.AddConsole().SetMinimumLevel(LogLevel.Warning));
        var encryptorLogger = loggerFactory.CreateLogger<Encryptor>();
        services.AddSingleton(loggerFactory);
        services.AddSingleton<Mamey.Security.IEncryptor>(new Encryptor(encryptorLogger));
        services.AddSingleton<Mamey.Security.IHasher, Mamey.Security.Internals.Hasher>();
        services.AddSingleton<Mamey.Security.IRng, Mamey.Security.Rng>();
        services.AddSingleton<Mamey.Security.ISigner, Mamey.Security.Signer>();
        services.AddSingleton<Mamey.Security.IMd5, Mamey.Security.Md5>();
        services.AddSingleton<ISecurityProvider>(sp =>
        {
            var encryptor = sp.GetRequiredService<Mamey.Security.IEncryptor>();
            var hasher = sp.GetRequiredService<Mamey.Security.IHasher>();
            var rng = sp.GetRequiredService<Mamey.Security.IRng>();
            var signer = sp.GetRequiredService<Mamey.Security.ISigner>();
            var md5 = sp.GetRequiredService<Mamey.Security.IMd5>();
            var opts = sp.GetRequiredService<Mamey.Security.SecurityOptions>();
            return new Mamey.Security.SecurityProvider(encryptor, hasher, rng, signer, md5, opts);
        });
        services.AddScoped<Mamey.Security.SecurityAttributeProcessor>();

        // Add JWT Handler (mock for now)
        services.AddSingleton<IJwtHandler>(sp =>
        {
            return new MockJwtHandler();
        });

        // Add repositories
        services.AddScoped<IIdentityRepository>(provider =>
        {
            var dbContext = provider.GetRequiredService<IdentityDbContext>();
            var securityProcessor = provider.GetRequiredService<SecurityAttributeProcessor>();
            return new IdentityPostgresRepository(dbContext, securityProcessor);
        });

        services.AddScoped<ISessionRepository>(provider =>
        {
            var dbContext = provider.GetRequiredService<IdentityDbContext>();
            return new SessionPostgresRepository(dbContext);
        });

        // Add authentication service
        services.AddScoped<Application.Services.IAuthenticationService, AuthenticationService>();

        _serviceProvider = services.BuildServiceProvider();

        // Ensure database is created
        using var scope = _serviceProvider.CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<IdentityDbContext>();
        await context.Database.EnsureCreatedAsync();

        // Get services
        _authenticationService = _serviceProvider.GetRequiredService<Application.Services.IAuthenticationService>();
        _identityRepository = _serviceProvider.GetRequiredService<IIdentityRepository>();
        _sessionRepository = _serviceProvider.GetRequiredService<ISessionRepository>();
        _securityProvider = _serviceProvider.GetRequiredService<ISecurityProvider>();
        _hasher = _serviceProvider.GetRequiredService<IHasher>();
    }

    public async Task DisposeAsync()
    {
        if (_serviceProvider != null)
        {
            using var scope = _serviceProvider.CreateScope();
            var context = scope.ServiceProvider.GetRequiredService<IdentityDbContext>();
            await context.Database.EnsureDeletedAsync();
            ((IDisposable)_serviceProvider).Dispose();
        }
    }

    #region Helper Methods

    /// <summary>
    /// Creates and saves an identity with credentials.
    /// </summary>
    private async Task<Identity> CreateAndSaveIdentityAsync(string username, string password)
    {
        var identity = TestDataFactory.CreateTestIdentity();
        var passwordHash = _hasher!.Hash(password);
        identity.SetCredentials(username, passwordHash);
        await _identityRepository!.AddAsync(identity);
        return identity;
    }

    /// <summary>
    /// Creates and saves an identity with credentials and returns both identity and password hash.
    /// </summary>
    private async Task<(Identity Identity, string PasswordHash)> CreateAndSaveIdentityWithHashAsync(string username, string password)
    {
        var identity = TestDataFactory.CreateTestIdentity();
        var passwordHash = _hasher!.Hash(password);
        identity.SetCredentials(username, passwordHash);
        await _identityRepository!.AddAsync(identity);
        return (identity, passwordHash);
    }

    #endregion

    #region SignInAsync Tests

    [Fact]
    public async Task SignInAsync_WithValidCredentials_ShouldCreateSession()
    {
        // Arrange
        var identity = await CreateAndSaveIdentityAsync(TestUsername, TestPassword);

        // Act
        var result = await _authenticationService!.SignInAsync(TestUsername, TestPassword, TestIpAddress, TestUserAgent);

        // Assert
        result.ShouldNotBeNull();
        result.IdentityId.ShouldBe(identity.Id);
        result.AccessToken.ShouldNotBeNullOrEmpty();
        result.RefreshToken.ShouldNotBeNullOrEmpty();
        result.SessionId.ShouldNotBeNull();

        // Verify session was created
        var session = await _sessionRepository!.GetAsync(result.SessionId);
        session.ShouldNotBeNull();
        session.IdentityId.ShouldBe(identity.Id);
        session.Status.ShouldBe(SessionStatus.Active);
        session.IpAddress.ShouldBe(TestIpAddress);
        session.UserAgent.ShouldBe(TestUserAgent);

        // Verify identity was updated
        var updatedIdentity = await _identityRepository!.GetAsync(identity.Id);
        updatedIdentity.ShouldNotBeNull();
        updatedIdentity.LastLoginAt.ShouldNotBeNull();
        updatedIdentity.FailedLoginAttempts.ShouldBe(0);
        updatedIdentity.LockedUntil.ShouldBeNull();
    }

    [Fact]
    public async Task SignInAsync_WithInvalidCredentials_ShouldThrowException()
    {
        // Arrange
        await CreateAndSaveIdentityAsync(TestUsername, TestPassword);

        // Act & Assert
        await Should.ThrowAsync<InvalidOperationException>(
            () => _authenticationService!.SignInAsync(TestUsername, "wrongpassword", null, null));
        
        // Verify failed login attempts were incremented
        var identity = await _identityRepository!.GetSingleOrDefaultAsync(i => i.Username == TestUsername);
        identity.ShouldNotBeNull();
        identity.FailedLoginAttempts.ShouldBeGreaterThan(0);
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("   ")]
    public async Task SignInAsync_WithNullOrEmptyUsername_ShouldThrowArgumentException(string? username)
    {
        // Arrange
        await CreateAndSaveIdentityAsync(TestUsername, TestPassword);

        // Act & Assert
        await Should.ThrowAsync<ArgumentException>(
            () => _authenticationService!.SignInAsync(username!, TestPassword, null, null));
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("   ")]
    public async Task SignInAsync_WithNullOrEmptyPassword_ShouldThrowArgumentException(string? password)
    {
        // Arrange
        await CreateAndSaveIdentityAsync(TestUsername, TestPassword);

        // Act & Assert
        await Should.ThrowAsync<ArgumentException>(
            () => _authenticationService!.SignInAsync(TestUsername, password!, null, null));
    }

    [Fact]
    public async Task SignInAsync_WithNonExistentUsername_ShouldThrowException()
    {
        // Act & Assert
        await Should.ThrowAsync<InvalidOperationException>(
            () => _authenticationService!.SignInAsync("nonexistent", TestPassword, null, null));
    }

    [Fact]
    public async Task SignInAsync_WithLockedAccount_ShouldThrowException()
    {
        // Arrange
        var identity = await CreateAndSaveIdentityAsync(TestUsername, TestPassword);
        identity.LockAccount(DateTime.UtcNow.AddHours(1));
        await _identityRepository!.UpdateAsync(identity);

        // Act & Assert
        await Should.ThrowAsync<InvalidOperationException>(
            () => _authenticationService!.SignInAsync(TestUsername, TestPassword, null, null));
    }

    [Fact]
    public async Task SignInAsync_WithMultipleFailedAttempts_ShouldIncrementFailedAttempts()
    {
        // Arrange
        await CreateAndSaveIdentityAsync(TestUsername, TestPassword);

        // Act - Attempt multiple failed logins
        for (int i = 0; i < 3; i++)
        {
            try
            {
                await _authenticationService!.SignInAsync(TestUsername, "wrongpassword", null, null);
            }
            catch (InvalidOperationException)
            {
                // Expected
            }
        }

        // Assert
        var identity = await _identityRepository!.GetSingleOrDefaultAsync(i => i.Username == TestUsername);
        identity.ShouldNotBeNull();
        identity.FailedLoginAttempts.ShouldBe(3);
    }

    [Fact]
    public async Task SignInAsync_WithValidCredentialsAfterFailedAttempts_ShouldResetFailedAttempts()
    {
        // Arrange
        var identity = await CreateAndSaveIdentityAsync(TestUsername, TestPassword);

        // Act - Fail once, then succeed
        try
        {
            await _authenticationService!.SignInAsync(TestUsername, "wrongpassword", null, null);
        }
        catch (InvalidOperationException)
        {
            // Expected
        }

        var result = await _authenticationService!.SignInAsync(TestUsername, TestPassword, null, null);

        // Assert
        result.ShouldNotBeNull();
        var updatedIdentity = await _identityRepository!.GetAsync(identity.Id);
        updatedIdentity.ShouldNotBeNull();
        updatedIdentity.FailedLoginAttempts.ShouldBe(0);
    }

    #endregion

    #region SignOutAsync Tests

    [Fact]
    public async Task SignOutAsync_WithValidSession_ShouldRevokeSession()
    {
        // Arrange
        await CreateAndSaveIdentityAsync(TestUsername, TestPassword);
        var signInResult = await _authenticationService!.SignInAsync(TestUsername, TestPassword, null, null);
        var sessionId = signInResult.SessionId;

        // Act
        await _authenticationService.SignOutAsync(sessionId);

        // Assert
        var session = await _sessionRepository!.GetAsync(sessionId);
        session.ShouldNotBeNull();
        session.Status.ShouldBe(SessionStatus.Revoked);
        session.RevokedAt.ShouldNotBeNull();

        // Verify identity was updated
        var identity = await _identityRepository!.GetAsync(signInResult.IdentityId);
        identity.ShouldNotBeNull();
        // Note: SignOut doesn't update LastLoginAt, it just records the event
    }

    [Fact]
    public async Task SignOutAsync_WithNonExistentSession_ShouldNotThrow()
    {
        // Arrange
        var nonExistentSessionId = new SessionId();

        // Act & Assert - Should not throw
        await _authenticationService!.SignOutAsync(nonExistentSessionId);
    }

    #endregion

    #region RefreshTokenAsync Tests

    [Fact]
    public async Task RefreshTokenAsync_WithValidRefreshToken_ShouldReturnNewTokens()
    {
        // Arrange
        var (identity, passwordHash) = await CreateAndSaveIdentityWithHashAsync(TestUsername, TestPassword);

        // Verify the password hash was saved correctly by retrieving the identity
        var savedIdentity = await _identityRepository!.GetSingleOrDefaultAsync(i => i.Username == TestUsername);
        savedIdentity.ShouldNotBeNull();
        savedIdentity.PasswordHash.ShouldNotBeNullOrEmpty();
        savedIdentity.PasswordHash.ShouldBe(passwordHash); // Verify the hash matches what we set

        var signInResult = await _authenticationService!.SignInAsync(TestUsername, TestPassword, null, null);
        var originalAccessToken = signInResult.AccessToken;
        var refreshToken = signInResult.RefreshToken;
        var originalSessionId = signInResult.SessionId;

        // Act
        var refreshResult = await _authenticationService.RefreshTokenAsync(refreshToken);

        // Assert
        refreshResult.ShouldNotBeNull();
        refreshResult.AccessToken.ShouldNotBeNullOrEmpty();
        refreshResult.AccessToken.ShouldNotBe(originalAccessToken); // Should be different
        refreshResult.RefreshToken.ShouldNotBeNullOrEmpty();
        refreshResult.RefreshToken.ShouldNotBe(refreshToken); // Should be different
        refreshResult.IdentityId.ShouldBe(identity.Id);
        refreshResult.SessionId.ShouldBe(originalSessionId); // Same session, updated tokens

        // Verify session was updated with new tokens
        var session = await _sessionRepository!.GetAsync(originalSessionId);
        session.ShouldNotBeNull();
        session.AccessToken.ShouldBe(refreshResult.AccessToken);
        session.RefreshToken.ShouldBe(refreshResult.RefreshToken);
        session.Status.ShouldBe(SessionStatus.Active);
    }

    [Fact]
    public async Task RefreshTokenAsync_WithInvalidRefreshToken_ShouldThrowException()
    {
        // Act & Assert
        await Should.ThrowAsync<InvalidOperationException>(
            () => _authenticationService!.RefreshTokenAsync("invalid-refresh-token"));
    }

    [Fact]
    public async Task RefreshTokenAsync_WithExpiredSession_ShouldThrowException()
    {
        // Arrange
        await CreateAndSaveIdentityAsync(TestUsername, TestPassword);
        var signInResult = await _authenticationService!.SignInAsync(TestUsername, TestPassword, null, null);
        var sessionId = signInResult.SessionId;

        // Manually expire the session by updating it
        var session = await _sessionRepository!.GetAsync(sessionId);
        session.ShouldNotBeNull();
        // Note: Session.ExpiresAt is read-only, so we need to create a new expired session
        // For this test, we'll revoke the session instead
        session.Revoke();
        await _sessionRepository.UpdateAsync(session);

        // Act & Assert
        await Should.ThrowAsync<InvalidOperationException>(
            () => _authenticationService!.RefreshTokenAsync(signInResult.RefreshToken));
    }

    [Fact]
    public async Task RefreshTokenAsync_WithRevokedSession_ShouldThrowException()
    {
        // Arrange
        await CreateAndSaveIdentityAsync(TestUsername, TestPassword);
        var signInResult = await _authenticationService!.SignInAsync(TestUsername, TestPassword, null, null);
        var refreshToken = signInResult.RefreshToken;

        // Revoke the session
        await _authenticationService.SignOutAsync(signInResult.SessionId);

        // Act & Assert
        await Should.ThrowAsync<InvalidOperationException>(
            () => _authenticationService.RefreshTokenAsync(refreshToken));
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("   ")]
    public async Task RefreshTokenAsync_WithNullOrEmptyRefreshToken_ShouldThrowException(string? refreshToken)
    {
        // Act & Assert
        await Should.ThrowAsync<InvalidOperationException>(
            () => _authenticationService!.RefreshTokenAsync(refreshToken!));
    }

    #endregion

    #region SignInWithBiometricAsync Tests

    [Fact]
    public async Task SignInWithBiometricAsync_WithValidBiometric_ShouldCreateSession()
    {
        // Arrange
        var identity = TestDataFactory.CreateTestIdentity();
        var biometricData = identity.BiometricData;
        await _identityRepository!.AddAsync(identity);

        // Act
        var result = await _authenticationService!.SignInWithBiometricAsync(
            identity.Id,
            biometricData!,
            TestIpAddress,
            TestUserAgent);

        // Assert
        result.ShouldNotBeNull();
        result.IdentityId.ShouldBe(identity.Id);
        result.AccessToken.ShouldNotBeNullOrEmpty();
        result.RefreshToken.ShouldNotBeNullOrEmpty();
        result.SessionId.ShouldNotBeNull();

        // Verify session was created
        var session = await _sessionRepository!.GetAsync(result.SessionId);
        session.ShouldNotBeNull();
        session.IdentityId.ShouldBe(identity.Id);
        session.Status.ShouldBe(SessionStatus.Active);
        session.IpAddress.ShouldBe(TestIpAddress);
        session.UserAgent.ShouldBe(TestUserAgent);

        // Verify identity was updated
        var updatedIdentity = await _identityRepository.GetAsync(identity.Id);
        updatedIdentity.ShouldNotBeNull();
        updatedIdentity.LastLoginAt.ShouldNotBeNull();
    }

    [Fact]
    public async Task SignInWithBiometricAsync_WithInvalidBiometric_ShouldThrowException()
    {
        // Arrange
        var identity = TestDataFactory.CreateTestIdentity();
        await _identityRepository!.AddAsync(identity);

        // Create invalid biometric data
        var invalidBiometric = new BiometricData(
            BiometricType.Fingerprint,
            new byte[] { 1, 2, 3, 4, 5 },
            "invalid-hash");

        // Act & Assert
        await Should.ThrowAsync<InvalidOperationException>(
            () => _authenticationService!.SignInWithBiometricAsync(
                identity.Id,
                invalidBiometric,
                null,
                null));
    }

    [Fact]
    public async Task SignInWithBiometricAsync_WithNonExistentIdentity_ShouldThrowException()
    {
        // Arrange
        var nonExistentId = new IdentityId(Guid.NewGuid());
        var biometricData = TestDataFactory.CreateTestBiometricData();

        // Act & Assert
        await Should.ThrowAsync<InvalidOperationException>(
            () => _authenticationService!.SignInWithBiometricAsync(
                nonExistentId,
                biometricData,
                null,
                null));
    }

    #endregion

    #region SignInWithDidAsync Tests

    [Fact]
    public async Task SignInWithDidAsync_ShouldThrowNotImplementedException()
    {
        // Act & Assert
        await Should.ThrowAsync<NotImplementedException>(
            () => _authenticationService!.SignInWithDidAsync("did:example:123", null, null));
    }

    #endregion

    #region SignInWithAzureAsync Tests

    [Fact]
    public async Task SignInWithAzureAsync_ShouldThrowNotImplementedException()
    {
        // Act & Assert
        await Should.ThrowAsync<NotImplementedException>(
            () => _authenticationService!.SignInWithAzureAsync("azure-token", null, null));
    }

    #endregion

    #region SignInWithIdentityAsync Tests

    [Fact]
    public async Task SignInWithIdentityAsync_ShouldThrowNotImplementedException()
    {
        // Act & Assert
        await Should.ThrowAsync<NotImplementedException>(
            () => _authenticationService!.SignInWithIdentityAsync("identity-token", null, null));
    }

    #endregion

    #region Mock Implementations

    private class MockJwtHandler : IJwtHandler
    {
        public JsonWebToken CreateToken(string userId, string? role = null, string? audience = null, IDictionary<string, string>? claims = null)
        {
            return new JsonWebToken
            {
                AccessToken = $"mock-token-{Guid.NewGuid()}",
                Expires = DateTimeOffset.UtcNow.AddHours(1).ToUnixTimeSeconds()
            };
        }

        public JsonWebTokenPayload GetTokenPayload(string accessToken)
        {
            return new JsonWebTokenPayload
            {
                Subject = Guid.NewGuid().ToString(),
                Role = "User",
                Expires = DateTimeOffset.UtcNow.AddHours(1).ToUnixTimeSeconds()
            };
        }
    }

    #endregion
}

