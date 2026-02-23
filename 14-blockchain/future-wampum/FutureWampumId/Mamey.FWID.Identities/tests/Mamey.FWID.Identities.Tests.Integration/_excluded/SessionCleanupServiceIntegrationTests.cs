using Mamey.FWID.Identities.Domain.Entities;
using Mamey.FWID.Identities.Domain.Repositories;
using Mamey.FWID.Identities.Domain.ValueObjects;
using Mamey.FWID.Identities.Infrastructure.EF;
using Mamey.FWID.Identities.Infrastructure.EF.Repositories;
using Mamey.FWID.Identities.Infrastructure.Services.Cleanup;
using Mamey.FWID.Identities.Tests.Shared.Factories;
using Mamey.FWID.Identities.Tests.Shared.Fixtures;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Shouldly;
using Xunit;

namespace Mamey.FWID.Identities.Tests.Integration.Services;

/// <summary>
/// Integration tests for SessionCleanupService background worker.
/// </summary>
[Collection("Integration")]
public class SessionCleanupServiceIntegrationTests : IClassFixture<PostgreSQLFixture>, IAsyncLifetime
{
    private readonly PostgreSQLFixture _fixture;
    private IServiceProvider? _serviceProvider;
    private ISessionRepository? _sessionRepository;
    private IIdentityRepository? _identityRepository;
    private SessionCleanupService? _cleanupService;

    public SessionCleanupServiceIntegrationTests(PostgreSQLFixture fixture)
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

        // Add cleanup options
        var cleanupOptions = new CleanupOptions
        {
            SessionCleanupEnabled = true,
            SessionCleanupInterval = TimeSpan.FromSeconds(1) // Fast interval for testing
        };
        services.AddSingleton(Options.Create(cleanupOptions));

        // Add cleanup service
        services.AddScoped<SessionCleanupService>();

        _serviceProvider = services.BuildServiceProvider();

        // Ensure database is created
        using var scope = _serviceProvider.CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<IdentityDbContext>();
        await context.Database.EnsureCreatedAsync();

        // Get services
        _sessionRepository = _serviceProvider.GetRequiredService<ISessionRepository>();
        _identityRepository = _serviceProvider.GetRequiredService<IIdentityRepository>();
        _cleanupService = _serviceProvider.GetRequiredService<SessionCleanupService>();
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
    public async Task ExecuteAsync_WithExpiredSessions_ShouldRevokeExpiredSessions()
    {
        // Arrange
        var identity = TestDataFactory.CreateTestIdentity();
        await _identityRepository!.AddAsync(identity);

        // Create an expired session
        var expiredSession = new Session(
            new SessionId(),
            identity.Id,
            "access-token",
            "refresh-token",
            DateTime.UtcNow.AddHours(-1)); // Expired
        await _sessionRepository!.AddAsync(expiredSession);

        // Create an active session
        var activeSession = new Session(
            new SessionId(),
            identity.Id,
            "access-token-active",
            "refresh-token-active",
            DateTime.UtcNow.AddHours(1)); // Not expired
        await _sessionRepository.AddAsync(activeSession);

        // Verify expired session exists and is active
        var expiredBefore = await _sessionRepository.GetAsync(expiredSession.Id);
        expiredBefore!.Status.ShouldBe(SessionStatus.Active);

        // Act - Execute cleanup manually
        using var scope = _serviceProvider!.CreateScope();
        var cleanupService = scope.ServiceProvider.GetRequiredService<SessionCleanupService>();
        // BackgroundService.ExecuteAsync is protected, so we need to start/stop the service
        await cleanupService.StartAsync(CancellationToken.None);
        await Task.Delay(100); // Give it time to process
        await cleanupService.StopAsync(CancellationToken.None);

        // Assert
        var expiredAfter = await _sessionRepository.GetAsync(expiredSession.Id);
        expiredAfter!.Status.ShouldBe(SessionStatus.Revoked);
        expiredAfter.RevokedAt.ShouldNotBeNull();

        // Active session should remain active
        var activeAfter = await _sessionRepository.GetAsync(activeSession.Id);
        activeAfter!.Status.ShouldBe(SessionStatus.Active);
    }
}

