using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Minio;
using Mamey.Persistence.Minio.Infrastructure.Resilience;
using Microsoft.Extensions.Options;

namespace Mamey.Persistence.Minio;

/// <summary>
/// Configuration options for Minio service.
/// </summary>
public class MinioOptions
{
    /// <summary>
    /// Gets or sets the Minio server endpoint.
    /// </summary>
    public string Endpoint { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the access key for authentication.
    /// </summary>
    public string AccessKey { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the secret key for authentication.
    /// </summary>
    public string SecretKey { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the default bucket name.
    /// </summary>
    public string Bucket { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the region for the Minio server.
    /// </summary>
    public string? Region { get; set; }

    /// <summary>
    /// Gets or sets the session token for temporary credentials.
    /// </summary>
    public string? SessionToken { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether to use SSL/TLS.
    /// </summary>
    public bool UseSSL { get; set; } = true;

    /// <summary>
    /// Gets or sets the timeout for operations in seconds.
    /// </summary>
    public int TimeoutSeconds { get; set; } = 60;

    /// <summary>
    /// Gets or sets the retry count for failed operations.
    /// </summary>
    public int RetryCount { get; set; } = 3;

    /// <summary>
    /// Gets or sets the retry policy configuration.
    /// </summary>
    public RetryPolicy RetryPolicy { get; set; } = new();

    /// <summary>
    /// Gets or sets the circuit breaker policy configuration.
    /// </summary>
    public CircuitBreakerPolicy? CircuitBreaker { get; set; }
}

/// <summary>
/// Extension methods for configuring Minio services.
/// </summary>
public static class Extensions
{
    /// <summary>
    /// Adds Minio services to the service collection.
    /// </summary>
    /// <param name="builder">The Mamey builder.</param>
    /// <returns>The Mamey builder for chaining.</returns>
    public static IMameyBuilder AddMinio(this IMameyBuilder builder)
    {
        var options = builder.GetOptions<MinioOptions>("minio");
        
        // Validate required options
        if (string.IsNullOrEmpty(options.Endpoint))
            throw new ArgumentException("Minio Endpoint is required.", nameof(options));
        if (string.IsNullOrEmpty(options.AccessKey))
            throw new ArgumentException("Minio AccessKey is required.", nameof(options));
        if (string.IsNullOrEmpty(options.SecretKey))
            throw new ArgumentException("Minio SecretKey is required.", nameof(options));

        // Register MinioOptions as IOptions<MinioOptions> for DI consumers (e.g., SimpleMinioService)
        builder.Services.AddSingleton(Options.Create(options));

        // Register Minio client
        builder.Services.AddScoped<IMinioClient>(provider =>
        {
            var clientBuilder = new MinioClient()
                .WithEndpoint(options.Endpoint)
                .WithCredentials(options.AccessKey, options.SecretKey);

            if (options.UseSSL)
                clientBuilder.WithSSL();

            if (!string.IsNullOrEmpty(options.Region))
                clientBuilder.WithRegion(options.Region);

            if (!string.IsNullOrEmpty(options.SessionToken))
                clientBuilder.WithSessionToken(options.SessionToken);

            return clientBuilder.Build();
        });

        // Register resilience infrastructure
        builder.Services.AddSingleton<IRetryPolicyExecutor, RetryPolicyExecutor>();

        // Register domain services
        builder.Services.AddScoped<IBucketService, Services.BucketService>();
        builder.Services.AddScoped<IObjectService, Services.ObjectService>();
        builder.Services.AddScoped<IPresignedUrlService, Services.PresignedUrlService>();
        builder.Services.AddScoped<IBucketPolicyService, Services.BucketPolicyService>();
        builder.Services.AddScoped<ILifecycleService, Services.LifecycleService>();
        builder.Services.AddScoped<IObjectLockService, Services.ObjectLockService>();
        builder.Services.AddScoped<Services.MultipartUploadService>();

        // Register simple Minio service (for backward compatibility)
        builder.Services.AddScoped<SimpleMinioService>();

        return builder;
    }
}