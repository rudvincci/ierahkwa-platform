using Mamey.CQRS;
using Mamey.CQRS.Events;
using Mamey.FWID.Identities.Domain.Repositories;
using Mamey.FWID.Identities.Infrastructure.EF;
using Mamey.FWID.Identities.Infrastructure.EF.Repositories;
using Mamey.FWID.Identities.Tests.Shared.Fixtures;
using Mamey.Security;
using Mamey.Security.Internals;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Xunit;

namespace Mamey.FWID.Identities.Tests.Integration.Queries;

/// <summary>
/// Base class for query integration tests with shared setup.
/// </summary>
public abstract class BaseQueryIntegrationTests : IAsyncLifetime
{
    protected readonly PostgreSQLFixture Fixture;
    protected IServiceProvider? ServiceProvider;
    protected IIdentityRepository? Repository;
    protected IMemoryCache? MemoryCache;

    protected BaseQueryIntegrationTests(PostgreSQLFixture fixture)
    {
        Fixture = fixture;
    }

    public virtual async Task InitializeAsync()
    {
        // Configure services
        var services = new ServiceCollection();

        // Add logging
        services.AddLogging(builder => builder.AddConsole().SetMinimumLevel(LogLevel.Warning));

        // Add Entity Framework Core
        services.AddDbContext<IdentityDbContext>(options =>
        {
            options.UseNpgsql(Fixture.ConnectionString);
            options.EnableSensitiveDataLogging();
        });

        // Add Security services (manual registration for tests)
        var securityOptions = new Mamey.Security.SecurityOptions
        {
            Encryption = new Mamey.Security.SecurityOptions.EncryptionOptions
            {
                Enabled = true,
                Key = "12345678901234567890123456789012" // 32 characters for AES-256
            }
        };
        services.AddSingleton(securityOptions);
        var loggerFactory = services.BuildServiceProvider().GetRequiredService<ILoggerFactory>();
        var encryptorLogger = loggerFactory.CreateLogger<Encryptor>();
        services.AddSingleton<Mamey.Security.IEncryptor>(new Encryptor(encryptorLogger));
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

        // Add memory cache
        services.AddMemoryCache();

        // Add repository
        services.AddScoped<IIdentityRepository>(provider =>
        {
            var dbContext = provider.GetRequiredService<IdentityDbContext>();
            var securityProcessor = provider.GetRequiredService<SecurityAttributeProcessor>();
            return new IdentityPostgresRepository(dbContext, securityProcessor);
        });

        // Allow derived classes to add additional services
        ConfigureServices(services);

        ServiceProvider = services.BuildServiceProvider();

        // Ensure database is created
        using var scope = ServiceProvider.CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<IdentityDbContext>();
        await context.Database.EnsureCreatedAsync();

        // Get services
        Repository = ServiceProvider.GetRequiredService<IIdentityRepository>();
        MemoryCache = ServiceProvider.GetRequiredService<IMemoryCache>();
    }

    public virtual async Task DisposeAsync()
    {
        if (ServiceProvider != null)
        {
            using var scope = ServiceProvider.CreateScope();
            var context = scope.ServiceProvider.GetRequiredService<IdentityDbContext>();
            await context.Database.EnsureDeletedAsync();
        }

        if (ServiceProvider is IDisposable disposable)
        {
            disposable.Dispose();
        }
    }

    /// <summary>
    /// Override to add additional services for specific test classes.
    /// </summary>
    protected virtual void ConfigureServices(IServiceCollection services)
    {
    }
}

