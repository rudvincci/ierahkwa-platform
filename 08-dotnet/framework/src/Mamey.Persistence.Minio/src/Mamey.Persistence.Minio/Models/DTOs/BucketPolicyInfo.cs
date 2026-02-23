namespace Mamey.Persistence.Minio.Models.DTOs;

/// <summary>
/// Represents bucket policy information.
/// </summary>
public class BucketPolicyInfo
{
    /// <summary>
    /// Gets or sets the policy version.
    /// </summary>
    public string Version { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the policy statements.
    /// </summary>
    public List<PolicyStatement> Statement { get; set; } = new();
}

/// <summary>
/// Represents a policy statement.
/// </summary>
public class PolicyStatement
{
    /// <summary>
    /// Gets or sets the statement effect.
    /// </summary>
    public string Effect { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the principal.
    /// </summary>
    public object? Principal { get; set; }

    /// <summary>
    /// Gets or sets the action.
    /// </summary>
    public object? Action { get; set; }

    /// <summary>
    /// Gets or sets the resource.
    /// </summary>
    public object? Resource { get; set; }

    /// <summary>
    /// Gets or sets the condition.
    /// </summary>
    public object? Condition { get; set; }
}

/// <summary>
/// Represents notification configuration.
/// </summary>
public class NotificationConfiguration
{
    /// <summary>
    /// Gets or sets the topic configurations.
    /// </summary>
    public List<TopicConfiguration> TopicConfigurations { get; set; } = new();

    /// <summary>
    /// Gets or sets the queue configurations.
    /// </summary>
    public List<QueueConfiguration> QueueConfigurations { get; set; } = new();

    /// <summary>
    /// Gets or sets the cloud function configurations.
    /// </summary>
    public List<CloudFunctionConfiguration> CloudFunctionConfigurations { get; set; } = new();
}

/// <summary>
/// Represents a topic configuration.
/// </summary>
public class TopicConfiguration
{
    /// <summary>
    /// Gets or sets the topic ARN.
    /// </summary>
    public string TopicArn { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the events.
    /// </summary>
    public List<string> Events { get; set; } = new();

    /// <summary>
    /// Gets or sets the filter.
    /// </summary>
    public NotificationFilter? Filter { get; set; }
}

/// <summary>
/// Represents a queue configuration.
/// </summary>
public class QueueConfiguration
{
    /// <summary>
    /// Gets or sets the queue ARN.
    /// </summary>
    public string QueueArn { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the events.
    /// </summary>
    public List<string> Events { get; set; } = new();

    /// <summary>
    /// Gets or sets the filter.
    /// </summary>
    public NotificationFilter? Filter { get; set; }
}

/// <summary>
/// Represents a cloud function configuration.
/// </summary>
public class CloudFunctionConfiguration
{
    /// <summary>
    /// Gets or sets the cloud function ARN.
    /// </summary>
    public string CloudFunctionArn { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the events.
    /// </summary>
    public List<string> Events { get; set; } = new();

    /// <summary>
    /// Gets or sets the filter.
    /// </summary>
    public NotificationFilter? Filter { get; set; }
}

/// <summary>
/// Represents a notification filter.
/// </summary>
public class NotificationFilter
{
    /// <summary>
    /// Gets or sets the key filter.
    /// </summary>
    public KeyFilter? Key { get; set; }
}

/// <summary>
/// Represents a key filter.
/// </summary>
public class KeyFilter
{
    /// <summary>
    /// Gets or sets the filter rules.
    /// </summary>
    public List<FilterRule> FilterRules { get; set; } = new();
}

/// <summary>
/// Represents a filter rule.
/// </summary>
public class FilterRule
{
    /// <summary>
    /// Gets or sets the filter rule name.
    /// </summary>
    public string Name { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the filter rule value.
    /// </summary>
    public string Value { get; set; } = string.Empty;
}
