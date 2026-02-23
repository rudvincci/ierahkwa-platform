using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Minio;
using Mamey.Persistence.Minio;
using Mamey.Persistence.Minio.Infrastructure.Resilience;
using Mamey.Persistence.Minio.Services;
using Testcontainers.Minio;
using Testcontainers;
using Xunit;

namespace Mamey.Persistence.Minio.Tests.Integration;

/// <summary>
/// Test fixture for Minio integration tests using TestContainers.
/// </summary>
public class MinioContainerFixture : IAsyncLifetime
{
    private readonly MinioContainer _minioContainer;
    public IServiceProvider ServiceProvider { get; private set; } = null!;

    public MinioContainerFixture()
    {
        _minioContainer = new MinioBuilder()
            .WithImage("minio/minio:latest")
            .WithPortBinding(9000, 9000)
            .WithPortBinding(9090, 9090)
            .WithCommand("server", "/data", "--console-address", ":9090")
            .WithEnvironment("MINIO_ROOT_USER", "minioadmin")
            .WithEnvironment("MINIO_ROOT_PASSWORD", "minioadmin")
            .Build();
    }

    public async Task InitializeAsync()
    {
        // Start the Minio container
        await _minioContainer.StartAsync();

        // Wait a bit for Minio to be ready
        await Task.Delay(2000);

        // Configure services
        var services = new ServiceCollection();

        // Add logging
        services.AddLogging(builder => builder.AddConsole().SetMinimumLevel(LogLevel.Warning));

        // Configure Minio options
        var minioOptions = new MinioOptions
        {
            Endpoint = $"{_minioContainer.Hostname}:{_minioContainer.GetMappedPublicPort(9000)}",
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

        services.AddSingleton(Options.Create(minioOptions));

        // Add Minio client
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

        // Add Minio services
        services.AddScoped<IBucketService, BucketService>();
        services.AddScoped<IObjectService, ObjectService>();
        services.AddScoped<IPresignedUrlService, PresignedUrlService>();
        services.AddScoped<IBucketPolicyService, BucketPolicyService>();
        services.AddScoped<ILifecycleService, LifecycleService>();
        services.AddScoped<IObjectLockService, ObjectLockService>();

        ServiceProvider = services.BuildServiceProvider();
    }

    public async Task DisposeAsync()
    {
        (ServiceProvider as IDisposable)?.Dispose();
        await _minioContainer.DisposeAsync();
    }
}
