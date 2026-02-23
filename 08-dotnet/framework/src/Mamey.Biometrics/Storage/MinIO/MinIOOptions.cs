using System.ComponentModel.DataAnnotations;

namespace Mamey.Biometrics.Storage.MinIO;

/// <summary>
/// MinIO configuration options.
/// </summary>
public class MinIOOptions
{
    /// <summary>
    /// Configuration section name for appsettings.json
    /// </summary>
    public const string SectionName = "MinIO";

    /// <summary>
    /// MinIO endpoint URL
    /// </summary>
    [Required]
    public string Endpoint { get; set; } = string.Empty;

    /// <summary>
    /// Access key
    /// </summary>
    [Required]
    public string AccessKey { get; set; } = string.Empty;

    /// <summary>
    /// Secret key
    /// </summary>
    [Required]
    public string SecretKey { get; set; } = string.Empty;

    /// <summary>
    /// Bucket name for storing images
    /// </summary>
    [Required]
    public string BucketName { get; set; } = "biometric-images";

    /// <summary>
    /// Use SSL
    /// </summary>
    public bool UseSSL { get; set; } = false;

    /// <summary>
    /// Region
    /// </summary>
    public string Region { get; set; } = "us-east-1";

    /// <summary>
    /// Session token (for temporary credentials)
    /// </summary>
    public string? SessionToken { get; set; }

    /// <summary>
    /// Connection timeout in seconds
    /// </summary>
    [Range(1, 300)]
    public int ConnectionTimeoutSeconds { get; set; } = 30;

    /// <summary>
    /// Read/write timeout in seconds
    /// </summary>
    [Range(1, 300)]
    public int ReadWriteTimeoutSeconds { get; set; } = 60;
}
