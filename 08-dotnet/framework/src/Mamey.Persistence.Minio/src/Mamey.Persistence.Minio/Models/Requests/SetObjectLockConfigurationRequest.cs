using Mamey.Persistence.Minio.Models.DTOs;

namespace Mamey.Persistence.Minio.Models.Requests;

/// <summary>
/// Request for setting object lock configuration.
/// </summary>
public class SetObjectLockConfigurationRequest
{
    /// <summary>
    /// Gets or sets the bucket name.
    /// </summary>
    public string BucketName { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the object lock configuration.
    /// </summary>
    public ObjectLockConfiguration Configuration { get; set; } = new();
}
