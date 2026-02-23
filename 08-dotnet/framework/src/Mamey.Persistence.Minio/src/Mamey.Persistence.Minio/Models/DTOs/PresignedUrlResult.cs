using System;

namespace Mamey.Persistence.Minio.Models.DTOs;

/// <summary>
/// Represents the result of a presigned URL operation.
/// </summary>
public class PresignedUrlResult
{
    /// <summary>
    /// Gets or sets the presigned URL.
    /// </summary>
    public string Url { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the expiration date of the URL.
    /// </summary>
    public DateTime Expiration { get; set; }

    /// <summary>
    /// Gets or sets the HTTP method for the URL.
    /// </summary>
    public string Method { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the bucket name.
    /// </summary>
    public string BucketName { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the object name.
    /// </summary>
    public string ObjectName { get; set; } = string.Empty;
}

/// <summary>
/// Represents the result of a presigned POST policy operation.
/// </summary>
public class PresignedPostPolicyResult
{
    /// <summary>
    /// Gets or sets the URL for the POST request.
    /// </summary>
    public string Url { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the form fields for the POST request.
    /// </summary>
    public Dictionary<string, string> FormFields { get; set; } = new();

    /// <summary>
    /// Gets or sets the expiration date of the policy.
    /// </summary>
    public DateTime Expiration { get; set; }
}
