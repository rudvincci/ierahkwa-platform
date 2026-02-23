using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Minio;
using Mamey.Persistence.Minio;
using Mamey.Persistence.Minio.Infrastructure.Resilience;
using Mamey.Persistence.Minio.Services;
using Mamey.Portal.Citizenship.Infrastructure.Persistence;
using Mamey.Portal.Tenant.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using DotNet.Testcontainers.Configurations;
using Testcontainers.Minio;
using Testcontainers.PostgreSql;

namespace Mamey.Portal.Integration.Tests.Fixtures;

public sealed class PortalIntegrationFixture : IAsyncLifetime
{
    private readonly PostgreSqlContainer _postgres;
    private readonly MinioContainer _minio;
    private ServiceProvider _minioServices = null!;

    static PortalIntegrationFixture()
    {
        TestcontainersSettings.ResourceReaperEnabled = false;
    }

    public PortalIntegrationFixture()
    {
        _postgres = new PostgreSqlBuilder()
            .WithImage("postgres:16-alpine")
            .WithDatabase("portal_test")
            .WithUsername("test")
            .WithPassword("test")
            .Build();

        _minio = new MinioBuilder()
            .WithImage("minio/minio:RELEASE.2025-01-20T14-49-07Z")
            .WithCommand("server", "/data", "--console-address", ":9090")
            .WithEnvironment("MINIO_ROOT_USER", "minioadmin")
            .WithEnvironment("MINIO_ROOT_PASSWORD", "minioadmin")
            .Build();
    }

    public string PostgresConnectionString => _postgres.GetConnectionString();

    public IBucketService Buckets => _minioServices.GetRequiredService<IBucketService>();
    public IObjectService Objects => _minioServices.GetRequiredService<IObjectService>();

    public async Task InitializeAsync()
    {
        Console.WriteLine("PortalIntegrationFixture: starting containers.");
        await Task.WhenAll(_postgres.StartAsync(), _minio.StartAsync());
        Console.WriteLine("PortalIntegrationFixture: containers started.");

        var services = new ServiceCollection();
        var options = new MinioOptions
        {
            Endpoint = $"{_minio.Hostname}:{_minio.GetMappedPublicPort(9000)}",
            AccessKey = "minioadmin",
            SecretKey = "minioadmin",
            UseSSL = false,
            RetryPolicy = new RetryPolicy
            {
                MaxRetries = 3,
                InitialDelay = TimeSpan.FromMilliseconds(100),
                BackoffMultiplier = 2.0,
                UseJitter = true
            },
            CircuitBreaker = new CircuitBreakerPolicy
            {
                FailureThreshold = 5,
                DurationOfBreak = TimeSpan.FromMilliseconds(5000),
                SamplingDuration = TimeSpan.FromMilliseconds(10000)
            }
        };

        Console.WriteLine("PortalIntegrationFixture: configuring Minio services.");
        services.AddSingleton(Options.Create(options));
        services.AddSingleton<IMinioClient>(provider =>
        {
            var cfg = provider.GetRequiredService<IOptions<MinioOptions>>().Value;
            return new MinioClient()
                .WithEndpoint(cfg.Endpoint)
                .WithCredentials(cfg.AccessKey, cfg.SecretKey)
                .WithSSL(cfg.UseSSL)
                .Build();
        });

        services.AddSingleton<IRetryPolicyExecutor, RetryPolicyExecutor>();
        services.AddScoped<IBucketService, BucketService>();
        services.AddScoped<IObjectService, ObjectService>();
        services.AddScoped<IPresignedUrlService, PresignedUrlService>();
        services.AddScoped<IBucketPolicyService, BucketPolicyService>();
        services.AddScoped<ILifecycleService, LifecycleService>();
        services.AddScoped<IObjectLockService, ObjectLockService>();

        _minioServices = services.BuildServiceProvider();

        Console.WriteLine("PortalIntegrationFixture: running EF migrations.");
        await EnsureMigratedAsync();
        Console.WriteLine("PortalIntegrationFixture: migrations complete.");
    }

    public async Task DisposeAsync()
    {
        await _minio.DisposeAsync();
        await _postgres.DisposeAsync();
        _minioServices.Dispose();
    }

    public CitizenshipDbContext CreateCitizenshipDbContext()
    {
        var options = new DbContextOptionsBuilder<CitizenshipDbContext>()
            .UseNpgsql(PostgresConnectionString, o => o.MigrationsAssembly("Mamey.Portal.Citizenship.Infrastructure"))
            .Options;

        return new CitizenshipDbContext(options);
    }

    public TenantDbContext CreateTenantDbContext()
    {
        var options = new DbContextOptionsBuilder<TenantDbContext>()
            .UseNpgsql(PostgresConnectionString, o => o.MigrationsAssembly("Mamey.Portal.Tenant.Infrastructure"))
            .Options;

        return new TenantDbContext(options);
    }

    private async Task EnsureMigratedAsync()
    {
        await using var citizenshipDb = CreateCitizenshipDbContext();
        await using var tenantDb = CreateTenantDbContext();

        await citizenshipDb.Database.MigrateAsync();
        await tenantDb.Database.MigrateAsync();
    }
}

[CollectionDefinition("PortalIntegration")]
public sealed class PortalIntegrationCollection : ICollectionFixture<PortalIntegrationFixture>
{
}
