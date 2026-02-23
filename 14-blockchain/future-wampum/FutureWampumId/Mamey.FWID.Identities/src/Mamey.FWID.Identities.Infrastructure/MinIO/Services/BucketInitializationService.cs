using Mamey.FWID.Identities.Application.Services;
using Mamey.Persistence.Minio;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace Mamey.FWID.Identities.Infrastructure.MinIO.Services;

/// <summary>
/// Background service that ensures MinIO buckets exist on startup.
/// </summary>
internal class BucketInitializationService : IHostedService
{
    private readonly IServiceScopeFactory _serviceScopeFactory;
    private readonly MinioOptions _options;
    private readonly ILogger<BucketInitializationService> _logger;
    private static readonly string[] RequiredBuckets = { "fwid-biometrics", "fwid-documents" };

    public BucketInitializationService(
        IServiceScopeFactory serviceScopeFactory,
        IOptions<MinioOptions> options,
        ILogger<BucketInitializationService> logger)
    {
        _serviceScopeFactory = serviceScopeFactory ?? throw new ArgumentNullException(nameof(serviceScopeFactory));
        _options = options?.Value ?? throw new ArgumentNullException(nameof(options));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    public async Task StartAsync(CancellationToken cancellationToken)
    {
        _logger.LogInformation("Initializing MinIO buckets for Identities service...");

        using var scope = _serviceScopeFactory.CreateScope();
        var bucketService = scope.ServiceProvider.GetRequiredService<IBucketService>();

        foreach (var bucketName in RequiredBuckets)
        {
            try
            {
                // Check if bucket exists
                var exists = await bucketService.BucketExistsAsync(bucketName, cancellationToken);

                if (!exists)
                {
                    _logger.LogInformation("Bucket '{BucketName}' does not exist. Creating it...", bucketName);
                    await bucketService.MakeBucketAsync(bucketName, cancellationToken);
                    _logger.LogInformation("Successfully created bucket '{BucketName}'", bucketName);
                }
                else
                {
                    _logger.LogInformation("Bucket '{BucketName}' already exists", bucketName);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to initialize MinIO bucket '{BucketName}'", bucketName);
                // Continue with other buckets - don't throw
            }
        }
    }

    public Task StopAsync(CancellationToken cancellationToken)
    {
        // Nothing to clean up
        return Task.CompletedTask;
    }
}


