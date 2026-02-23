using Mamey.FWID.Identities.Domain.Entities;
using Mamey.FWID.Identities.Domain.Repositories;
using Mamey.FWID.Identities.Domain.ValueObjects;
using Mamey.FWID.Identities.Infrastructure.EF;
using Mamey.FWID.Identities.Infrastructure.EF.Repositories;
using Mamey.FWID.Identities.Tests.Shared.Factories;
using Mamey.FWID.Identities.Tests.Shared.Fixtures;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Shouldly;
using Xunit;

namespace Mamey.FWID.Identities.Tests.Integration.Repositories;

/// <summary>
/// Integration tests for Session repository with real database.
/// </summary>
[Collection("Integration")]
public class SessionRepositoryIntegrationTests : IClassFixture<PostgreSQLFixture>, IAsyncLifetime
{
    private readonly PostgreSQLFixture _fixture;
    private IServiceProvider? _serviceProvider;
    private ISessionRepository? _repository;
    private IIdentityRepository? _identityRepository;

    public SessionRepositoryIntegrationTests(PostgreSQLFixture fixture)
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

        // Add repositories
        services.AddScoped<ISessionRepository>(provider =>
        {
            var dbContext = provider.GetRequiredService<IdentityDbContext>();
            return new SessionPostgresRepository(dbContext);
        });

        services.AddScoped<IIdentityRepository>(provider =>
        {
            var dbContext = provider.GetRequiredService<IdentityDbContext>();
            var securityProcessor = provider.GetRequiredService<Mamey.Security.SecurityAttributeProcessor>();
            return new IdentityPostgresRepository(dbContext, securityProcessor);
        });

        // Add Security services (needed for Identity repository)
        var securityOptions = new Mamey.Security.SecurityOptions
        {
            Encryption = new Mamey.Security.SecurityOptions.EncryptionOptions
            {
                Enabled = true,
                Key = "12345678901234567890123456789012"
            }
        };
        services.AddSingleton(securityOptions);
        var loggerFactory = services.BuildServiceProvider().GetRequiredService<ILoggerFactory>();
        var encryptorLogger = loggerFactory.CreateLogger<Mamey.Security.Internals.Encryptor>();
        services.AddSingleton<Mamey.Security.IEncryptor>(new Mamey.Security.Internals.Encryptor(encryptorLogger));
        services.AddSingleton<Mamey.Security.IHasher, Mamey.Security.Internals.Hasher>();
        services.AddSingleton<Mamey.Security.IRng, Mamey.Security.Rng>();
        services.AddSingleton<Mamey.Security.ISigner, Mamey.Security.Signer>();
        services.AddSingleton<Mamey.Security.IMd5, Mamey.Security.Md5>();
        services.AddSingleton<Mamey.Security.ISecurityProvider>(sp =>
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

        _serviceProvider = services.BuildServiceProvider();

        // Ensure database is created
        using var scope = _serviceProvider.CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<IdentityDbContext>();
        await context.Database.EnsureCreatedAsync();

        // Get services
        _repository = _serviceProvider.GetRequiredService<ISessionRepository>();
        _identityRepository = _serviceProvider.GetRequiredService<IIdentityRepository>();
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

    [Fact]
    public async Task AddAsync_WithValidSession_ShouldPersistSession()
    {
        // Arrange
        var identity = TestDataFactory.CreateTestIdentity();
        await _identityRepository!.AddAsync(identity);

        var session = new Session(
            new SessionId(),
            identity.Id,
            "access-token",
            "refresh-token",
            DateTime.UtcNow.AddHours(24),
            "192.168.1.1",
            "Mozilla/5.0");

        // Act
        await _repository!.AddAsync(session);

        // Assert
        var retrieved = await _repository.GetAsync(session.Id);
        retrieved.ShouldNotBeNull();
        retrieved.IdentityId.ShouldBe(identity.Id);
        retrieved.AccessToken.ShouldBe("access-token");
        retrieved.RefreshToken.ShouldBe("refresh-token");
        retrieved.Status.ShouldBe(SessionStatus.Active);
    }

    [Fact]
    public async Task GetByRefreshTokenAsync_WithValidToken_ShouldReturnSession()
    {
        // Arrange
        var identity = TestDataFactory.CreateTestIdentity();
        await _identityRepository!.AddAsync(identity);

        var refreshToken = "refresh-token-123";
        var session = new Session(
            new SessionId(),
            identity.Id,
            "access-token",
            refreshToken,
            DateTime.UtcNow.AddHours(24));
        await _repository!.AddAsync(session);

        // Act
        var retrieved = await _repository.GetByRefreshTokenAsync(refreshToken);

        // Assert
        retrieved.ShouldNotBeNull();
        retrieved.Id.ShouldBe(session.Id);
        retrieved.RefreshToken.ShouldBe(refreshToken);
    }

    [Fact]
    public async Task GetByIdentityIdAsync_WithValidIdentity_ShouldReturnSessions()
    {
        // Arrange
        var identity = TestDataFactory.CreateTestIdentity();
        await _identityRepository!.AddAsync(identity);

        var session1 = new Session(
            new SessionId(),
            identity.Id,
            "access-token-1",
            "refresh-token-1",
            DateTime.UtcNow.AddHours(24));
        var session2 = new Session(
            new SessionId(),
            identity.Id,
            "access-token-2",
            "refresh-token-2",
            DateTime.UtcNow.AddHours(24));

        await _repository!.AddAsync(session1);
        await _repository.AddAsync(session2);

        // Act
        var sessions = await _repository.GetByIdentityIdAsync(identity.Id);

        // Assert
        sessions.ShouldNotBeNull();
        sessions.Count.ShouldBeGreaterThanOrEqualTo(2);
        sessions.ShouldContain(s => s.Id == session1.Id);
        sessions.ShouldContain(s => s.Id == session2.Id);
    }

    [Fact]
    public async Task UpdateAsync_WithModifiedSession_ShouldPersistChanges()
    {
        // Arrange
        var identity = TestDataFactory.CreateTestIdentity();
        await _identityRepository!.AddAsync(identity);

        var session = new Session(
            new SessionId(),
            identity.Id,
            "access-token",
            "refresh-token",
            DateTime.UtcNow.AddHours(24));
        await _repository!.AddAsync(session);

        // Act
        session.UpdateTokens("new-access-token", "new-refresh-token");
        await _repository.UpdateAsync(session);

        // Assert
        var retrieved = await _repository.GetAsync(session.Id);
        retrieved.ShouldNotBeNull();
        retrieved.AccessToken.ShouldBe("new-access-token");
        retrieved.RefreshToken.ShouldBe("new-refresh-token");
    }

    // TODO: Implement GetExpiredSessionsAsync in ISessionRepository
    // [Fact]
    // public async Task GetExpiredSessions_ShouldReturnOnlyExpiredSessions()
    // {
    //     // Arrange
    //     var identity = TestDataFactory.CreateTestIdentity();
    //     await _identityRepository!.AddAsync(identity);
    //
    //     var activeSession = new Session(
    //         new SessionId(),
    //         identity.Id,
    //         "access-token-active",
    //         "refresh-token-active",
    //         DateTime.UtcNow.AddHours(1)); // Not expired
    //
    //     var expiredSession = new Session(
    //         new SessionId(),
    //         identity.Id,
    //         "access-token-expired",
    //         "refresh-token-expired",
    //         DateTime.UtcNow.AddHours(-1)); // Expired
    //
    //     await _repository!.AddAsync(activeSession);
    //     await _repository.AddAsync(expiredSession);
    //
    //     // Act
    //     var expiredSessions = await _repository.GetExpiredSessionsAsync();
    //
    //     // Assert
    //     expiredSessions.ShouldNotBeNull();
    //     expiredSessions.ShouldContain(s => s.Id == expiredSession.Id);
    //     expiredSessions.ShouldNotContain(s => s.Id == activeSession.Id);
    // }
}

