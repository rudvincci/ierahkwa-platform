using Mamey.FWID.Notifications.Application.Services;
using Mamey.Persistence.Minio;
using Mamey.Persistence.Minio.Models.DTOs;
using Mamey.Persistence.Minio.Models.Requests;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace Mamey.FWID.Notifications.Infrastructure.MinIO.Services;

/// <summary>
/// Service for storing and retrieving notification attachments in MinIO.
/// Inherits from generic MinIO services and provides service-specific methods.
/// </summary>
internal class NotificationStorageService : INotificationStorageService
{
    private readonly IObjectService _objectService;
    private readonly IPresignedUrlService _presignedUrlService;
    private readonly MinioOptions _options;
    private readonly ILogger<NotificationStorageService> _logger;
    private const string BucketName = "notifications";

    public NotificationStorageService(
        IObjectService objectService,
        IPresignedUrlService presignedUrlService,
        IOptions<MinioOptions> options,
        ILogger<NotificationStorageService> logger)
    {
        _objectService = objectService ?? throw new ArgumentNullException(nameof(objectService));
        _presignedUrlService = presignedUrlService ?? throw new ArgumentNullException(nameof(presignedUrlService));
        _options = options?.Value ?? throw new ArgumentNullException(nameof(options));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    public async Task<string> UploadAttachmentAsync(Stream attachmentStream, string fileName, string contentType, CancellationToken cancellationToken = default)
    {
        if (attachmentStream == null)
            throw new ArgumentNullException(nameof(attachmentStream));
        if (string.IsNullOrEmpty(fileName))
            throw new ArgumentException("File name cannot be null or empty.", nameof(fileName));

        var objectName = GenerateObjectName(fileName);

        _logger.LogInformation("Uploading notification attachment: ObjectName={ObjectName}, ContentType={ContentType}",
            objectName, contentType);

        var metadata = new Dictionary<string, string>
        {
            { "fileName", fileName },
            { "contentType", contentType },
            { "uploadedAt", DateTime.UtcNow.ToString("O") }
        };

        await _objectService.UploadStreamAsync(
            BucketName,
            objectName,
            attachmentStream,
            contentType,
            metadata,
            cancellationToken);

        _logger.LogInformation("Successfully uploaded notification attachment: ObjectName={ObjectName}", objectName);

        return objectName;
    }

    public async Task<Stream> DownloadAttachmentAsync(string objectName, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrEmpty(objectName))
            throw new ArgumentException("Object name cannot be null or empty.", nameof(objectName));

        _logger.LogInformation("Downloading notification attachment: ObjectName={ObjectName}", objectName);

        var stream = await _objectService.DownloadAsStreamAsync(BucketName, objectName, null, cancellationToken);

        _logger.LogInformation("Successfully downloaded notification attachment: ObjectName={ObjectName}", objectName);

        return stream;
    }

    public async Task<string> GetPresignedUrlAsync(string objectName, TimeSpan expiry, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrEmpty(objectName))
            throw new ArgumentException("Object name cannot be null or empty.", nameof(objectName));

        _logger.LogInformation("Generating presigned URL for notification attachment: ObjectName={ObjectName}, Expiry={ExpirySeconds}s",
            objectName, expiry.TotalSeconds);

        var request = new PresignedUrlRequest
        {
            BucketName = BucketName,
            ObjectName = objectName,
            ExpiresInSeconds = (int)expiry.TotalSeconds
        };

        var result = await _presignedUrlService.PresignedGetObjectAsync(request, cancellationToken);

        _logger.LogInformation("Successfully generated presigned URL for notification attachment: ObjectName={ObjectName}",
            objectName);

        return result.Url;
    }

    public async Task DeleteAttachmentAsync(string objectName, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrEmpty(objectName))
            throw new ArgumentException("Object name cannot be null or empty.", nameof(objectName));

        _logger.LogInformation("Deleting notification attachment: ObjectName={ObjectName}", objectName);

        await _objectService.RemoveObjectAsync(BucketName, objectName, cancellationToken);

        _logger.LogInformation("Successfully deleted notification attachment: ObjectName={ObjectName}", objectName);
    }

    /// <summary>
    /// Generates an object name using the pattern: {timestamp}/{fileName}
    /// </summary>
    private static string GenerateObjectName(string fileName)
    {
        var timestamp = DateTime.UtcNow.ToString("yyyyMMddHHmmss");
        var sanitizedFileName = Path.GetFileName(fileName); // Remove path components
        return $"{timestamp}/{sanitizedFileName}";
    }
}







