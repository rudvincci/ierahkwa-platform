using Mamey.Persistence.Minio;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace Mamey.FWID.Notifications.Infrastructure.MinIO.Services;

/// <summary>
/// Background service that ensures MinIO buckets exist on startup.
/// </summary>
internal class BucketInitializationService : IHostedService
{
    private readonly IBucketService _bucketService;
    private readonly MinioOptions _options;
    private readonly ILogger<BucketInitializationService> _logger;
    private const string BucketName = "notifications";

    public BucketInitializationService(
        IBucketService bucketService,
        IOptions<MinioOptions> options,
        ILogger<BucketInitializationService> logger)
    {
        _bucketService = bucketService ?? throw new ArgumentNullException(nameof(bucketService));
        _options = options?.Value ?? throw new ArgumentNullException(nameof(options));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    public async Task StartAsync(CancellationToken cancellationToken)
    {
        _logger.LogInformation("Initializing MinIO buckets for Notifications service...");

        try
        {
            // Check if bucket exists
            var exists = await _bucketService.BucketExistsAsync(BucketName, cancellationToken);

            if (!exists)
            {
                _logger.LogInformation("Bucket '{BucketName}' does not exist. Creating it...", BucketName);
                await _bucketService.MakeBucketAsync(BucketName, cancellationToken);
                _logger.LogInformation("Successfully created bucket '{BucketName}'", BucketName);
            }
            else
            {
                _logger.LogInformation("Bucket '{BucketName}' already exists", BucketName);
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to initialize MinIO bucket '{BucketName}'", BucketName);
            // Don't throw - allow service to start even if bucket initialization fails
            // The bucket will be created on first use or can be created manually
        }
    }

    public Task StopAsync(CancellationToken cancellationToken)
    {
        // Nothing to clean up
        return Task.CompletedTask;
    }
}







