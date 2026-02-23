using Mamey.Persistence.Minio.Models.DTOs;

namespace Mamey.Persistence.Minio.Models.Requests;

/// <summary>
/// Request for setting bucket notifications.
/// </summary>
public class SetBucketNotificationsRequest
{
    /// <summary>
    /// Gets or sets the bucket name.
    /// </summary>
    public string BucketName { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the notification configuration.
    /// </summary>
    public NotificationConfiguration Configuration { get; set; } = new();
}
