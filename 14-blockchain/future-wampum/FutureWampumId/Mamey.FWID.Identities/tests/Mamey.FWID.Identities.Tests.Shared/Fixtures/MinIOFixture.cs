using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Minio;
using Mamey.Persistence.Minio;
using Mamey.Persistence.Minio.Services;
using Mamey.Persistence.Minio.Infrastructure.Resilience;
using Testcontainers.Minio;
using Xunit;

namespace Mamey.FWID.Identities.Tests.Shared.Fixtures;

/// <summary>
/// Test fixture for MinIO integration tests using Testcontainers.NET.
/// </summary>
public class MinIOFixture : IAsyncLifetime
{
    private readonly MinioContainer _minioContainer;
    public IServiceProvider ServiceProvider { get; private set; } = null!;
    public string Endpoint { get; private set; } = string.Empty;
    public string AccessKey { get; private set; } = "minioadmin";
    public string SecretKey { get; private set; } = "minioadmin";
    public IBucketService BucketService { get; private set; } = null!;
    public IObjectService ObjectService { get; private set; } = null!;
    public IPresignedUrlService PresignedUrlService { get; private set; } = null!;

    public MinIOFixture()
    {
        _minioContainer = new MinioBuilder()
            .WithImage("minio/minio:latest")
            .WithPortBinding(9000, true)
            .WithPortBinding(9090, true)
            .WithCommand("server", "/data", "--console-address", ":9090")
            .WithEnvironment("MINIO_ROOT_USER", AccessKey)
            .WithEnvironment("MINIO_ROOT_PASSWORD", SecretKey)
            .Build();
    }

    public async Task InitializeAsync()
    {
        // Start the MinIO container
        await _minioContainer.StartAsync();

        // Wait a bit for MinIO to be ready
        await Task.Delay(2000);

        var publicPort = _minioContainer.GetMappedPublicPort(9000);
        Endpoint = $"{_minioContainer.Hostname}:{publicPort}";

        // Configure services
        var services = new ServiceCollection();

        // Add logging
        services.AddLogging(builder => builder.AddConsole().SetMinimumLevel(LogLevel.Warning));

        // Configure MinIO options
        var minioOptions = new MinioOptions
        {
            Endpoint = Endpoint,
            AccessKey = AccessKey,
            SecretKey = SecretKey,
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

        services.AddSingleton(Options.Create(minioOptions));

        // Add MinIO client
        services.AddSingleton<IMinioClient>(provider =>
        {
            var options = provider.GetRequiredService<IOptions<MinioOptions>>().Value;
            return new MinioClient()
                .WithEndpoint(options.Endpoint)
                .WithCredentials(options.AccessKey, options.SecretKey)
                .WithSSL(options.UseSSL)
                .Build();
        });

        // Add retry policy executor
        services.AddSingleton<IRetryPolicyExecutor, RetryPolicyExecutor>();

        // Add MinIO services
        services.AddScoped<IBucketService, BucketService>();
        services.AddScoped<IObjectService, ObjectService>();
        services.AddScoped<IPresignedUrlService, PresignedUrlService>();
        services.AddScoped<IBucketPolicyService, BucketPolicyService>();
        services.AddScoped<ILifecycleService, LifecycleService>();
        services.AddScoped<IObjectLockService, ObjectLockService>();

        ServiceProvider = services.BuildServiceProvider();

        // Get services
        BucketService = ServiceProvider.GetRequiredService<IBucketService>();
        ObjectService = ServiceProvider.GetRequiredService<IObjectService>();
        PresignedUrlService = ServiceProvider.GetRequiredService<IPresignedUrlService>();
    }

    public async Task DisposeAsync()
    {
        if (ServiceProvider is IDisposable disposable)
        {
            disposable.Dispose();
        }
        await _minioContainer.DisposeAsync();
    }
}

