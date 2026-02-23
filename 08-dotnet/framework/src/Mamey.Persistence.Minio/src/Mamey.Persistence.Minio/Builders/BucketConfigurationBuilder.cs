using Mamey.Persistence.Minio.Models.DTOs;
using Mamey.Persistence.Minio.Models.Requests;

namespace Mamey.Persistence.Minio.Builders;

/// <summary>
/// Fluent builder for configuring bucket settings.
/// </summary>
public class BucketConfigurationBuilder
{
    private readonly string _bucketName;
    private readonly Dictionary<string, string> _tags = new();
    private readonly List<LifecycleRule> _lifecycleRules = new();
    private readonly List<NotificationConfiguration> _notifications = new();
    private bool _versioningEnabled = false;
    private bool _versioningSuspended = false;
    private bool _mfaDeleteEnabled = false;
    private ObjectLockConfiguration? _objectLockConfig;
    private string? _policyJson;
    private string? _encryptionAlgorithm;
    private string? _encryptionKeyId;

    internal BucketConfigurationBuilder(string bucketName)
    {
        _bucketName = bucketName;
    }

    /// <summary>
    /// Enables versioning for the bucket.
    /// </summary>
    /// <param name="mfaDeleteEnabled">Whether MFA delete is enabled.</param>
    /// <returns>The builder instance.</returns>
    public BucketConfigurationBuilder WithVersioning(bool mfaDeleteEnabled = false)
    {
        _versioningEnabled = true;
        _versioningSuspended = false;
        _mfaDeleteEnabled = mfaDeleteEnabled;
        return this;
    }

    /// <summary>
    /// Suspends versioning for the bucket.
    /// </summary>
    /// <returns>The builder instance.</returns>
    public BucketConfigurationBuilder SuspendVersioning()
    {
        _versioningEnabled = false;
        _versioningSuspended = true;
        return this;
    }

    /// <summary>
    /// Disables versioning for the bucket.
    /// </summary>
    /// <returns>The builder instance.</returns>
    public BucketConfigurationBuilder DisableVersioning()
    {
        _versioningEnabled = false;
        _versioningSuspended = false;
        return this;
    }

    /// <summary>
    /// Adds a tag to the bucket.
    /// </summary>
    /// <param name="key">The tag key.</param>
    /// <param name="value">The tag value.</param>
    /// <returns>The builder instance.</returns>
    public BucketConfigurationBuilder WithTag(string key, string value)
    {
        _tags[key] = value;
        return this;
    }

    /// <summary>
    /// Adds multiple tags to the bucket.
    /// </summary>
    /// <param name="tags">The tags dictionary.</param>
    /// <returns>The builder instance.</returns>
    public BucketConfigurationBuilder WithTags(Dictionary<string, string> tags)
    {
        foreach (var kvp in tags)
        {
            _tags[kvp.Key] = kvp.Value;
        }
        return this;
    }

    /// <summary>
    /// Adds a lifecycle rule to the bucket.
    /// </summary>
    /// <param name="rule">The lifecycle rule.</param>
    /// <returns>The builder instance.</returns>
    public BucketConfigurationBuilder WithLifecycleRule(LifecycleRule rule)
    {
        _lifecycleRules.Add(rule);
        return this;
    }

    /// <summary>
    /// Adds a lifecycle rule with expiration.
    /// </summary>
    /// <param name="id">The rule ID.</param>
    /// <param name="prefix">The object prefix filter.</param>
    /// <param name="expirationDays">The expiration days.</param>
    /// <param name="status">The rule status.</param>
    /// <returns>The builder instance.</returns>
    public BucketConfigurationBuilder WithExpirationRule(string id, string? prefix = null, int? expirationDays = null, LifecycleRuleStatus status = LifecycleRuleStatus.Enabled)
    {
        var rule = new LifecycleRule
        {
            Id = id,
            Status = status,
            Filter = new LifecycleFilter { Prefix = prefix }
        };

        if (expirationDays.HasValue)
        {
            rule.Expiration = new LifecycleExpiration { Days = expirationDays.Value };
        }

        _lifecycleRules.Add(rule);
        return this;
    }

    /// <summary>
    /// Adds a lifecycle rule with transition.
    /// </summary>
    /// <param name="id">The rule ID.</param>
    /// <param name="prefix">The object prefix filter.</param>
    /// <param name="transitionDays">The transition days.</param>
    /// <param name="storageClass">The target storage class.</param>
    /// <param name="status">The rule status.</param>
    /// <returns>The builder instance.</returns>
    public BucketConfigurationBuilder WithTransitionRule(string id, string? prefix = null, int? transitionDays = null, string storageClass = "STANDARD_IA", LifecycleRuleStatus status = LifecycleRuleStatus.Enabled)
    {
        var rule = new LifecycleRule
        {
            Id = id,
            Status = status,
            Filter = new LifecycleFilter { Prefix = prefix }
        };

        if (transitionDays.HasValue)
        {
            rule.Transitions.Add(new LifecycleTransition
            {
                Days = transitionDays.Value,
                StorageClass = storageClass
            });
        }

        _lifecycleRules.Add(rule);
        return this;
    }

    /// <summary>
    /// Sets object lock configuration for the bucket.
    /// </summary>
    /// <param name="enabled">Whether object lock is enabled.</param>
    /// <param name="defaultRetentionMode">The default retention mode.</param>
    /// <param name="defaultRetentionDays">The default retention days.</param>
    /// <returns>The builder instance.</returns>
    public BucketConfigurationBuilder WithObjectLock(bool enabled = true, ObjectRetentionMode? defaultRetentionMode = null, int? defaultRetentionDays = null)
    {
        _objectLockConfig = new ObjectLockConfiguration
        {
            ObjectLockEnabled = enabled
        };

        if (defaultRetentionMode.HasValue && defaultRetentionDays.HasValue)
        {
            _objectLockConfig.DefaultRetention = new DefaultRetention
            {
                Mode = defaultRetentionMode.Value,
                Days = defaultRetentionDays.Value
            };
        }

        return this;
    }

    /// <summary>
    /// Sets bucket policy.
    /// </summary>
    /// <param name="policyJson">The policy JSON string.</param>
    /// <returns>The builder instance.</returns>
    public BucketConfigurationBuilder WithPolicy(string policyJson)
    {
        _policyJson = policyJson;
        return this;
    }

    /// <summary>
    /// Sets server-side encryption for the bucket.
    /// </summary>
    /// <param name="algorithm">The encryption algorithm.</param>
    /// <param name="keyId">The key ID for KMS encryption.</param>
    /// <returns>The builder instance.</returns>
    public BucketConfigurationBuilder WithEncryption(string algorithm = "AES256", string? keyId = null)
    {
        _encryptionAlgorithm = algorithm;
        _encryptionKeyId = keyId;
        return this;
    }

    /// <summary>
    /// Adds a notification configuration.
    /// </summary>
    /// <param name="notification">The notification configuration.</param>
    /// <returns>The builder instance.</returns>
    public BucketConfigurationBuilder WithNotification(NotificationConfiguration notification)
    {
        _notifications.Add(notification);
        return this;
    }

    /// <summary>
    /// Builds the bucket configuration.
    /// </summary>
    /// <returns>The bucket configuration.</returns>
    public BucketConfiguration Build()
    {
        return new BucketConfiguration
        {
            BucketName = _bucketName,
            VersioningEnabled = _versioningEnabled,
            VersioningSuspended = _versioningSuspended,
            MfaDeleteEnabled = _mfaDeleteEnabled,
            Tags = _tags.Count > 0 ? _tags : null,
            LifecycleRules = _lifecycleRules.Count > 0 ? _lifecycleRules : null,
            ObjectLockConfiguration = _objectLockConfig,
            PolicyJson = _policyJson,
            EncryptionAlgorithm = _encryptionAlgorithm,
            EncryptionKeyId = _encryptionKeyId,
            Notifications = _notifications.Count > 0 ? _notifications : null
        };
    }
}

/// <summary>
/// Configuration for bucket settings.
/// </summary>
public class BucketConfiguration
{
    /// <summary>
    /// Gets or sets the bucket name.
    /// </summary>
    public string BucketName { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets whether versioning is enabled.
    /// </summary>
    public bool VersioningEnabled { get; set; }

    /// <summary>
    /// Gets or sets whether versioning is suspended.
    /// </summary>
    public bool VersioningSuspended { get; set; }

    /// <summary>
    /// Gets or sets whether MFA delete is enabled.
    /// </summary>
    public bool MfaDeleteEnabled { get; set; }

    /// <summary>
    /// Gets or sets the bucket tags.
    /// </summary>
    public Dictionary<string, string>? Tags { get; set; }

    /// <summary>
    /// Gets or sets the lifecycle rules.
    /// </summary>
    public List<LifecycleRule>? LifecycleRules { get; set; }

    /// <summary>
    /// Gets or sets the object lock configuration.
    /// </summary>
    public ObjectLockConfiguration? ObjectLockConfiguration { get; set; }

    /// <summary>
    /// Gets or sets the bucket policy JSON.
    /// </summary>
    public string? PolicyJson { get; set; }

    /// <summary>
    /// Gets or sets the encryption algorithm.
    /// </summary>
    public string? EncryptionAlgorithm { get; set; }

    /// <summary>
    /// Gets or sets the encryption key ID.
    /// </summary>
    public string? EncryptionKeyId { get; set; }

    /// <summary>
    /// Gets or sets the notification configurations.
    /// </summary>
    public List<NotificationConfiguration>? Notifications { get; set; }
}
