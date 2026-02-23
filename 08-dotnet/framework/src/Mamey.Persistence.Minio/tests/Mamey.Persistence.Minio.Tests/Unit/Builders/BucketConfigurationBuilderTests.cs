using Mamey.Persistence.Minio.Builders;
using Mamey.Persistence.Minio.Models.DTOs;
using Mamey.Persistence.Minio.Tests.Helpers;
using FluentAssertions;
using Xunit;

namespace Mamey.Persistence.Minio.Tests.Unit.Builders;

/// <summary>
/// Unit tests for BucketConfigurationBuilder.
/// </summary>
public class BucketConfigurationBuilderTests
{
    [Fact]
    public void Build_ShouldCreateBucketConfiguration_WithBasicProperties()
    {
        // Arrange
        var bucketName = TestDataGenerator.CreateBucketName();

        // Act
        var config = bucketName
            .ConfigureBucket()
            .Build();

        // Assert
        config.Should().NotBeNull();
        config.BucketName.Should().Be(bucketName);
        config.VersioningEnabled.Should().BeFalse();
        config.VersioningSuspended.Should().BeFalse();
        config.MfaDeleteEnabled.Should().BeFalse();
        config.Tags.Should().BeNull();
        config.LifecycleRules.Should().BeNull();
        config.ObjectLockConfiguration.Should().BeNull();
        config.PolicyJson.Should().BeNull();
        config.EncryptionAlgorithm.Should().BeNull();
        config.EncryptionKeyId.Should().BeNull();
        config.Notifications.Should().BeNull();
    }

    [Fact]
    public void WithVersioning_ShouldEnableVersioning()
    {
        // Arrange
        var bucketName = TestDataGenerator.CreateBucketName();

        // Act
        var config = bucketName
            .ConfigureBucket()
            .WithVersioning()
            .Build();

        // Assert
        config.VersioningEnabled.Should().BeTrue();
        config.VersioningSuspended.Should().BeFalse();
        config.MfaDeleteEnabled.Should().BeFalse();
    }

    [Fact]
    public void WithVersioning_ShouldEnableVersioningWithMfaDelete()
    {
        // Arrange
        var bucketName = TestDataGenerator.CreateBucketName();

        // Act
        var config = bucketName
            .ConfigureBucket()
            .WithVersioning(mfaDeleteEnabled: true)
            .Build();

        // Assert
        config.VersioningEnabled.Should().BeTrue();
        config.VersioningSuspended.Should().BeFalse();
        config.MfaDeleteEnabled.Should().BeTrue();
    }

    [Fact]
    public void SuspendVersioning_ShouldSuspendVersioning()
    {
        // Arrange
        var bucketName = TestDataGenerator.CreateBucketName();

        // Act
        var config = bucketName
            .ConfigureBucket()
            .SuspendVersioning()
            .Build();

        // Assert
        config.VersioningEnabled.Should().BeFalse();
        config.VersioningSuspended.Should().BeTrue();
        config.MfaDeleteEnabled.Should().BeFalse();
    }

    [Fact]
    public void DisableVersioning_ShouldDisableVersioning()
    {
        // Arrange
        var bucketName = TestDataGenerator.CreateBucketName();

        // Act
        var config = bucketName
            .ConfigureBucket()
            .DisableVersioning()
            .Build();

        // Assert
        config.VersioningEnabled.Should().BeFalse();
        config.VersioningSuspended.Should().BeFalse();
        config.MfaDeleteEnabled.Should().BeFalse();
    }

    [Fact]
    public void WithTag_ShouldAddSingleTag()
    {
        // Arrange
        var bucketName = TestDataGenerator.CreateBucketName();

        // Act
        var config = bucketName
            .ConfigureBucket()
            .WithTag("environment", "production")
            .Build();

        // Assert
        config.Tags.Should().NotBeNull();
        config.Tags.Should().ContainKey("environment");
        config.Tags["environment"].Should().Be("production");
    }

    [Fact]
    public void WithTags_ShouldAddMultipleTags()
    {
        // Arrange
        var bucketName = TestDataGenerator.CreateBucketName();
        var tags = TestDataGenerator.CreateSampleBucketTags();

        // Act
        var config = bucketName
            .ConfigureBucket()
            .WithTags(tags)
            .Build();

        // Assert
        config.Tags.Should().NotBeNull();
        config.Tags.Should().ContainKey("environment");
        config.Tags.Should().ContainKey("team");
        config.Tags.Should().ContainKey("cost-center");
        config.Tags.Should().ContainKey("project");
    }

    [Fact]
    public void WithLifecycleRule_ShouldAddLifecycleRule()
    {
        // Arrange
        var bucketName = TestDataGenerator.CreateBucketName();
        var rule = new LifecycleRule
        {
            Id = "test-rule",
            Status = LifecycleRuleStatus.Enabled,
            Filter = new LifecycleFilter { Prefix = "logs/" },
            Expiration = new LifecycleExpiration { Days = 30 }
        };

        // Act
        var config = bucketName
            .ConfigureBucket()
            .WithLifecycleRule(rule)
            .Build();

        // Assert
        config.LifecycleRules.Should().NotBeNull();
        config.LifecycleRules.Should().HaveCount(1);
        config.LifecycleRules[0].Id.Should().Be("test-rule");
        config.LifecycleRules[0].Status.Should().Be(LifecycleRuleStatus.Enabled);
    }

    [Fact]
    public void WithExpirationRule_ShouldAddExpirationRule()
    {
        // Arrange
        var bucketName = TestDataGenerator.CreateBucketName();

        // Act
        var config = bucketName
            .ConfigureBucket()
            .WithExpirationRule("delete-old-logs", "logs/", 30)
            .Build();

        // Assert
        config.LifecycleRules.Should().NotBeNull();
        config.LifecycleRules.Should().HaveCount(1);
        var rule = config.LifecycleRules[0];
        rule.Id.Should().Be("delete-old-logs");
        rule.Status.Should().Be(LifecycleRuleStatus.Enabled);
        rule.Filter.Prefix.Should().Be("logs/");
        rule.Expiration.Should().NotBeNull();
        rule.Expiration.Days.Should().Be(30);
    }

    [Fact]
    public void WithTransitionRule_ShouldAddTransitionRule()
    {
        // Arrange
        var bucketName = TestDataGenerator.CreateBucketName();

        // Act
        var config = bucketName
            .ConfigureBucket()
            .WithTransitionRule("archive-documents", "documents/", 90, "GLACIER")
            .Build();

        // Assert
        config.LifecycleRules.Should().NotBeNull();
        config.LifecycleRules.Should().HaveCount(1);
        var rule = config.LifecycleRules[0];
        rule.Id.Should().Be("archive-documents");
        rule.Status.Should().Be(LifecycleRuleStatus.Enabled);
        rule.Filter.Prefix.Should().Be("documents/");
        rule.Transitions.Should().NotBeNull();
        rule.Transitions.Should().HaveCount(1);
        rule.Transitions[0].Days.Should().Be(90);
        rule.Transitions[0].StorageClass.Should().Be("GLACIER");
    }

    [Fact]
    public void WithObjectLock_ShouldConfigureObjectLock()
    {
        // Arrange
        var bucketName = TestDataGenerator.CreateBucketName();

        // Act
        var config = bucketName
            .ConfigureBucket()
            .WithObjectLock(enabled: true, ObjectRetentionMode.Compliance, 2555)
            .Build();

        // Assert
        config.ObjectLockConfiguration.Should().NotBeNull();
        config.ObjectLockConfiguration.ObjectLockEnabled.Should().BeTrue();
        config.ObjectLockConfiguration.DefaultRetention.Should().NotBeNull();
        config.ObjectLockConfiguration.DefaultRetention.Mode.Should().Be(ObjectRetentionMode.Compliance);
        config.ObjectLockConfiguration.DefaultRetention.Days.Should().Be(2555);
    }

    [Fact]
    public void WithObjectLock_ShouldConfigureObjectLockWithoutDefaultRetention()
    {
        // Arrange
        var bucketName = TestDataGenerator.CreateBucketName();

        // Act
        var config = bucketName
            .ConfigureBucket()
            .WithObjectLock(enabled: true)
            .Build();

        // Assert
        config.ObjectLockConfiguration.Should().NotBeNull();
        config.ObjectLockConfiguration.ObjectLockEnabled.Should().BeTrue();
        config.ObjectLockConfiguration.DefaultRetention.Should().BeNull();
    }

    [Fact]
    public void WithPolicy_ShouldSetPolicyJson()
    {
        // Arrange
        var bucketName = TestDataGenerator.CreateBucketName();
        var policyJson = """
            {
                "Version": "2012-10-17",
                "Statement": [
                    {
                        "Effect": "Allow",
                        "Principal": "*",
                        "Action": "s3:GetObject",
                        "Resource": "arn:aws:s3:::test-bucket/*"
                    }
                ]
            }
            """;

        // Act
        var config = bucketName
            .ConfigureBucket()
            .WithPolicy(policyJson)
            .Build();

        // Assert
        config.PolicyJson.Should().Be(policyJson);
    }

    [Fact]
    public void WithEncryption_ShouldSetEncryptionSettings()
    {
        // Arrange
        var bucketName = TestDataGenerator.CreateBucketName();

        // Act
        var config = bucketName
            .ConfigureBucket()
            .WithEncryption("AES256", "my-kms-key-id")
            .Build();

        // Assert
        config.EncryptionAlgorithm.Should().Be("AES256");
        config.EncryptionKeyId.Should().Be("my-kms-key-id");
    }

    [Fact]
    public void WithEncryption_ShouldSetEncryptionWithoutKeyId()
    {
        // Arrange
        var bucketName = TestDataGenerator.CreateBucketName();

        // Act
        var config = bucketName
            .ConfigureBucket()
            .WithEncryption("AES256")
            .Build();

        // Assert
        config.EncryptionAlgorithm.Should().Be("AES256");
        config.EncryptionKeyId.Should().BeNull();
    }

    [Fact]
    public void WithNotification_ShouldAddNotification()
    {
        // Arrange
        var bucketName = TestDataGenerator.CreateBucketName();
        var notification = new NotificationConfiguration
        {
            Id = "notification-1",
            Events = new List<string> { "s3:ObjectCreated:*" },
            Filter = new NotificationFilter { Prefix = "documents/" }
        };

        // Act
        var config = bucketName
            .ConfigureBucket()
            .WithNotification(notification)
            .Build();

        // Assert
        config.Notifications.Should().NotBeNull();
        config.Notifications.Should().HaveCount(1);
        config.Notifications[0].Id.Should().Be("notification-1");
    }

    [Fact]
    public void Build_ShouldCombineAllSettings()
    {
        // Arrange
        var bucketName = TestDataGenerator.CreateBucketName();
        var tags = TestDataGenerator.CreateSampleBucketTags();
        var policyJson = """{"Version": "2012-10-17", "Statement": []}""";

        // Act
        var config = bucketName
            .ConfigureBucket()
            .WithVersioning(mfaDeleteEnabled: true)
            .WithTags(tags)
            .WithExpirationRule("delete-old-logs", "logs/", 30)
            .WithTransitionRule("archive-documents", "documents/", 90, "GLACIER")
            .WithObjectLock(enabled: true, ObjectRetentionMode.Compliance, 2555)
            .WithPolicy(policyJson)
            .WithEncryption("AES256", "my-kms-key-id")
            .Build();

        // Assert
        config.BucketName.Should().Be(bucketName);
        config.VersioningEnabled.Should().BeTrue();
        config.MfaDeleteEnabled.Should().BeTrue();
        config.Tags.Should().NotBeNull();
        config.Tags.Should().HaveCount(4);
        config.LifecycleRules.Should().NotBeNull();
        config.LifecycleRules.Should().HaveCount(2);
        config.ObjectLockConfiguration.Should().NotBeNull();
        config.ObjectLockConfiguration.ObjectLockEnabled.Should().BeTrue();
        config.PolicyJson.Should().Be(policyJson);
        config.EncryptionAlgorithm.Should().Be("AES256");
        config.EncryptionKeyId.Should().Be("my-kms-key-id");
    }

    [Theory]
    [InlineData("")]
    [InlineData(" ")]
    [InlineData(null)]
    public void ConfigureBucket_ShouldThrowArgumentException_WhenBucketNameIsInvalid(string? bucketName)
    {
        // Act & Assert
        Assert.Throws<ArgumentException>(() => bucketName!.ConfigureBucket());
    }

    [Fact]
    public void WithTag_ShouldThrowArgumentException_WhenKeyIsNull()
    {
        // Arrange
        var bucketName = TestDataGenerator.CreateBucketName();

        // Act & Assert
        Assert.Throws<ArgumentException>(() => bucketName
            .ConfigureBucket()
            .WithTag(null!, "value"));
    }

    [Fact]
    public void WithTag_ShouldThrowArgumentException_WhenValueIsNull()
    {
        // Arrange
        var bucketName = TestDataGenerator.CreateBucketName();

        // Act & Assert
        Assert.Throws<ArgumentException>(() => bucketName
            .ConfigureBucket()
            .WithTag("key", null!));
    }

    [Fact]
    public void WithTags_ShouldThrowArgumentNullException_WhenTagsIsNull()
    {
        // Arrange
        var bucketName = TestDataGenerator.CreateBucketName();

        // Act & Assert
        Assert.Throws<ArgumentNullException>(() => bucketName
            .ConfigureBucket()
            .WithTags(null!));
    }

    [Fact]
    public void WithLifecycleRule_ShouldThrowArgumentNullException_WhenRuleIsNull()
    {
        // Arrange
        var bucketName = TestDataGenerator.CreateBucketName();

        // Act & Assert
        Assert.Throws<ArgumentNullException>(() => bucketName
            .ConfigureBucket()
            .WithLifecycleRule(null!));
    }

    [Theory]
    [InlineData("")]
    [InlineData(" ")]
    [InlineData(null)]
    public void WithExpirationRule_ShouldThrowArgumentException_WhenIdIsInvalid(string? id)
    {
        // Arrange
        var bucketName = TestDataGenerator.CreateBucketName();

        // Act & Assert
        Assert.Throws<ArgumentException>(() => bucketName
            .ConfigureBucket()
            .WithExpirationRule(id!, "logs/", 30));
    }

    [Theory]
    [InlineData("")]
    [InlineData(" ")]
    [InlineData(null)]
    public void WithTransitionRule_ShouldThrowArgumentException_WhenIdIsInvalid(string? id)
    {
        // Arrange
        var bucketName = TestDataGenerator.CreateBucketName();

        // Act & Assert
        Assert.Throws<ArgumentException>(() => bucketName
            .ConfigureBucket()
            .WithTransitionRule(id!, "documents/", 90, "GLACIER"));
    }

    [Theory]
    [InlineData(0)]
    [InlineData(-1)]
    [InlineData(-30)]
    public void WithExpirationRule_ShouldThrowArgumentException_WhenDaysIsInvalid(int days)
    {
        // Arrange
        var bucketName = TestDataGenerator.CreateBucketName();

        // Act & Assert
        Assert.Throws<ArgumentException>(() => bucketName
            .ConfigureBucket()
            .WithExpirationRule("test-rule", "logs/", days));
    }

    [Theory]
    [InlineData(0)]
    [InlineData(-1)]
    [InlineData(-90)]
    public void WithTransitionRule_ShouldThrowArgumentException_WhenDaysIsInvalid(int days)
    {
        // Arrange
        var bucketName = TestDataGenerator.CreateBucketName();

        // Act & Assert
        Assert.Throws<ArgumentException>(() => bucketName
            .ConfigureBucket()
            .WithTransitionRule("test-rule", "documents/", days, "GLACIER"));
    }

    [Theory]
    [InlineData("")]
    [InlineData(" ")]
    [InlineData(null)]
    public void WithTransitionRule_ShouldThrowArgumentException_WhenStorageClassIsInvalid(string? storageClass)
    {
        // Arrange
        var bucketName = TestDataGenerator.CreateBucketName();

        // Act & Assert
        Assert.Throws<ArgumentException>(() => bucketName
            .ConfigureBucket()
            .WithTransitionRule("test-rule", "documents/", 90, storageClass!));
    }

    [Theory]
    [InlineData("")]
    [InlineData(" ")]
    [InlineData(null)]
    public void WithPolicy_ShouldThrowArgumentException_WhenPolicyJsonIsInvalid(string? policyJson)
    {
        // Arrange
        var bucketName = TestDataGenerator.CreateBucketName();

        // Act & Assert
        Assert.Throws<ArgumentException>(() => bucketName
            .ConfigureBucket()
            .WithPolicy(policyJson!));
    }

    [Theory]
    [InlineData("")]
    [InlineData(" ")]
    [InlineData(null)]
    public void WithEncryption_ShouldThrowArgumentException_WhenAlgorithmIsInvalid(string? algorithm)
    {
        // Arrange
        var bucketName = TestDataGenerator.CreateBucketName();

        // Act & Assert
        Assert.Throws<ArgumentException>(() => bucketName
            .ConfigureBucket()
            .WithEncryption(algorithm!));
    }

    [Fact]
    public void WithNotification_ShouldThrowArgumentNullException_WhenNotificationIsNull()
    {
        // Arrange
        var bucketName = TestDataGenerator.CreateBucketName();

        // Act & Assert
        Assert.Throws<ArgumentNullException>(() => bucketName
            .ConfigureBucket()
            .WithNotification(null!));
    }
}
