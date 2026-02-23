namespace Mamey.FWID.Notifications.Application.Services;

/// <summary>
/// Service for storing notification attachments in MinIO.
/// </summary>
internal interface INotificationStorageService
{
    /// <summary>
    /// Uploads a notification attachment.
    /// </summary>
    Task<string> UploadAttachmentAsync(Stream attachmentStream, string fileName, string contentType, CancellationToken cancellationToken = default);

    /// <summary>
    /// Downloads a notification attachment.
    /// </summary>
    Task<Stream> DownloadAttachmentAsync(string objectName, CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets a presigned URL for a notification attachment.
    /// </summary>
    Task<string> GetPresignedUrlAsync(string objectName, TimeSpan expiry, CancellationToken cancellationToken = default);

    /// <summary>
    /// Deletes a notification attachment.
    /// </summary>
    Task DeleteAttachmentAsync(string objectName, CancellationToken cancellationToken = default);
}







