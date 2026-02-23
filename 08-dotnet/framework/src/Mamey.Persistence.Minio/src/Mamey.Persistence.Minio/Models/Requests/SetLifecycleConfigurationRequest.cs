using Mamey.Persistence.Minio.Models.DTOs;

namespace Mamey.Persistence.Minio.Models.Requests;

/// <summary>
/// Request for setting lifecycle configuration.
/// </summary>
public class SetLifecycleConfigurationRequest
{
    /// <summary>
    /// Gets or sets the bucket name.
    /// </summary>
    public string BucketName { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the lifecycle configuration.
    /// </summary>
    public LifecycleConfiguration Configuration { get; set; } = new();
}
