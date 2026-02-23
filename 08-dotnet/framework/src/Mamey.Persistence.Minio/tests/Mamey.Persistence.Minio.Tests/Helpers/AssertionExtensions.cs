using FluentAssertions;
using Mamey.Persistence.Minio.Models.DTOs;

namespace Mamey.Persistence.Minio.Tests.Helpers;

/// <summary>
/// Extension methods for custom assertions in tests.
/// </summary>
public static class AssertionExtensions
{
    /// <summary>
    /// Asserts that an ObjectMetadata has the expected properties.
    /// </summary>
    /// <param name="metadata">The metadata to assert.</param>
    /// <param name="expectedName">The expected object name.</param>
    /// <param name="expectedSize">The expected object size.</param>
    /// <param name="expectedContentType">The expected content type.</param>
    public static void ShouldHaveExpectedProperties(this ObjectMetadata metadata, string expectedName, long expectedSize, string? expectedContentType = null)
    {
        metadata.Should().NotBeNull();
        metadata.Name.Should().Be(expectedName);
        metadata.Size.Should().Be(expectedSize);
        metadata.LastModified.Should().BeCloseTo(DateTime.UtcNow, TimeSpan.FromMinutes(1));
        metadata.ETag.Should().NotBeNullOrEmpty();
        
        if (expectedContentType != null)
        {
            metadata.ContentType.Should().Be(expectedContentType);
        }
    }
    
    /// <summary>
    /// Asserts that a BucketInfo has the expected properties.
    /// </summary>
    /// <param name="bucketInfo">The bucket info to assert.</param>
    /// <param name="expectedName">The expected bucket name.</param>
    public static void ShouldHaveExpectedProperties(this BucketInfo bucketInfo, string expectedName)
    {
        bucketInfo.Should().NotBeNull();
        bucketInfo.Name.Should().Be(expectedName);
        bucketInfo.CreationDate.Should().BeCloseTo(DateTime.UtcNow, TimeSpan.FromDays(1));
    }
    
    /// <summary>
    /// Asserts that a BucketVersioningInfo has the expected status.
    /// </summary>
    /// <param name="versioningInfo">The versioning info to assert.</param>
    /// <param name="expectedStatus">The expected versioning status.</param>
    /// <param name="expectedMfaDelete">The expected MFA delete status.</param>
    public static void ShouldHaveExpectedVersioningStatus(this BucketVersioningInfo versioningInfo, VersioningStatus expectedStatus, MfaDeleteStatus expectedMfaDelete = MfaDeleteStatus.Disabled)
    {
        versioningInfo.Should().NotBeNull();
        versioningInfo.Status.Should().Be(expectedStatus);
        versioningInfo.MfaDelete.Should().Be(expectedMfaDelete);
    }
    
    /// <summary>
    /// Asserts that a MultipartUploadProgress has the expected properties.
    /// </summary>
    /// <param name="progress">The progress to assert.</param>
    /// <param name="expectedTotalSize">The expected total size.</param>
    /// <param name="expectedTotalParts">The expected total parts.</param>
    public static void ShouldHaveExpectedProgress(this MultipartUploadProgress progress, long expectedTotalSize, int expectedTotalParts)
    {
        progress.Should().NotBeNull();
        progress.TotalSize.Should().Be(expectedTotalSize);
        progress.TotalParts.Should().Be(expectedTotalParts);
        progress.Percentage.Should().BeInRange(0, 100);
        progress.BytesUploaded.Should().BeInRange(0, expectedTotalSize);
        progress.CompletedParts.Should().BeInRange(0, expectedTotalParts);
    }
    
    /// <summary>
    /// Asserts that a LifecycleConfiguration has the expected number of rules.
    /// </summary>
    /// <param name="configuration">The configuration to assert.</param>
    /// <param name="expectedRuleCount">The expected number of rules.</param>
    public static void ShouldHaveExpectedRuleCount(this LifecycleConfiguration configuration, int expectedRuleCount)
    {
        configuration.Should().NotBeNull();
        configuration.Rules.Should().HaveCount(expectedRuleCount);
    }
    
    /// <summary>
    /// Asserts that a LifecycleRule has the expected properties.
    /// </summary>
    /// <param name="rule">The rule to assert.</param>
    /// <param name="expectedId">The expected rule ID.</param>
    /// <param name="expectedStatus">The expected rule status.</param>
    public static void ShouldHaveExpectedRuleProperties(this LifecycleRule rule, string expectedId, LifecycleRuleStatus expectedStatus)
    {
        rule.Should().NotBeNull();
        rule.Id.Should().Be(expectedId);
        rule.Status.Should().Be(expectedStatus);
        rule.Filter.Should().NotBeNull();
    }
    
    /// <summary>
    /// Asserts that an ObjectLockConfiguration has the expected properties.
    /// </summary>
    /// <param name="configuration">The configuration to assert.</param>
    /// <param name="expectedEnabled">The expected enabled status.</param>
    public static void ShouldHaveExpectedLockConfiguration(this ObjectLockConfiguration configuration, bool expectedEnabled)
    {
        configuration.Should().NotBeNull();
        configuration.ObjectLockEnabled.Should().Be(expectedEnabled);
        
        if (expectedEnabled)
        {
            configuration.DefaultRetention.Should().NotBeNull();
        }
    }
    
    /// <summary>
    /// Asserts that a dictionary contains the expected metadata.
    /// </summary>
    /// <param name="metadata">The metadata dictionary to assert.</param>
    /// <param name="expectedKeys">The expected keys.</param>
    public static void ShouldContainExpectedMetadata(this Dictionary<string, string>? metadata, params string[] expectedKeys)
    {
        metadata.Should().NotBeNull();
        metadata.Should().NotBeEmpty();
        
        foreach (var key in expectedKeys)
        {
            metadata.Should().ContainKey(key);
            metadata[key].Should().NotBeNullOrEmpty();
        }
    }
    
    /// <summary>
    /// Asserts that a stream contains the expected data.
    /// </summary>
    /// <param name="stream">The stream to assert.</param>
    /// <param name="expectedData">The expected data.</param>
    public static void ShouldContainExpectedData(this Stream stream, byte[] expectedData)
    {
        stream.Should().NotBeNull();
        stream.Position = 0;
        
        var actualData = new byte[expectedData.Length];
        var bytesRead = stream.Read(actualData, 0, actualData.Length);
        
        bytesRead.Should().Be(expectedData.Length);
        actualData.Should().Equal(expectedData);
    }
}



