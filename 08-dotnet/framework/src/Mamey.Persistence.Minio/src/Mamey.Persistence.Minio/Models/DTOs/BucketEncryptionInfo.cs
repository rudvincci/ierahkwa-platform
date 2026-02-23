namespace Mamey.Persistence.Minio.Models.DTOs;

/// <summary>
/// Represents the encryption configuration of a bucket.
/// </summary>
public class BucketEncryptionInfo
{
    /// <summary>
    /// Gets or sets the encryption rules.
    /// </summary>
    public List<EncryptionRule> Rules { get; set; } = new();
}

/// <summary>
/// Represents an encryption rule.
/// </summary>
public class EncryptionRule
{
    /// <summary>
    /// Gets or sets the server-side encryption configuration.
    /// </summary>
    public ServerSideEncryptionConfiguration ApplyServerSideEncryptionByDefault { get; set; } = new();

    /// <summary>
    /// Gets or sets a value indicating whether the rule applies to all objects.
    /// </summary>
    public bool BucketKeyEnabled { get; set; }
}

/// <summary>
/// Represents server-side encryption configuration.
/// </summary>
public class ServerSideEncryptionConfiguration
{
    /// <summary>
    /// Gets or sets the server-side encryption algorithm.
    /// </summary>
    public string SSEAlgorithm { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the KMS master key ID (for SSE-KMS).
    /// </summary>
    public string? KMSMasterKeyID { get; set; }
}
